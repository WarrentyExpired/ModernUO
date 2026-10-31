using System;
using Server;
using Server.Items;
using ModernUO.Serialization;

namespace Server.Custom.Dungeons
{
    [SerializationGenerator(0)] // ModernUO Auto-Serialization
    public partial class PulsingBloodClot : Item
    {
        [Constructible] // Spelled with an 'i' for ModernUO
        public PulsingBloodClot() : base(0x122A)
        {
            Name = "Pulsing Blood Clot";
            Weight = 1.0;
            Hue = 1157;
        }

        public PulsingBloodClot(Serial serial) : base(serial)
        {
        }
    }
}
