using System;
using ModernUO.Serialization;
using Server.Items;

namespace Server.Mobiles
{
    [SerializationGenerator(0, false)]
    public partial class SmolderingLion : BaseCreature
    {
        public override string DefaultName => "a smoldering lion";

        [Constructible]
        public SmolderingLion() : base(AIType.AI_Melee, FightMode.Closest)
        {
            Body = 1128;
            BaseSoundID = 0x69;

            SetStr(250, 300);
            SetDex(100, 150);
            SetInt(50, 100);

            SetHits(150, 2000);
            SetDamage(8, 14);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Fire, 50);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 60, 70);
            SetResistance(ResistanceType.Cold, 20, 30);
            SetResistance(ResistanceType.Poison, 10, 20);
            SetResistance(ResistanceType.Energy, 20, 30);

            SetSkill(SkillName.MagicResist, 50.0, 70.0);
            SetSkill(SkillName.Tactics, 60.0, 80.0);
            SetSkill(SkillName.Wrestling, 60.0, 80.0);

            Fame = 3500;
            Karma = -3500;

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 75.1;

            PackItem(new SulfurousAsh(Utility.RandomMinMax(2, 4)));
        }

        public override string CorpseName => "a smoldering lion corpse";
        public override bool ReacquireOnMovement => true;
        public override int Meat => 2;
        public override int Hides => 10;
        public override HideType HideType => HideType.Spined;
        public override FoodType FavoriteFood => FoodType.Meat;
        public override PackInstinct PackInstinct => PackInstinct.Feline;

        private static MonsterAbility[] _abilities = { MonsterAbilities.FireBreath };
        public override MonsterAbility[] GetMonsterAbilities() => _abilities;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Meager);
        }
    }
}
