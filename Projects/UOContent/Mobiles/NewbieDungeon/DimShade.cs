using ModernUO.Serialization;

namespace Server.Mobiles
{
    [SerializationGenerator(0, false)]
    public partial class DimShade : BaseCreature
    {
        [Constructible]
        public DimShade() : base(AIType.AI_Mage)
        {
            Body = 26;
            Hue = 0x4001;
            BaseSoundID = 0x482;

            SetStr(46, 60);
            SetDex(46, 55);
            SetInt(26, 40);

            SetHits(46, 60);

            SetDamage(6, 9);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Cold, 50);

            SetResistance(ResistanceType.Physical, 25, 30);
            SetResistance(ResistanceType.Cold, 15, 25);
            SetResistance(ResistanceType.Poison, 10, 20);

            SetSkill(SkillName.EvalInt, 35.1, 50.0);
            SetSkill(SkillName.Magery, 35.1, 50.0);
            SetSkill(SkillName.MagicResist, 55.1, 70.0);
            SetSkill(SkillName.Tactics, 45.1, 60.0);
            SetSkill(SkillName.Wrestling, 45.1, 55.0);

            Fame = 2000;
            Karma = -2000;

            VirtualArmor = 28;

            PackReg(10);
        }

        public override string CorpseName => "a ghostly corpse";
        public override string DefaultName => "a dim shade";

        public override bool BleedImmune => true;

        public override OppositionGroup OppositionGroup => OppositionGroup.FeyAndUndead;

        public override Poison PoisonImmune => Poison.Lethal;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
        }
    }
}
