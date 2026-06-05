using System;
using Server.Mobiles;
using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class LegacyStashBox : Item
{
    [Constructible]
    public LegacyStashBox() : base(0xB807) // FIXED: Large Wooden Gold Inlaid Box graphic
    {
        Movable = false;
        Name = "Legacy Stash Vault";
    }

    public override void OnDoubleClick(Mobile from)
    {
        if (from is not PlayerMobile pm)
            return;

        if (!from.InRange(GetWorldLocation(), 3))
        {
            from.SendLocalizedMessage(500446); // That is too far away.
            return;
        }

        if (pm.StashBox == null)
        {
            pm.SendMessage(0x22, "Error: Your legacy stash could not be found.");
            return;
        }
        pm.StashBox.OpenVault(pm);
        //pm.Backpack.DropItem(pm.StashBox);
        //pm.StashBox.DisplayTo(pm);
        //pm.BankBox.DropItem(pm.StashBox);
        pm.SendMessage(0x5A, $"Accessing secure vault storage. Current Load: {pm.StashBox.Items.Count}/125 slots used.");
    }
}
