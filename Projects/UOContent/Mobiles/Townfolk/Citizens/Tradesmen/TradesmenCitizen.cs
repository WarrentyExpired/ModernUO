using System;
using Server;
using Server.Items;
using Server.Mobiles;
using ModernUO.Serialization;

namespace Server.Mobiles
{
    [SerializationGenerator(0, false)]
    public abstract partial class TradesmanCitizen : BaseCreature
    {
        private DateTime _NextChatter;

        public override bool InitialInnocent => true;
        public override bool CanTeach => false;
        public override bool ClickTitle => true;

        [Constructible]
        public TradesmanCitizen() : base(AIType.AI_Animal, FightMode.None)
        {
            SpeechHue = 48;
            Hue = Race.Human.RandomSkinHue();

            if (Utility.RandomBool())
            {
                Body = 0x191; // Female
                Name = NameList.RandomName("female");
            }
            else
            {
                Body = 0x190; // Male
                Name = NameList.RandomName("male");
                Utility.AssignRandomFacialHair(this, HairHue);
            }

            Utility.AssignRandomHair(this);
            Title = "the tradesman";

            SetStr(100);
            SetDex(100);
            SetInt(100);
            SetHits(200, 300);

            DressTradesman();

            CantWalk = true;
            _NextChatter = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(5, 10));
        }

        public virtual void DressTradesman()
        {
            AddItem(new Shirt(Utility.RandomNeutralHue()));
            AddItem(new LongPants(Utility.RandomNeutralHue()));
            AddItem(new Boots(Utility.RandomNeutralHue()));
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Deleted) return;

            // Check timer independently of player movement steps
            if (DateTime.UtcNow >= _NextChatter)
            {
                bool playerNearby = false;
                foreach (Mobile m in GetMobilesInRange(8))
                {
                    if (m != null && m.Player && m.Alive && !m.Hidden)
                    {
                        playerNearby = true;
                        break;
                    }
                }

                if (playerNearby)
                {
                    SayRandomChatter();
                    // Set normal interval between chatter lines
                    _NextChatter = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(15, 30));
                }
                else
                {
                    // Re-check every 5 seconds until a player enters range
                    _NextChatter = DateTime.UtcNow + TimeSpan.FromSeconds(5);
                }
            }
        }

        public virtual void SayRandomChatter()
        {
            string[] lines = {
                "Lovely weather today.",
                "Always so much work to do...",
                "Have you heard any news from the capital?",
                "I need to visit the market later."
            };
            Say(lines[Utility.Random(lines.Length)]);
        }

        public TradesmanCitizen(Serial serial) : base(serial)
        {
        }
    }
}
