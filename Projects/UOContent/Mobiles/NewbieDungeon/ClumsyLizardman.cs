using ModernUO.Serialization;
using Server.Misc;

namespace Server.Mobiles
{
    [SerializationGenerator(0, false)]
    public partial class ClumsyLizardman : BaseCreature
    {
        [Constructible]
        public ClumsyLizardman() : base(AIType.AI_Melee)
        {
            Name = NameList.RandomName("lizardman");
            Body = Utility.RandomList(35, 36);
            BaseSoundID = 417;

            SetStr(56, 90);
            SetDex(36, 43);
            SetInt(36, 60);

            SetHits(48, 62);

            SetDamage(5, 7);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 25, 30);
            SetResistance(ResistanceType.Fire, 5, 10);
            SetResistance(ResistanceType.Cold, 5, 10);
            SetResistance(ResistanceType.Poison, 10, 20);

            SetSkill(SkillName.MagicResist, 35.1, 60.0);
            SetSkill(SkillName.Tactics, 35.1, 50.0);
            SetSkill(SkillName.Wrestling, 30.1, 50.0);

            Fame = 750;
            Karma = -750;

            VirtualArmor = 28;
        }

        public override string CorpseName => "a clumsy lizardman corpse";
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
