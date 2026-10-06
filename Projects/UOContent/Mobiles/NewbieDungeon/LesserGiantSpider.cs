using ModernUO.Serialization;
using Server.Items;

namespace Server.Mobiles
{
    [SerializationGenerator(0, false)]
    public partial class LesserGiantSpider : BaseCreature
    {
        [Constructible]
        public LesserGiantSpider() : base(AIType.AI_Melee)
        {
            Body = 28;
            BaseSoundID = 0x388;
            Hue = 0x2F0;

            SetStr(46, 60);
            SetDex(36, 55);
            SetInt(36, 60);

            SetHits(36, 50);
            SetMana(0);

            SetDamage(5, 11);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 15, 20);
            SetResistance(ResistanceType.Poison, 25, 35);

            SetSkill(SkillName.Poisoning, 20.1, 50.0);
            SetSkill(SkillName.MagicResist, 25.1, 40.0);
            SetSkill(SkillName.Tactics, 35.1, 50.0);
            SetSkill(SkillName.Wrestling, 30.1, 45.0);

            Fame = 200;
            Karma = -200;

            VirtualArmor = 16;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 49.1;

            PackItem(new SpidersSilk(5));
        }

        public override string CorpseName => "a lesser giant spider corpse";
        public override string DefaultName => "a lesser giant spider";

        public override FoodType FavoriteFood => FoodType.Meat;
        public override PackInstinct PackInstinct => PackInstinct.Arachnid;
        public override Poison PoisonImmune => Poison.Lesser;
        public override Poison HitPoison => Poison.Lesser;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Poor);
        }
    }
}
