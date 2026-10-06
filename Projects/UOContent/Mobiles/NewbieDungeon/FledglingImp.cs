using ModernUO.Serialization;

namespace Server.Mobiles
{
    [SerializationGenerator(0, false)]
    public partial class FledglingImp : BaseCreature
    {
        [Constructible]
        public FledglingImp() : base(AIType.AI_Mage)
        {
            Body = 74;
            BaseSoundID = 422;

            SetStr(71, 85);
            SetDex(41, 60);
            SetInt(66, 85);

            SetHits(55, 70);

            SetDamage(6, 9);

            SetDamageType(ResistanceType.Physical, 0);
            SetDamageType(ResistanceType.Fire, 50);
            SetDamageType(ResistanceType.Poison, 50);

            SetResistance(ResistanceType.Physical, 25, 35);
            SetResistance(ResistanceType.Fire, 40, 50);
            SetResistance(ResistanceType.Cold, 20, 30);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.EvalInt, 20.1, 30.0);
            SetSkill(SkillName.Magery, 50.1, 70.0);
            SetSkill(SkillName.MagicResist, 20.1, 30.0);
            SetSkill(SkillName.Tactics, 42.1, 50.0);
            SetSkill(SkillName.Wrestling, 40.1, 44.0);

            Fame = 1200;
            Karma = -1200;

            VirtualArmor = 30;

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 65.1;
        }

        public override string CorpseName => "a fledgling imp corpse";
        public override string DefaultName => "a fledgling imp";

        public override int Meat => 1;
        public override int Hides => 3;
        public override HideType HideType => HideType.Spined;
        public override FoodType FavoriteFood => FoodType.Meat;
        public override PackInstinct PackInstinct => PackInstinct.Daemon;
        public override bool CanFly => true;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
            AddLoot(LootPack.MedScrolls, 2);
        }
    }
}
