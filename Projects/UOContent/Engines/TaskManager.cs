using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Server;
using Server.Commands;
using Server.Engines.Spawners;
using Server.Items;

namespace Server.Engines.Tasks;

public static class TaskManager
{
    private static Timer _timer30Min;
    private static Timer _timer1Hour;
    private static Timer _timer4Hour;
    private static Timer _timer24Hour;

    public static void Initialize()
    {
        EventSink.ServerStarted += OnServerBoot;
        _timer30Min = Timer.DelayCall(TimeSpan.FromMinutes(30), TimeSpan.FromMinutes(30), () => On30MinuteTick(null));
        _timer1Hour = Timer.DelayCall(TimeSpan.FromHours(1), TimeSpan.FromHours(1), () => On1HourTick(null));
        _timer4Hour = Timer.DelayCall(TimeSpan.FromHours(4), TimeSpan.FromHours(4), () => On4HourTick(null));
        _timer24Hour = Timer.DelayCall(TimeSpan.FromHours(24), TimeSpan.FromHours(24), () => On24HourTick(null));

        CommandSystem.Register("ForceTask30", AccessLevel.Developer, e => On30MinuteTick(e.Mobile));
        CommandSystem.Register("ForceTask1H", AccessLevel.Developer, e => On1HourTick(e.Mobile));
        CommandSystem.Register("ForceTask4H", AccessLevel.Developer, e => On4HourTick(e.Mobile));
        CommandSystem.Register("ForceTask24H", AccessLevel.Developer, e => On24HourTick(e.Mobile));
    }

    // ==========================================
    // 30-MINUTE INTERVAL LOGIC (Trap Reshuffle)
    // ==========================================
    public static void On30MinuteTick(Mobile from = null)
    {
    }

    // ==========================================
    // 1-HOUR INTERVAL LOGIC
    // ==========================================
    public static void On1HourTick(Mobile from = null)
    {
    }

    // ==========================================
    // 4-HOUR INTERVAL LOGIC
    // ==========================================
    public static void On4HourTick(Mobile from = null)
    {
        RespawnPedestals(from);
    }


    // ==========================================
    // 24-HOUR INTERVAL LOGIC
    // ==========================================
    public static void On24HourTick(Mobile from = null)
    {
    }

    // ==========================================
    // RUN ONCE ON SERVER BOOT
    // ==========================================
    private static void OnServerBoot()
    {
        // Check if dungeon traps are disabled globally and if true deletes any spawners and traps.
        if (!Settings.S_DungeonTraps)
        {
            List<Item> toDelete = new List<Item>();
            foreach (Item item in World.Items.Values)
            {
                if (item is BaseTrap)
                {
                    toDelete.Add(item);
                    continue;
                }
                if (item is BaseSpawner spawner)
                {
                    bool spawnsTraps = false;
                    foreach (var entry in spawner.Entries)
                    {
                        if (entry?.SpawnedName != null && entry.SpawnedName.IndexOf("Trap", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            spawnsTraps = true;
                            break;
                        }
                    }
                    if (spawnsTraps)
                    {
                        toDelete.Add(item);
                    }
                }
            }
            int count = toDelete.Count;
            for (int i = 0; i < count; i++)
            {
                if (toDelete[i] != null && !toDelete[i].Deleted)
                {
                    toDelete[i].Delete();
                }
            }
            if (count > 0)
            {
                Console.WriteLine($"TaskManager: S_DungeonTraps is FALSE. Purged {count} physical traps and trap spawners.");
            }
        }
    }

    private static void RespawnPedestals(Mobile from = null)
    {
        string targetFolder = Path.Combine(Core.BaseDirectory, "Data", "Spawns", "Pedestals");
        if (!Directory.Exists(targetFolder))
        {
            Directory.CreateDirectory(targetFolder);
            return;
        }
        string[] jsonFiles = Directory.GetFiles(targetFolder, "*.json", SearchOption.AllDirectories);
        if (jsonFiles.Length == 0)
        {
            return;
        }
        Utility.PushColor(ConsoleColor.Green);
        Console.WriteLine($"[PedestalBootTask] Refreshing {jsonFiles.Length} pedestal spawner file(s)...");
        Utility.PopColor();
        Mobile executer = from ?? World.Mobiles.Values.FirstOrDefault(m => m.AccessLevel >= AccessLevel.GameMaster);
        if (executer == null)
        {
            Console.WriteLine("[TaskManager] Automated pedestal respawn skipped. A GM must be online to execute the [is command.");
            return;
        }
        foreach (string filePath in jsonFiles)
        {
            string relativePath = Path.GetRelativePath(Core.BaseDirectory, filePath);
            CommandSystem.Handle(executer, $"[is {relativePath}");
        }
    }
}
