using ModernUO.Serialization;
using Server.Items;

namespace Server.Mobiles
{
    [SerializationGenerator(0, false)]
    public partial class GiantScorpion : BaseCreature
    {
        [Constructible]
        public GiantScorpion() : base(AIType.AI_Melee)
        {
            Body = 48;
            BaseSoundID = 397;

            SetStr(73, 115);
            SetDex(76, 95);
            SetInt(16, 30);

            SetHits(58, 72);

            SetDamage(5, 7);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Poison, 40);

            SetResistance(ResistanceType.Physical, 25, 30);
            SetResistance(ResistanceType.Fire, 10, 15);
            SetResistance(ResistanceType.Cold, 20, 25);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 10, 15);
            SetSkill(SkillName.Poisoning, 80.1);
            SetSkill(SkillName.MagicResist, 30.1);
            SetSkill(SkillName.Tactics, 65.0);
            SetSkill(SkillName.Wrestling, 60.0);

            Fame = 1500;
            Karma = -1500;

            VirtualArmor = 28;

            PackGold(Utility.Random(100, 180));

        }

        public override string CorpseName => "a giant scorpion corpse";
        public override string DefaultName => "a giant scorpion";

        public override int GetAngerSound() => 0x269;

        public override int GetIdleSound() => 0x269;

        public override int GetAttackSound() => 0x186;

        public override int GetHurtSound() => 0x1BE;

        public override int GetDeathSound() => 0x8E;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Gems, Utility.RandomMinMax(1, 2));
        }

        public override bool IsEnemy(Mobile m)
        {
            return base.IsEnemy(m);
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);
        }
    }
}
