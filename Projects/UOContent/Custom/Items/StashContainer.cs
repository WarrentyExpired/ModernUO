using System;
using Server.Items;
using Server.Mobiles;
using ModernUO.Serialization;

namespace Server.Items
{
    [SerializationGenerator(0, false)]
    public partial class StashContainer : Container
    {
        public override int DefaultMaxItems => 125;
        public override int DefaultMaxWeight => 0;

        // Background lease timer tracking
        private Timer _hideTimer;

        public StashContainer() : base(0x9AB) // Secure Chest graphic
        {
            Movable = false; // Prevents dragging the chest icon out of the pack
            Weight = 0.0;    // Weighs nothing while temporarily leased
        }

        public void OpenVault(PlayerMobile pm)
        {
            if (pm.Backpack == null) return;

            StopHideTimer();

            // 1. Lease the box to the active backpack layout so the client syncs
            pm.Backpack.DropItem(this);

            // 2. Open the visual grid layout window
            DisplayTo(pm);

            // 3. Begin the 20-second countdown to secure and close the vault
            _hideTimer = Timer.DelayCall(TimeSpan.FromSeconds(20.0), () => HideVault(pm));
        }

        public void RefreshTimer(PlayerMobile pm)
        {
            if (_hideTimer != null && pm != null)
            {
                StopHideTimer();
                _hideTimer = Timer.DelayCall(TimeSpan.FromSeconds(20.0), () => HideVault(pm));
            }
        }

        private void HideVault(PlayerMobile pm)
        {
            _hideTimer = null;

            if (pm != null && pm.BankBox != null)
            {
                pm.BankBox.DropItem(this);
                if (pm != null && pm.NetState != null)
                {
                    pm.SendMessage(0x22, "Vault connection closed down automatically to secure your items.");
                }
            }
        }

        public void StopHideTimer()
        {
            if (_hideTimer != null)
            {
                _hideTimer.Stop();
                _hideTimer = null;
            }
        }

        public override bool IsAccessibleTo(Mobile check)
        {
            if (check is PlayerMobile pm && pm.StashBox == this)
            {
                // Toggle item interactive safety properties depending on world coordinates
                bool inSafeZone = check.Region != null && check.Region.Name.Equals("the Chamber of Destiny", StringComparison.OrdinalIgnoreCase);

                foreach (Item item in Items)
                {
                    item.Movable = inSafeZone;
                }

                return true;
            }

            return base.IsAccessibleTo(check);
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (Items.Count >= DefaultMaxItems)
            {
                from.SendMessage(0x22, "Your legacy stash box is completely full! (125/125 Items)");
                return false;
            }

            if (from is PlayerMobile pm)
            {
                RefreshTimer(pm); // Reset the 20-second lease because they are interacting!
            }

            return base.OnDragDrop(from, dropped);
        }

        public override bool OnDragDropInto(Mobile from, Item dropped, Point3D p)
        {
            if (Items.Count >= DefaultMaxItems)
            {
                from.SendMessage(0x22, "Your legacy stash box is completely full! (125/125 Items)");
                return false;
            }

            if (from is PlayerMobile pm)
            {
                RefreshTimer(pm); // Reset the 20-second lease because they are interacting!
            }

            return base.OnDragDropInto(from, dropped, p);
        }
    }
}
