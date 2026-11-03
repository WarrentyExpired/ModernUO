using System;
using ModernUO.Serialization;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Custom
{
    [SerializationGenerator(0)]
    public partial class ThiefPedestalProp : Item
    {
        [SerializableField(0)]
        private ThiefPedestal m_Pedestal;

        public ThiefPedestalProp(ThiefPedestal pedestal) : base(0xE76)
        {
            m_Pedestal = pedestal;
            Hue = 0x8A5;
            Name = "a tempting bag of loot";
            Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (m_Pedestal != null && !m_Pedestal.Deleted)
            {
                m_Pedestal.OnDoubleClick(from);
            }
        }

        public ThiefPedestalProp(Serial serial) : base(serial) { }
    }

    [SerializationGenerator(0)]
    public partial class ThiefPedestal : Item
    {
        [SerializableField(0)]
        private int m_TrapDifficulty;

        [SerializableField(1)]
        private ThiefPedestalProp m_PropBag;

        // Tracks how many times a player has failed
        [SerializableField(2)]
        private int m_Tries;

        [Constructible]
        public ThiefPedestal() : base(0x32F2)
        {
            Movable = false;
            Name = "an ancient pedestal";

            m_TrapDifficulty = Utility.RandomMinMax(1, 6);
        }

        public void SinkAndDestroy()
        {
            if (Deleted) return;

            Effects.SendLocationEffect(Location, Map, 0x3728, 10);
            Effects.PlaySound(Location, Map, 0x247);
            Delete();
        }

        public override void OnDelete()
        {
            if (m_PropBag != null && !m_PropBag.Deleted)
            {
                m_PropBag.Delete();
            }

            base.OnDelete();
        }

        public override void OnMapChange()
        {
            base.OnMapChange();

            if (Map != null && Map != Map.Internal)
            {
                if (m_PropBag == null || m_PropBag.Deleted)
                {
                    SpawnPropBag();
                }
                else
                {
                    m_PropBag.MoveToWorld(new Point3D(X, Y, Z + 6), Map);
                }
            }
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            base.OnLocationChange(oldLocation);

            if (Map != null && Map != Map.Internal && m_PropBag != null && !m_PropBag.Deleted)
            {
                m_PropBag.MoveToWorld(new Point3D(X, Y, Z + 6), Map);
            }
        }

        public void SpawnPropBag()
        {
            if (m_PropBag != null && !m_PropBag.Deleted) return;
            if (Map == null || Map == Map.Internal) return;

            m_PropBag = new ThiefPedestalProp(this);
            m_PropBag.MoveToWorld(new Point3D(X, Y, Z + 6), Map);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 1))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045);
                return;
            }

            if (m_PropBag == null || m_PropBag.Deleted)
            {
                from.SendMessage("The pedestal is currently empty. The mechanism is resetting.");
                return;
            }

            // Calculate max tries: Base 5 attempts, +1 attempt for every 200 Luck
            int playerLuck = Math.Max(0, from.Luck);
            int maxTries = 5 + (playerLuck / 200);

            if (m_Tries >= maxTries)
            {
                from.SendMessage(33, "Your fingers were not nimble enough and the counterweight slams down! The mechanism destroys the loot.");
                m_PropBag.Delete();
                m_PropBag = null;
                Timer.DelayCall(TimeSpan.FromSeconds(3), SinkAndDestroy);
                return;
            }

            from.SendMessage("You attempt to carefully swap the bag with a counterweight...");

            // 1. Snooping / Trap Check (Linear 0-125 Chance)
            if (!from.CheckSkill(SkillName.Snooping, 0, 125))
            {
                m_Tries++;

                // If Snooping fails, roll RemoveTrap to see if they dodge the explosion/poison
                if (from.CheckSkill(SkillName.RemoveTrap, 0, 100))
                {
                    from.SendMessage(54, "You trigger a pressure plate, but your knowledge of traps allows you to pull your hands back just in time!");
                }
                else
                {
                    from.SendMessage(33, "You miscalculate the exact weight needed for the pressure plate!");
                    TriggerTrap(from);
                }
                return;
            }

            // 2. Stealing Check (Linear 0-125 Chance)
            if (!from.CheckSkill(SkillName.Stealing, 0, 125))
            {
                m_Tries++;
                from.SendMessage(33, "Your hand slips during the swap! You carefully reset your grip to try again.");
                return;
            }

            // 3. Success
            from.SendMessage(89, "In a blur of motion, you swap the bag with a stone! The trap remains silent.");
            from.PlaySound(0x248);

            m_PropBag.Delete();
            m_PropBag = null;

            ThiefPedestalLootBag loot = new ThiefPedestalLootBag();
            from.AddToBackpack(loot);

            Timer.DelayCall(TimeSpan.FromSeconds(3), SinkAndDestroy);
        }

        private void TriggerTrap(Mobile m)
        {
            // Note: The trap no longer deletes the bag! It only punishes the player's health.
            if (Utility.RandomBool())
            {
                m.PlaySound(0x22F);
                Effects.SendLocationEffect(Location, Map, 0x3728, 15);

                Poison trapPoison = m_TrapDifficulty switch
                {
                    1 => Poison.Lesser,
                    2 => Poison.Regular,
                    3 => Poison.Greater,
                    4 => Poison.Deadly,
                    _ => Poison.Lethal
                };

                m.ApplyPoison(m, trapPoison);
                m.SendMessage(33, "A hidden nozzle sprays you with a potent toxin!");
            }
            else
            {
                m.PlaySound(0x207);
                Effects.SendLocationEffect(Location, Map, 0x36BD, 15);

                int damage = Utility.RandomMinMax(20, 35);
                AOS.Damage(m, damage, 100, 0, 0, 0, 0);
                m.SendMessage(33, "The pedestal detonates, blasting you with shrapnel!");
            }
        }

        public ThiefPedestal(Serial serial) : base(serial) { }

        [AfterDeserialization]
        private void AfterDeserialization()
        {
            if (m_PropBag == null || m_PropBag.Deleted)
            {
                SpawnPropBag();
            }
        }
    }
}
