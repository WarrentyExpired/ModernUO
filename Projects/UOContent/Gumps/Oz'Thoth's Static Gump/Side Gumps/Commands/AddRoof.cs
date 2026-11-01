/*
 * Oz'Thoth's Static Tool - Roof Category Gump
 * Modernized for .NET 10 / ModernUO
 */

using System;
using Server.Commands;

namespace Server.Gumps.OzThoth;

/// <summary>
/// Category menu for selecting roof sub-gumps.
/// </summary>
public sealed class AddRoofGump : CategoryGumpBase
{
    private static readonly CategoryEntry[] RoofCategories =
    [
        new("Part 1", "AddRoofPt1"),
        new("Part 2", "AddRoofPt2"),
        new("Part 3", "AddRoofPt3"),
        new("Part 4", "AddRoofPt4")
    ];

    protected override string Title => "Roofs";
    protected override ReadOnlySpan<CategoryEntry> Categories => RoofCategories;
    protected override CategoryGumpBase CreateInstance() => new AddRoofGump();

    public static void Initialize()
    {
        CommandSystem.Register("AddRoof", AccessLevel.GameMaster, AddRoof_OnCommand);
    }

    [Usage("AddRoof")]
    [Description("Displays a menu from which you can interactively add roofs.")]
    private static void AddRoof_OnCommand(CommandEventArgs e)
    {
        e.Mobile.CloseGump<AddRoofGump>();
        e.Mobile.SendGump(new AddRoofGump());
    }

    public AddRoofGump() : base()
    {
    }
}
