using ModernUO.Serialization;

namespace Server.Mobiles
{
    [TypeAlias("Server.Mobiles.Gianttoad")]
    [SerializationGenerator(0, false)]
    public partial class CaveToad : BaseCreature
    {
        [Constructible]
        public CaveToad() : base(AIType.AI_Melee)
        {
            Body = 80;
            BaseSoundID = 0x26B;
            Hue = 0x2A0;

            SetStr(76, 100);
            SetDex(6, 25);
            SetInt(11, 20);

            SetHits(26, 30);
            SetMana(0);

            SetDamage(1, 6);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 20, 25);
            SetResistance(ResistanceType.Fire, 5, 10);
            SetResistance(ResistanceType.Energy, 5, 10);

            SetSkill(SkillName.MagicResist, 15.1, 20.0);
            SetSkill(SkillName.Tactics, 20.1, 30.0);
            SetSkill(SkillName.Wrestling, 20.1, 40.0);

            Fame = 250;
            Karma = 0;

            VirtualArmor = 24;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 29.1;
        }

        public override string CorpseName => "a cave toad corpse";
        public override string DefaultName => "a cave toad";

        public override int Hides => 3;
        //public override HideType HideType => HideType.Normal;
        public override FoodType FavoriteFood => FoodType.Fish | FoodType.Meat;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Poor);
        }
    }
}
