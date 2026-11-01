/*
 * Oz'Thoth's Static Tool - Misc 2 Category Gump
 * Modernized for .NET 10 / ModernUO
 */

using System;
using Server.Commands;

namespace Server.Gumps.OzThoth;

/// <summary>
/// Category menu for selecting miscellaneous sub-gumps (Part 2).
/// </summary>
public sealed class AddMiscStatic2Gump : CategoryGumpBase
{
    private static readonly CategoryEntry[] MiscCategories =
    [
        new("Part 1", "AddMisc2Pt1"),
        new("Part 2", "AddMisc2Pt2"),
        new("Part 3", "AddMisc2Pt3"),
        new("Part 4", "AddMisc2Pt4"),
        new("Part 5", "AddMisc2Pt5"),
        new("Part 6", "AddMisc2Pt6"),
        new("Part 7", "AddMisc2Pt7"),
        new("Part 8", "AddMisc2Pt8"),
        new("Part 9", "AddMisc2Pt9"),
        new("Part 10", "AddMisc2Pt10"),
        new("Part 11", "AddMisc2Pt11"),
        new("Part 12", "AddMisc2Pt12"),
        new("Part 13", "AddMisc2Pt13"),
        new("Part 14", "AddMisc2Pt14"),
        new("Part 15", "AddMisc2Pt15"),
        new("Part 16", "AddMisc2Pt16"),
        new("Part 17", "AddMisc2Pt17"),
        new("Part 18", "AddMisc2Pt18"),
        new("Part 19", "AddMisc2Pt19"),
        new("Part 20", "AddMisc2Pt20")
    ];

    protected override string Title => "Misc. 2";
    protected override ReadOnlySpan<CategoryEntry> Categories => MiscCategories;
    protected override CategoryGumpBase CreateInstance() => new AddMiscStatic2Gump();

    public static void Initialize()
    {
        CommandSystem.Register("AddMiscStatic2", AccessLevel.GameMaster, AddMiscStatic2_OnCommand);
    }

    [Usage("AddMiscStatic2")]
    [Description("Displays a menu from which you can interactively add miscellaneous statics (Part 2).")]
    private static void AddMiscStatic2_OnCommand(CommandEventArgs e)
    {
        e.Mobile.CloseGump<AddMiscStatic2Gump>();
        e.Mobile.SendGump(new AddMiscStatic2Gump());
    }

    public AddMiscStatic2Gump() : base()
    {
    }
}
