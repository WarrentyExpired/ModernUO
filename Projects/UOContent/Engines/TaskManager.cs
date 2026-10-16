using System;
using System.Collections.Generic;
using System.Linq;
using Server;
using Server.Commands;
using Server.Engines.Spawners;
using Server.Items;

namespace Server.Engines.Tasks;

public static class TaskManager
{
    private static Timer _timer30Min;
    private static Timer _timer1Hour;
    private static Timer _timer24Hour;

    public static void Initialize()
    {
        EventSink.ServerStarted += OnServerBoot;

        _timer30Min = Timer.DelayCall(TimeSpan.FromMinutes(30), TimeSpan.FromMinutes(30), () => On30MinuteTick(null));

        _timer1Hour = Timer.DelayCall(TimeSpan.FromHours(1), TimeSpan.FromHours(1), () => On1HourTick(null));

        _timer24Hour = Timer.DelayCall(TimeSpan.FromHours(24), TimeSpan.FromHours(24), () => On24HourTick(null));

        CommandSystem.Register("ForceTask30", AccessLevel.Developer, e => On30MinuteTick(e.Mobile));
        CommandSystem.Register("ForceTask1H", AccessLevel.Developer, e => On1HourTick(e.Mobile));
        CommandSystem.Register("ForceTask24H", AccessLevel.Developer, e => On24HourTick(e.Mobile));
    }

    // ==========================================
    // RUN ONCE ON SERVER BOOT
    // ==========================================
    private static void OnServerBoot()
    {
        // 1. Check if dungeon traps are disabled globally
        if (!Settings.S_DungeonTraps)
        {
            List<Item> toDelete = new List<Item>();

            foreach (Item item in World.Items.Values)
            {
                // 2. Mark any physical BaseTrap items on the floor
                if (item is BaseTrap)
                {
                    toDelete.Add(item);
                    continue;
                }

                // 3. Mark any type of Spawner (Standard or RegionSpawner) that spawns traps
                if (item is BaseSpawner spawner)
                {
                    bool spawnsTraps = false;

                    // Loop through the actual SpawnerEntry objects to check their SpawnedName
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

            // 4. Safely delete everything we collected
            int count = toDelete.Count;
            for (int i = 0; i < count; i++)
            {
                if (toDelete[i] != null && !toDelete[i].Deleted)
                {
                    toDelete[i].Delete();
                }
            }

            // Output a console log so you know it ran successfully on boot
            if (count > 0)
            {
                Console.WriteLine($"TaskManager: S_DungeonTraps is FALSE. Purged {count} physical traps and trap spawners.");
            }
        }
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
        // Add hourly tasks here
    }

    // ==========================================
    // 24-HOUR INTERVAL LOGIC
    // ==========================================
    public static void On24HourTick(Mobile from = null)
    {
        // Add daily tasks here
    }
}
