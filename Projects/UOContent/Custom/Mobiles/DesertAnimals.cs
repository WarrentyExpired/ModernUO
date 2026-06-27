using System;
using System.Text.Json;
using ModernUO.Serialization;
using Server.Engines.Spawners;
using Server.Json;

namespace Server.Custom.Spawners
{
    [SerializationGenerator(0)]
    public partial class DesertAnimalSpawner : Spawner
    {
        private static readonly string[] _animalTypes =
        {
            "Scorpion", "DireWolf", "HellCat", "DesertOstard",
            "FrenziedOstard", "RidgeBack", "LavaSnake", "Bird", "LavaSerpent"
        };

        [Constructible]
        public DesertAnimalSpawner(int amount)
            : base(amount, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(10), 0, default, _animalTypes)
        {
            Name = "Desert Animal Spawner";
        }

        public DesertAnimalSpawner(DynamicJson json, JsonSerializerOptions options) : base(json, options)
        {
        }
    }
}
