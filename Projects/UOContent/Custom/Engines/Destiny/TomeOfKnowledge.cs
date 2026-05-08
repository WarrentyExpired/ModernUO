using System;
using Server.Gumps;
using Server.Mobiles;
using ModernUO.Serialization;

namespace Server.Items
{
    [SerializationGenerator(0)]
    public partial class TomeOfKnowledge : Item
    {
        [Constructible]
        public TomeOfKnowledge() : base(0x9981)
        {
            Name = "Tome of Previous Knowledge";
            Weight = 1.0;
            LootType = LootType.Blessed;
            Hue = 1161; // Ancient teal
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from is PlayerMobile pm)
            {
                // Basic range check to ensure they have it in their pack or nearby
                if (!from.InRange(GetWorldLocation(), 2))
                {
                    from.SendLocalizedMessage(500446); // That is too far away.
                    return;
                }

                pm.CloseGump<KnowledgeGump>();
                pm.SendGump(new KnowledgeGump(pm));
            }
        }

        public TomeOfKnowledge(Serial serial) : base(serial) { }
    }
}
