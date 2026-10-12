using Server;
using Server.Mobiles;
using Server.Custom;
using System.Collections.Generic;
using System;

namespace Server.Regions
{
    public class GreetingInnRegion : TownRegion
    {
        public GreetingInnRegion(string name, Map map, int priority, params Rectangle3D[] area)
            : base(name, map, priority, area)
        {
        }
        public override bool MountsAllowed => false;
        public override TimeSpan GetLogoutDelay(Mobile m)
        {
            return TimeSpan.Zero;
        }
        public override void OnEnter(Mobile m)
        {
            base.OnEnter(m);
            if (m.Player)
            {
                m.SendMessage($"Welcome to <BASEFONT COLOR='#FFD700'>{this.Name}</BASEFONT>.");
            }
            if (m is PlayerMobile player )
            {
                if ( player.Mounted )
                {
                    Server.Custom.AutoStable.StoreMount(player);
                }
            }
        }

        public override void OnExit(Mobile m)
        {
            base.OnExit(m);
            if ( m is PlayerMobile player )
            {
                Server.Custom.AutoStable.RestoreMount(player);
            }
        }
    }
}
