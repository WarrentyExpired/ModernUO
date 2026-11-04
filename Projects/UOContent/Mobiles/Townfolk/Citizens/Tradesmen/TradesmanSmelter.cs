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

namespace Server.Items
{
    [SerializationGenerator(0, false)]
    public partial class ForgeHit : Item
    {
        [SerializableField(0)]
        private Item _smeltedItem;

        [Constructible]
        public ForgeHit() : base(0xFB1)
        {
            Name = "forge";
            Movable = false;
        }

        public void Smelt(Mobile from)
        {
            if (from == null || from.Deleted) return;

            from.Direction = from.GetDirectionTo(Location);

            from.Animate(11, 5, 1, true, false, 0);

            if (_smeltedItem == null || _smeltedItem.Deleted)
            {
                _smeltedItem = new Item(0x19B8);
                _smeltedItem.Movable = false;
                _smeltedItem.Name = "smelting ore";

                _smeltedItem.MoveToWorld(new Point3D(X, Y, Z + 5), Map);
                from.PlaySound(0x2B); // Metal clink
            }
            else
            {
                if (_smeltedItem.ItemID == 0x19B8)
                {
                    _smeltedItem.ItemID = 0x1BF3;
                    _smeltedItem.Name = "fresh ingots";
                    from.PlaySound(0x208); // Fire sizzle/quench sound
                }
                else
                {
                    _smeltedItem.ItemID = 0x19B8;
                    _smeltedItem.Name = "smelting ore";
                    from.PlaySound(0x2B); // Metal clink
                }
            }
        }

        public override void OnDelete()
        {
            if (_smeltedItem != null && !_smeltedItem.Deleted)
            {
                _smeltedItem.Delete();
            }
            base.OnDelete();
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            base.OnLocationChange(oldLocation);
            if (_smeltedItem != null && !_smeltedItem.Deleted)
            {
                _smeltedItem.Location = new Point3D(X, Y, Z + 5);
            }
        }

        public override void OnMapChange()
        {
            base.OnMapChange();
            if (_smeltedItem != null && !_smeltedItem.Deleted)
            {
                _smeltedItem.Map = Map;
            }
        }

        public ForgeHit(Serial serial) : base(serial)
        {
        }
    }
}
