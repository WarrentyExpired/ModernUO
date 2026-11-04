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

            CantWalk = false;
            _NextChatter = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(10, 25));
        }

        public virtual void DressTradesman()
        {
            AddItem(new Shirt(Utility.RandomNeutralHue()));
            AddItem(new LongPants(Utility.RandomNeutralHue()));
            AddItem(new Boots(Utility.RandomNeutralHue()));
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            base.OnMovement(m, oldLocation);

            if (m.Player && m.Alive && !m.Hidden && DateTime.UtcNow >= _NextChatter && InRange(m, 8))
            {
                if (Utility.RandomBool())
                {
                    SayRandomChatter();
                }

                _NextChatter = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(30, 60));
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
