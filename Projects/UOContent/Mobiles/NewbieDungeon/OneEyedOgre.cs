using ModernUO.Serialization;
using Server.Items;

namespace Server.Mobiles
{
    [SerializationGenerator(0, false)]
    public partial class OneEyedOgre : BaseCreature
    {
        [Constructible]
        public OneEyedOgre() : base(AIType.AI_Melee)
        {
            Body = 1;
            BaseSoundID = 427;

            SetStr(100, 115);
            SetDex(46, 65);
            SetInt(46, 70);

            SetHits(100, 117);
            SetMana(0);

            SetDamage(9, 11);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 30, 35);
            SetResistance(ResistanceType.Fire, 15, 25);
            SetResistance(ResistanceType.Cold, 15, 25);
            SetResistance(ResistanceType.Poison, 15, 25);
            SetResistance(ResistanceType.Energy, 25);

            SetSkill(SkillName.MagicResist, 55.1, 70.0);
            SetSkill(SkillName.Tactics, 40.1, 50.0);
            SetSkill(SkillName.Wrestling, 40.1, 60.0);

            Fame = 1500;
            Karma = -1500;

            VirtualArmor = 32;

            PackItem(new Club());
        }

        public override string CorpseName => "an ogre corpse";
        public override string DefaultName => "a one eyed ogre";

        public override bool CanRummageCorpses => true;
        public override int TreasureMapLevel => 1;
        public override int Meat => 2;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
            AddLoot(LootPack.Potions);
        }
    }
}
