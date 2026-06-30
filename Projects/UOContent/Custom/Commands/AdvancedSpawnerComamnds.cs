using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Server;
using Server.Commands;
using Server.Custom.Spawners;

namespace Server.Custom.Commands
{
    public class AdvancedSpawnersCommands
    {
        public static void Initialize()
        {
            CommandSystem.Register("ExportAdvancedSpawners", AccessLevel.Administrator, ExportAdvancedSpawners_OnCommand);
            CommandSystem.Register("ImportAdvancedSpawners", AccessLevel.Administrator, ImportAdvancedSpawners_OnCommand);
        }

        [Usage("ExportAdvancedSpawners")]
        [Description("Exports all AdvancedSpawners to Data/Spawners/AdvancedSpawner/Exported/advancespawner-<timestamp>.json")]
        public static void ExportAdvancedSpawners_OnCommand(CommandEventArgs e)
        {
            string exportDir = Path.Combine(Core.BaseDirectory, "Data", "Spawners", "AdvancedSpawner", "Exported");

            if (!Directory.Exists(exportDir))
            {
                Directory.CreateDirectory(exportDir);
            }

            string timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            string fileName = $"advancespawner-{timestamp}.json";
            string fullPath = Path.Combine(exportDir, fileName);

            List<AdvancedSpawnerDTO> exportList = new List<AdvancedSpawnerDTO>();

            foreach (Item item in World.Items.Values)
            {
                if (item is AdvancedSpawner spawner && spawner.Map != null && spawner.Map != Map.Internal)
                {
                    AdvancedSpawnerDTO dto = new AdvancedSpawnerDTO
                    {
                        Map = spawner.Map.Name,
                        X = spawner.X,
                        Y = spawner.Y,
                        Z = spawner.Z,
                        GlobalMaxCount = spawner.GlobalMaxCount,
                        Range = spawner.Range,
                        RadarInterval = spawner.RadarInterval,
                        MinSpawnDelay = spawner.MinSpawnDelay,
                        MaxSpawnDelay = spawner.MaxSpawnDelay,
                        GracePeriod = spawner.GracePeriod,
                        Running = spawner.Running,
                        Entries = new List<AdvancedSpawnerEntryDTO>()
                    };

                    foreach (var entry in spawner.Entries)
                    {
                        dto.Entries.Add(new AdvancedSpawnerEntryDTO
                        {
                            MobName = entry.MobName,
                            MaxCount = entry.MaxCount,
                            Probability = entry.Probability
                        });
                    }

                    exportList.Add(dto);
                }
            }

            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(exportList, options);
                File.WriteAllText(fullPath, jsonString);

                e.Mobile.SendMessage(38, $"Exported {exportList.Count} Advanced Spawners.");
                e.Mobile.SendMessage(38, $"Saved to: Data/Spawners/AdvancedSpawner/Exported/{fileName}");
            }
            catch (Exception ex)
            {
                e.Mobile.SendMessage(33, "Error during export. Check console for details.");
                Console.WriteLine($"[AdvancedSpawner Export Error] {ex.Message}");
            }
        }

        [Usage("ImportAdvancedSpawners")]
        [Description("Imports AdvancedSpawners from all .json files in Data/Spawners/AdvancedSpawner/")]
        public static void ImportAdvancedSpawners_OnCommand(CommandEventArgs e)
        {
            string importDir = Path.Combine(Core.BaseDirectory, "Data", "Spawners", "AdvancedSpawner");

            if (!Directory.Exists(importDir))
            {
                e.Mobile.SendMessage(33, "The import directory does not exist. Nothing to import.");
                return;
            }

            string[] files = Directory.GetFiles(importDir, "*.json");

            if (files.Length == 0)
            {
                e.Mobile.SendMessage(33, "No .json files found in Data/Spawners/AdvancedSpawner/.");
                return;
            }

            int importCount = 0;

            foreach (string file in files)
            {
                try
                {
                    string jsonString = File.ReadAllText(file);
                    List<AdvancedSpawnerDTO> importList = JsonSerializer.Deserialize<List<AdvancedSpawnerDTO>>(jsonString);

                    if (importList == null) continue;

                    foreach (var dto in importList)
                    {
                        Map map = Map.Parse(dto.Map);
                        if (map == null || map == Map.Internal) continue;

                        AdvancedSpawner spawner = new AdvancedSpawner
                        {
                            GlobalMaxCount = dto.GlobalMaxCount,
                            Range = dto.Range,
                            RadarInterval = dto.RadarInterval,
                            MinSpawnDelay = dto.MinSpawnDelay,
                            MaxSpawnDelay = dto.MaxSpawnDelay,
                            GracePeriod = dto.GracePeriod,
                            Running = dto.Running
                        };

                        foreach (var edto in dto.Entries)
                        {
                            spawner.Entries.Add(new AdvancedSpawnerEntry
                            {
                                MobName = edto.MobName,
                                MaxCount = edto.MaxCount,
                                Probability = edto.Probability
                            });
                        }

                        spawner.MoveToWorld(new Point3D(dto.X, dto.Y, dto.Z), map);
                        importCount++;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[AdvancedSpawner Import Error] Failed to process file: {file}");
                    Console.WriteLine($"Exception: {ex.Message}");
                }
            }

            e.Mobile.SendMessage(0x3E, $"Successfully imported {importCount} Advanced Spawners from {files.Length} files.");
        }
    }

    public class AdvancedSpawnerDTO
    {
        public string Map { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int GlobalMaxCount { get; set; }
        public int Range { get; set; }
        public TimeSpan RadarInterval { get; set; }
        public TimeSpan MinSpawnDelay { get; set; }
        public TimeSpan MaxSpawnDelay { get; set; }
        public TimeSpan GracePeriod { get; set; }
        public bool Running { get; set; }
        public List<AdvancedSpawnerEntryDTO> Entries { get; set; }
    }

    public class AdvancedSpawnerEntryDTO
    {
        public string MobName { get; set; }
        public int MaxCount { get; set; }
        public int Probability { get; set; }
    }
}
