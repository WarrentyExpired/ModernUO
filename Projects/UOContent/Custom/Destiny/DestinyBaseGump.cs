using System;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Gumps
{
    public abstract class DestinyBaseGump : Gump
    {
        protected PlayerMobile m_Player;

        public DestinyBaseGump(PlayerMobile pm, string categoryTitle) : base(50, 50)
        {
            m_Player = pm;

            AddPage(0);

            // Main Frame
            AddBackground(0, 0, 900, 730, 9270);
            AddAlphaRegion(10, 10, 880, 710);

            // Sidebar
            AddAlphaRegion(20, 20, 180, 690);
            AddLabel(45, 35, 1152, "ALTAR OF DESTINY");
            AddLabel(40, 60, 0x480, $"Points: {m_Player.DestinyPoints:N0}");

            // --- TABS ---
            AddButton(30, 120, 4005, 4007, 900, GumpButtonType.Reply, 0);
            AddLabel(65, 120, 1152, "Dashboard");

            AddButton(30, 160, 4005, 4007, 901, GumpButtonType.Reply, 0);
            AddLabel(65, 160, 1152, "Templates");

            AddButton(30, 200, 4005, 4007, 902, GumpButtonType.Reply, 0);
            AddLabel(65, 200, 1152, "Core Upgrades");

            AddButton(30, 240, 4005, 4007, 903, GumpButtonType.Reply, 0);
            AddLabel(65, 240, 1152, "Skill Resonance");
            // Leave
            AddButton(30, 670, 4017, 4019, 0, GumpButtonType.Reply, 0);
            AddLabel(65, 670, 33, "Leave Altar");

            // Header for Content Area
            AddLabel(220, 35, 0x3F, $"Category: {categoryTitle}");
            AddImageTiled(220, 55, 650, 2, 0x2424);
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            if (info.ButtonID == 0) return;

            base.OnResponse(sender, info);

            // Handle Global Tab Switching
            switch (info.ButtonID)
            {
                case 900: m_Player.SendGump(new DestinyDashboardGump(m_Player)); break;
                case 901: m_Player.SendGump(new DestinyTemplatesGump(m_Player)); break;
                case 902: m_Player.SendGump(new DestinyCoreGump(m_Player)); break;
                case 903: m_Player.SendGump(new DestinySkillsGump(m_Player, 1)); break;
            }
        }

        protected void DrawUpgradeEntry(int x, int y, int buttonID, string name, int cost)
        {
            AddButton(x, y, 4005, 4007, buttonID, GumpButtonType.Reply, 0);
            AddLabel(x + 35, y, 1152, name);
            AddLabel(x + 550, y, 0x480, $"{cost:N0} Pts");
        }

        protected bool Purchase(int cost)
        {
            if (m_Player.DestinyPoints >= cost)
            {
                m_Player.DestinyPoints -= cost;
                m_Player.TotalPointsSpent += cost;
                m_Player.SendMessage(0x3F, "The Altar hums as your destiny is rewritten.");
                return true;
            }
            m_Player.SendMessage(0x22, "The Altar remains silent. You lack the Destiny Points.");
            return false;
        }

        protected int GetCurrentCost(int baseCost, int currentPurchases)
        {
            int priceTier = currentPurchases / 5;
            return (int)(baseCost * Math.Pow(1.5, priceTier));
        }
    }
}
