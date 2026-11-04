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

namespace Server.Items
{
    [SerializationGenerator(0, false)]
    public partial class ButcherHit : Item
    {
        private static readonly int[] _MeatGraphics = new int[]
        {
            0x1871, // Meat faces south
            0x1872, // Meat faces east
            0x1873, // Meat faces south
            0x1874 // Meat faces east
        };

        [Constructible]
        public ButcherHit() : base(0x1874)
        {
            ItemID = _MeatGraphics[Utility.Random(_MeatGraphics.Length)];
            Name = "meat";
            Movable = false;
        }

        [Constructible]
        public ButcherHit(int itemID) : base(itemID)
        {
            Name = "meat";
            Movable = false;
        }

        public (Point3D loc, Direction dir) GetWorkerPlacement(Point3D baseLoc)
        {
            switch (ItemID)
            {
                case 0x1871:
                    return (new Point3D(baseLoc.X, baseLoc.Y + 1, baseLoc.Z), Direction.North);
                case 0x1872:
                    return (new Point3D(baseLoc.X + 1, baseLoc.Y, baseLoc.Z), Direction.East);
                case 0x1873:
                    return (new Point3D(baseLoc.X, baseLoc.Y + 1, baseLoc.Z), Direction.North);
                case 0x1874:
                    return (new Point3D(baseLoc.X + 1, baseLoc.Y, baseLoc.Z), Direction.East);


                default:
                    if (Utility.RandomBool())
                    {
                        return (new Point3D(baseLoc.X - 1, baseLoc.Y, baseLoc.Z), Direction.East);
                    }
                    return (new Point3D(baseLoc.X, baseLoc.Y - 1, baseLoc.Z), Direction.South);
            }
        }

        public void Strike(Mobile from)
        {
            if (from == null || from.Deleted) return;

            from.Direction = from.GetDirectionTo(Location);
            from.Animate(11, 5, 1, true, false, 0);
            from.PlaySound(0x13F);
        }

        public ButcherHit(Serial serial) : base(serial)
        {
        }
    }
}
