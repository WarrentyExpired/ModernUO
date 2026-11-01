/*
 * Oz'Thoth's Static Tool - Animation Part 6 Gump
 * Modernized for .NET 10 / ModernUO
 */

using System;
using Server.Commands;

namespace Server.Gumps.OzThoth;

/// <summary>
/// Gump for selecting animation statics (Part 6).
/// </summary>
public sealed class AddAnimStPt6Gump : StaticItemGumpBase
{
    private static readonly int[] AnimationItemIds =
    [
        0xA112, 0xA136, 0xA14B, 0xA157, 0xA15D, 0xA164, 0xA16B, 0xA17B, 0xA193,
        0xA197, 0xA19A, 0xA1A0, 0xA1B4, 0xA1B7, 0xA1BC, 0xA1C1, 0xA1CA, 0xA1DF,
        0xA1E2, 0xA1E5, 0xA1F4, 0xA205, 0xA20D, 0xA210, 0xA213, 0xA222, 0xA226,
        0xA22F, 0xA23A, 0xA243, 0xA25E, 0xA266, 0xA27E, 0xA285, 0xA28D, 0xA292,
        0xA298, 0xA2A3, 0xA2A7, 0xA2AB, 0xA2B9, 0xA2BE, 0xA574, 0xA592, 0xA595,
        0xA598, 0xA59B, 0xA944, 0xA948, 0xA94D, 0xA952, 0xA956, 0xA95A, 0xA95E,
        0xA962
    ];

    protected override ReadOnlySpan<int> ItemIds => AnimationItemIds;
    protected override string CommandFormat => "{0}M Add Static {1}";
    protected override StaticItemGumpBase CreateInstance(int page) => new AddAnimStPt6Gump(page);

    public static void Initialize()
    {
        CommandSystem.Register("AddAnimStPt6", AccessLevel.GameMaster, AddAnimStPt6_OnCommand);
    }

    [Usage("AddAnimStPt6")]
    [Description("Displays a menu for adding animation statics (Part 6).")]
    private static void AddAnimStPt6_OnCommand(CommandEventArgs e)
    {
        e.Mobile.CloseGump<AddAnimStPt6Gump>();
        e.Mobile.SendGump(new AddAnimStPt6Gump());
    }

    public AddAnimStPt6Gump(int page = 0) : base(page)
    {
    }
}
