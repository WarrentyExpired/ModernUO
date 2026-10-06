using ModernUO.Serialization;

namespace Server.Mobiles
{
    [SerializationGenerator(0, false)]
    public partial class LesserHarpy : BaseCreature
    {
        [Constructible]
        public LesserHarpy() : base(AIType.AI_Melee)
        {
            Body = 30;
            BaseSoundID = 402;

            SetStr(96, 120);
            SetDex(86, 110);
            SetInt(51, 75);

            SetHits(38, 42);

            SetDamage(3, 5);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 25, 30);
            SetResistance(ResistanceType.Fire, 10, 20);
            SetResistance(ResistanceType.Cold, 10, 30);
            SetResistance(ResistanceType.Poison, 20, 30);
            SetResistance(ResistanceType.Energy, 10, 20);

            SetSkill(SkillName.MagicResist, 50.1, 65.0);
            SetSkill(SkillName.Tactics, 40.1, 70.0);
            SetSkill(SkillName.Wrestling, 30.1, 60.0);

            Fame = 500;
            Karma = -500;

            VirtualArmor = 28;
        }

        public override string CorpseName => "a lesser harpy corpse";
        public override string DefaultName => "a lesser harpy";

        public override bool CanRummageCorpses => true;
        public override int Meat => 2;
        public override MeatType MeatType => MeatType.Bird;
        public override int Feathers => 20;
        public override bool CanFly => true;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager, 2);
        }

        public override int GetAttackSound() => 916;

        public override int GetAngerSound() => 916;

        public override int GetDeathSound() => 917;

        public override int GetHurtSound() => 919;

        public override int GetIdleSound() => 918;
    }
}
