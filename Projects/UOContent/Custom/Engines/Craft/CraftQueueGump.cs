using System;
using Server.Gumps;
using Server.Items;
using Server.Network;

namespace Server.Engines.Craft;

public class CraftQueueGump : DynamicGump
{
    private const int LabelHue = 0x480;
    private const int LabelColor = 0x7FFF;
    private readonly Mobile _from;
    private readonly CraftSystem _craftSystem;
    private readonly BaseTool _tool;

    public override bool Singleton => true;

    public CraftQueueGump(Mobile from, CraftSystem craftSystem, BaseTool tool) : base(40, 40)
    {
        _from = from;
        _craftSystem = craftSystem;
        _tool = tool;
    }

    protected override void BuildLayout(ref DynamicGumpBuilder builder)
    {
        builder.AddPage();
        builder.AddBackground(0, 0, 530, 437, 5054);
        builder.AddImageTiled(10, 10, 510, 22, 2624);
        builder.AddImageTiled(10, 37, 510, 300, 2624);
        builder.AddImageTiled(10, 342, 510, 85, 2624);
        builder.AddAlphaRegion(10, 10, 510, 417);

        builder.AddLabel(15, 12, LabelHue, "CRAFTING WORKSHOP MANAGED QUEUE");

        // FIXED: Shifted headers to support the new Material column slot
        builder.AddLabel(25, 42, LabelHue, "Item Name");
        builder.AddLabel(175, 42, LabelHue, "Material");
        builder.AddLabel(285, 42, LabelHue, "Amount");
        builder.AddLabel(370, 42, LabelHue, "Made/Failed/Excep");

        // Action controls bottom footer row block
        builder.AddButton(15, 395, 4014, 4016, 0);
        builder.AddHtmlLocalized(50, 397, 100, 18, 1044150, LabelColor); // BACK

        bool running = CraftQueueManager.IsRunning(_from);
        builder.AddButton(180, 395, running ? 4017 : 4005, running ? 4019 : 4007, 1);
        builder.AddLabel(215, 397, LabelHue, running ? "PAUSE ACTION" : "START QUEUE");

        builder.AddButton(370, 395, 4020, 4022, 2);
        builder.AddLabel(405, 397, LabelHue, "CLEAR ALL");

        var queue = CraftQueueManager.GetQueue(_from);

        if (queue.Count == 0)
        {
            builder.AddLabel(40, 150, LabelHue, "Your active crafting queue is completely empty.");
            return;
        }

        for (int i = 0; i < queue.Count && i < 10; i++)
        {
            var entry = queue[i];
            int yOffset = 70 + (i * 26);

            // Row text details (Item Name)
            if (entry.Item.NameNumber > 0)
                builder.AddHtmlLocalized(25, yOffset, 140, 18, entry.Item.NameNumber, LabelColor);
            else
                builder.AddLabel(25, yOffset, LabelHue, entry.Item.NameString);

            // Material Column Grid Renderer
            var resCol = entry.Item.UseSubRes2 ? _craftSystem.CraftSubRes2 : _craftSystem.CraftSubRes;
            int targetIdx = entry.Item.UseSubRes2 ? entry.ResourceIndex2 : entry.ResourceIndex;

            // Normalize layout index continuity for display safety
            if (targetIdx < 0 && resCol.Init)
            {
                targetIdx = 0;
            }

            if (resCol.Init && targetIdx >= 0 && targetIdx < resCol.Count)
            {
                var subRes = resCol.GetAt(targetIdx);

                // 1. Separate camel case words (e.g., "DullCopperIngot" -> "Dull Copper Ingot")
                string matName = System.Text.RegularExpressions.Regex.Replace(subRes.ItemType.Name, "([a-z])([A-Z])", "$1 $2");

                // 2. FIXED: Strip out resource wrappers so they don't bleed into the Count column!
                matName = matName.Replace("Ingot", "")
                                 .Replace("Leather", "")
                                 .Replace("Hides", "")
                                 .Replace("Board", "")
                                 .Replace("Log", "")
                                 .Trim();

                // Fallback protection check
                if (string.IsNullOrWhiteSpace(matName))
                {
                    matName = "Iron";
                }

                // Render as a clean, uniform text string label in uppercase
                builder.AddLabel(175, yOffset, LabelHue, matName.ToUpper());
            }
            else
            {
                builder.AddLabel(175, yOffset, LabelHue, "IRON");
            }

            // Quantitative control handles (Shifted right)
            builder.AddLabel(285, yOffset, LabelHue, entry.TargetAmount.ToString());
            builder.AddButton(325, yOffset, 4011, 4012, 400 + i); // Set button

            // Performance log counters (Shifted right)
            string stats = $"{entry.Successes} / {entry.Failures} / {entry.Exceptionals}";
            builder.AddLabel(370, yOffset, LabelHue, stats);

            // Trash can removal line slot button
            builder.AddButton(485, yOffset, 4017, 4019, 300 + i);
        }
    }

    public override void OnResponse(NetState sender, in RelayInfo info)
    {
        int id = info.ButtonID;

        if (id == 0)
        {
            _from.SendGump(new CraftGump(_from, _craftSystem, _tool, null));
            return;
        }
        if (id == 1)
        {
            if (CraftQueueManager.IsRunning(_from))
                CraftQueueManager.PauseQueue(_from);
            else
                CraftQueueManager.StartQueue(_from, _craftSystem, _tool);

            _from.SendGump(new CraftQueueGump(_from, _craftSystem, _tool));
            return;
        }
        if (id == 2)
        {
            CraftQueueManager.ClearQueue(_from);
            _from.SendGump(new CraftQueueGump(_from, _craftSystem, _tool));
            return;
        }

        var queue = CraftQueueManager.GetQueue(_from);
        if (id >= 400 && id < 500)
        {
            int index = id - 400;
            if (index < queue.Count)
            {
                _from.SendGump(new CraftQueueAmountGump(_from, _craftSystem, _tool, index));
                return;
            }
        }

        if (id >= 100 && id < 200) // Decrease Volume
        {
            int index = id - 100;
            if (index < queue.Count)
                queue[index].TargetAmount = Math.Max(1, queue[index].TargetAmount - 1);
        }
        else if (id >= 200 && id < 300) // Increase Volume
        {
            int index = id - 200;
            if (index < queue.Count)
                queue[index].TargetAmount++;
        }
        else if (id >= 300 && id < 400) // Line Removal
        {
            int index = id - 300;
            if (index < queue.Count)
            {
                queue.RemoveAt(index);
                if (queue.Count == 0)
                    CraftQueueManager.PauseQueue(_from);
            }
        }

        _from.SendGump(new CraftQueueGump(_from, _craftSystem, _tool));
    }
}
