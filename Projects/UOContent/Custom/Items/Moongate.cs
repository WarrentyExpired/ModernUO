using System;
using Server;
using Server.Items;
using Server.Gumps;
using Server.Mobiles;
using Server.Spells;
using ModernUO.Serialization;

namespace Server.Items
{
    [SerializationGenerator(0)]
    public partial class CustomMoongate : Item
    {
        [Constructible]
        public CustomMoongate() : base(0xF6C)
        {
            Movable = false;
            //Hue = 0;
            Light = LightType.Circle300;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.Player)
                return;

            if (from.InRange(GetWorldLocation(), 1))
            {
                UseGate(from);
            }
            else
            {
                from.SendLocalizedMessage(500446); // That is too far away.
            }
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (m.Player)
            {
                UseGate(m);
            }

            return true;
        }

        public void UseGate(Mobile m)
        {
            if (m.Criminal)
            {
                m.SendLocalizedMessage(1005561, "", 0x22); // Thou'rt a criminal...
                return;
            }

            if (SpellHelper.CheckCombat(m))
            {
                m.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee...
                return;
            }

            m.SendGump(new CustomMoongateGump(m, this));
        }
    }
}
