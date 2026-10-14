using System;
using ModernUO.Serialization;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class CustomMoongate : Item
{
    [Constructible]
    public CustomMoongate() : base(0xF6C)
    {
        Movable = false;
        Light = LightType.Circle300;
        Name = "a moongate";
    }

    // Required for ModernUO Serialization
    public CustomMoongate(Serial serial) : base(serial)
    {
    }

    // Triggers when a player walks onto the gate
    public override bool OnMoveOver(Mobile m)
    {
        if (m.Player && m.Alive)
        {
            SendGateGump(m);
        }
        return true; // Return true to allow the player to step onto the tile
    }

    // Triggers when a player double-clicks the gate
    public override void OnDoubleClick(Mobile from)
    {
        if (!from.Player || !from.Alive)
        {
            return;
        }

        if (from.InRange(GetWorldLocation(), 2))
        {
            SendGateGump(from);
        }
        else
        {
            from.SendLocalizedMessage(500446); // That is too far away.
        }
    }

    private void SendGateGump(Mobile m)
    {
        if (m is PlayerMobile pm)
        {
            // Close any existing moongate gumps before opening a new one
            pm.CloseGump<CustomMoongateGump>();
            pm.SendGump(new CustomMoongateGump(this));
        }
    }
}
