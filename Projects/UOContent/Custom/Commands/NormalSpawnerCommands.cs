using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Server.Engines.Spawners;
using Server.Json;

namespace Server.Commands
{
    public static class ExportNormalSpawner
    {
        public static void Configure()
        {
            CommandSystem.Register("ExportNormalSpawners", AccessLevel.Developer, ExportNormalSpawners_OnCommand);
        }

        [Usage("ExportNormalSpawners")]
        public static void ExportNormalSpawners_OnCommand(CommandEventArgs e)
        {
            var from = e.Mobile;
            var map = from.Map;

            if (map == null || map == Map.Internal) return;

            from.SendMessage($"Using Native Serialization to export {map.Name} spawners...");

            var options = JsonConfig.GetOptions(new TextDefinitionConverterFactory());
            var spawnRecords = new List<DynamicJson>();
            int count = 0;

            foreach (Item item in World.Items.Values)
            {
                if (item is BaseSpawner spawner && item.Map == map && item.Parent == null)
                {
                    var dynamicJson = DynamicJson.Create(spawner.GetType());
                    spawner.ToJson(dynamicJson, options);

                    spawnRecords.Add(dynamicJson);
                    count++;
                }
            }

            if (count > 0)
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                string fileName = $"export_{timestamp}.json";
                string folderPath = Path.Combine(Core.BaseDirectory, "Data", "Spawners", "Normal", "Exported");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string path = Path.Combine(folderPath, fileName);

                try
                {
                    JsonConfig.Serialize(path, spawnRecords, options);
                    from.SendMessage($"{count} spawners exported to Data/Spawners/Normal/Exported/{fileName}");
                    from.SendMessage("These can be re-imported using [ImportNormalSpawner.");
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
