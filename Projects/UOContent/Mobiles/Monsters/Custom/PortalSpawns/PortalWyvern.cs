using System;
using Server;
using ModernUO.Serialization;
using Server.Items;

namespace Server.Mobiles
{
    [SerializationGenerator(0, false)]
    public partial class PortalWyvern : AncientWyvern
    {
        private DateTime _DespawnTime;

        [Constructible]
        public PortalWyvern() : base()
        {
            _DespawnTime = DateTime.UtcNow + TimeSpan.FromMinutes(3);
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Deleted) return;

            if (Combatant != null)
            {
                _DespawnTime = DateTime.UtcNow + TimeSpan.FromMinutes(3);
            }
            else if (DateTime.UtcNow > _DespawnTime)
            {
                Effects.SendLocationEffect(Location, Map, 0x3728, 10, 10);
                PlaySound(0x1FD);
                Delete();
            }
        }

        public PortalWyvern(Serial serial) : base(serial)
        {
        }
    }
}
