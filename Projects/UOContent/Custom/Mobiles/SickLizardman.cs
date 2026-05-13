using ModernUO.Serialization;
using Server.Misc;

namespace Server.Mobiles
{
    [SerializationGenerator(0, false)]
    public partial class SickLizardman : BaseCreature
    {
        [Constructible]
        public SickLizardman() : base(AIType.AI_Melee)
        {
            //Name = NameList.RandomName("sick lizardman");
            Body = Utility.RandomList(35, 36);
            BaseSoundID = 417;

            SetStr(36, 46);
            SetDex(28, 48);
            SetInt(16, 20);

            SetHits(35, 45);

            SetDamage(2, 5);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 25, 30);
            SetResistance(ResistanceType.Fire, 5, 10);
            SetResistance(ResistanceType.Cold, 5, 10);
            SetResistance(ResistanceType.Poison, 10, 20);

            SetSkill(SkillName.MagicResist, 35.1, 60.0);
            SetSkill(SkillName.Tactics, 55.1, 80.0);
            SetSkill(SkillName.Wrestling, 50.1, 70.0);

            Fame = 700;
            Karma = -700;

            VirtualArmor = 12;
        }

        public override string CorpseName => "a lizardman corpse";
        public override string DefaultName => "a sick lizardman";
        public override InhumanSpeech SpeechType => InhumanSpeech.Lizardman;

        public override bool CanRummageCorpses => true;
        public override int Meat => 1;
        public override int Hides => 12;
        public override HideType HideType => HideType.Spined;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
            // TODO: weapon
        }
    }
}
