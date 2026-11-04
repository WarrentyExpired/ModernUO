using System;
using Server;
using Server.Mobiles;
using ModernUO.Serialization;

namespace Server.Mobiles
{
    [SerializationGenerator(0, false)]
    public partial class MonsterCitizen : BaseCreature
    {
        private DateTime _NextChatter;
        public override bool InitialInnocent => true;

        private static readonly (int Body, string NameList, int SoundID)[] MonsterProfiles =
        {
            (17, "orc", 0x45A),             // Orc
            (1077, "ratman", 437),          // Ratman
            (35, "lizardman", 417),         // Lizardman 1
            (36, "lizardman", 417),         // Lizardman 2
            (4, "gargoyle vendor", 372),    // Gargoyle
            (7, "orc", 0x45A),              // Orc Captain
            (26, "ghost", 0x482)            // Civilized Shade
        };

        [Constructible]
        public MonsterCitizen() : base(AIType.AI_Animal, FightMode.None)
        {
            Title = "the civilized";
            var profile = MonsterProfiles[Utility.Random(MonsterProfiles.Length)];
            Body = profile.Body;
            Name = NameList.RandomName(profile.NameList);
            BaseSoundID = profile.SoundID;
            Karma = 1000;
            Fame = 1000;
            _NextChatter = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(5, 15));
        }

        public override void OnThink()
        {
            base.OnThink();
            if (Deleted) return;

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
                    string[] lines = GetChatterLines();
                    Say(lines[Utility.Random(lines.Length)]);
                }

                _NextChatter = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(10, 15));
            }
        }

        private string[] GetChatterLines()
        {
            switch ((int)Body)
            {
                case 17: // Orc
                case 7:  // Orc Captain
                    return new string[] {
                        "*grunts amicably*",
                        "Me like town. No heroes trying to stab me.",
                        "Guards no chase me here.",
                        "You have shiny coins for trading?"
                    };
                case 1077: // Ratman
                    return new string[] {
                        "*sniffs the air*",
                        "Squeak! The cheese here is much better.",
                        "Shiny things... you have shiny things?",
                        "No traps in this town, yes-yes."
                    };
                case 35: // Lizardman 1
                case 36: // Lizardman 2
                    return new string[] {
                        "*hisses softly*",
                        "Sun feels warm on scales here.",
                        "Swamp is cold. Town is good.",
                        "Sssstay a while."
                    };
                case 4: // Gargoyle
                    return new string[] {
                        "To seek knowledge is the path of wisdom.",
                        "The humans here are... tolerable.",
                        "Order must be maintained.",
                        "Do you require my assistance?"
                    };
                case 26: // Ghost
                    return new string[] {
                        "*wails softly*",
                        "So cold... but peaceful.",
                        "Do you remember the old days?",
                        "I just watch the living scurry about."
                    };
                default:
                    return new string[] {
                        "Lovely weather we're having today.",
                        "Just another quiet day in town."
                    };
            }
        }

        public override bool IsHarmfulCriminal(Mobile target) => true;

        public MonsterCitizen(Serial serial) : base(serial)
        {
        }
    }
}
