using System;
using Server;
using Server.Items;
using ModernUO.Serialization;
using Server.Misc;
using Server.Custom;

namespace Server.Mobiles
{
    [SerializationGenerator(0, false)]
    public partial class Citizen : BaseCreature
    {
        private DateTime _NextChatter;

        private static readonly string[] _ChatterLines = new string[]
        {
            // General
            "Lovely weather we're having today.",
            "I need to visit the provisioner soon.",
            "Have you seen my lost dog?",
            "The guards seem on edge lately.",
            "Just another quiet day in town.",

            // Real Dungeon
            "I heard the {rdungeon} is more dangerous than usual...",
            "Stay away from {rdungeon}, I lost a good friend there.",

            // Real Town
            "Did you hear the recent news from {rtown}?",

            // Fake Dungeon
            "Did you hear that {fdungeon} has been destroyed?",

            // Fake Town
            "I'm thinking of packing up and moving to {ftown}.",

            // Active Dungeon of the Day
            "I saw a huge group of adventurers heading toward {activedungeon} this morning.",
            "They say the magical energies are shifting around {activedungeon} today."
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

                    if (line.Contains("{rdungeon}"))
                        line = line.Replace("{rdungeon}", Server.Misc.RandomThings.GetRandomRealDungeon());

                    if (line.Contains("{rtown}"))
                        line = line.Replace("{rtown}", Server.Misc.RandomThings.GetRandomRealTown());

                    if (line.Contains("{fdungeon}"))
                        line = line.Replace("{fdungeon}", Server.Misc.RandomThings.GetRandomFakeDungeon());

                    if (line.Contains("{ftown}"))
                        line = line.Replace("{ftown}", Server.Misc.RandomThings.GetRandomFakeTown());

                    if (line.Contains("{activedungeon}"))
                    {
                        string dotd = Server.Custom.DailyDungeonManager.ActiveDungeon;

                        if (string.IsNullOrEmpty(dotd))
                            dotd = "one of the local dungeons";

                        line = line.Replace("{activedungeon}", dotd);
                    }

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
