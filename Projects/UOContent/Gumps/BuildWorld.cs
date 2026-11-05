using System;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Network;

namespace Server.Gumps
{
    public class WorldBuildingGump : Gump
    {
        private Mobile m_From;

        public static void Configure()
        {
            CommandSystem.Register("BuildWorld", AccessLevel.GameMaster, WorldBuilding_OnCommand);
            CommandSystem.Register("WipeWildMobs", AccessLevel.GameMaster, WipeWildMobs_OnCommand);
        }

        [Usage("BuildWorld")]
        public static void WorldBuilding_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new WorldBuildingGump(e.Mobile));
        }

        [Usage("WipeWildMobs")]
        [Description("Deletes all uncontrolled BaseCreatures (excluding Player Vendors) to prep for a clean spawn.")]
        public static void WipeWildMobs_OnCommand(CommandEventArgs e)
        {
            List<Mobile> toDelete = new List<Mobile>();

            foreach (Mobile m in World.Mobiles.Values)
            {
                // Exclude controlled pets, summoned creatures, and Player Vendors
                if (m is BaseCreature bc && !bc.Controlled && !bc.Summoned && !(bc is PlayerVendor))
                {
                    toDelete.Add(bc);
                }
            }

            foreach (Mobile m in toDelete)
            {
                m.Delete();
            }

            e.Mobile.SendMessage(0x3F, $"Wiped {toDelete.Count} wild creatures from the world.");
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

            AddButton(20, y, 4005, 4007, 4, GumpButtonType.Reply, 0);
            AddLabel(55, y, 1152, "Generate Dungeon Stuff (Json)");
            y += 30;

            /*
            AddButton(20, y, 4005, 4007, 5, GumpButtonType.Reply, 0);
            AddLabel(55, y, 1152, "Wipe Wild Creatures (Cleanup)");
            y += 30;
            */

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
                case 2:
                        InvokeCommand("Decorate", from); break;
                        InvokeCommand("SignGen", from); break;
                case 3:
                    //InvokeCommand("WipeWildMobs", from);
                    InvokeCommand("IS Data/Spawns/Overworld/**.json", from);
                    InvokeCommand("IS Data/Spawns/Underworld/**.json", from);
                    break;
                case 4:
                    InvokeCommand("IS Data/Spawns/Underworld/DungeonCrates/**.json", from);
                    InvokeCommand("IS Data/Spawns/Underworld/Pedestals/**.json", from);
                    break;
                //case 5: InvokeCommand("WipeWildMobs", from); break;

                case 7:
                    InvokeCommand("ClearAll", from);
                    break;

                case 8: // THE FULL AUTOMATION
                    from.SendMessage(0x3F, "Starting full world population...");

                    // Surgically remove orphaned proxy spawns without wiping player data
                    //InvokeCommand("WipeWildMobs", from);

                    InvokeCommand("TelGen", from);
                    InvokeCommand("Decorate", from);
                    InvokeCommand("SignGen", from);
                    InvokeCommand("IS Data/Spawns/Overworld/**.json", from);
                    InvokeCommand("IS Data/Spawns/Underworld/**.json", from);
                    InvokeCommand("IS Data/Spawns/Underworld/DungeonCrates/**.json", from);
                    InvokeCommand("IS Data/Spawns/Underworld/Pedestals/**.json", from);

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
