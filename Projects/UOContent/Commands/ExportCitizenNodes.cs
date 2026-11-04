using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Server;
using Server.Commands;
using Server.Items;

namespace Server.Commands
{
    public static class ExportCitizenNodes
    {
        public static void Configure()
        {
            CommandSystem.Register("ExportCitizenNodes", AccessLevel.GameMaster, ExportCitizenNodes_OnCommand);
        }

        [Usage("ExportCitizenNodes")]
        [Aliases("ExportCitizenNodes", "ECN")]
        [Description("Exports all deployed CitizenNodes into a decoration .cfg file format.")]
        private static void ExportCitizenNodes_OnCommand(CommandEventArgs e)
        {
            var mobile = e.Mobile;

            var nodes = World.Items.Values
                .OfType<CitizenNode>()
                .Where(n => !n.Deleted && n.Map != null && n.Map != Map.Internal)
                .ToList();

            if (nodes.Count == 0)
            {
                mobile.SendMessage("No active CitizenNodes were found in the world.");
                return;
            }

            string folderPath = Path.Combine(Core.BaseDirectory, "Data", "Decoration", "Exported");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string filePath = Path.Combine(folderPath, "CitizenNodes.cfg");

            using (var writer = new StreamWriter(filePath, false))
            {
                writer.WriteLine("# Auto-generated CitizenNode export file");
                writer.WriteLine($"# Exported on {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                writer.WriteLine();

                // Group nodes by NodeType so the config file remains clean and organized
                foreach (var group in nodes.GroupBy(n => n.NodeType))
                {
                    int itemID = group.First().ItemID;
                    writer.WriteLine($"CitizenNode 0x{itemID:X} (NodeType={group.Key})");

                    foreach (var node in group)
                    {
                        writer.WriteLine($"{node.X} {node.Y} {node.Z}");
                    }

                    writer.WriteLine();
                }
            }

            mobile.SendMessage($"Successfully exported {nodes.Count} CitizenNode(s) to '{filePath}'.");
        }
    }
}
