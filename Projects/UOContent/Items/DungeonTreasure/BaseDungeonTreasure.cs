using System;
using ModernUO.Serialization;
using Server.Items;

namespace Server.Custom
{
    // ==========================================
    // 1. THE LOW-TIER BAG (Unlocked, Trapped, Tiers 0-3)
    // ==========================================
    [SerializationGenerator(0, false)]
    public partial class DungeonLootBag : BaseDungeonTreasure
    {
        private static readonly (int, int)[] _appearances =
        {
            (0xE76, 0x3D),  // Bag
            (0xE75, 0x3C),  // Backpack
            (0xFFBA, 0x3C), // Backpack
            (0xFFB9, 0x3C), // Backpack
            (0xFFA6, 0x3C), // Backpack
            (0xFFA5, 0x3C), // Backpack
            (0xE79, 0x41)   // Pouch
        };

        [Constructible]
        public DungeonLootBag() : base(0xE76)
        {
            Name = "a discarded loot bag";
            (ItemID, GumpID) = _appearances.RandomElement();

            // Roll until we get a tier between 0 and 3
            int tier;
            do { tier = GetWeightedTier(); } while (tier < 0 || tier > 3);
            MasterTier = tier;

            // Security
            Locked = false;
            SetupTrap();

            // Generate
            GenerateLoot();
        }

        public DungeonLootBag(Serial serial) : base(serial) { }
    }

    // ==========================================
    // 2. THE HIGH-TIER CRATE (Locked, Trapped, Tiers 3-6)
    // ==========================================
    [SerializationGenerator(0, false)]
    public partial class DungeonLootCrate : BaseDungeonTreasure
    {
        private static readonly (int, int)[] _appearances =
        {
            (0x9A9, 0x44), // Small Crate
            (0xE3F, 0x44), // Medium Crate
            (0xE3D, 0x44), // Large Crate
            (0x9A8, 0x4B), // Metal Box
            (0xE43, 0x49), // Wooden Chest
            (0xE77, 0x3E), // Barrel
            (0x9AA, 0x43)  // Wooden Box
        };

        [Constructible]
        public DungeonLootCrate() : base(0x9A9)
        {
            Name = "a secured dungeon crate";
            (ItemID, GumpID) = _appearances.RandomElement();

            // Roll until we get a tier between 3 and 6
            int tier;
            do { tier = GetWeightedTier(); } while (tier < 3 || tier > 6);
            MasterTier = tier;

            // Security
            Locked = true;
            LockLevel = 50 + (MasterTier * 10);
            MaxLockLevel = LockLevel + 25;
            RequiredSkill = LockLevel;
            SetupTrap();

            // Generate
            GenerateLoot();
        }

        public DungeonLootCrate(Serial serial) : base(serial) { }
    }


    // ==========================================
    // 3. THE ABSTRACT BASE BLUEPRINT
    // ==========================================
    [SerializationGenerator(0, false)]
    public abstract partial class BaseDungeonTreasure : LockableContainer
    {
        [SerializableField(0)]
        private int m_MasterTier;

        public BaseDungeonTreasure(int itemID) : base(itemID)
        {
            Movable = false;
            Hue = Utility.Random(1, 1000);
        }

        public BaseDungeonTreasure(Serial serial) : base(serial) { }

        public override bool Decays => true;
        public override bool IsDecoContainer => false;

        private readonly TimeSpan _decayTime = TimeSpan.FromMinutes(Utility.RandomMinMax(45, 60));
        public override TimeSpan DecayTime => _decayTime;

        public override Rectangle2D Bounds
        {
            get
            {
                if (GumpID == 0x3D) return new Rectangle2D(29, 34, 108, 94);
                if (GumpID == 0x3C) return new Rectangle2D(44, 65, 142, 94);
                if (GumpID == 0x41) return new Rectangle2D(35, 38, 110, 78);
                if (GumpID == 0x44) return new Rectangle2D(20, 10, 150, 90);
                if (GumpID == 0x4B) return new Rectangle2D(16, 51, 168, 73);
                if (GumpID == 0x49) return new Rectangle2D(18, 105, 144, 73);
                if (GumpID == 0x3E) return new Rectangle2D(33, 36, 109, 112);
                if (GumpID == 0x9AA) return new Rectangle2D(16, 51, 168, 73);
                return new Rectangle2D(18, 20, 125, 62);
            }
        }

        protected void SetupTrap()
        {
            // Core UO Traps: 1 = Dart, 2 = Poison, 3 = Explosion, 4 = Magic
            TrapType = (TrapType)Utility.RandomMinMax(1, 4);
            TrapPower = 10 + (MasterTier * 15); // Scales damage/poison level with the tier
            TrapLevel = MasterTier;
        }

        protected static int GetWeightedTier()
        {
            var roll = Utility.Random(100);

            if (roll < 25) return 0;
            else if (roll < 55) return 1;
            else if (roll < 75) return 2;
            else if (roll < 90) return 3;
            else if (roll < 96) return 4;
            else if (roll < 99) return 5;
            else return 6;
        }

        private static void GetRandomLevel(int pwr, out int attributeCount, out int min, out int max)
        {
            if (pwr == 1)
            {
                attributeCount = 1;
                min = 10;
                max = 20;
            }
            else if (pwr == 2)
            {
                attributeCount = Utility.RandomMinMax(1, 2);
                min = 20;
                max = 40;
            }
            else if (pwr == 3)
            {
                attributeCount = Utility.RandomMinMax(2, 3);
                min = 30;
                max = 60;
            }
            else if (pwr == 4)
            {
                attributeCount = Utility.RandomMinMax(2, 4);
                min = 40;
                max = 80;
            }
            else if (pwr == 5)
            {
                attributeCount = Utility.RandomMinMax(3, 5);
                min = 60;
                max = 80;
            }
            else if (pwr == 6)
            {
                attributeCount = Utility.RandomMinMax(4, 5);
                min = 60;
                max = 100;
            }
            else
            {
                attributeCount = 1;
                min = 5;
                max = 10;
            }
        }

        private static void GetRandomResists(int pwr, out int phy, out int fire, out int cold, out int poison, out int energy)
        {
            if (pwr == 1)
            {
                phy = Utility.RandomMinMax(1, 3);
                fire = Utility.RandomMinMax(1, 3);
                cold = Utility.RandomMinMax(1, 3);
                poison = Utility.RandomMinMax(1, 3);
                energy = Utility.RandomMinMax(1, 3);
            }
            else if (pwr == 2)
            {
                phy = Utility.RandomMinMax(2, 5);
                fire = Utility.RandomMinMax(2, 5);
                cold = Utility.RandomMinMax(2, 5);
                poison = Utility.RandomMinMax(2, 5);
                energy = Utility.RandomMinMax(2, 5);
            }
            else if (pwr == 3)
            {
                phy = Utility.RandomMinMax(4, 7);
                fire = Utility.RandomMinMax(4, 7);
                cold = Utility.RandomMinMax(4, 7);
                poison = Utility.RandomMinMax(4, 7);
                energy = Utility.RandomMinMax(4, 7);
            }
            else if (pwr == 4)
            {
                phy = Utility.RandomMinMax(6, 10);
                fire = Utility.RandomMinMax(6, 10);
                cold = Utility.RandomMinMax(6, 10);
                poison = Utility.RandomMinMax(6, 10);
                energy = Utility.RandomMinMax(6, 10);
            }
            else if (pwr == 5)
            {
                phy = Utility.RandomMinMax(9, 12);
                fire = Utility.RandomMinMax(9, 12);
                cold = Utility.RandomMinMax(9, 12);
                poison = Utility.RandomMinMax(9, 12);
                energy = Utility.RandomMinMax(9, 12);
            }
            else if (pwr == 6)
            {
                phy = Utility.RandomMinMax(12, 15);
                fire = Utility.RandomMinMax(12, 15);
                cold = Utility.RandomMinMax(12, 15);
                poison = Utility.RandomMinMax(12, 15);
                energy = Utility.RandomMinMax(12, 15);
            }
            else
            {
                phy = 0;
                fire = 0;
                cold = 0;
                poison = 0;
                energy = 0;
            }
        }

        protected void GenerateLoot()
        {
            int dropCount = Utility.RandomMinMax(Math.Max(1, MasterTier -2), MasterTier + 1);

            // Weapons
            for (var i = 0; i < dropCount; i++)
            {
                var weapon = Loot.RandomWeapon();
                int itemPower = Utility.RandomMinMax(0, MasterTier);
                GetRandomLevel(itemPower, out var attributeCount, out var min, out var max);
                BaseRunicTool.ApplyAttributesTo(weapon, attributeCount, min, max);
                DropItem(weapon);
            }

            // Armor or Shield
            for (var i = 0; i < dropCount; i++)
            {
                var armor = Loot.RandomArmorOrShield();

                // Roll #1: Determines the magical properties (Attributes)
                int attributePower = Utility.RandomMinMax(0, MasterTier);
                GetRandomLevel(attributePower, out var attributeCount, out var min, out var max);
                BaseRunicTool.ApplyAttributesTo(armor, attributeCount, min, max);

                // Roll #2: Determines the physical/elemental defense (Resists)
                int resistPower = Utility.RandomMinMax(0, MasterTier);
                GetRandomResists(resistPower, out var phy, out var fire, out var cold, out var poison, out var energy);

                armor.PhysicalBonus = phy;
                armor.FireBonus = fire;
                armor.ColdBonus = cold;
                armor.PoisonBonus = poison;
                armor.EnergyBonus = energy;
                DropItem(armor);
            }

            // Jewelry
            for (var i = 0; i < dropCount; i++)
            {
                var jewelry = Loot.RandomJewelry();
                int itemPower = Utility.RandomMinMax(0, MasterTier);
                GetRandomLevel(itemPower, out var attributeCount, out var min, out var max);
                BaseRunicTool.ApplyAttributesTo(jewelry, attributeCount, min, max);
                DropItem(jewelry);
            }

            // Gems
            for (var i = 0; i < dropCount; i++)
            {
                var gem = Loot.RandomGem();
                gem.Amount = Utility.RandomMinMax(1, dropCount);
                DropItem(gem);
            }

            // Reagents
            for (var i = 0; i < dropCount; i++)
            {
                var mReagent = Loot.RandomReagent();
                mReagent.Amount = Utility.RandomMinMax(1, dropCount);
                DropItem(mReagent);
            }

            for (var i = 0; i < dropCount; i++)
            {
                var nReagent = Loot.RandomNecromancyReagent();
                nReagent.Amount = Utility.RandomMinMax(1, dropCount);
                DropItem(nReagent);
            }

            // Magic Scrolls
            int mlevel = 0;
            if (MasterTier >= 4)
            {
                mlevel = 63;
            }
            else if (MasterTier >= 2)
            {
                mlevel = 46;
            }
            else
            {
                mlevel = 23;
            }
            for (var i = 0; i < dropCount; i++)
            {
                var mscroll = Loot.RandomScroll(0, mlevel, SpellbookType.Regular);
                mscroll.Amount = Utility.RandomMinMax(1, 3);
                DropItem(mscroll);
            }

            int nlevel = 0;
            if (MasterTier < 3)
            {
                nlevel = 7;
            }
            else
            {
                nlevel = 15;
            }
            for (var i = 0; i < dropCount; i++)
            {
                var nscroll = Loot.RandomScroll(0, nlevel, SpellbookType.Necromancer);
                nscroll.Amount = Utility.RandomMinMax(1, 3);
                DropItem(nscroll);
            }

            // Misc extras bandages/arrows/blankscrolls/potions/ETC
            if (Utility.RandomBool())
            {
                DropItem(new Arrow(Utility.RandomMinMax(10, dropCount * 10)));
            }
            else
            {
                DropItem(new Bolt(Utility.RandomMinMax(10, dropCount * 10)));
            }
            if (Utility.RandomBool())
                DropItem(new Bandage(Utility.RandomMinMax(10, dropCount * 10)));
            if (Utility.RandomBool())
                DropItem(new BlankScroll(Utility.RandomMinMax(1, dropCount)));
            for (var i = 0; i < dropCount; i++)
            {
                var potion = Loot.RandomPotion();
                potion.Amount = Utility.RandomMinMax(1, 3);
                DropItem(potion);
            }
            if (Utility.RandomBool())
                DropItem(new RecallRune());

            // Gold
            int minGold = 100 + (MasterTier * 100);
            DropItem(new Gold(Utility.RandomMinMax(minGold, minGold + 99)));
        }
    }
}
