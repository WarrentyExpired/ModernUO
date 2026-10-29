using ModernUO.Serialization;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
    [SerializationGenerator(0, false)]
    public partial class SkeletonArcher : BaseCreature
    {
        [Constructible]
        public SkeletonArcher() : base(AIType.AI_Archer)
        {
            Body = 1139;
            BaseSoundID = 0x48D;

            SetStr(106, 140);
            SetDex(76, 101);
            SetInt(80, 120);

            SetHits(108, 138);

            SetDamage(4, 10);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 40, 55);
            SetResistance(ResistanceType.Fire, 10, 20);
            SetResistance(ResistanceType.Cold, 10, 20);
            SetResistance(ResistanceType.Poison, 10, 20);
            SetResistance(ResistanceType.Energy, 10, 20);

            SetSkill(SkillName.Anatomy, 40.2, 80.0);
            SetSkill(SkillName.Archery, 50.1, 75.0);
            SetSkill(SkillName.MagicResist, 35.1, 60.0);
            SetSkill(SkillName.Tactics, 30.1, 55.0);
            SetSkill(SkillName.Wrestling, 50.1, 75.0);

            Fame = 4500;
            Karma = -4500;

            VirtualArmor = 56;

            AddItem(new Bow());
            PackItem(new Arrow(Utility.RandomMinMax(50, 70)));
        }

        public override string CorpseName => "a skeleton archer corpse";
        public override string DefaultName => "a sketeton archer";
        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lesser;

        public override OppositionGroup OppositionGroup => OppositionGroup.FeyAndUndead;
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Poor);
        }
    }
}
