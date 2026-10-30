using ModernUO.Serialization;
using Server.Items;

namespace Server.Mobiles
{
    [SerializationGenerator(0, false)]
    public partial class HellishNightmare : BaseMount
    {
        public override string DefaultName => "a hellish nightmare";

        [Constructible]
        public HellishNightmare() : base(1131, 1131, AIType.AI_Mage)
        {
            BaseSoundID = Core.AOS ? 0xA8 : 0x16A;

            // Publish 97
            if (Core.TOL)
            {
                if (Utility.RandomDouble() < 0.3)
                {
                    SetStr(296, 315);
                    ControlSlots = 2;
                }
                else
                {
                    SetStr(496, 525);
                    ControlSlots = 3;
                }
            }
            else
            {
                SetStr(696, 725);
                ControlSlots = 3;
            }

            SetDex(116, 145);
            SetInt(116, 145);

            SetHits(498, 715);

            SetDamage(16, 22);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Fire, 65);
            SetDamageType(ResistanceType.Energy, 30);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 60, 70);
            SetResistance(ResistanceType.Cold, 60, 70);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 20, 30);

            SetSkill(SkillName.EvalInt, 40.4, 90.0);
            SetSkill(SkillName.Magery, 40.4, 90.0);
            SetSkill(SkillName.MagicResist, 85.3, 100.0);
            SetSkill(SkillName.Tactics, 97.6, 100.0);
            SetSkill(SkillName.Wrestling, 80.5, 92.5);

            Fame = 18000;
            Karma = -18000;

            VirtualArmor = 60;

            Tamable = true;
            MinTameSkill = 100.1;

            Body = 1131;
            ItemID = 1131;

            PackItem(new SulfurousAsh(Utility.RandomMinMax(3, 5)));
        }

        public override int StepsMax => 6400;
        public override string CorpseName => "a hellish nightmare corpse";
        public override int Meat => 5;
        public override int Hides => 10;
        public override HideType HideType => HideType.Barbed;
        public override FoodType FavoriteFood => FoodType.Meat;
        public override bool CanAngerOnTame => true;

        private static MonsterAbility[] _abilities = { MonsterAbilities.FireBreath };
        public override MonsterAbility[] GetMonsterAbilities() => _abilities;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Average);
            AddLoot(LootPack.LowScrolls);
            AddLoot(LootPack.Potions);
        }

        public override int GetAttackSound() => 0x0AB;
        public override int GetAngerSound() => 0x16A;
        public override int GetDeathSound() => 0x0AC;
        public override int GetIdleSound() => 0x0A8;
        public override int GetHurtSound() => 0x0AA;
    }
}
