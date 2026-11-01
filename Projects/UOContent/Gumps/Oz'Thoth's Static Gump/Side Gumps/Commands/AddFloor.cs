/*
 * Oz'Thoth's Static Tool - Floor Category Gump
 * Modernized for .NET 10 / ModernUO
 */

using System;
using Server.Commands;

namespace Server.Gumps.OzThoth;

/// <summary>

/// Category menu for selecting floor sub-gumps.
/// </summary>
public sealed class AddFloorGump : CategoryGumpBase
{
    private static readonly CategoryEntry[] FloorCategories =
    [
        new("Part 1", "AddFloorPt1"),
        new("Part 2", "AddFloorPt2"),
        new("Part 3", "AddFloorPt3"),
        new("Part 4", "AddFloorPt4"),
        new("Part 5", "AddFloorPt5"),
        new("Part 6", "AddFloorPt6"),
        new("Part 7", "AddFloorPt7"),
        new("Part 8", "AddFloorPt8")
    ];

    protected override string Title => "Floor";
    protected override ReadOnlySpan<CategoryEntry> Categories => FloorCategories;
    protected override CategoryGumpBase CreateInstance() => new AddFloorGump();

    public static void Initialize()
    {
        CommandSystem.Register("AddFloor", AccessLevel.GameMaster, AddFloor_OnCommand);
    }

    [Usage("AddFloor")]
    [Description("Displays a menu from which you can interactively add floors.")]
    private static void AddFloor_OnCommand(CommandEventArgs e)
    {
        e.Mobile.CloseGump<AddFloorGump>();
        e.Mobile.SendGump(new AddFloorGump());
    }

    public AddFloorGump() : base()
    {
    }
}
