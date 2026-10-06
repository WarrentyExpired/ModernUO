using ModernUO.Serialization;
using Server.Items;

namespace Server.Mobiles
{
    [TypeAlias("Server.Mobiles.BoneMage")]
    [SerializationGenerator(0, false)]
    public partial class DustyBoneMagi : BaseCreature
    {
        [Constructible]
        public DustyBoneMagi() : base(AIType.AI_Mage)
        {
            Body = 148;
            BaseSoundID = 451;

            SetStr(46, 70);
            SetDex(36, 55);
            SetInt(80, 110);

            SetHits(46, 60);

            SetDamage(3, 7);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 35, 40);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 20, 30);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.EvalInt, 30.1, 40.0);
            SetSkill(SkillName.Magery, 40.1, 60.0);
            SetSkill(SkillName.MagicResist, 55.1, 70.0);
            SetSkill(SkillName.Tactics, 45.1, 60.0);
            SetSkill(SkillName.Wrestling, 45.1, 55.0);
            SetSkill(SkillName.Necromancy, 49, 59.1);
            SetSkill(SkillName.SpiritSpeak, 40.0, 59.0);

            Fame = 1200;
            Karma = -1200;

            VirtualArmor = 38;

            PackReg(3);
            PackNecroReg(3, 10);
            PackItem(new Bone());
        }

        public override string CorpseName => "a dusty skeletal corpse";
        public override string DefaultName => "a dusty bone mage";

        public override bool BleedImmune => true;

        public override OppositionGroup OppositionGroup => OppositionGroup.FeyAndUndead;

        public override Poison PoisonImmune => Poison.Regular;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
            AddLoot(LootPack.LowScrolls);
            AddLoot(LootPack.Potions);
        }
    }
}
