using System;
using System.Collections.Generic;
using ModernUO.Serialization;
using Server.Collections;
using Server.Factions;
using Server.Gumps;
using Server.Maps;
using Server.Mobiles;
using Server.Network;
using Server.Spells;

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

    public CustomMoongate(Serial serial) : base(serial)
    {
    }

    public override bool OnMoveOver(Mobile m)
    {
        if (m.Player && m.Alive)
        {
            UseGate(m);
        }
        return true;
    }

    public override void OnDoubleClick(Mobile from)
    {
        if (!from.Player || !from.Alive)
        {
            return;
        }

        if (from.InRange(GetWorldLocation(), 2))
        {
            UseGate(from);
        }
        else
        {
            from.SendLocalizedMessage(500446); // That is too far away.
        }
    }

    public bool CanUseGate(Mobile m, bool sendMessage = true)
    {
        if (m == null || !m.Alive || !m.Player)
            return false;

        if (m.Criminal)
        {
            if (sendMessage)
                m.SendLocalizedMessage(1005561, "", 0x22); // Thou'rt a criminal and cannot escape so easily.
            return false;
        }

        if (SpellHelper.CheckCombat(m))
        {
            if (sendMessage)
                m.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??
            return false;
        }

        if (m.Spell != null)
        {
            if (sendMessage)
                m.SendLocalizedMessage(1049616); // You are too busy to do that at the moment.
            return false;
        }

        return true;
    }

    public bool UseGate(Mobile m)
    {
        if (!CanUseGate(m))
            return false;

        SendGateGump(m);
        return true;
    }

    private void SendGateGump(Mobile m)
    {
        if (m is PlayerMobile pm)
        {
            // Close any existing moongate gumps before opening a new one
            pm.CloseGump<CustomMoongateGump>();
            pm.SendGump(new CustomMoongateGump(this));
            Effects.PlaySound(pm.Location, pm.Map, 0x20E);
            Timer.DelayCall(() =>
            {
                if (pm.Alive && pm.Map == Map && pm.InRange(Location, 1))
                {
                    pm.Combatant = null;
                    pm.Warmode = false;
                    pm.Hidden = true;
                }
            });
        }
    }
}
