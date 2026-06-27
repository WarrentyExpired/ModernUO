using System;
using System.Text.Json;
using ModernUO.Serialization;
using Server.Engines.Spawners;
using Server.Json;

namespace Server.Custom.Spawners
{
    [SerializationGenerator(0)]
    public partial class WinterAnimalSpawner : Spawner
    {
        private static readonly string[] _animalTypes =
        {
            "PolarBear", "Eagle", "WhiteWolf", "SnowLeopard", "Walrus",
            "MountainGoat", "IceSnake", "JackRabbit", "IceSerpent", "Bird"
        };

        [Constructible]
        public WinterAnimalSpawner(int amount)
            : base(amount, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(10), 0, default, _animalTypes)
        {
            Name = "Winter Animal Spawner";
        }

        public WinterAnimalSpawner(DynamicJson json, JsonSerializerOptions options) : base(json, options)
        {
        }
    }
}
