using System;
using Server.Mobiles;
using Server.Network;
using ModernUO.Serialization;

namespace Server.Items
{
    [SerializationGenerator(0)]
    public partial class DestinyGate : Item
    {
        [Constructible]
        public DestinyGate() : base(0xF6C) // The classic blue moongate
        {
            Movable = false;
            Name = "Gate to Hesperia";
            Hue = 1161; // Matching your teal/blue theme
            Light = LightType.Circle300;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from is PlayerMobile pm)
            {
                if (!from.InRange(GetWorldLocation(), 2))
                {
                    from.SendLocalizedMessage(500446); // That is too far away.
                    return;
                }

                // THE VALIDATION CHECK
                if (!pm.HasPickedTemplate)
                {
                    from.SendMessage(0x22, "Your destiny remains unwritten. You must embrace a path at the Altar before you can depart.");
                    from.PlaySound(0x5C3); // Ominous failure sound
                    return;
                }

                // SUCCESS: Teleport to your world coordinates
                // Replace these coordinates with your actual world starting point
                from.MoveToWorld(new Point3D(767, 1131, 3), Map.Ilshenar);

                from.PlaySound(0x1FE); // Portal sound
                from.FixedEffect(0x3701, 10, 15); // Teleport spark
                from.SendMessage(0x3F, "You step forth into the world of Hesperia.");
            }
        }

        public DestinyGate(Serial serial) : base(serial) { }
    }
}
