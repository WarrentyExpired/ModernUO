using System;
using System.Text.Json;
using ModernUO.Serialization;
using Server.Engines.Spawners;
using Server.Json;

namespace Server.Custom.Spawners
{
    [SerializationGenerator(0)]
    public partial class PlainsAnimalSpawner : Spawner
    {
        private static readonly string[] _animalTypes =
        {
            "BlackBear", "BrownBear", "GrizzlyBear", "Chicken", "Eagle",
            "GreyWolf", "Bull", "Cow", "Cougar", "Goat", "GreatHart", "Hind",
            "Pig", "Horse", "RidableLlama", "Snake", "Bird", "Mongbat"
        };

        [Constructible]
        public PlainsAnimalSpawner(int amount)
            : base(amount, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(10), 0, default, _animalTypes)
        {
            Name = "Plains Animal Spawner";
        }

        public PlainsAnimalSpawner(DynamicJson json, JsonSerializerOptions options) : base(json, options)
        {
        }
    }
}
