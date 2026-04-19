using System;
using System.Collections.Generic;
using Server.Network;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;

namespace Server.Gumps
{
    public class CustomMoongateGump : Gump
    {
        private Mobile _from;
        private Item _gate;
        private string _selection;

        // Your 5 Custom Lands
        private static Dictionary<string, List<MoonEntry>> _lands = new()
        {
            // Felucca is Lodor Map 0
            { "Lodor", new List<MoonEntry> {
                new("Greensky Moongate", new Point3D(4199, 2516, 7), Map.Felucca),
                new("Islegem Moongate", new Point3D(2497, 1981, 5), Map.Felucca),
                new("Portshine Moongate", new Point3D(1045, 2258, 6), Map.Felucca),
                new("South Moongate", new Point3D(2350, 3619, 6), Map.Felucca),
                new("Springvale Moongate", new Point3D(4276, 1841, 16), Map.Felucca),
                new("Whisper Moongate", new Point3D(719, 962, 6), Map.Felucca),
                new("Winterlands Moongate", new Point3D(2876, 733, 9), Map.Felucca)
            } },
            // Trammel is Sosaria Map 1
            { "Sosaria", new List<MoonEntry>() {
                new("Central Moongate", new Point3D(2518, 1529, 3), Map.Trammel),
                new("Clues Moongate", new Point3D(3723, 2155, 4), Map.Trammel),
                new("Devil Guard Moongate", new Point3D(1779, 1714, 6), Map.Trammel),
                new("East Moongate", new Point3D(3718, 1136, 0), Map.Trammel),
                new("Frozen Isle Moongate", new Point3D(4970, 1297, 4), Map.Trammel),
                new("Montor Moongate", new Point3D(2548, 2685, 4), Map.Trammel),
                new("Moon Moongate", new Point3D(963, 514, 4), Map.Trammel),
                new("West Moongate", new Point3D(1052, 1570, 2), Map.Trammel),
                new("Yew Moongate", new Point3D(1792, 913, 27), Map.Trammel)
            } },
            { "Isles of Dread", new List<MoonEntry>() },
            { "Serpents Island", new List<MoonEntry>() },
            { "Savaged Empire", new List<MoonEntry>() }
        };

        public CustomMoongateGump(Mobile from, Item gate, string selection = null) : base(50, 50)
        {
            _from = from;
            _gate = gate;
            _selection = selection;

            AddPage(0);
            AddBackground(0, 0, 450, 320, 9270);
            AddAlphaRegion(10, 10, 430, 300);

            if (_selection == null)
                RenderLandSelection();
            else
                RenderLocationSelection();
        }

        private void RenderLandSelection()
        {
            AddLabel(65, 15, 1152, "SELECT DESTINATION LAND");
            int y = 45;

            int id = 1;
            foreach (string landName in _lands.Keys)
            {
                AddButton(20, y, 4005, 4007, id);
                AddLabel(55, y, 1152, landName);
                y += 30;
                id++;
            }
        }

        private void RenderLocationSelection()
        {
            AddLabel(65, 15, 1152, $"LOCATIONS: {_selection.ToUpper()}");
            int y = 45;

            var locations = _lands[_selection];
            for (int i = 0; i < locations.Count; i++)
            {
                AddButton(20, y, 4005, 4007, 100 + i);
                AddLabel(55, y, 1152, locations[i].Name);
                y += 30;
            }

            AddButton(20, 270, 4017, 4019, 0); // Back
            AddLabel(55, 270, 1152, "BACK");
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            if (info.ButtonID == 0)
            {
                if (_selection != null) _from.SendGump(new CustomMoongateGump(_from, _gate));
                return;
            }

            if (_selection == null)
            {
                int id = 1;
                foreach (string land in _lands.Keys)
                {
                    if (info.ButtonID == id)
                    {
                        _from.SendGump(new CustomMoongateGump(_from, _gate, land));
                        break;
                    }
                    id++;
                }
            }
            else
            {
                int index = info.ButtonID - 100;
                var entries = _lands[_selection];
                if (index >= 0 && index < entries.Count)
                {
                    var dest = entries[index];
                    BaseCreature.TeleportPets(_from, dest.Loc, dest.Map);
                    _from.MoveToWorld(dest.Loc, dest.Map);
                    _from.PlaySound(0x1FE);
                }
            }
        }
    }

    public class MoonEntry
    {
        public string Name { get; set; }
        public Point3D Loc { get; set; }
        public Map Map { get; set; }
        public MoonEntry(string name, Point3D loc, Map map) { Name = name; Loc = loc; Map = map; }
    }
}
