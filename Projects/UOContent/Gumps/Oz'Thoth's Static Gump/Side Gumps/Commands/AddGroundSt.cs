/*
 * Oz'Thoth's Static Tool - Ground Category Gump
 * Modernized for .NET 10 / ModernUO
 */

using System;
using Server.Commands;

namespace Server.Gumps.OzThoth;

/// <summary>
/// Category menu for selecting ground sub-gumps.
/// </summary>
public sealed class AddGroundStGump : CategoryGumpBase
{
    private static readonly CategoryEntry[] GroundCategories =
    [
        new("Part 1", "AddGroundStPt1"),
        new("Part 2", "AddGroundStPt2"),
        new("Part 3", "AddGroundStPt3"),
        new("Part 4", "AddGroundStPt4"),
        new("Part 5", "AddGroundStPt5")
    ];

    protected override string Title => "Ground";
    protected override ReadOnlySpan<CategoryEntry> Categories => GroundCategories;
    protected override CategoryGumpBase CreateInstance() => new AddGroundStGump();

    public static void Initialize()
    {
        CommandSystem.Register("AddGroundSt", AccessLevel.GameMaster, AddGroundSt_OnCommand);
    }

    [Usage("AddGroundSt")]
    [Description("Displays a menu from which you can interactively add ground statics.")]
    private static void AddGroundSt_OnCommand(CommandEventArgs e)
    {
        e.Mobile.CloseGump<AddGroundStGump>();
        e.Mobile.SendGump(new AddGroundStGump());
    }

    public AddGroundStGump() : base()
    {
    }
}
