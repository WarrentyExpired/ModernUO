using System;
using Server;
using ModernUO.Serialization;

namespace Server.Items
{
    [SerializationGenerator(0, false)]
    public partial class CampfireHit : Item
    {
        [Constructible]
        public CampfireHit() : base(0xFF8D)
        {
            Name = "a campfire";
            Movable = false;
            Light = LightType.Circle225;
        }

        public CampfireHit(Serial serial) : base(serial)
        {
        }
    }
}
