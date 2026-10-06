using ModernUO.Serialization;
using Server.Items;

namespace Server.Mobiles
{
    [SerializationGenerator(0, false)]
    public partial class CowardlyBrigand : BaseCreature
    {
        [Constructible]
        public CowardlyBrigand() : base(AIType.AI_Melee)
        {
            SpeechHue = Utility.RandomDyedHue();
            Title = "the cowardly brigand";
            Hue = Race.Human.RandomSkinHue();

            if (Female = Utility.RandomBool())
            {
                Body = 0x191;
                Name = NameList.RandomName("female");
                AddItem(new Skirt(Utility.RandomNeutralHue()));
            }
            else
            {
                Body = 0x190;
                Name = NameList.RandomName("male");
                AddItem(new ShortPants(Utility.RandomNeutralHue()));
            }

            SetStr(56, 700);
            SetDex(31, 65);
            SetInt(61, 75);

            SetDamage(6, 12);

            SetSkill(SkillName.Fencing, 46.0, 67.5);
            SetSkill(SkillName.Macing, 45.0, 67.5);
            SetSkill(SkillName.MagicResist, 25.0, 47.5);
            SetSkill(SkillName.Swords, 45.0, 67.5);
            SetSkill(SkillName.Tactics, 45.0, 67.5);
            SetSkill(SkillName.Wrestling, 15.0, 37.5);

            Fame = 500;
            Karma = -500;

            AddItem(new Boots(Utility.RandomNeutralHue()));
            AddItem(new FancyShirt());
            AddItem(new Bandana());

            AddItem(
                Utility.Random(7) switch
                {
                    0 => new Longsword(),
                    1 => new Cutlass(),
                    2 => new Broadsword(),
                    3 => new Axe(),
                    4 => new Club(),
                    5 => new Dagger(),
                    _ => new Spear() // 6
                }
            );

            Utility.AssignRandomHair(this);
        }

        public override bool ClickTitle => false;

        public override bool AlwaysMurderer => true;

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.9)
            {
                c.DropItem(new SeveredHumanEars());
            }
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
        }
    }
}
