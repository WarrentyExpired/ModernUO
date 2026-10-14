using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Items;
namespace Server.Gumps;

public class CustomMoongateGump : Gump
{
    // ======================================================================
    // FILL IN YOUR DESTINATIONS HERE
    // Map placeholders are set to Trammel. Update them to Trammel, Ilshenar, etc. as needed.
    // ======================================================================
    private static readonly Destination[] _destinations =
    {
        new("Bastion Moongate", new Point3D(1920, 2111, 0), Map.Trammel),
        new("Rivermouth Moongate", new Point3D(1075, 1793, 2), Map.Trammel),
        new("Myrish Moongate", new Point3D(1338, 2791, 2), Map.Trammel),
        new("Peninsula Moongate", new Point3D(2037, 1398, 0), Map.Trammel),
        new("Trails Moongate", new Point3D(3169, 2871, 0), Map.Trammel),
        new("Coldbarrow Moongate", new Point3D(3285, 2331, 0), Map.Trammel),
        new("Draco Island Moongate", new Point3D(3495, 3491, 2), Map.Trammel),
        new("Piece Island Moongate", new Point3D(754, 3458, 0), Map.Trammel),
        new("Shiver Island Moongate", new Point3D(3012, 445, 8), Map.Trammel)
    };

    private readonly CustomMoongate _gate;

    public CustomMoongateGump(CustomMoongate gate) : base(100, 100)
    {
        _gate = gate;
        Closable = true;
        Disposable = true;
        Resizable = false;

        AddPage(0);

        // Background and Alpha Region for a clean, modern look
        AddBackground(0, 0, 350, 370, 9270);
        AddAlphaRegion(10, 10, 330, 350);

        AddHtml(0, 15, 350, 20, "<CENTER><BASEFONT COLOR=#FFFFFF>Select Destination</BASEFONT></CENTER>", false, false);

        // Dynamically build the 8 buttons based on the array above
        int yOffset = 50;
        for (int i = 0; i < _destinations.Length; i++)
        {
            // ButtonID is offset by 1 because ButtonID 0 is always reserved for right-click/close
            AddButton(25, yOffset, 4005, 4007, i + 1, GumpButtonType.Reply, 0);
            AddLabel(60, yOffset, 1152, _destinations[i].Name); // 1152 is a crisp white text hue

            yOffset += 30; // Spacing between buttons
        }
    }

    public override void OnResponse(NetState sender, in RelayInfo info)
    {
        var mobile = sender.Mobile;

        if (mobile == null || mobile.Deleted || !mobile.Alive)
            return;

        var buttonId = info.ButtonID;

        // If they right-clicked to close the gump, do nothing
        if (buttonId == 0)
            return;

        if (_gate == null || _gate.Deleted || mobile.Map != _gate.Map || !mobile.InRange(_gate.GetWorldLocation(), 2))
        {
            mobile.SendMessage("You have moved too far away from the moongate.");
            return;
        }

        // Subtract 1 from the ButtonID to get the actual array index (0 through 7)
        var index = buttonId - 1;

        if (index >= 0 && index < _destinations.Length)
        {
            var dest = _destinations[index];

            if (dest.Map == null || dest.Map == Map.Internal)
            {
                mobile.SendMessage("That destination is currently unavailable.");
                return;
            }

            // Move the player to the selected destination
            mobile.MoveToWorld(dest.Location, dest.Map);

            // Play teleport sound and standard moongate visual effect
            mobile.PlaySound(0x1FE);
            Effects.SendLocationEffect(mobile.Location, mobile.Map, 0x3728, 10, 10);
        }
    }

    // Helper class to neatly store the destination data
    private class Destination
    {
        public string Name { get; }
        public Point3D Location { get; }
        public Map Map { get; }

        public Destination(string name, Point3D loc, Map map)
        {
            Name = name;
            Location = loc;
            Map = map;
        }
    }
}
