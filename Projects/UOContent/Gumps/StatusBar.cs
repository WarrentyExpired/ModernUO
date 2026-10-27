using System;
using System.Collections.Concurrent;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Commands;
using Server.Network;

namespace Server.Custom.UI
{
    public class StatusBar
    {
        // Safely tracks if a player prefers the vertical (true) or horizontal (false) layout
        public static ConcurrentDictionary<Mobile, bool> Preferences = new ConcurrentDictionary<Mobile, bool>();

        public static void Initialize()
        {
            CommandSystem.Register("StatusBar", AccessLevel.Player, new CommandEventHandler(OnCommand));
        }

        [Usage("StatusBar")]
        [Description("Opens a quick-view status bar for Fame, Karma, Hunger, Thirst, Tithing, and Kills.")]
        public static void OnCommand(CommandEventArgs e)
        {
            if (e.Mobile is PlayerMobile pm)
            {
                // Get their saved preference, defaulting to horizontal (false)
                bool isVert = Preferences.GetOrAdd(pm, false);

                // ModernUO Generics: Notice the brackets and empty parentheses!
                pm.CloseGump<StatusBarGump>();
                pm.SendGump(new StatusBarGump(pm, isVert));
            }
        }

        public static void Refresh(Mobile from, bool force = false)
        {
            if (from is PlayerMobile pm && pm.NetState != null)
            {
                // ModernUO Generics: HasGump takes 0 arguments now
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
        private PlayerMobile m_Player;
        private bool m_IsVertical;

        public StatusBarGump(PlayerMobile pm, bool isVertical) : base(50, 50)
        {
            m_Player = pm;
            m_IsVertical = isVertical;

            Closable = true;
            Disposable = true;
            Draggable = true;
            Resizable = false;

            AddPage(0);

            if (m_IsVertical)
            {
                // Expanded height by 25px to accommodate the new line
                AddBackground(0, 0, 150, 167, 9270);

                AddHtml(15, 15, 120, 20, $"<BASEFONT COLOR=#FCCA03>Fame:</BASEFONT> <BASEFONT COLOR=#FFFFFF>{pm.Fame}</BASEFONT>", false, false);
                AddHtml(15, 40, 120, 20, $"<BASEFONT COLOR=#FCCA03>Karma:</BASEFONT> <BASEFONT COLOR=#FFFFFF>{pm.Karma}</BASEFONT>", false, false);
                AddHtml(15, 65, 120, 20, $"<BASEFONT COLOR=#FCCA03>Hunger:</BASEFONT> <BASEFONT COLOR=#FFFFFF>{pm.Hunger}/20</BASEFONT>", false, false);
                AddHtml(15, 90, 120, 20, $"<BASEFONT COLOR=#FCCA03>Thirst:</BASEFONT> <BASEFONT COLOR=#FFFFFF>{pm.Thirst}/20</BASEFONT>", false, false);
                AddHtml(15, 115, 120, 20, $"<BASEFONT COLOR=#FCCA03>Tithing:</BASEFONT> <BASEFONT COLOR=#FFFFFF>{pm.TithingPoints}</BASEFONT>", false, false);

                // Shifted the toggle button down 25px
                AddButton(115, 140, 9720, 9722, 2, GumpButtonType.Reply, 0);
            }
            else
            {
                // Expanded width by 100px to accommodate the new column
                AddBackground(0, 0, 540, 45, 9270);

                AddHtml(15, 12, 110, 20, $"<BASEFONT COLOR=#FCCA03>Fame:</BASEFONT> <BASEFONT COLOR=#FFFFFF>{pm.Fame}</BASEFONT>", false, false);
                AddHtml(110, 12, 110, 20, $"<BASEFONT COLOR=#FCCA03>Karma:</BASEFONT> <BASEFONT COLOR=#FFFFFF>{pm.Karma}</BASEFONT>", false, false);
                AddHtml(215, 12, 110, 20, $"<BASEFONT COLOR=#FCCA03>Hunger:</BASEFONT> <BASEFONT COLOR=#FFFFFF>{pm.Hunger}/20</BASEFONT>", false, false);
                AddHtml(315, 12, 110, 20, $"<BASEFONT COLOR=#FCCA03>Thirst:</BASEFONT> <BASEFONT COLOR=#FFFFFF>{pm.Thirst}/20</BASEFONT>", false, false);
                AddHtml(415, 12, 110, 20, $"<BASEFONT COLOR=#FCCA03>Tithing:</BASEFONT> <BASEFONT COLOR=#FFFFFF>{pm.TithingPoints}</BASEFONT>", false, false);

                // Shifted the toggle button right 100px
                AddButton(510, 15, 9720, 9722, 2, GumpButtonType.Reply, 0);
            }
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            if (m_Player == null || m_Player.Deleted)
                return;

            switch (info.ButtonID)
            {
                case 1: // Refresh the gump without changing layout
                    m_Player.SendGump(new StatusBarGump(m_Player, m_IsVertical));
                    break;
                case 2: // Toggle the layout and save their preference
                    StatusBar.Preferences[m_Player] = !m_IsVertical;
                    m_Player.SendGump(new StatusBarGump(m_Player, !m_IsVertical));
                    break;
            }
        }
    }
}
