using System;
using ModernUO.Serialization;

namespace Server.Items
{
    [SerializationGenerator(0)]
    public partial class PassageEssence : Item
    {
        [Constructible]
        public PassageEssence() : base(0x223C) // Scroll Graphic ID
        {
            Name = "Essence of the Passage Guardian";
            Hue = 1161; // Matches your ancient soul aura glow
            Weight = 0.1;
            LootType = LootType.Regular;
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.SendMessage("This primordial matrix belongs inside your Tome of Previous Knowledge.");
        }

        public PassageEssence(Serial serial) : base(serial) { }
    }
}
