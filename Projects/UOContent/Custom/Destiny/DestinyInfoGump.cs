using System;
using Server.Commands;
using Server.Network;
using Server.Mobiles;

namespace Server.Gumps
{
    public class DestinyInfoGump : Gump
    {
        private PlayerMobile m_Player;

        public static void Initialize()
        {
            CommandSystem.Register("Destiny", AccessLevel.Player, Destiny_OnCommand);
        }

        [Usage("Destiny")]
        [Description("Displays your Roguelite progression and Destiny Points.")]
        public static void Destiny_OnCommand(CommandEventArgs e)
        {
            if (e.Mobile is PlayerMobile pm)
                e.Mobile.SendGump(new DestinyInfoGump(pm));
        }

        public DestinyInfoGump(PlayerMobile from) : base(50, 50)
        {
            m_Player = from;
            AddPage(0);
            AddBackground(0, 0, 350, 340, 9270);
            AddAlphaRegion(10, 10, 330, 320);
            AddLabel(110, 25, 1152, "DESTINY DASHBOARD");
            int y = 65;
            AddLabel(30, y, 0x3F, "--- Current Essence ---");
            y += 30;
            AddLabel(40, y, 1152, "Unspent Points:");
            AddLabel(230, y, 0x480, m_Player.DestinyPoints.ToString());
            y += 25;
            AddLabel(40, y, 1152, "Stat Cap:");
            AddLabel(230, y, 1152, m_Player.MaxStatCap.ToString());
            y += 25;
            AddLabel(40, y, 1152, "Skill Cap:");
            AddLabel(230, y, 1152, (m_Player.MaxSkillCap / 10.0).ToString("F1"));
            y += 45;
            AddLabel(30, y, 0x3F, "--- Lifetime Legacy ---");
            y += 30;
            AddLabel(40, y, 1152, "Total Points Earned:");
            AddLabel(230, y, 0x480, m_Player.LifetimeDestinyPoints.ToString());
            y += 25;
            AddLabel(40, y, 1152, "Total Points Spent:");
            AddLabel(230, y, 1152, m_Player.TotalPointsSpent.ToString());
            y += 25;
            AddLabel(40, y, 1152, "Total Deaths:");
            AddLabel(230, y, 33, m_Player.TotalDeaths.ToString());
            AddLabel(75, 300, 0x3F, "Visit the Hall to spend points.");
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
        }
    }
}
