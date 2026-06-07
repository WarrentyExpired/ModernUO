using System;
using Server.Mobiles;
using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class ExtractionTotem : Item
{
    [Constructible]
    public ExtractionTotem() : base(0xB0DA)
    {
        Movable = false;
        Name = "Extraction Totem";
        Hue = 0;
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
        pm.SendMessage(0x3F, $"Vault link established. Stash Box capacity: {pm.StashBox.Items.Count}/125 items.");
    }
}
