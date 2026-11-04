using System;
using Server;
using Server.Items;
using ModernUO.Serialization;

namespace Server.Mobiles
{
    [SerializationGenerator(0, false)]
    public partial class TradesmanSmith : TradesmanCitizen
    {
        private DateTime _NextWork;
        private AnvilHit _MyAnvil;

        [Constructible]
        public TradesmanSmith()
        {
            Title = "the blacksmith";
            CantWalk = true;

            _NextWork = DateTime.UtcNow + TimeSpan.FromSeconds(2);
        }

        public override void DressTradesman()
        {
            AddItem(new LongPants(Utility.RandomNeutralHue()));
            AddItem(new Boots());

            AddItem(new HalfApron(Utility.RandomList(0x906, 0x8A5, 0x89A)));

            Item hammer = new SmithHammer();
            hammer.Movable = false;
            AddItem(hammer);
        }

        public override void SayRandomChatter()
        {
            string[] lines = {
                "This iron is stubborn today.",
                "A good blade takes time and patience.",
                "Hot work, but someone has to do it.",
                "Need your armor patched up?"
            };
            Say(lines[Utility.Random(lines.Length)]);
        }

        public override void OnThink()
        {
            base.OnThink();

            if (DateTime.UtcNow >= _NextWork)
            {
                if (_MyAnvil == null || _MyAnvil.Deleted)
                {
                    foreach (Item item in GetItemsInRange(1))
                    {
                        if (item is AnvilHit anvil)
                        {
                            _MyAnvil = anvil;
                            break;
                        }
                    }
                }

                if (_MyAnvil != null && !_MyAnvil.Deleted)
                {
                    _MyAnvil.Strike(this);
                }

                _NextWork = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(2, 5));
            }
        }

        public TradesmanSmith(Serial serial) : base(serial)
        {
        }
    }
}
