using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using Server.Accounting;
using Server.Collections;
using Server.Commands;
using Server.Maps;
using Server.Misc;
using Server.Multis;
using Server.Network;
using Server.Prompts;
using Server.Saves;
using Server.Text;

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
            AddBackground(0, 0, 450, 320, 9270);
            AddAlphaRegion(10, 10, 430, 300);

            AddLabel(65, 15, 1152, "WORLD BUILDING");
            int y = 45;

            AddLabel(20, y, 1152, "Core Generation");
            y += 30;

            AddButton(20, y, 4005, 4007, 1);
            AddLabel(55, y, 1152, "Teleporter Gen (TelGen) from Data/Decoration/teleporter.json");
            y += 30;

            AddButton(20, y, 4005, 4007, 2);
            AddLabel(55, y, 1152, "World Decoration from Data/Decoration/<Map Folder>/");
            y += 30;

            AddButton(20, y, 4005, 4007, 3);
            AddLabel(55, y, 1152, "Add Spawners from Data/Spawners/*.json");
            y += 30;

            AddButton(20, y, 4005, 4007, 4);
            AddLabel(55, y, 1152, "Export Spawners from Map to Data/Spawners/exported.json");
            y += 30;

            AddButton(20, y, 4005, 4007, 5);
            AddLabel(55, y, 1152, "Generate ThruDoors from Data/Decoration/thrudoors.json");
            y += 30;

            AddButton(20, y, 4017, 4019, 0);
            AddLabel(55, y, 1152, "Close Menu");
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            Mobile from = sender.Mobile;

            switch (info.ButtonID)
            {
                case 1: InvokeCommand("TelGen"); break;
                case 2: InvokeCommand("Decorate"); break;
                case 3: InvokeCommand("GenerateSpawners Data/Spawners/**.json"); break;
                case 4: InvokeCommand("ExportSpawners"); break;
                case 5: InvokeCommand("GenThruDoor"); break;
                default: return;
            }
            from.SendGump(new WorldBuildingGump(from));
        }
        private void InvokeCommand(string c)
        {
            CommandSystem.Handle(m_From, $"{CommandSystem.Prefix}{c}");
        }
    }
}
