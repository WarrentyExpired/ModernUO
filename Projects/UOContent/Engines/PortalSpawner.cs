using System;
using ModernUO.Serialization;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Custom.Events
{
    [SerializationGenerator(0)]
    public partial class PortalSpawner : Item
    {
        private bool _triggered;

        [Constructible]
        public PortalSpawner() : base(0xFF93)
        {
            Name = "a horrific tear";
            Hue = 1157;
            Movable = false;
            Light = LightType.Circle300;
        }

        public PortalSpawner(Serial serial) : base(serial)
        {
        }

        private bool _zAdjusted;

        public override void OnMapChange()
        {
            base.OnMapChange();

            // Now it checks the moment it officially enters the world
            if (!_zAdjusted && Map != null && Map != Map.Internal)
            {
                _zAdjusted = true;

                // Wait one tick for the spawner to finish snapping it to the floor
                Timer.DelayCall(TimeSpan.Zero, () =>
                {
                    if (!Deleted)
                    {
                        Z += 5; // Adjust as needed
                    }
                });
            }
        }

        public override bool HandlesOnMovement => true;

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            base.OnMovement(m, oldLocation);

            if (_triggered || Deleted)
                return;

            if (m.AccessLevel > AccessLevel.Player || !m.Alive || !(m is PlayerMobile))
                return;

            if (Utility.InRange(Location, m.Location, 8))
            {
                TriggerAmbush(m);
            }
        }

        private void TriggerAmbush(Mobile target)
        {
            _triggered = true;

            Effects.PlaySound(Location, Map, 0x20F);
            Effects.SendLocationEffect(Location, Map, 0x3728, 10, 10);
            Effects.SendLocationEffect(Location, Map, 0x3709, 10, 30, 1153, 0);

            try
            {
                BaseCreature ambushMonster = null;
                string ambushMessage = "A terrifying creature bursts from the portal!";

                switch (Utility.Random(2))
                {
                    case 0:
                        ambushMonster = new PortalDaemon();
                        ambushMessage = "A daemon tears through the portal to ambush you!";
                        break;
                    case 1:
                        ambushMonster = new PortalWyvern();
                        ambushMessage = "A furious dragon bursts through the portal to ambush you!";
                        break;
                }

                if (ambushMonster != null)
                {
                    ambushMonster.MoveToWorld(Location, Map);
                    ambushMonster.PlaySound(ambushMonster.GetAngerSound());

                    ambushMonster.Combatant = target;

                    target.SendMessage(89, ambushMessage);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error spawning portal monster: {e.Message}");
            }

            Delete();
        }

    }
}
