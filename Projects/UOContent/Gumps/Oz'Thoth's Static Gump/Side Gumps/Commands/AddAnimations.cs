/*
 * Oz'Thoth's Static Tool - Animations Category Gump
 * Modernized for .NET 10 / ModernUO
 */

using System;
using Server.Commands;

namespace Server.Gumps.OzThoth;

/// <summary>
/// Category menu for selecting animation sub-gumps.
/// </summary>
public sealed class AddAnimationStaticGump : CategoryGumpBase
{
    private static readonly CategoryEntry[] AnimationCategories =
    [
        new("Part 1", "AddAnimStPt1"),
        new("Part 2", "AddAnimStPt2"),
        new("Part 3", "AddAnimStPt3"),
        new("Part 4", "AddAnimStPt4"),
        new("Part 5", "AddAnimStPt5")
    ];

    protected override string Title => "Animations";
    protected override ReadOnlySpan<CategoryEntry> Categories => AnimationCategories;
    protected override CategoryGumpBase CreateInstance() => new AddAnimationStaticGump();

    public static void Initialize()
    {
        CommandSystem.Register("AddAnimationStatic", AccessLevel.GameMaster, AddAnimationStatic_OnCommand);
    }

    [Usage("AddAnimationStatic")]
    [Description("Displays a menu from which you can interactively add animated statics.")]
    private static void AddAnimationStatic_OnCommand(CommandEventArgs e)
    {
        e.Mobile.CloseGump<AddAnimationStaticGump>();
        e.Mobile.SendGump(new AddAnimationStaticGump());
    }

    public AddAnimationStaticGump() : base()
    {
    }
}
