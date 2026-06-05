using System;
using Server.Mobiles;
using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class ExtractionTotem : Item
{
    [Constructible]
    public ExtractionTotem() : base(0xA1F3) // FIXED: Bank Vault Door graphic
    {
        Movable = false;
        Name = "Extraction Totem";
        Hue = 0; // Customize if you want a spectral/magic look later
    }

    public override void OnDoubleClick(Mobile from)
    {
        if (from is not PlayerMobile pm)
            return;

        // Proximity Safety: Ensure they aren't accessing it from across the room
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
        pm.SendMessage(0x3F, $"Vault link established. Stash Box capacity: {pm.StashBox.Items.Count}/125 items.");
    }
}
