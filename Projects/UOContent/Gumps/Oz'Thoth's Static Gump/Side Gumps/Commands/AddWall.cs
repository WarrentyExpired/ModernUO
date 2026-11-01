/*
 * Oz'Thoth's Static Tool - Wall Category Gump
 * Modernized for .NET 10 / ModernUO
 */

using System;
using Server.Commands;

namespace Server.Gumps.OzThoth;

/// <summary>
/// Category menu for selecting wall sub-gumps.
/// </summary>
public sealed class AddWallGump : CategoryGumpBase
{
    private static readonly CategoryEntry[] WallCategories =
    [
        new("Part 1", "AddWallPt1"),
        new("Part 2", "AddWallPt2"),
        new("Part 3", "AddWallPt3"),
        new("Part 4", "AddWallPt4"),
        new("Part 5", "AddWallPt5"),
        new("Part 6", "AddWallPt6")
    ];

    protected override string Title => "Walls";
    protected override ReadOnlySpan<CategoryEntry> Categories => WallCategories;
    protected override CategoryGumpBase CreateInstance() => new AddWallGump();

    public static void Initialize()
    {
        CommandSystem.Register("AddWall", AccessLevel.GameMaster, AddWall_OnCommand);
    }

    [Usage("AddWall")]
    [Description("Displays a menu from which you can interactively add walls.")]
    private static void AddWall_OnCommand(CommandEventArgs e)
    {
        e.Mobile.CloseGump<AddWallGump>();
        e.Mobile.SendGump(new AddWallGump());
    }

    public AddWallGump() : base()
    {
    }
}
