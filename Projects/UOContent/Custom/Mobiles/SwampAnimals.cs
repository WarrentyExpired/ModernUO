using System;
using System.Text.Json;
using ModernUO.Serialization;
using Server.Engines.Spawners;
using Server.Json;

namespace Server.Custom.Spawners
{
    [SerializationGenerator(0)]
    public partial class SwampAnimalSpawner : Spawner
    {
        private static readonly string[] _animalTypes =
        {
            "BrownBear", "Crane", "TimberWolf", "Panther", "Boar",
            "GiantToad", "Gorilla", "Pig", "SwampDragon", "Alligator",
            "GiantSerpent", "Snake", "GiantRat", "SewerRat", "Slime",
            "Bird", "Mongbat"
        };

        [Constructible]
        public SwampAnimalSpawner(int amount)
            : base(amount, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(10), 0, default, _animalTypes)
        {
            Name = "Swamp Animal Spawner";
        }

        public SwampAnimalSpawner(DynamicJson json, JsonSerializerOptions options) : base(json, options)
        {
        }
    }
}
