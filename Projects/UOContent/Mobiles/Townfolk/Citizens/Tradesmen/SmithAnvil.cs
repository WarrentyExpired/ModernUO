using System;
using Server;
using ModernUO.Serialization;

namespace Server.Items
{
    [SerializationGenerator(0, false)]
    public partial class AnvilHit : Item
    {
        [SerializableField(0)]
        private Item _ForgedItem;

        private static readonly int[] _ForgedGraphics =
        {
            0x1BEF, // Iron Ingot
            0x1B73, // Buckler
            0x1BEF, // Iron Ingot
            0x13B9, // Viking Sword
            0x1BEF,
            0x1401, // Kryss
            0x1BEF,
            0x1B76 // Heater Shield
        };

        [Constructible]
        public AnvilHit() : base(0xFAF)
        {
            Name = "anvil";
            Movable = false;
        }

        public void Strike(Mobile from)
        {
            if (from == null || from.Deleted) return;

            from.Direction = from.GetDirectionTo(Location);

            from.Animate(11, 5, 1, true, false, 0);
            from.PlaySound(0x2A);

            if (_ForgedItem == null || _ForgedItem.Deleted)
            {
                _ForgedItem = new Item(_ForgedGraphics[Utility.Random(_ForgedGraphics.Length)]);
                _ForgedItem.Movable = false;
                _ForgedItem.Name = "a forged item";

                _ForgedItem.MoveToWorld(new Point3D(X, Y, Z + 5), Map);
            }
            else
            {
                _ForgedItem.ItemID = _ForgedGraphics[Utility.Random(_ForgedGraphics.Length)];
            }
        }

        public override void OnDelete()
        {
            if (_ForgedItem != null && !_ForgedItem.Deleted)
            {
                _ForgedItem.Delete();
            }
            base.OnDelete();
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            base.OnLocationChange(oldLocation);
            if (_ForgedItem != null && !_ForgedItem.Deleted)
            {
                _ForgedItem.Location = new Point3D(X, Y, Z + 5);
            }
        }

        public override void OnMapChange()
        {
            base.OnMapChange();
            if (_ForgedItem != null && !_ForgedItem.Deleted)
            {
                _ForgedItem.Map = Map;
            }
        }

        public AnvilHit(Serial serial) : base(serial)
        {
        }
    }
}
