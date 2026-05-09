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

            // Increased height to 500 to accommodate the extra button
            AddBackground(0, 0, 450, 500, 9270);
            AddAlphaRegion(10, 10, 430, 480);

            AddLabel(165, 15, 1152, "WORLD BUILDING MENU");
            int y = 45;

            // --- CATEGORY: CORE GENERATION ---
            AddLabel(20, y, 1152, "--- Core Generation ---");
            y += 30;

            AddButton(20, y, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddLabel(55, y, 1152, "Generate Teleporters (GenTele)");
            y += 30;

            AddButton(20, y, 4005, 4007, 2, GumpButtonType.Reply, 0);
            AddLabel(55, y, 1152, "Decorate World (Decorate)");
            y += 30;

            AddButton(20, y, 4005, 4007, 3, GumpButtonType.Reply, 0);
            AddLabel(55, y, 1152, "Generate Spawners (Json)");
            y += 30;

            AddButton(20, y, 4005, 4007, 6, GumpButtonType.Reply, 0);
            AddLabel(55, y, 1152, "Generate Signs (SignGen)");
            y += 40;

            // --- CATEGORY: UTILITIES ---
            AddLabel(20, y, 1152, "--- Utilities ---");
            y += 30;

            AddButton(20, y, 4005, 4007, 4, GumpButtonType.Reply, 0);
            AddLabel(55, y, 1152, "Export All Spawners");
            y += 30;

            AddButton(20, y, 4005, 4007, 5, GumpButtonType.Reply, 0);
            AddLabel(55, y, 1152, "Generate Thru-Doors");
            y += 40;

            // --- CATEGORY: DANGER ZONE ---
            AddLabel(20, y, 33, "--- Danger Zone ---");
            y += 30;

            AddButton(20, y, 4017, 4019, 7, GumpButtonType.Reply, 0);
            AddLabel(55, y, 33, "CLEAR ENTIRE MAP (Be Careful!)");
            y += 35;

            // NEW BUTTON: POPULATE AND DECORATE
            AddButton(20, y, 4005, 4007, 8, GumpButtonType.Reply, 0);
            AddLabel(55, y, 0x3F, "POPULATE AND DECORATE (Full Automation)");
            y += 40;

            // CLOSE
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
                case 1: InvokeCommand("GenTele", from); break;
                case 2: InvokeCommand("Decorate", from); break;
                case 3: InvokeCommand("GenerateSpawners Data/Spawners/**.json", from); break;
                case 4: InvokeCommand("ExportAllSpawners", from); break;
                case 5: InvokeCommand("GenThruDoors", from); break;
                case 6: InvokeCommand("SignGen", from); break;
                case 7: InvokeCommand("ClearAll", from); break;

                case 8: // THE FULL AUTOMATION
                    from.SendMessage(0x3F, "Starting full world population...");
                    InvokeCommand("GenTele", from);
                    InvokeCommand("Decorate", from);
                    InvokeCommand("GenerateSpawners Data/Spawners/**.json", from);
                    InvokeCommand("SignGen", from);
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
