using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class DungeonChest : LockableContainer
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public int Level { get; set; }

        [Constructible]
        public DungeonChest(int treasureLevel) : base(RandomChestType())
        {
            RandomChestName();
            Level = treasureLevel;
            Movable = false;

            if (Level > 0)
            {
                Locked = true;
                LockLevel = Level * 20;
                RequiredSkill = Level * 20;

                if (Utility.RandomDouble() < 0.5)
                {
                    TrapType = TrapType.ExplosionTrap;
                    TrapPower = Level * 15;
                }
            }
        }

        public DungeonChest(Serial serial) : base(serial) { }

        public override void Serialize(IGenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
            writer.Write(Level);
        }

        public override void Deserialize(IGenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            Level = reader.ReadInt();
        }

        private static int RandomChestType()
        {
           return Utility.RandomList(0x9A9, 0xE3F, 0xE3D, 0xE43, 0x9AA, 0x2811, 0x280F);
        }

        private void RandomChestName()
        {
            string[] names = { "a dusty chest", "a worn box", "a lost treasure", "a reinforced trunk" };
            Name = names[Utility.Random(names.Length)];
        }

        public override void Open(Mobile from)
        {
            if (Items.Count == 0)
            {
                GenerateLoot(Level, from);
            }

            base.Open(from);
        }

        private void GenerateLoot(int level, Mobile from)
        {
            LootPack pack = level switch
            {
                1 => LootPack.Meager,
                2 => LootPack.Average,
                3 => LootPack.Rich,
                4 => LootPack.FilthyRich,
                5 => LootPack.UltraRich,
                6 => LootPack.SuperBoss,
                0 or _ => LootPack.Poor
            };

            int baseMin = (level + 1) * 40;
            int baseMax = (level + 1) * 60;
            int goldAmount = Utility.RandomMinMax(baseMin, baseMax) + (from.Luck / 10);

            DropItem(new Gold(goldAmount));
            pack.Generate(from, this, false, from.Luck);
        }
    }
}
