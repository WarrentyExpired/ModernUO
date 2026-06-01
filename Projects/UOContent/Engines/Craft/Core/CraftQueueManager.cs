using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Gumps;

namespace Server.Engines.Craft;

public enum CraftResult
{
    Failure,
    Success,
    Exceptional
}

public class CraftQueueEntry
{
    public CraftItem Item { get; init; }
    public int ResourceIndex { get; init; }
    public int ResourceIndex2 { get; init; }
    public int TargetAmount { get; set; }
    public int Completed { get; set; }
    public int Successes { get; set; }
    public int Failures { get; set; }
    public int Exceptionals { get; set; }

    public CraftQueueEntry(CraftItem item, int resourceIndex, int resourceIndex2)
    {
        Item = item;
        ResourceIndex = resourceIndex;
        ResourceIndex2 = resourceIndex2;
        TargetAmount = 1;
    }
}

public static class CraftQueueManager
{
    private static readonly Dictionary<Mobile, List<CraftQueueEntry>> _queues = new();
    private static readonly Dictionary<Mobile, bool> _running = new();

    public static List<CraftQueueEntry> GetQueue(Mobile m)
    {
        if (!_queues.TryGetValue(m, out var list))
        {
            list = new List<CraftQueueEntry>();
            _queues[m] = list;
        }
        return list;
    }

    public static bool IsRunning(Mobile m)
    {
        return _running.TryGetValue(m, out var status) && status;
    }

    public static void AddToQueue(Mobile m, CraftItem item, CraftSystem system)
    {
        var queue = GetQueue(m);
        var context = system.GetContext(m);

        int resIndex = context?.LastResourceIndex ?? -1;
        int resIndex2 = context?.LastResourceIndex2 ?? -1;

        var resCol = item.UseSubRes2 ? system.CraftSubRes2 : system.CraftSubRes;
        if (resCol.Init)
        {
            if (item.UseSubRes2 && resIndex2 < 0) resIndex2 = 0;
            if (!item.UseSubRes2 && resIndex < 0) resIndex = 0;
        }

        var existing = queue.Find(e => e.Item == item && e.ResourceIndex == resIndex && e.ResourceIndex2 == resIndex2);

        if (existing != null)
        {
            existing.TargetAmount++;
        }
        else
        {
            queue.Add(new CraftQueueEntry(item, resIndex, resIndex2));
        }
    }

    public static void ClearQueue(Mobile m)
    {
        PauseQueue(m);
        GetQueue(m).Clear();
    }

    public static void PauseQueue(Mobile m)
    {
        _running[m] = false;
    }

    public static void StartQueue(Mobile m, CraftSystem system, BaseTool tool)
    {
        if (GetQueue(m).Count == 0) return;

        _running[m] = true;
        TriggerNextCraft(m, system, tool);
    }

    public static void HandleQueueItemCompleted(Mobile m, CraftResult result, CraftSystem system, BaseTool tool)
    {
        if (m == null || !IsRunning(m)) return;

        var queue = GetQueue(m);
        CraftQueueEntry activeEntry = queue.Find(e => e.Successes < e.TargetAmount);

        if (activeEntry == null)
        {
            m.SendMessage(0x35, "Your crafting queue has finished processing successfully!");
            PauseQueue(m);
            m.SendGump(new CraftQueueGump(m, system, tool));
            return;
        }

        activeEntry.Completed++;
        switch (result)
        {
            case CraftResult.Success:
                activeEntry.Successes++;
                break;
            case CraftResult.Exceptional:
                activeEntry.Successes++;
                activeEntry.Exceptionals++;
                break;
            case CraftResult.Failure:
                activeEntry.Failures++;
                break;
        }

        // ADDED: Tool validation and auto-scavenge interceptor loop
        if (tool == null || tool.Deleted || tool.UsesRemaining < 1)
        {
            BaseTool backupTool = FindReplacementTool(m, tool);
            if (backupTool != null)
            {
                tool = backupTool; // Seamlessly hot-swap the reference down the chain!
                m.SendMessage(0x35, "Your tool broke! Automatically swapped to a backup found in your pack.");
            }
            else
            {
                m.SendMessage(0x22, "Your crafting tool broke and no replacement was found. Queue paused.");
                PauseQueue(m);
                m.SendGump(new CraftQueueGump(m, system, tool));
                return;
            }
        }

        m.SendGump(new CraftQueueGump(m, system, tool));

        Timer.DelayCall(TimeSpan.FromMilliseconds(500), () =>
        {
            if (IsRunning(m))
            {
                TriggerNextCraft(m, system, tool);
            }
        });
    }

    private static void TriggerNextCraft(Mobile m, CraftSystem system, BaseTool tool)
    {
        if (m == null || m.Deleted || !IsRunning(m))
        {
            PauseQueue(m);
            return;
        }

        // ADDED: Fallback check in case a tool breaks outside standard execution paths
        if (tool == null || tool.Deleted || tool.UsesRemaining < 1)
        {
            BaseTool backupTool = FindReplacementTool(m, tool);
            if (backupTool != null)
            {
                tool = backupTool;
            }
            else
            {
                PauseQueue(m);
                m.SendGump(new CraftQueueGump(m, system, tool));
                return;
            }
        }

        var queue = GetQueue(m);
        CraftQueueEntry activeEntry = queue.Find(e => e.Successes < e.TargetAmount);

        if (activeEntry == null)
        {
            m.SendMessage(0x35, "Your crafting queue has finished processing successfully!");
            PauseQueue(m);
            m.SendGump(new CraftQueueGump(m, system, tool));
            return;
        }

        int cannotCraft = system.CanCraft(m, tool, activeEntry.Item.ItemType);
        if (cannotCraft > 0)
        {
            PauseQueue(m);
            m.SendGump(new CraftGump(m, system, tool, cannotCraft));
            m.SendGump(new CraftQueueGump(m, system, tool));
            return;
        }

        Type resourceType = null;
        var resCol = activeEntry.Item.UseSubRes2 ? system.CraftSubRes2 : system.CraftSubRes;
        int targetIdx = activeEntry.Item.UseSubRes2 ? activeEntry.ResourceIndex2 : activeEntry.ResourceIndex;

        if (resCol.Init && targetIdx >= 0 && targetIdx < resCol.Count)
        {
            resourceType = resCol.GetAt(targetIdx).ItemType;
        }

        system.CreateItem(m, activeEntry.Item.ItemType, resourceType, tool, activeEntry.Item);
    }

    // ADDED: Scans backpack and equipped layers for an identical crafting tool variant
    private static BaseTool FindReplacementTool(Mobile from, BaseTool brokenTool)
    {
        if (from == null || brokenTool == null) return null;

        Type toolType = brokenTool.GetType();

        // 1. Scan Backpack
        if (from.Backpack != null)
        {
            foreach (var item in from.Backpack.FindItems())
            {
                if (item != brokenTool && !item.Deleted && item.GetType() == toolType && item is BaseTool backup && backup.UsesRemaining > 0)
                {
                    return backup;
                }
            }
        }

        // 2. Scan Equipped Layers (Hands) just in case they hold their tools
        Item oneHand = from.FindItemOnLayer(Layer.OneHanded);
        if (oneHand != null && oneHand != brokenTool && !oneHand.Deleted && oneHand.GetType() == toolType && oneHand is BaseTool t1 && t1.UsesRemaining > 0)
            return t1;

        Item twoHand = from.FindItemOnLayer(Layer.TwoHanded);
        if (twoHand != null && twoHand != brokenTool && !twoHand.Deleted && twoHand.GetType() == toolType && twoHand is BaseTool t2 && t2.UsesRemaining > 0)
            return t2;

        return null;
    }
}
