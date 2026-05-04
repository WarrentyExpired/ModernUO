using System;
using System.Collections.Generic;
using Server.Network;
using Server.Mobiles;
using Server.Destiny;
using Server.Items;
namespace Server.Gumps
{
    public class DestinyDashboardGump : DestinyBaseGump
    {
        public DestinyDashboardGump(PlayerMobile pm) : base(pm, "Dashboard")
        {
            int y = 80;
            AddLabel(220, y, 0x3F, "--- Current Essence ---"); y += 30;

            AddLabel(230, y, 1152, "Stat Cap:");
            AddLabel(400, y, 0x481, m_Player.MaxStatCap.ToString()); y += 25;

            AddLabel(230, y, 1152, "Skill Cap:");
            AddLabel(400, y, 0x481, (m_Player.MaxSkillCap / 10.0).ToString("F1")); y += 45;

            AddLabel(220, y, 0x3F, "--- Lifetime Legacy ---"); y += 30;

            AddLabel(230, y, 1152, "Total Points Earned:");
            AddLabel(400, y, 0x480, m_Player.LifetimeDestinyPoints.ToString()); y += 25;

            AddLabel(230, y, 1152, "Total Deaths:");
            AddLabel(400, y, 33, m_Player.TotalDeaths.ToString());
        }
    }
}
