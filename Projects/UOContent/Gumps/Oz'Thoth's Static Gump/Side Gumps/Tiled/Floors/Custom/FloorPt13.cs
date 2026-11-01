/*
 * Oz'Thoth's Static Tool - Floor Part 13 Gump
 * Modernized for .NET 10 / ModernUO
 */

using System;
using Server.Commands;

namespace Server.Gumps.OzThoth;

/// <summary>
/// Gump for selecting floor statics (Part 13).
/// </summary>
public sealed class AddFloorPt13Gump : StaticItemGumpBase
{
    private static readonly int[] FloorItemIds =
    [
        43041, 43042, 43043, 43044, 43045, 43046, 43047, 43048,
        43049, 43050, 43051, 43052, 43053, 43054, 43055, 43056,
        43057, 43058, 43059, 43060, 43061, 43062, 43063, 43064,
        43065, 43066, 43067, 43068, 43069, 43070, 43071, 43072,
        43073, 43074, 43075, 43077, 43078, 43079, 43080, 43081,
        43082, 43083, 43084, 43085, 43086, 43087, 43088, 43089,
        43090, 43091, 43092, 43094, 43095, 43096, 43097, 43098,
        43099, 43100, 43101, 43102, 43103, 43104, 43105, 43106,
        43107, 43108, 43109, 43111, 43112, 43113, 43114, 43115,
        43116, 43117, 43118, 43119, 43120, 43121, 43122, 43123,
        43124, 43125, 43126, 43128, 43129, 43130, 43131, 43132,
        43133, 43134, 43135, 43136, 43137, 43138, 43139, 43140,
        43141, 43142, 43143, 43145, 43146, 43147, 43148, 43149,
        43150, 43151, 43152, 43153, 43154, 43155, 43156, 43157,
        43158, 43159, 43160, 43267, 43268, 43269, 43270, 43271,
        43272, 43273, 43274, 43275, 43276, 43277, 43278, 43279,
        43280, 43281, 43282
    ];

    protected override ReadOnlySpan<int> ItemIds => FloorItemIds;
    protected override string CommandFormat => "{0}Tile Static {1}";
    protected override StaticItemGumpBase CreateInstance(int page) => new AddFloorPt13Gump(page);

    public static void Initialize()
    {
        CommandSystem.Register("AddFloorPt13", AccessLevel.GameMaster, AddFloorPt13_OnCommand);
    }

    [Usage("AddFloorPt13")]
    [Description("Displays a menu for adding floor statics (Part 13).")]
    private static void AddFloorPt13_OnCommand(CommandEventArgs e)
    {
        e.Mobile.CloseGump<AddFloorPt13Gump>();
        e.Mobile.SendGump(new AddFloorPt13Gump());
    }

    public AddFloorPt13Gump(int page = 0) : base(page)
    {
    }
}
