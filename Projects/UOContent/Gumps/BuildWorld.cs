using System;
using Server.Commands;
using Server.Network;

namespace Server.Gumps
{
    public class WorldBuildingGump : Gump
    {
        private Mobile m_From;

        public static void Configure()
        {
            CommandSystem.Register("BuildWorld", AccessLevel.GameMaster, WorldBuilding_OnCommand);
        }

        [Usage("BuildWorld")]
        public static void WorldBuilding_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new WorldBuildingGump(e.Mobile));
        }

        public WorldBuildingGump(Mobile from) : base(50, 50)
        {
            m_From = from;

            AddPage(0);

            AddBackground(0, 0, 450, 500, 9270);
            AddAlphaRegion(10, 10, 430, 480);

            AddLabel(165, 15, 1152, "WORLD BUILDING MENU");
            int y = 45;

            AddLabel(20, y, 1152, "--- Core Generation ---");
            y += 30;

            AddButton(20, y, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddLabel(55, y, 1152, "Generate Teleporters (TelGen)");
            y += 30;

            AddButton(20, y, 4005, 4007, 2, GumpButtonType.Reply, 0);
            AddLabel(55, y, 1152, "Decorate World (Decorate)");
            y += 30;

            AddButton(20, y, 4005, 4007, 3, GumpButtonType.Reply, 0);
            AddLabel(55, y, 1152, "Generate Spawners (Json)");
            y += 30;

            AddLabel(20, y, 33, "--- Danger Zone ---");
            y += 30;

            AddButton(20, y, 4017, 4019, 7, GumpButtonType.Reply, 0);
            AddLabel(55, y, 33, "CLEAR ENTIRE MAP (Be Careful!)");
            y += 35;

            AddButton(20, y, 4005, 4007, 8, GumpButtonType.Reply, 0);
            AddLabel(55, y, 0x3F, "POPULATE AND DECORATE (Full Automation)");
            y += 40;

            AddButton(20, y, 4017, 4019, 0, GumpButtonType.Reply, 0);
            AddLabel(55, y, 1152, "Close Menu");
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (info.ButtonID == 0)
                return;

            switch (info.ButtonID)
            {
                case 1: InvokeCommand("TelGen", from); break;
                case 2: InvokeCommand("Decorate", from); break;
                case 3: InvokeCommand("IS Data/Spawns/Overworld/**.json", from);
                        InvokeCommand("IS Data/Spawns/Underworld/**.json", from);
                        break;
                case 7: InvokeCommand("ClearAll", from); break;

                case 8: // THE FULL AUTOMATION
                    from.SendMessage(0x3F, "Starting full world population...");
                    InvokeCommand("TelGen", from);
                    InvokeCommand("Decorate", from);
                    InvokeCommand("IS Data/Spawns/Overworld/**.json", from);
                    InvokeCommand("IS Data/Spawns/Underworld/**.json", from);
                    from.SendMessage(0x3F, "World population complete.");
                    break;
            }
            from.SendGump(new WorldBuildingGump(from));
        }

        private void InvokeCommand(string cmd, Mobile from)
        {
            try
            {
                CommandSystem.Handle(from, string.Format("{0}{1}", CommandSystem.Prefix, cmd));
            }
            catch (Exception ex)
            {
                from.SendMessage($"Error executing command '{cmd}': {ex.Message}");
            }
        }
    }
}
