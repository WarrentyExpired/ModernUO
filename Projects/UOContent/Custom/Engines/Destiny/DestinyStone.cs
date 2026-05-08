using System;
using Server.Gumps;
using Server.Mobiles;
using ModernUO.Serialization;

namespace Server.Items
{
    [SerializationGenerator(0)]
    public partial class DestinyStone : Item
    {
        [Constructible]
        public DestinyStone() : base(3805)
        {
            Movable = false;
            Name = "The Altar of Destiny";
            Hue = 1175; // A ghostly white/grey
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from is PlayerMobile pm)
            {
                if (!from.InRange(GetWorldLocation(), 3))
                {
                    from.SendLocalizedMessage(500446); // That is too far away.
                    return;
                }

                pm.CloseGump<DestinyMasterGump>();
                pm.SendGump(new DestinyMasterGump(pm, 0));
            }
        }
        public DestinyStone(Serial serial) : base(serial) { }
    }
}
