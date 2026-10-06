using ModernUO.Serialization;
using Server.Misc;

namespace Server.Mobiles
{
    [SerializationGenerator(0, false)]
    public partial class SicklyRatmanMage : BaseCreature
    {
        [Constructible]
        public SicklyRatmanMage() : base(AIType.AI_Mage)
        {
            Name = NameList.RandomName("ratman");
            Body = 0x8F;
            BaseSoundID = 437;

            SetStr(906, 130);
            SetDex(50, 70);
            SetInt(100, 130);

            SetHits(88, 108);

            SetDamage(4, 11);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 40, 45);
            SetResistance(ResistanceType.Fire, 10, 20);
            SetResistance(ResistanceType.Cold, 10, 20);
            SetResistance(ResistanceType.Poison, 10, 20);
            SetResistance(ResistanceType.Energy, 10, 20);

            SetSkill(SkillName.EvalInt, 30.1, 60.0);
            SetSkill(SkillName.Magery, 40.1, 65.0);
            SetSkill(SkillName.MagicResist, 65.1, 90.0);
            SetSkill(SkillName.Tactics, 30.1, 55.0);
            SetSkill(SkillName.Wrestling, 30.1, 55.0);

            Fame = 2500;
            Karma = -2500;

            VirtualArmor = 44;

            PackReg(6);

            if (Utility.RandomDouble() < 0.02)
            {
                PackStatue();
            }
        }

        public override string CorpseName => "a sickly ratman mage corpse";
        public override InhumanSpeech SpeechType => InhumanSpeech.Ratman;

        public override bool CanRummageCorpses => true;
        public override int Meat => 1;
        public override int Hides => 4;
        //public override HideType HideType => HideType.Spined;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.LowScrolls);
        }
    }
}
