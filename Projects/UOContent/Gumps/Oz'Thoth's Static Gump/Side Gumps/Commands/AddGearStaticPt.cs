/*
 * Oz'Thoth's Static Tool - Gear Category Gump
 * Modernized for .NET 10 / ModernUO
 */

using System;
using Server.Commands;

namespace Server.Gumps.OzThoth;

/// <summary>
/// Category menu for selecting gear sub-gumps.
/// </summary>
public sealed class AddGearGump : CategoryGumpBase
{
    private static readonly CategoryEntry[] GearCategories =
    [
        new("Part 1", "AddGearPt1"),
        new("Part 2", "AddGearPt2"),
        new("Part 3", "AddGearPt3"),
        new("Part 4", "AddGearPt4"),
        new("Part 5", "AddGearPt5")
    ];

    protected override string Title => "Gear";
    protected override ReadOnlySpan<CategoryEntry> Categories => GearCategories;
    protected override CategoryGumpBase CreateInstance() => new AddGearGump();

    public static void Initialize()
    {
        CommandSystem.Register("AddGearStatic", AccessLevel.GameMaster, AddGearStatic_OnCommand);
    }

    [Usage("AddGearStatic")]
    [Description("Displays a menu from which you can interactively add gear.")]
    private static void AddGearStatic_OnCommand(CommandEventArgs e)
    {
        e.Mobile.CloseGump<AddGearGump>();
        e.Mobile.SendGump(new AddGearGump());
    }

    public AddGearGump() : base()
    {
    }
}
