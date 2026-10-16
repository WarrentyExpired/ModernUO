using System;
using Server;
using Server.Gumps;
using Server.Misc; // Needed for NameList.RandomName

namespace Server.Gumps
{
    public class TreasureLocationGump : Gump
    {
        public TreasureLocationGump(int level, Point2D loc, Map map) : base(100, 100)
        {
            Closable = true;
            Disposable = true;
            Resizable = false;

            AddPage(0);

            AddBackground(0, 0, 350, 260, 9270);
            AddAlphaRegion(10, 10, 330, 240);

            // Header
            AddHtml(0, 15, 350, 20, $"<CENTER><BASEFONT COLOR=#FFD700>Level {level} Treasure Map</BASEFONT></CENTER>", false, false);

            // Fetch the narrative lore
            string flavorText = GetFlavorText(level, loc);

            // Story Text Area
            AddHtml(25, 50, 300, 120, $"<BASEFONT COLOR=#FFFFFF>{flavorText}</BASEFONT>", false, false);

            // Footer / Clean Coordinate Readout
            AddHtml(0, 180, 350, 20, $"<CENTER><BASEFONT COLOR=#AAAAAA>Facet: {map.Name}</BASEFONT></CENTER>", false, false);
            AddHtml(0, 205, 350, 20, $"<CENTER><BASEFONT COLOR=#FFFFFF>Target Coordinates: {loc.X}, {loc.Y}</BASEFONT></CENTER>", false, false);
        }

        private string GetFlavorText(int level, Point2D loc)
        {
            // We use standard "male" or "female" generic name lists to ensure it never crashes looking for a custom list
            string randomMage = NameList.RandomName(Utility.RandomBool() ? "male" : "female");

            return level switch
            {
                1 => $"In my haste I had to dump my treasure at {loc.X}, {loc.Y}. I hope to return for it soon before the scavengers find it.",

                2 => $"The local guard patrol got too close. I buried the stash at {loc.X}, {loc.Y} and marked the spot. They'll never find it.",

                3 => $"We lost half the crew to the beasts in these woods. I hid the remaining loot at {loc.X}, {loc.Y} so we can travel light.",

                4 => $"The dead walk this land. I dared not carry the gold any further. I sealed it away at {loc.X}, {loc.Y}, but I fear they watch it now.",

                5 => $"The Evil Mage {randomMage} found me! I stashed the treasure at {loc.X}, {loc.Y}. Be warned though, the treasure has a curse on it that I haven't figured out how to break yet.",

                6 => $"A creature of ancient, unimaginable power claimed this territory. I buried the chest deep at {loc.X}, {loc.Y}. If you are reading this, I am already dead.",

                _ => $"A modest stash of goods hidden away at {loc.X}, {loc.Y}."
            };
        }
    }
}
