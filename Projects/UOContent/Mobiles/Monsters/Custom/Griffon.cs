using System;
using ModernUO.Serialization;
using Server.Items;

namespace Server.Mobiles
{
    [SerializationGenerator(0, false)]
    public partial class Griffon : BaseMount
    {
        public override string DefaultName => "a griffon";

        [Constructible]
        public Griffon() : base(1136, 1136, AIType.AI_Melee)
        {
            BaseSoundID = 0x4FE;

            SetStr(200, 250);
            SetDex(120, 140);
            SetInt(60, 80);

            SetHits(180, 220);
            SetDamage(12, 16);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 20, 30);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 60.0, 75.0);
            SetSkill(SkillName.Tactics, 70.0, 85.0);
            SetSkill(SkillName.Wrestling, 70.0, 85.0);

            Fame = 4000;
            Karma = 0; // Neutral

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 80.1;
        }

        public override int Meat => 3;
        public override int Feathers => 36;
        public override FoodType FavoriteFood => FoodType.Meat;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Meager);
        }
    }
}
