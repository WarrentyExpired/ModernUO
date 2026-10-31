using System;
using Server;
using Server.Items;
using ModernUO.Serialization;

namespace Server.Mobiles
{
    [SerializationGenerator(0, false)]
    public partial class Citizen : BaseCreature
    {
        private DateTime _NextChatter;

        private static readonly string[] _ChatterLines = new string[]
        {
            "Lovely weather we're having today.",
            "I heard the dungeons are more dangerous than usual...",
            "Just another quiet day in town.",
            "I need to visit the provisioner soon.",
            "Have you seen my lost dog?",
            "The guards seem on edge lately."
        };

        // Add your custom titles here!
        private static readonly string[] _FemaleTitles = new string[]
        {
            "the citizen", "the wanderer", "the peasant",
            "the warrior", "the mage", "the archer"
        };

        private static readonly string[] _MaleTitles = new string[]
        {
            "the citizen", "the wanderer", "the peasant",
            "the warrior", "the mage", "the archer"
        };

        [Constructible]
        public Citizen() : base(AIType.AI_Animal, FightMode.None)
        {
            SetSpeed(0.2, 0.4);
            Hue = Race.Human.RandomSkinHue();
            SpeechHue = 48;
            //Blessed = true;

            if (Female = Utility.RandomBool())
            {
                Body = 0x191;
                Name = NameList.RandomName("female");
                Title = _FemaleTitles[Utility.Random(_FemaleTitles.Length)];
            }
            else
            {
                Body = 0x190;
                Name = NameList.RandomName("male");
                Title = _MaleTitles[Utility.Random(_MaleTitles.Length)];
            }

            switch (Title)
            {
                case "the citizen":
                    if (Body == 0x191)
                    {
                        AddItem(new FancyDress(Utility.RandomNeutralHue()));
                        AddItem(new FloppyHat(Utility.RandomNeutralHue()));
                        AddItem(new Sandals());
                    }
                    else
                    {
                        AddItem(new LongPants(Utility.RandomNeutralHue()));
                        AddItem(new FancyShirt(Utility.RandomNeutralHue()));
                        AddItem(new Boots());
                    }
                    break;
                case "the wanderer":
                    AddItem(new QuarterStaff());
                    AddItem(new Doublet(Utility.RandomNeutralHue()));
                    AddItem(new LongPants(Utility.RandomNeutralHue()));
                    AddItem(new BodySash(Utility.RandomNeutralHue()));
                    AddItem(new WideBrimHat(Utility.RandomNeutralHue()));
                    AddItem(new Boots());
                    break;
                case "the peasant":
                    if (Body == 0x191)
                    {
                        AddItem(new Shirt(Utility.RandomNeutralHue()));
                    }
                    AddItem(new ShortPants(Utility.RandomNeutralHue()));
                    AddItem(new SkullCap(Utility.RandomNeutralHue()));
                    AddItem(new Sandals());
                    break;
                case "the warrior":
                    AddItem(new VikingSword());
                    AddItem(new HeaterShield());
                    AddItem(new NorseHelm());
                    AddItem(new PlateGloves());
                    AddItem(new Doublet(Utility.RandomNeutralHue()));
                    AddItem(new PlateLegs());
                    break;
                case "the mage":
                    AddItem(new Spellbook());
                    AddItem(new Robe(Utility.RandomNeutralHue()));
                    AddItem(new WizardsHat(Utility.RandomNeutralHue()));
                    AddItem(new Sandals());
                    break;
                case "the archer":
                    AddItem(new Bow());
                    AddItem(new FeatheredHat(Utility.RandomNeutralHue()));
                    AddItem(new LongPants(Utility.RandomNeutralHue()));
                    AddItem(new Shirt(Utility.RandomNeutralHue()));
                    AddItem(new ThighBoots());
                    AddItem(new BodySash(Utility.RandomNeutralHue()));
                    break;
                default:
                    AddItem(new Shirt(Utility.RandomNeutralHue()));
                    AddItem(new LongPants(Utility.RandomNeutralHue()));
                    AddItem(new Shoes());
                    break;
            }

            Utility.AssignRandomHair(this);

            if (Body != 0x191)
            {
                Utility.AssignRandomFacialHair(this);
            }

            _NextChatter = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(5, 15));
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Deleted)
                return;

            if (DateTime.UtcNow > _NextChatter)
            {
                bool playerNearby = false;
                foreach (Mobile m in GetMobilesInRange(8))
                {
                    if (m != null && m.Player && m.Alive)
                    {
                        playerNearby = true;
                        break;
                    }
                }

                if (playerNearby)
                {
                    string line = _ChatterLines[Utility.Random(_ChatterLines.Length)];
                    Say(line);
                }

                _NextChatter = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(30, 60));
            }
        }

        public Citizen(Serial serial) : base(serial)
        {
        }
    }
}
