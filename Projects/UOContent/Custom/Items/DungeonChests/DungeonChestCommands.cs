using System;
using System.IO;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Items;

namespace Server.Commands.Custom
{
    public class DungeonChestCommands
    {
        private static readonly string ConfigFolder = "Data/Spawners/DungeonChests";
        private static readonly string ExportFolder = Path.Combine(ConfigFolder, "Exported");

        public static void Initialize()
        {
            if (!Directory.Exists(ConfigFolder)) Directory.CreateDirectory(ConfigFolder);
            if (!Directory.Exists(ExportFolder)) Directory.CreateDirectory(ExportFolder);

            CommandSystem.Register("ExportDungeonChests", AccessLevel.GameMaster, new CommandEventHandler(ExportDungeonChests_OnCommand));
            CommandSystem.Register("ImportDungeonChests", AccessLevel.GameMaster, new CommandEventHandler(ImportDungeonChests_OnCommand));
        }

        private static void LogMessage(Mobile m, string text)
        {
            m?.SendMessage(text);
            Console.WriteLine(text);
        }

        [Usage("ExportDungeonChests")]
        public static void ExportDungeonChests_OnCommand(CommandEventArgs e)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string exportFile = Path.Combine(ExportFolder, $"exported-dungeonchests-{timestamp}.cfg");

            using (StreamWriter writer = new StreamWriter(exportFile))
            {
                foreach (Item item in World.Items.Values)
                {
                    if (item is DungeonChestSpawner s)
                    {
                        writer.WriteLine($"{s.ChestLevel},{s.RespawnTime.TotalMinutes},{s.X},{s.Y},{s.Z},{s.Map}");
                    }
                }
            }
            e.Mobile.SendMessage($"Dungeon configuration saved to {exportFile}!");
        }

        [Usage("ImportDungeonChests")]
        public static void ImportDungeonChests_OnCommand(CommandEventArgs e)
        {
            RunImport(e.Mobile);
        }

        public static void RunImport(Mobile from = null)
        {
            string[] files = Directory.GetFiles(ConfigFolder, "*.cfg");

            if (files.Length == 0)
            {
                LogMessage(from, $"Error: No .cfg files found in {ConfigFolder}!");
                return;
            }

            LogMessage(from, "Wiping old dungeon chests...");

            List<Item> toDelete = new List<Item>();
            foreach (Item item in World.Items.Values)
            {
                if (item is DungeonChestSpawner || item is DungeonChest)
                {
                    toDelete.Add(item);
                }
            }

            foreach (Item item in toDelete)
            {
                item.Delete();
            }

            int loaded = 0;
            foreach (string file in files)
            {
                using (StreamReader reader = new StreamReader(file))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] split = line.Split(',');
                        if (split.Length >= 6)
                        {
                            DungeonChestSpawner s = new DungeonChestSpawner(int.Parse(split[0]), double.Parse(split[1]));
                            s.MoveToWorld(new Point3D(int.Parse(split[2]), int.Parse(split[3]), int.Parse(split[4])), Map.Parse(split[5]));
                            loaded++;
                        }
                    }
                }
            }

            LogMessage(from, $"Dungeon reset! Loaded {loaded} spawners from {files.Length} files.");
        }
    }
}
