using System;
using Server;
using Server.Mobiles;
using ModernUO.Serialization;

namespace Server.Mobiles
{
    [SerializationGenerator(0, false)]
    public partial class FriendlyTownMonster : BaseCreature
    {
        private DateTime _NextChatter;
        public override bool InitialInnocent => true;

        [Constructible]
        public FriendlyTownMonster() : base(AIType.AI_Animal, FightMode.None)
        {
            Name = NameList.RandomName("orc");
            Title = "the civilized";
            Body = Utility.RandomList(17, 1075, 1072);
            BaseSoundID = 0x45A;

            Karma = 1000;
            Fame = 1000;
            _NextChatter = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(10, 20));
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
                    string[] lines = {
                        "*grunts amicably*",
                        "Me like town. No heroes trying to stab me.",
                        "Guards no chase me here.",
                        "You have shiny coins for trading?"
                    };
                    Say(lines[Utility.Random(lines.Length)]);
                }

                _NextChatter = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(30, 60));
            }
        }

        public override bool IsHarmfulCriminal(Mobile target) => true;

        public FriendlyTownMonster(Serial serial) : base(serial)
        {
        }
    }
}
