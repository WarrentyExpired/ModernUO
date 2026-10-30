using System;
using ModernUO.Serialization;
using Server.Items;

namespace Server.Mobiles
{
    [SerializationGenerator(0, false)]
    public partial class EvilGriffon : BaseMount
    {
        public override string DefaultName => "an evil griffon";

        [Constructible]
        public EvilGriffon() : base(1137, 1137, AIType.AI_Melee)
        {
            BaseSoundID = 0x4FE;

            SetStr(220, 270);
            SetDex(130, 150);
            SetInt(70, 90);

            SetHits(200, 240);
            SetDamage(14, 18);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Energy, 50); // Darker variant hits with a bit of energy

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 40, 50);
            SetResistance(ResistanceType.Cold, 20, 30);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 65.0, 80.0);
            SetSkill(SkillName.Tactics, 75.0, 90.0);
            SetSkill(SkillName.Wrestling, 75.0, 90.0);

            Fame = 5000;
            Karma = -5000; // Aggressive

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 85.1;
        }

        public override int Meat => 3;
        public override int Feathers => 36;
        public override FoodType FavoriteFood => FoodType.Meat;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average, 2);
        }
    }
}
