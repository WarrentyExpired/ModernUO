using ModernUO.Serialization;

namespace Server.Mobiles
{
    [SerializationGenerator(0, false)]
    public partial class LostTroll : BaseCreature
    {
        [Constructible]
        public LostTroll() : base(AIType.AI_Melee)
        {
            Body = Utility.RandomList(53, 54);
            BaseSoundID = 461;

            SetStr(100, 125);
            SetDex(26, 45);
            SetInt(46, 70);

            SetHits(86, 103);

            SetDamage(8, 13);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 25, 35);
            SetResistance(ResistanceType.Cold, 15, 25);
            SetResistance(ResistanceType.Poison, 5, 15);
            SetResistance(ResistanceType.Energy, 5, 15);

            SetSkill(SkillName.MagicResist, 45.1, 60.0);
            SetSkill(SkillName.Tactics, 30.1, 50.0);
            SetSkill(SkillName.Wrestling, 30.1, 50.0);

            Fame = 1500;
            Karma = -1500;

            VirtualArmor = 40;
        }

        public override string CorpseName => "a troll corpse";
        public override string DefaultName => "a lost troll";

        public override bool CanRummageCorpses => true;
        public override int TreasureMapLevel => 1;
        public override int Meat => 2;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
        }
    }
}
