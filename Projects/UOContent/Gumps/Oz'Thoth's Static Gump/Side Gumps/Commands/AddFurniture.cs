/*
 * Oz'Thoth's Static Tool - Furniture Category Gump
 * Modernized for .NET 10 / ModernUO
 */

using System;
using Server.Commands;

namespace Server.Gumps.OzThoth;

/// <summary>
/// Category menu for selecting furniture sub-gumps.
/// </summary>
public sealed class AddFurnitureGump : CategoryGumpBase
{
    private static readonly CategoryEntry[] FurnitureCategories =
    [
        new("Part 1", "AddFurnPt1"),
        new("Part 2", "AddFurnPt2"),
        new("Part 3", "AddFurnPt3"),
        new("Part 4", "AddFurnPt4")
    ];

    protected override string Title => "Furniture";
    protected override ReadOnlySpan<CategoryEntry> Categories => FurnitureCategories;
    protected override CategoryGumpBase CreateInstance() => new AddFurnitureGump();

    public static void Initialize()
    {
        CommandSystem.Register("AddFurniture", AccessLevel.GameMaster, AddFurniture_OnCommand);
    }

    [Usage("AddFurniture")]
    [Description("Displays a menu from which you can interactively add furniture.")]
    private static void AddFurniture_OnCommand(CommandEventArgs e)
    {
        e.Mobile.CloseGump<AddFurnitureGump>();
        e.Mobile.SendGump(new AddFurnitureGump());
    }

    public AddFurnitureGump() : base()
    {
    }
}
