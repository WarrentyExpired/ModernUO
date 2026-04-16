using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using Server.Collections;
using Server.Items;
using Server.Json;
using Server.Network;

namespace Server.Commands
{
    public struct ThruDoorDefinition
    {
        [JsonPropertyName("src")]
        public WorldLocation Source { get; set; }

        [JsonPropertyName("dst")]
        public WorldLocation Destination { get; set; }

        [JsonPropertyName("id")]
        public int ItemID { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("rules")]
        public int Rules { get; set; }
    }

    public static class GenThruDoor
    {
        private static readonly string ThruDoorJsonPath = Path.Combine(Core.BaseDirectory, "Data/Decoration/thrudoors.json");

        public static void Configure()
        {
            CommandSystem.Register("GenThruDoor", AccessLevel.Developer, ThruGen_OnCommand);
        }

        [Usage("GenThurDoor")]
        public static void ThruGen_OnCommand(CommandEventArgs e)
        {
            var from = e.Mobile;
            if (!File.Exists(ThruDoorJsonPath))
            {
                from.SendMessage($"JSON file not found at {ThruDoorJsonPath}");
                return;
            }

            from.SendMessage("Generating ThruDoors, please wait...");
            int count = 0;

            try
            {
                // CHANGED: Switched JsonUtility.Deserialize to JsonConfig.Deserialize to match your engine
                List<ThruDoorDefinition> defs = JsonConfig.Deserialize<List<ThruDoorDefinition>>(ThruDoorJsonPath);

                if (defs == null)
                {
                    from.SendMessage("Failed to load data: JSON file is empty or invalid.");
                    return;
                }

                foreach (ThruDoorDefinition def in defs)
                {
                    using (var queue = PooledRefQueue<Item>.Create())
                    {
                        foreach (var item in def.Source.Map.GetItemsAt<ThruDoor>(def.Source))
                        {
                            queue.Enqueue(item);
                        }

                        while (queue.Count > 0)
                        {
                            queue.Dequeue().Delete();
                        }
                    }

                    ThruDoor door = new ThruDoor(def.ItemID);
                    door.PointDest = def.Destination;
                    door.MapDest = def.Destination.Map;
                    door.Name = def.Name;
                    door.Rules = def.Rules;

                    door.MoveToWorld(def.Source, def.Source.Map);
                    count++;
                }
                from.SendMessage($"{count} ThruDoors generated successfully.");
            }
            catch (Exception ex)
            {
                from.SendMessage($"Error during generation: {ex.Message}");
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
