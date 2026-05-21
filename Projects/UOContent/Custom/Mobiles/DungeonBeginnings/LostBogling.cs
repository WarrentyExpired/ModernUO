using ModernUO.Serialization;
using Server.Engines.Plants;
using Server.Items;

namespace Server.Mobiles
{
    [SerializationGenerator(0, false)]
    public partial class LostBogling : BaseCreature
    {
        [Constructible]
        public LostBogling() : base(AIType.AI_Melee)
        {
            Body = 779;
            BaseSoundID = 422;

            SetStr(30, 60);
            SetDex(42, 50);
            SetInt(10, 15);

            SetHits(33, 45);

            SetDamage(2, 5);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 20, 25);
            SetResistance(ResistanceType.Fire, 10, 20);
            SetResistance(ResistanceType.Cold, 15, 25);
            SetResistance(ResistanceType.Poison, 15, 25);
            SetResistance(ResistanceType.Energy, 15, 25);

            SetSkill(SkillName.MagicResist, 75.1, 100.0);
            SetSkill(SkillName.Tactics, 55.1, 80.0);
            SetSkill(SkillName.Wrestling, 55.1, 75.0);

            Fame = 450;
            Karma = -450;

            VirtualArmor = 12;

            PackItem(new Log(4));
            PackItem(new Seed());
        }

        public override string CorpseName => "a plant corpse";
        public override string DefaultName => "a lost bogling";

        public override int Hides => 6;
        public override int Meat => 1;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
        }
    }
}
