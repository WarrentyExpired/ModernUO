using System;
using Server;
using ModernUO.Serialization;
using Server.Items;

namespace Server.Mobiles
{
    [SerializationGenerator(0, false)]
    public partial class PortalDaemon : Daemon
    {
        // Removed [Save] - it will now self-clean automatically on server restarts!
        private DateTime _DespawnTime;

        [Constructible]
        public PortalDaemon() : base()
        {
            // Give it 3 minutes of inactivity before it despawns
            _DespawnTime = DateTime.UtcNow + TimeSpan.FromMinutes(3);
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Deleted) return;

            if (Combatant != null)
            {
                // As long as it is fighting a player or pet, constantly reset the clock
                _DespawnTime = DateTime.UtcNow + TimeSpan.FromMinutes(3);
            }
            else if (DateTime.UtcNow > _DespawnTime)
            {
                // The clock ran out and it has no combatant. Clean it up!
                Effects.SendLocationEffect(Location, Map, 0x3728, 10, 10); // Smoke puff
                PlaySound(0x1FD); // Eerie fade out sound
                Delete();
            }
        }

        public PortalDaemon(Serial serial) : base(serial)
        {
        }
    }
}
