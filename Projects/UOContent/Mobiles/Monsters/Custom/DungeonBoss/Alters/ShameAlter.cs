using System;
using Server;
using Server.Items;
using Server.Mobiles;
using ModernUO.Serialization;

namespace Server.Custom.Dungeons
{
    [SerializationGenerator(0)]
    public partial class ShameAlter : BossAlter
    {
        public override Type SummonItem => typeof(PulsingBloodClot);

        public override Type BossType => typeof(CrystallineTitan);

        [Constructible]
        public ShameAlter() : base(0xFFBD)
        {
            Name = "Altar of the Bloodstone Titan";
            Hue = 1157;
        }

        public ShameAlter(Serial serial) : base(serial)
        {
        }
    }
}
