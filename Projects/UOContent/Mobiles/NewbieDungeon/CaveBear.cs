using ModernUO.Serialization;

namespace Server.Mobiles
{
    [TypeAlias("Server.Mobiles.Grizzlybear")]
    [SerializationGenerator(0, false)]
    public partial class CaveBear : BaseCreature
    {
        [Constructible]
        public CaveBear() : base(AIType.AI_Animal, FightMode.Aggressor)
        {
            Body = 212;
            BaseSoundID = 0xA3;
            Hue = 0x08FD;

            SetStr(50, 75);
            SetDex(51, 75);
            SetInt(16, 40);

            SetHits(56, 73);
            SetMana(0);

            SetDamage(3, 8);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 25, 35);
            SetResistance(ResistanceType.Cold, 15, 25);
            SetResistance(ResistanceType.Poison, 5, 10);
            SetResistance(ResistanceType.Energy, 5, 10);

            SetSkill(SkillName.MagicResist, 25.1, 40.0);
            SetSkill(SkillName.Tactics, 50.1, 70.0);
            SetSkill(SkillName.Wrestling, 45.1, 70.0);

            Fame = 1000;
            Karma = 0;

            VirtualArmor = 24;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 59.1;
        }

        public override string CorpseName => "a cave bear corpse";
        public override string DefaultName => "a cave bear";

        public override int Meat => 1;
        public override int Hides => 4;
        public override FoodType FavoriteFood => FoodType.Fish | FoodType.FruitsAndVeggies | FoodType.Meat;
        public override PackInstinct PackInstinct => PackInstinct.Bear;
    }
}
