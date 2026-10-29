using ModernUO.Serialization;
using Server.Engines.Plants;
using Server.Items;

namespace Server.Mobiles
{
    [SerializationGenerator(0, false)]
    public partial class DarkLion : BaseMount
    {
        public override string DefaultName => "a dark lion";

        [Constructible]
        public DarkLion() : base(1132, 1132, AIType.AI_Melee)
        {

            SetStr(801, 1010);
            SetDex(171, 270);
            SetInt(301, 325);

            SetHits(901, 1100);
            SetMana(60);

            SetDamage(15, 25);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Energy, 50);

            SetResistance(ResistanceType.Physical, 55, 70);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 15, 25);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 60, 70);

            SetSkill(SkillName.Anatomy, 75.1, 80.0);
            SetSkill(SkillName.MagicResist, 85.1, 100.0);
            SetSkill(SkillName.Tactics, 100.1, 110.0);
            SetSkill(SkillName.Wrestling, 100.1, 120.0);

            Fame = 18000;
            Karma = -18000;

            Tamable = true;
            ControlSlots = 4;
            MinTameSkill = 98.7;
        }

        public override string CorpseName => "a dark lion corpse";
        public override double WeaponAbilityChance => 0.07; /* 1 in 15 chance of using per landed hit */

        public override bool StatLossAfterTame => true;

        public override int TreasureMapLevel => 5;
        public override int Meat => 16;
        public override int Hides => 60;
        public override FoodType FavoriteFood => FoodType.Meat;
        public override bool CanAngerOnTame => true;

        private static MonsterAbility[] _abilities = { MonsterAbilities.BloodBathAttack };
        public override MonsterAbility[] GetMonsterAbilities() => _abilities;


        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 3);
            AddLoot(LootPack.Gems, 4);
        }
    }
}
