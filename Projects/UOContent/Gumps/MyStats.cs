using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Commands;
using Server.Network;

namespace Server.Custom.UI
{
    public class PlayerStats
    {
        public static void Initialize()
        {
            CommandSystem.Register("MyStats", AccessLevel.Player, new CommandEventHandler(OnCommand));
        }

        [Usage("MyStats")]
        [Description("Opens the player statistics gump.")]
        public static void OnCommand(CommandEventArgs e)
        {
            if (e.Mobile is PlayerMobile pm)
            {
                pm.CloseGump<PlayerStatsGump>();
                pm.SendGump(new PlayerStatsGump(pm));
            }
        }
    }

    public class PlayerStatsGump : Gump
    {
        public PlayerStatsGump(PlayerMobile pm) : base(50, 50)
        {
            Closable = true;
            Disposable = true;
            Draggable = true;
            Resizable = false;

            AddPage(0);

            AddBackground(0, 0, 300, 260, 9270);
            AddAlphaRegion(10, 10, 280, 240);

            AddLabel(85, 15, 1152, "PLAYER STATISTICS");

            int y = 55;

            AddLabel(25, y, 1152, "Total Player Deaths:");
            AddLabel(180, y, 1152, pm.TotalPlayerDeaths.ToString("N0"));
            y += 30;

            AddLabel(25, y, 1152, "Total Monsters Killed:");
            AddLabel(180, y, 1152, pm.TotalMonstersKilled.ToString("N0"));
            y += 30;

            AddLabel(25, y, 1152, "Total Gold Looted:");
            AddLabel(180, y, 1152, pm.TotalGoldLooted.ToString("N0"));
            y += 30;

            AddLabel(25, y, 1152, "Spendable Kill Points:");
            AddLabel(180, y, 1152, pm.MonsterKillPoints.ToString("N0"));
            y += 30;

            AddLabel(25, y, 1152, "Lifetime Kill Points:");
            AddLabel(180, y, 1152, pm.LifetimeKillPoints.ToString("N0"));
            y += 45;

            AddButton(25, y, 4017, 4019, 0, GumpButtonType.Reply, 0);
            AddLabel(60, y, 1152, "Close Menu");
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            // ButtonID 0 handles the close button automatically, so no additional logic is needed here
        }
    }
}
