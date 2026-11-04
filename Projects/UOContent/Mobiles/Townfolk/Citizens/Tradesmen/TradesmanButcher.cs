using System;
using Server;
using Server.Items;
using ModernUO.Serialization;

namespace Server.Mobiles
{
    [SerializationGenerator(0, false)]
    public partial class TradesmanButcher : TradesmanCitizen
    {
        private DateTime _NextWork;
        private ButcherHit _MyMeat;

        [Constructible]
        public TradesmanButcher()
        {
            Title = "the butcher";
            CantWalk = true;

            _NextWork = DateTime.UtcNow + TimeSpan.FromSeconds(2);
        }

        public override void DressTradesman()
        {
            AddItem(new LongPants(Utility.RandomNeutralHue()));
            AddItem(new Boots());

            AddItem(new FullApron(Utility.RandomList(0x906, 0x8A5, 0x89A)));

            Item cleaver = new Cleaver();
            cleaver.Movable = false;
            AddItem(cleaver);
        }

        public override void SayRandomChatter()
        {
            string[] lines = {
                "This should last a while.",
                "AhHh! Hit bone again.",
                "Thankless work, but someone has to do it.",
                "Need some quality ribs?"
            };
            Say(lines[Utility.Random(lines.Length)]);
        }

        public override void OnThink()
        {
            base.OnThink();

            if (DateTime.UtcNow >= _NextWork)
            {
                if (_MyMeat == null || _MyMeat.Deleted)
                {
                    foreach (Item item in GetItemsInRange(1))
                    {
                        if (item is ButcherHit meat)
                        {
                            _MyMeat = meat;
                            break;
                        }
                    }
                }

                if (_MyMeat != null && !_MyMeat.Deleted)
                {
                    _MyMeat.Strike(this);
                }

                _NextWork = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(2, 5));
            }
        }

        public TradesmanButcher(Serial serial) : base(serial)
        {
        }
    }
}
