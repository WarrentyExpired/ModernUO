using ModernUO.Serialization;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
    [SerializationGenerator(0, false)]
    public partial class ElderlyOrcishMage : BaseCreature
    {
        [Constructible]
        public ElderlyOrcishMage() : base(AIType.AI_Mage)
        {
            Body = 140;
            BaseSoundID = 0x45A;

            SetStr(50, 75);
            SetDex(45, 50);
            SetInt(80, 110);

            SetHits(35, 45);

            SetDamage(4, 10);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 25, 35);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 20, 30);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.EvalInt, 30.1, 35.5);
            SetSkill(SkillName.Magery, 45.1, 62.5);
            SetSkill(SkillName.MagicResist, 30.1, 50.0);
            SetSkill(SkillName.Tactics, 20.1, 40.0);
            SetSkill(SkillName.Wrestling, 22.1, 30.0);

            Fame = 1500;
            Karma = -1500;

            VirtualArmor = 15;

            PackReg(6);

            if (Utility.RandomDouble() < 0.05)
            {
                PackItem(new OrcishKinMask());
            }
        }

        public override string CorpseName => "a glowing orc corpse";
        public override InhumanSpeech SpeechType => InhumanSpeech.Orc;

        public override string DefaultName => "an elderly orcish mage";

        public override bool CanRummageCorpses => true;
        public override int TreasureMapLevel => 1;
        public override int Meat => 1;

        public override OppositionGroup OppositionGroup => OppositionGroup.SavagesAndOrcs;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.LowScrolls);
        }

        public override bool IsEnemy(Mobile m) =>
            (!m.Player || m.FindItemOnLayer<OrcishKinMask>(Layer.Helm) == null) && base.IsEnemy(m);

        public override void AggressiveAction(Mobile aggressor, bool criminal)
        {
            base.AggressiveAction(aggressor, criminal);

            if (aggressor.FindItemOnLayer(Layer.Helm) is OrcishKinMask item)
            {
                AOS.Damage(aggressor, 50, 0, 100, 0, 0, 0);
                item.Delete();
                aggressor.FixedParticles(0x36BD, 20, 10, 5044, EffectLayer.Head);
                aggressor.PlaySound(0x307);
            }
        }
    }
}
