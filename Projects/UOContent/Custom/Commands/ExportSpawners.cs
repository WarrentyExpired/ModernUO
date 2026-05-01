using System;
using System.Collections.Generic;
using System.IO;
using Server.Engines.Spawners;
using Server.Json;

namespace Server.Commands
{
    public static class ExportSpawners
    {
        public static void Configure()
        {
            CommandSystem.Register("ExportSpawners", AccessLevel.Developer, ExportSpawners_OnCommand);
        }

        [Usage("ExportSpawners")]
        public static void ExportSpawners_OnCommand(CommandEventArgs e)
        {
            var from = e.Mobile;
            var map = from.Map;

            if (map == null || map == Map.Internal) return;

            from.SendMessage($"Scanning {map.Name} for all spawner settings...");

            var spawnerList = new List<object>();
            int count = 0;

            foreach (Item item in World.Items.Values)
            {
                if (item is Spawner spawner && item.Map == map)
                {
                    var entries = new List<object>();
                    foreach (var entry in spawner.Entries)
                    {
                        entries.Add(new
                        {
                            name = entry.SpawnedName,
                            probability = entry.SpawnedProbability,
                            maxCount = entry.SpawnedMaxCount
                        });
                    }

                    // Define the 3D Box for the export
                    int x, y, z;

                    if (spawner.SpawnBounds != default)
                    {
                        // Use existing precise 3D bounds
                        x = spawner.SpawnBounds.X;
                        y = spawner.SpawnBounds.Y;
                        z = spawner.SpawnBounds.Z;
                    }
                    else
                    {
                        // Fallback to HomeRange calculation
                        // Z defaults to spawner Z, depth defaults to 0 (flat surface check)
                        x = spawner.X - spawner.HomeRange;
                        y = spawner.Y - spawner.HomeRange;
                        z = spawner.Z;

                    }
                    spawnerList.Add(new
                    {
                        type = "Spawner",
                        guid = Guid.NewGuid().ToString(),
                        location = new { x = spawner.X, y = spawner.Y, z = spawner.Z },
                        map = map.Name,
                        count = spawner.Count,
                        homeRange = spawner.HomeRange,
                        spawnPositionMode = spawner.SpawnPositionMode.ToString(),
                        group = spawner.Group,
                        minDelay = spawner.MinDelay,
                        maxDelay = spawner.MaxDelay,
                        team = spawner.Team,
                        isGroup = spawner.Group,
                        useSpiralScan = spawner.UseSpiralScan,
                        spawnLocationIsHome = spawner.SpawnLocationIsHome,
                        maxSpawnAttempts = spawner.MaxSpawnAttempts,
                        name = spawner.Name ?? "Spawner",
                        walkingRange = spawner.WalkingRange,
                        //spawnBounds = new { x1, y1, x2, y2 },
                        spawnBounds = new
                        {
                            x,
                            y,
                            z
                        },
                        entries = entries
                    });
                    count++;
                }
            }

            if (count > 0)
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                string fileName = $"exported_spawners_{timestamp}.json";

                // Define the subfolder path
                string folderPath = Path.Combine(Core.BaseDirectory, "Data", "Spawners");

                // Ensure the folder exists before saving
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string path = Path.Combine(folderPath, fileName);

                try
                {
                    JsonConfig.Serialize(path, spawnerList);
                    from.SendMessage($"{count} spawners exported to Data/Spawners/{fileName}");
                }
                catch (Exception ex)
                {
                    from.SendMessage($"Error saving JSON: {ex.Message}");
                }
            }
            else
            {
                from.SendMessage("No spawners found on this map.");
            }
        }
    }
}
