using ModernUO.Serialization;
using Server.Items;

namespace Server.Mobiles
{
    [SerializationGenerator(0, false)]
    public partial class FrailSkeleton : BaseCreature
    {
        [Constructible]
        public FrailSkeleton() : base(AIType.AI_Melee)
        {
            Body = Utility.RandomList(50, 56);
            BaseSoundID = 0x48D;

            SetStr(15, 20);
            SetDex(26, 35);
            SetInt(6, 10);

            SetHits(24, 38);

            SetDamage(1, 5);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 15, 20);
            SetResistance(ResistanceType.Fire, 5, 10);
            SetResistance(ResistanceType.Cold, 25, 40);
            SetResistance(ResistanceType.Poison, 25, 35);
            SetResistance(ResistanceType.Energy, 5, 15);

            SetSkill(SkillName.MagicResist, 45.1, 60.0);
            SetSkill(SkillName.Tactics, 45.1, 60.0);
            SetSkill(SkillName.Wrestling, 45.1, 55.0);

            Fame = 200;
            Karma = -200;

            VirtualArmor = 4;

            switch (Utility.Random(5))
            {
                case 0:
                    {
                        PackItem(new BoneArms());
                        break;
                    }
                case 1:
                    {
                        PackItem(new BoneChest());
                        break;
                    }
                case 2:
                    {
                        PackItem(new BoneGloves());
                        break;
                    }
                case 3:
                    {
                        PackItem(new BoneLegs());
                        break;
                    }
                case 4:
                    {
                        PackItem(new BoneHelm());
                        break;
                    }
            }
        }

        public override string CorpseName => "a skeletal corpse";
        public override string DefaultName => "a frail skeleton";

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lesser;

        public override OppositionGroup OppositionGroup => OppositionGroup.FeyAndUndead;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Poor);
        }
    }
}
