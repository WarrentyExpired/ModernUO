using System;
using Server;
using ModernUO.Serialization;

namespace Server.Items
{
    [SerializationGenerator(0, false)]
    public partial class AnvilHit : Item
    {
        // Tells ModernUO to automatically save our forged item reference during server saves
        [SerializableField(0)]
        private Item _ForgedItem;

        // A registry of item graphics to cycle through on the anvil
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

            // Snap the NPC to face the anvil directly
            from.Direction = from.GetDirectionTo(Location);

            // Action 11 is the human animation for swinging a 1-handed blunt weapon
            from.Animate(11, 5, 1, true, false, 0);
            from.PlaySound(0x2A);

            // Handle the visual forged item on top of the anvil
            if (_ForgedItem == null || _ForgedItem.Deleted)
            {
                // Create it on the first strike
                _ForgedItem = new Item(_ForgedGraphics[Utility.Random(_ForgedGraphics.Length)]);
                _ForgedItem.Movable = false;
                _ForgedItem.Name = "a forged item";

                // Elevate it by 5 Z-levels so it sits on top of the anvil mesh
                _ForgedItem.MoveToWorld(new Point3D(X, Y, Z + 5), Map);
            }
            else
            {
                // Cycle to a new random graphic with each subsequent strike
                _ForgedItem.ItemID = _ForgedGraphics[Utility.Random(_ForgedGraphics.Length)];
            }
        }

        // Cleanup: If a GM deletes the anvil, delete the forged weapon too
        public override void OnDelete()
        {
            if (_ForgedItem != null && !_ForgedItem.Deleted)
            {
                _ForgedItem.Delete();
            }
            base.OnDelete();
        }

        // QoL: If a GM moves the anvil, bring the forged weapon with it
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
