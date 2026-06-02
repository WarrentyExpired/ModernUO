using ModernUO.Serialization;
using Server.Items;

namespace Server.Mobiles
{
    [SerializationGenerator(0, false)]
    public partial class CrackedEarthElemental : BaseCreature
    {
        [Constructible]
        public CrackedEarthElemental() : base(AIType.AI_Melee)
        {
            Body = 14;
            BaseSoundID = 268;

            SetStr(75, 100);
            SetDex(30, 55);
            SetInt(31, 72);

            SetHits(46, 73);

            SetDamage(5, 13);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 30, 35);
            SetResistance(ResistanceType.Fire, 10, 20);
            SetResistance(ResistanceType.Cold, 10, 20);
            SetResistance(ResistanceType.Poison, 15, 25);
            SetResistance(ResistanceType.Energy, 15, 25);

            SetSkill(SkillName.MagicResist, 50.1, 95.0);
            SetSkill(SkillName.Tactics, 30.1, 50.0);
            SetSkill(SkillName.Wrestling, 30.1, 50.0);

            Fame = 1500;
            Karma = -1500;

            VirtualArmor = 15;

            PackItem(new FertileDirt(Utility.RandomMinMax(1, 4)));
            PackItem(new MandrakeRoot());

            PackItem(new IronOre(5)
            {
                ItemID = 0x19B7
            });
        }

        public override string CorpseName => "an earth elemental corpse";
        public override string DefaultName => "a cracked earth elemental";

        public override bool BleedImmune => true;
        public override int TreasureMapLevel => 1;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Meager);
            AddLoot(LootPack.Gems);
        }
    }
}
