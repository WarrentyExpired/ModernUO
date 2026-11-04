using System;
using Server;
using ModernUO.Serialization;

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

            // Snap the NPC to face the forge directly
            from.Direction = from.GetDirectionTo(Location);

            // Play the blunt swing animation
            from.Animate(11, 5, 1, true, false, 0);

            if (_smeltedItem == null || _smeltedItem.Deleted)
            {
                // Create Ore on the first strike
                _smeltedItem = new Item(0x19B8);
                _smeltedItem.Movable = false;
                _smeltedItem.Name = "smelting ore";

                // Elevate it so it sits on top of the forge
                _smeltedItem.MoveToWorld(new Point3D(X, Y, Z + 5), Map);
                from.PlaySound(0x2B); // Metal clink
            }
            else
            {
                // Toggle between Ore and Ingots
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

        // Cleanup: If a GM deletes the forge, delete the visual ore
        public override void OnDelete()
        {
            if (_smeltedItem != null && !_smeltedItem.Deleted)
            {
                _smeltedItem.Delete();
            }
            base.OnDelete();
        }

        // QoL: If a GM moves the forge, bring the visual ore with it
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
