/*
 * Oz'Thoth's Static Tool - Plants Category Gump
 * Modernized for .NET 10 / ModernUO
 */

using System;
using Server.Commands;

namespace Server.Gumps.OzThoth;

/// <summary>
/// Category menu for selecting plant sub-gumps.
/// </summary>
public sealed class AddPlantGump : CategoryGumpBase
{
    private static readonly CategoryEntry[] PlantCategories =
    [
        new("Part 1", "AddPlantPt1"),
        new("Part 2", "AddPlantPt2"),
        new("Part 3", "AddPlantPt3"),
        new("Part 4", "AddPlantPt4")
    ];

    protected override string Title => "Plants";
    protected override ReadOnlySpan<CategoryEntry> Categories => PlantCategories;
    protected override CategoryGumpBase CreateInstance() => new AddPlantGump();

    public static void Initialize()
    {
        CommandSystem.Register("AddPlants", AccessLevel.GameMaster, AddPlants_OnCommand);
    }

    [Usage("AddPlants")]
    [Description("Displays a menu from which you can interactively add plants.")]
    private static void AddPlants_OnCommand(CommandEventArgs e)
    {
        e.Mobile.CloseGump<AddPlantGump>();
        e.Mobile.SendGump(new AddPlantGump());
    }

    public AddPlantGump() : base()
    {
    }
}
