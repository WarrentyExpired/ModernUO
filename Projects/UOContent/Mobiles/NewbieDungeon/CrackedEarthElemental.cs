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
            SetDex(66, 85);
            SetInt(71, 92);

            SetHits(56, 73);

            SetDamage(9, 16);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 30, 35);
            SetResistance(ResistanceType.Fire, 10, 20);
            SetResistance(ResistanceType.Cold, 10, 20);
            SetResistance(ResistanceType.Poison, 15, 25);
            SetResistance(ResistanceType.Energy, 15, 25);

            SetSkill(SkillName.MagicResist, 50.1, 95.0);
            SetSkill(SkillName.Tactics, 50.1, 80.0);
            SetSkill(SkillName.Wrestling, 40.1, 80.0);

            Fame = 1500;
            Karma = -1500;

            VirtualArmor = 34;

            PackItem(new FertileDirt(Utility.RandomMinMax(1, 4)));
            PackItem(new MandrakeRoot());

            PackItem(new IronOre(2)
            {
                ItemID = 0x19B7
            });
        }

        public override string CorpseName => "an cracked earth elemental corpse";
        public override string DefaultName => "an cracked earth elemental";

        public override bool BleedImmune => true;
        public override int TreasureMapLevel => 1;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Poor);
            AddLoot(LootPack.Meager);
            AddLoot(LootPack.Gems);
        }
    }
}
