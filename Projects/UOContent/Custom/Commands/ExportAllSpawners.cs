using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Server.Engines.Spawners;
using Server.Json;

namespace Server.Commands
{
    public static class ExportAllSpawners
    {
        public static void Configure()
        {
            CommandSystem.Register("ExportAllSpawners", AccessLevel.Developer, ExportAllSpawners_OnCommand);
        }

        [Usage("ExportAllSpawners")]
        public static void ExportAllSpawners_OnCommand(CommandEventArgs e)
        {
            var from = e.Mobile;
            var map = from.Map;

            if (map == null || map == Map.Internal) return;

            from.SendMessage($"Using Native Serialization to export {map.Name} spawners...");

            // Use the server's native JSON options (crucial for Rectangle3D/TimeSpan support)
            var options = JsonConfig.GetOptions(new TextDefinitionConverterFactory());
            var spawnRecords = new List<DynamicJson>();
            int count = 0;

            foreach (Item item in World.Items.Values)
            {
                // Only grab spawners on the current map that aren't in containers
                if (item is BaseSpawner spawner && item.Map == map && item.Parent == null)
                {
                    // This is the "Magic" Native Part:
                    // It creates a JSON object that exactly matches the Spawner's internal variables
                    var dynamicJson = DynamicJson.Create(spawner.GetType());
                    spawner.ToJson(dynamicJson, options);

                    spawnRecords.Add(dynamicJson);
                    count++;
                }
            }

            if (count > 0)
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                string fileName = $"native_export_{timestamp}.json";
                string folderPath = Path.Combine(Core.BaseDirectory, "Data", "Spawners");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string path = Path.Combine(folderPath, fileName);

                try
                {
                    // Use the native Serializer with the native options
                    JsonConfig.Serialize(path, spawnRecords, options);
                    from.SendMessage($"{count} spawners exported to Data/Spawners/{fileName}");
                    from.SendMessage("These can be re-imported using [ImportSpawners.");
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
