using ModernUO.Serialization;

namespace Server.Mobiles
{
    /*[TypeAlias(
        "Server.Mobiles.BrownHorse",
        "Server.Mobiles.DirtyHorse",
        "Server.Mobiles.GrayHorse",
        "Server.Mobiles.TanHorse"
    )]*/
    [SerializationGenerator(0, false)]
    public partial class ClydesdaleHorse : BaseMount
    {
        private static readonly int[] m_IDs =
        {
            1140, 1144,
            1141, 1145,
            1142, 1146,
            1143, 1147
        };

        public override string DefaultName => "a clydesdale";

        [Constructible]
        public ClydesdaleHorse() : base(1140, 1143, AIType.AI_Animal, FightMode.Aggressor)
        {
            var random = Utility.Random(4);

            Body = m_IDs[random * 2];
            ItemID = m_IDs[random * 2 + 1];
            //Body = 1140;
            //ItemID = 1144;
            BaseSoundID = 0xA8;

            SetStr(22, 98);
            SetDex(56, 75);
            SetInt(6, 10);

            SetHits(28, 45);
            SetMana(0);

            SetDamage(3, 4);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 15, 20);

            SetSkill(SkillName.MagicResist, 25.1, 30.0);
            SetSkill(SkillName.Tactics, 29.3, 44.0);
            SetSkill(SkillName.Wrestling, 29.3, 44.0);

            Fame = 300;
            Karma = 300;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 29.1;
        }

        public override string CorpseName => "a horse corpse";

        public override int Meat => 3;
        public override int Hides => 10;
        public override FoodType FavoriteFood => FoodType.FruitsAndVeggies | FoodType.GrainsAndHay;
    }
}
