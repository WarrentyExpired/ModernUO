using System;
using Server;
using Server.Items;
using ModernUO.Serialization;

namespace Server.Mobiles
{
    [SerializationGenerator(0, false)]
    public partial class TradesmanSmelter : TradesmanCitizen
    {
        private DateTime _nextWork;
        private ForgeHit _myForge;

        [Constructible]
        public TradesmanSmelter()
        {
            Title = "the smelter";
            CantWalk = true;
            _nextWork = DateTime.UtcNow + TimeSpan.FromSeconds(2);
        }

        public override void DressTradesman()
        {
            AddItem(new Shirt(Utility.RandomNeutralHue()));
            AddItem(new ShortPants(Utility.RandomNeutralHue()));
            AddItem(new Boots());

            AddItem(new FullApron(Utility.RandomList(0x906, 0x8A5, 0x89A)));

            Item hammer = new SmithHammer();
            hammer.Movable = false;
            AddItem(hammer);
        }

        public override void SayRandomChatter()
        {
            string[] lines = {
                "Hot enough to melt dragon scales today.",
                "More ore, always more ore.",
                "Careful, don't burn yourself.",
                "Nothing beats the glow of fresh ingots."
            };
            Say(lines[Utility.Random(lines.Length)]);
        }

        public override void OnThink()
        {
            base.OnThink();

            if (DateTime.UtcNow >= _nextWork)
            {
                if (_myForge == null || _myForge.Deleted)
                {
                    foreach (Item item in GetItemsInRange(1))
                    {
                        if (item is ForgeHit forge)
                        {
                            _myForge = forge;
                            break;
                        }
                    }
                }

                if (_myForge != null && !_myForge.Deleted)
                {
                    _myForge.Smelt(this);
                }

                _nextWork = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(3, 6));
            }
        }

        public TradesmanSmelter(Serial serial) : base(serial)
        {
        }
    }
}
