/*
 * Oz'Thoth's Static Tool - Misc 1 Category Gump
 * Modernized for .NET 10 / ModernUO
 */

using System;
using Server.Commands;

namespace Server.Gumps.OzThoth;

/// <summary>
/// Category menu for selecting miscellaneous sub-gumps (Part 1).
/// </summary>
public sealed class AddMiscStatic1Gump : CategoryGumpBase
{
    private static readonly CategoryEntry[] MiscCategories =
    [
        new("Part 1", "AddMiscPt1"),
        new("Part 2", "AddMiscPt2"),
        new("Part 3", "AddMiscPt3"),
        new("Part 4", "AddMiscPt4"),
        new("Part 5", "AddMiscPt5"),
        new("Part 6", "AddMiscPt6"),
        new("Part 7", "AddMiscPt7"),
        new("Part 8", "AddMiscPt8"),
        new("Part 9", "AddMiscPt9"),
        new("Part 10", "AddMiscPt10"),
        new("Part 11", "AddMiscPt11"),
        new("Part 12", "AddMiscPt12"),
        new("Part 13", "AddMiscPt13"),
        new("Part 14", "AddMiscPt14"),
        new("Part 15", "AddMiscPt15"),
        new("Part 16", "AddMiscPt16"),
        new("Part 17", "AddMiscPt17"),
        new("Part 18", "AddMiscPt18"),
        new("Part 19", "AddMiscPt19"),
        new("Part 20", "AddMiscPt20")
    ];

    protected override string Title => "Misc. 1";
    protected override ReadOnlySpan<CategoryEntry> Categories => MiscCategories;
    protected override CategoryGumpBase CreateInstance() => new AddMiscStatic1Gump();

    public static void Initialize()
    {
        CommandSystem.Register("AddMiscStatic1", AccessLevel.GameMaster, AddMiscStatic1_OnCommand);
    }

    [Usage("AddMiscStatic1")]
    [Description("Displays a menu from which you can interactively add miscellaneous statics.")]
    private static void AddMiscStatic1_OnCommand(CommandEventArgs e)
    {
        e.Mobile.CloseGump<AddMiscStatic1Gump>();
        e.Mobile.SendGump(new AddMiscStatic1Gump());
    }

    public AddMiscStatic1Gump() : base()
    {
    }
}
