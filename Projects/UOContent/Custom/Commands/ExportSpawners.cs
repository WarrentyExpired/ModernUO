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

                    int x1, y1, x2, y2;
                    if (spawner.SpawnBounds != default)
                    {
                        x1 = spawner.SpawnBounds.X;
                        y1 = spawner.SpawnBounds.Y;
                        x2 = spawner.SpawnBounds.X + spawner.SpawnBounds.Width;
                        y2 = spawner.SpawnBounds.Y + spawner.SpawnBounds.Height;
                    }
                    else
                    {
                        x1 = spawner.X - spawner.HomeRange;
                        y1 = spawner.Y - spawner.HomeRange;
                        x2 = spawner.X + spawner.HomeRange;
                        y2 = spawner.Y + spawner.HomeRange;
                    }

                    spawnerList.Add(new
                    {
                        type = "Spawner",
                        guid = Guid.NewGuid().ToString(),
                        location = new { x = spawner.X, y = spawner.Y, z = spawner.Z },
                        map = map.Name,
                        count = spawner.Count,
                        group = spawner.Group,
                        minDelay = spawner.MinDelay,
                        maxDelay = spawner.MaxDelay,
                        useSpiralScan = spawner.UseSpiralScan, // Added this field
                        entries = entries,
                        name = spawner.Name ?? "Spawner",
                        walkingRange = spawner.WalkingRange,
                        maxSpawnAttempts = 0,
                        spawnBounds = new { x1, y1, x2, y2 }
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
