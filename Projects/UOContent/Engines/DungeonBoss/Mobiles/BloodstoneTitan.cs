using ModernUO.Serialization;

namespace Server.Mobiles
{
    [SerializationGenerator(0, false)]
    public partial class BloodstoneTitan : BaseCreature
    {
        [Constructible]
        public BloodstoneTitan() : base(AIType.AI_Mage)
        {
            Body = 829;
            Hue = 1157;
            BaseSoundID = 278;

            SetStr(1026, 1115);
            SetDex(80, 100);
            SetInt(550, 750);

            SetHits(1200, 1300);

            SetDamage(27, 37);

            SetDamageType(ResistanceType.Physical, 40);
            SetDamageType(ResistanceType.Poison, 30);
            SetDamageType(ResistanceType.Energy, 30);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 40, 50);
            SetResistance(ResistanceType.Poison, 50, 60);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.EvalInt, 99.1, 110.0);
            SetSkill(SkillName.Magery, 99.1, 110.0);
            SetSkill(SkillName.Meditation, 80.4, 100.0);
            SetSkill(SkillName.MagicResist, 80.1, 95.0);
            SetSkill(SkillName.Tactics, 99.1, 100.0);
            SetSkill(SkillName.Wrestling, 99.1, 100.0);

            Fame = 32000;
            Karma = -32000;

            VirtualArmor = 60;
        }

        public override string CorpseName => "a bloodstone corpse";
        public override string DefaultName => "a bloodstone titan";

        public override int TreasureMapLevel => 5;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.SuperBoss, 2);
            AddLoot(LootPack.SuperBoss, 2);
        }
    }
}
