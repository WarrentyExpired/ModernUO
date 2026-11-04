using System;
using Server;
using ModernUO.Serialization;

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
