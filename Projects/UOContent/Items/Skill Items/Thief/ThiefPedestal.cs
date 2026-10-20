using System;
using ModernUO.Serialization;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Custom
{
    [SerializationGenerator(0)]
    public partial class ThiefPedestal1 : ThiefPedestal { [Constructible] public ThiefPedestal1() : base(1) { } public ThiefPedestal1(Serial serial) : base(serial) { } }

    [SerializationGenerator(0)]
    public partial class ThiefPedestal2 : ThiefPedestal { [Constructible] public ThiefPedestal2() : base(2) { } public ThiefPedestal2(Serial serial) : base(serial) { } }

    [SerializationGenerator(0)]
    public partial class ThiefPedestal3 : ThiefPedestal { [Constructible] public ThiefPedestal3() : base(3) { } public ThiefPedestal3(Serial serial) : base(serial) { } }

    [SerializationGenerator(0)]
    public partial class ThiefPedestal4 : ThiefPedestal { [Constructible] public ThiefPedestal4() : base(4) { } public ThiefPedestal4(Serial serial) : base(serial) { } }

    [SerializationGenerator(0)]
    public partial class ThiefPedestal5 : ThiefPedestal { [Constructible] public ThiefPedestal5() : base(5) { } public ThiefPedestal5(Serial serial) : base(serial) { } }

    [SerializationGenerator(0)]
    public partial class ThiefPedestal6 : ThiefPedestal { [Constructible] public ThiefPedestal6() : base(6) { } public ThiefPedestal6(Serial serial) : base(serial) { } }
    // ==========================================
    // 1. THE CUSTOM BAG
    // ==========================================
    [SerializationGenerator(0)]
    public partial class ThiefPedestalBag : Bag
    {
        // Links this bag to the pedestal that spawned it
        [SerializableField(0)]
        private ThiefPedestal m_Pedestal;

        private Timer m_DecayTimer;

        public ThiefPedestalBag(ThiefPedestal pedestal)
        {
            m_Pedestal = pedestal;
            Hue = 0x8A5;
            Name = "a tempting bag of loot";
            Movable = false;
        }

        // Called when the player successfully heists the entire bag
        public void ClearTether()
        {
            m_Pedestal = null;
            m_DecayTimer?.Stop();
            m_DecayTimer = null;
        }

        // Triggers every time an item is removed from the bag
        public override void OnItemRemoved(Item item)
        {
            base.OnItemRemoved(item); // Match the new single-parameter signature

            // If the bag was already successfully stolen, act like a normal bag
            if (m_Pedestal == null) return;

            // Attempt to find the thief (if it went into their backpack, they are the RootParent)
            Mobile from = item.RootParent as Mobile;

            if (Items.Count == 0)
            {
                // Mechanic 1: The bag is empty. Destroy it immediately.
                if (from != null)
                {
                    from.SendMessage(89, "As you take the last item, the bag crumbles to dust!");
                }

                m_Pedestal.SinkAndDestroy();
                Delete();
            }
            else if (m_DecayTimer == null)
            {
                // Mechanic 2: The bag was disturbed but not emptied. Start a 5-minute decay.
                if (from != null)
                {
                    from.SendMessage(33, "The bag's magical tether has been disturbed! It begins to rapidly decay.");
                }

                m_DecayTimer = Timer.DelayCall(TimeSpan.FromMinutes(1), () =>
                {
                    if (m_Pedestal != null && !Deleted)
                    {
                        m_Pedestal.SinkAndDestroy();
                        Delete();
                    }
                });
            }
        }

        public ThiefPedestalBag(Serial serial) : base(serial) { }
    }


    // ==========================================
    // 2. THE PEDESTAL
    // ==========================================
    [SerializationGenerator(0)]
    public partial class ThiefPedestal : Item
    {
        [SerializableField(0)]
        [SerializedCommandProperty(AccessLevel.GameMaster)]
        private int m_Level;

        [SerializableField(1)]
        private ThiefPedestalBag m_LootBag;

        [Constructible]
        public ThiefPedestal() : this(1) { }

        [Constructible]
        public ThiefPedestal(int level) : base(0x32F2)
        {
            Movable = false;
            m_Level = level;
            Name = "an ancient pedestal";
            //SpawnBag();
        }

        public void SinkAndDestroy()
        {
            if (Deleted) return;

            Effects.SendLocationEffect(Location, Map, 0x3728, 10); // Puff of smoke
            Effects.PlaySound(Location, Map, 0x247); // Sound of grinding stone
            Delete();
        }

        public override void OnDelete()
        {
            // If the pedestal is deleted by a GM or Spawner, clean up the tethered bag
            if (m_LootBag != null && !m_LootBag.Deleted)
            {
                m_LootBag.Delete();
            }

            base.OnDelete();
        }

        public override void OnMapChange()
        {
            base.OnMapChange();

            if (Map != null && Map != Map.Internal)
            {
                if (m_LootBag == null || m_LootBag.Deleted)
                {
                    SpawnBag();
                }
                else
                {
                    // If a GM changes the facet, move the bag with it
                    m_LootBag.MoveToWorld(new Point3D(X, Y, Z + 6), Map);
                }
            }
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            base.OnLocationChange(oldLocation);

            // If a GM uses the [Move command, drag the bag with the pedestal
            if (Map != null && Map != Map.Internal && m_LootBag != null && !m_LootBag.Deleted)
            {
                m_LootBag.MoveToWorld(new Point3D(X, Y, Z + ItemData.Height), Map);
            }
        }

        public void SpawnBag()
        {
            if (m_LootBag != null && !m_LootBag.Deleted) return;
            if (Map == null || Map == Map.Internal) return;
            m_LootBag = new ThiefPedestalBag(this);

            // Fill it with scaling gold and gems
            m_LootBag.DropItem(new Gold(m_Level * 500, m_Level * 1000));

            for (int i = 0; i < m_Level; i++)
            {
                m_LootBag.DropItem(Loot.RandomGem());
            }

            // Move the bag to the exact top of the pedestal dynamically
            m_LootBag.MoveToWorld(new Point3D(X, Y, Z + 6), Map);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 1))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                return;
            }

            if (m_LootBag == null || m_LootBag.Deleted)
            {
                from.SendMessage("The pedestal is currently empty. The mechanism is resetting.");
                return;
            }

            from.SendMessage("You attempt to carefully swap the bag with a counterweight...");

            // --- THE UNIFIED TIER MATH ---
            double minSkill = m_Level switch
            {
                1 => 15.0,
                2 => 35.0,
                3 => 55.0,
                4 => 75.0,
                5 => 90.0,
                6 => 100.1,
                _ => m_Level * 16.7
            };

            double maxSkill = minSkill + 25.0;

            // 1. Snooping Check
            if (from.Skills.Snooping.Value < minSkill)
            {
                from.SendMessage(33, "You don't even understand how this pressure plate works!");
                TriggerTrap(from);
                return;
            }

            if (!from.CheckSkill(SkillName.Snooping, minSkill, maxSkill))
            {
                from.SendMessage(33, "You miscalculate the exact weight needed for the pressure plate!");
                TriggerTrap(from);
                return;
            }

            // 2. Stealing Check
            if (from.Skills.Stealing.Value < minSkill)
            {
                from.SendMessage(33, "Your hands aren't fast enough for a heist of this caliber!");
                TriggerTrap(from);
                return;
            }

            if (!from.CheckSkill(SkillName.Stealing, minSkill, maxSkill))
            {
                from.SendMessage(33, "Your hand slips during the swap!");
                TriggerTrap(from);
                return;
            }

            // 3. Success!
            from.SendMessage(89, "In a blur of motion, you swap the bag with a stone! The trap remains silent.");
            from.PlaySound(0x248);

            m_LootBag.ClearTether(); // Untether the bag so it stops decaying
            m_LootBag.Movable = true;
            from.AddToBackpack(m_LootBag);
            m_LootBag = null;

            Timer.DelayCall(TimeSpan.FromSeconds(3), SinkAndDestroy);
        }

        private void TriggerTrap(Mobile m)
        {
            // 50/50 chance to either deal physical damage or apply poison
            if (Utility.RandomBool())
            {
                // --- POISON TRAP ---
                m.PlaySound(0x22F); // Poison gas sound
                Effects.SendLocationEffect(Location, Map, 0x3728, 15); // Puff of smoke visual

                Poison trapPoison = m_Level switch
                {
                    1 => Poison.Lesser,
                    2 => Poison.Regular,
                    3 => Poison.Greater,
                    4 => Poison.Deadly,
                    _ => Poison.Lethal // Level 5 and 6 apply Lethal poison
                };

                m.ApplyPoison(m, trapPoison);
                m.SendMessage(33, "A hidden nozzle sprays you with a potent toxin!");
            }
            else
            {
                // --- EXPLOSION TRAP ---
                m.PlaySound(0x207); // Explosion sound
                Effects.SendLocationEffect(Location, Map, 0x36BD, 15); // Large explosion visual

                int damage = Utility.RandomMinMax(20, 35);

                // 100% Physical damage to respect UOR armor ratings
                AOS.Damage(m, damage, 100, 0, 0, 0, 0);
                m.SendMessage(33, "The pedestal detonates, blasting you with shrapnel!");
            }

            // Destroy the bag so players cannot just tank the hit and grab the loot
            if (m_LootBag != null && !m_LootBag.Deleted)
            {
                m_LootBag.Delete();
                m_LootBag = null;
                m.SendMessage(33, "The trap's mechanism destroys the loot in the process!");

                Timer.DelayCall(TimeSpan.FromSeconds(3), SinkAndDestroy);
            }
        }

        public ThiefPedestal(Serial serial) : base(serial) { }

        [AfterDeserialization]
        private void AfterDeserialization()
        {
            if (m_LootBag == null || m_LootBag.Deleted)
            {
                SpawnBag();
            }
        }
    }
}
