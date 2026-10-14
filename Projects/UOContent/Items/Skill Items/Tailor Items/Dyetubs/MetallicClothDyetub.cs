using ModernUO.Serialization;

namespace Server.Items
{
    [SerializationGenerator(0, false)]
    public partial class MetallicClothDyetub : DyeTub
    {
        [Constructible]
        public MetallicClothDyetub()
        {
            LootType = LootType.Blessed;

            // Assign your custom string name here
            Name = "a metallic cloth dyetub";
        }

        // The LabelNumber override has been removed so your custom Name displays correctly.

        public override bool MetallicHues => true;
    }
}
