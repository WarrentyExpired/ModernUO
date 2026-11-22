using System;
using System.Collections.Concurrent;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Commands;
using Server.Network;
using Server.Spells;
using Server.Engines.Bounties;

namespace Server.Custom.UI
{
    public class StatusBar
    {
        public static ConcurrentDictionary<Mobile, bool> Preferences = new ConcurrentDictionary<Mobile, bool>();

        public static void Initialize()
        {
            CommandSystem.Register("StatusBar", AccessLevel.Player, new CommandEventHandler(OnCommand));
        }

        [Usage("StatusBar")]
        [Description("Opens a quick-view Info Center for Fame, Karma, Hunger, Thirst, AR, MR, Dashboard, and Bounties.")]
        public static void OnCommand(CommandEventArgs e)
        {
            if (e.Mobile is PlayerMobile pm)
            {
                bool isVert = Preferences.GetOrAdd(pm, false);

                pm.CloseGump<StatusBarGump>();
                pm.SendGump(new StatusBarGump(pm, isVert));
            }
        }

        public static void Refresh(Mobile from, bool force = false)
        {
            if (from is PlayerMobile pm && pm.NetState != null)
            {
                if (pm.HasGump<StatusBarGump>() || force)
                {
                    bool isVert = Preferences.GetOrAdd(pm, false);

                    pm.CloseGump<StatusBarGump>();
                    pm.SendGump(new StatusBarGump(pm, isVert));
                }
            }
        }
    }

    public class StatusBarGump : Gump
    {
        private readonly PlayerMobile m_Player;
        private readonly bool m_IsVertical;

        public StatusBarGump(PlayerMobile pm, bool isVertical) : base(50, 50)
        {
            m_Player = pm;
            m_IsVertical = isVertical;

            Closable = true;
            Disposable = true;
            Draggable = true;
            Resizable = false;

            AddPage(0);

            int ar = (int)Math.Round(pm.ArmorRating);
            int mr = (int)Math.Round(SpellHelper.GetMagicArmorRating(pm), MidpointRounding.AwayFromZero);
            if (m_IsVertical)
            {
                AddBackground(0, 0, 165, 255, 9270);

                AddHtml(15, 14, 135, 20, $"<BASEFONT COLOR=#FCCA03>Fame:</BASEFONT> <BASEFONT COLOR=#FFFFFF>{pm.Fame}</BASEFONT>", false, false);
                AddHtml(15, 38, 135, 20, $"<BASEFONT COLOR=#FCCA03>Karma:</BASEFONT> <BASEFONT COLOR=#FFFFFF>{pm.Karma}</BASEFONT>", false, false);
                AddHtml(15, 62, 135, 20, $"<BASEFONT COLOR=#FCCA03>Hunger:</BASEFONT> <BASEFONT COLOR=#FFFFFF>{pm.Hunger}/20</BASEFONT>", false, false);
                AddHtml(15, 86, 135, 20, $"<BASEFONT COLOR=#FCCA03>Thirst:</BASEFONT> <BASEFONT COLOR=#FFFFFF>{pm.Thirst}/20</BASEFONT>", false, false);
                AddHtml(15, 110, 135, 20, $"<BASEFONT COLOR=#FCCA03>AR:</BASEFONT> <BASEFONT COLOR=#FFFFFF>{ar}</BASEFONT>", false, false);
                AddHtml(15, 134, 135, 20, $"<BASEFONT COLOR=#FCCA03>MR:</BASEFONT> <BASEFONT COLOR=#00E5FF>{mr}</BASEFONT>", false, false);

                // Quick-Launch Buttons
                AddButton(15, 164, 4005, 4007, 3, GumpButtonType.Reply, 0);
                AddHtml(50, 166, 100, 20, "<BASEFONT COLOR=#FFFFFF>Dashboard</BASEFONT>", false, false);

                AddButton(15, 190, 4005, 4007, 4, GumpButtonType.Reply, 0);
                AddHtml(50, 192, 100, 20, "<BASEFONT COLOR=#FFFFFF>Bounties</BASEFONT>", false, false);

                // Bottom Bar: Refresh (left) & Layout Toggle (right)
                AddButton(15, 220, 4011, 4013, 1, GumpButtonType.Reply, 0);
                AddHtml(50, 222, 65, 20, "<BASEFONT COLOR=#A8E6CF>Refresh</BASEFONT>", false, false);

                AddButton(130, 222, 9720, 9722, 2, GumpButtonType.Reply, 0);
            }
            else
            {
                AddBackground(0, 0, 745, 46, 9270);

                AddHtml(15, 13, 95, 20, $"<BASEFONT COLOR=#FCCA03>Fame:</BASEFONT> <BASEFONT COLOR=#FFFFFF>{pm.Fame}</BASEFONT>", false, false);
                AddHtml(105, 13, 100, 20, $"<BASEFONT COLOR=#FCCA03>Karma:</BASEFONT> <BASEFONT COLOR=#FFFFFF>{pm.Karma}</BASEFONT>", false, false);
                AddHtml(205, 13, 95, 20, $"<BASEFONT COLOR=#FCCA03>Hunger:</BASEFONT> <BASEFONT COLOR=#FFFFFF>{pm.Hunger}/20</BASEFONT>", false, false);
                AddHtml(305, 13, 90, 20, $"<BASEFONT COLOR=#FCCA03>Thirst:</BASEFONT> <BASEFONT COLOR=#FFFFFF>{pm.Thirst}/20</BASEFONT>", false, false);
                AddHtml(400, 13, 65, 20, $"<BASEFONT COLOR=#FCCA03>AR:</BASEFONT> <BASEFONT COLOR=#FFFFFF>{ar}</BASEFONT>", false, false);
                AddHtml(455, 13, 75, 20, $"<BASEFONT COLOR=#FCCA03>MR:</BASEFONT> <BASEFONT COLOR=#00E5FF>{mr}</BASEFONT>", false, false);

                // Dashboard Button
                AddButton(525, 12, 4005, 4007, 3, GumpButtonType.Reply, 0);
                AddHtml(555, 13, 70, 20, "<BASEFONT COLOR=#FFFFFF>Dashboard</BASEFONT>", false, false);

                // Bounties Button
                AddButton(620, 12, 4005, 4007, 4, GumpButtonType.Reply, 0);
                AddHtml(655, 13, 70, 20, "<BASEFONT COLOR=#FFFFFF>Bounty</BASEFONT>", false, false);

                // Refresh Button (compact icon)
                //AddButton(680, 14, 5401, 5401, 1, GumpButtonType.Reply, 0);

                // Orientation Toggle Button
                AddButton(715, 15, 9720, 9722, 2, GumpButtonType.Reply, 0);
            }
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            if (m_Player == null || m_Player.Deleted)
            {
                return;
            }

            switch (info.ButtonID)
            {
                case 1: // Refresh
                    m_Player.SendGump(new StatusBarGump(m_Player, m_IsVertical));
                    break;

                case 2: // Toggle layout
                    StatusBar.Preferences[m_Player] = !m_IsVertical;
                    m_Player.SendGump(new StatusBarGump(m_Player, !m_IsVertical));
                    break;

                case 3: // Open Dashboard + keep StatusBar open
                    m_Player.SendGump(new StatusBarGump(m_Player, m_IsVertical));
                    CommandSystem.Handle(m_Player, $"{CommandSystem.Prefix}dashboard");
                    break;

                case 4: // Open ActiveBountyGump + keep StatusBar open
                    m_Player.SendGump(new StatusBarGump(m_Player, m_IsVertical));
                    var profile = BountySystem.GetProfile(m_Player);
                    m_Player.CloseGump<ActiveBountyGump>();
                    m_Player.SendGump(new ActiveBountyGump(m_Player, profile));
                    break;
            }
        }
    }
}
