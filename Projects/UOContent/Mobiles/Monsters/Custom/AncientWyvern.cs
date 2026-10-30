using ModernUO.Serialization;

namespace Server.Mobiles
{
    [SerializationGenerator(0, false)]
    public partial class AncientWyvern : BaseCreature
    {
        [Constructible]
        public AncientWyvern() : base(AIType.AI_Mage)
        {
            Name = NameList.RandomName("ancient wyvern");
            Body = Utility.RandomList(1119, 1120, 1121, 1122, 1123, 1124, 1125);  //46;
            BaseSoundID = 362;

            SetStr(796, 825);
            SetDex(86, 105);
            SetInt(436, 475);

            SetHits(478, 495);

            SetDamage(16, 22);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Poison, 50);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 50, 60);
            SetResistance(ResistanceType.Cold, 40, 50);
            SetResistance(ResistanceType.Poison, 100 );
            SetResistance(ResistanceType.Energy, 50, 60);

            SetSkill(SkillName.EvalInt, 30.1, 40.0);
            SetSkill(SkillName.Magery, 30.1, 40.0);
            SetSkill(SkillName.Meditation, 52.5, 75.0);
            SetSkill(SkillName.MagicResist, 99.1, 100.0);
            SetSkill(SkillName.Tactics, 97.6, 100.0);
            SetSkill(SkillName.Wrestling, 90.1, 92.5);

            Fame = 15000;
            Karma = -15000;

            VirtualArmor = 60;

            Tamable = true;
            ControlSlots = 3;
            MinTameSkill = 93.9;
        }

        public override string CorpseName => "an ancient wyvern corpse";
        public override string DefaultName => "an ancient wyvern";

        public override bool ReacquireOnMovement => true;
        public override bool AutoDispel => true;
        public override HideType HideType => HideType.Barbed;
        public override int Hides => 40;
        public override int Meat => 19;
        public override int Scales => 12;
        public override ScaleType ScaleType => (ScaleType)Utility.Random(4);
        public override Poison PoisonImmune => Poison.Deadly;
        public override Poison HitPoison => Utility.RandomBool() ? Poison.Regular : Poison.Deadly;
        public override int TreasureMapLevel => 5;
        public override bool CanFly => true;
        public override bool CanAngerOnTame => true;

        private static MonsterAbility[] _abilities = { MonsterAbilities.FireBreath };
        public override MonsterAbility[] GetMonsterAbilities() => _abilities;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
            AddLoot(LootPack.Gems, 3);
        }

        public override int GetIdleSound() => 0x16B;

        public override int GetHurtSound() => 0x16D;
    }
}
