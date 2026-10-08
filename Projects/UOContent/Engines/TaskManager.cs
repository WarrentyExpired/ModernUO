using System;
using System.Collections.Generic;
using System.Linq;
using Server;
using Server.Commands;
using Server.Engines.Spawners;

namespace Server.Engines.Tasks;

public static class TaskManager
{
    private static Timer _timer30Min;
    private static Timer _timer1Hour;
    private static Timer _timer24Hour;

    public static void Initialize()
    {
        // 30-Minute Recurring Timer
        _timer30Min = Timer.DelayCall(TimeSpan.FromMinutes(30), TimeSpan.FromMinutes(30), () => On30MinuteTick(null));

        // 1-Hour Recurring Timer
        _timer1Hour = Timer.DelayCall(TimeSpan.FromHours(1), TimeSpan.FromHours(1), () => On1HourTick(null));

        // 24-Hour Recurring Timer
        _timer24Hour = Timer.DelayCall(TimeSpan.FromHours(24), TimeSpan.FromHours(24), () => On24HourTick(null));

        // Register developer commands for manual testing
        CommandSystem.Register("ForceTask30", AccessLevel.Developer, e => On30MinuteTick(e.Mobile));
        CommandSystem.Register("ForceTask1H", AccessLevel.Developer, e => On1HourTick(e.Mobile));
        CommandSystem.Register("ForceTask24H", AccessLevel.Developer, e => On24HourTick(e.Mobile));
    }

    // ==========================================
    // 30-MINUTE INTERVAL LOGIC (Trap Reshuffle)
    // ==========================================
    public static void On30MinuteTick(Mobile from = null)
    {
        //RespawnTrapSpawners(from);
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

    private static void RespawnTrapSpawners(Mobile from = null)
    {
        List<RegionSpawner> trapSpawners = World.Items.Values
            .OfType<RegionSpawner>()
            .Where(spawner => spawner.Running && IsTrapSpawner(spawner))
            .ToList();

        foreach (RegionSpawner spawner in trapSpawners)
        {
            spawner.Respawn();
        }

        string logMsg = $"[TaskManager] Successfully respawned {trapSpawners.Count} trap region spawners.";

        if (from != null)
        {
            from.SendMessage(0x35, logMsg);
        }

        Utility.PushColor(ConsoleColor.Green);
        Console.WriteLine(logMsg);
        Utility.PopColor();
    }

    private static bool IsTrapSpawner(RegionSpawner spawner)
    {
        if (spawner.Name != null && spawner.Name.Contains("Trap", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (spawner.Entries != null)
        {
            foreach (var entry in spawner.Entries)
            {
                if (entry.SpawnedName != null && entry.SpawnedName.Contains("Trap", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
        }

        return false;
    }
}
