/*
 * Oz'Thoth's Static Tool - Animation Part 5 Gump
 * Modernized for .NET 10 / ModernUO
 */

using System;
using Server.Commands;

namespace Server.Gumps.OzThoth;

/// <summary>
/// Gump for selecting animated static items (Part 5).
/// </summary>
public sealed class AddAnimStPt5Gump : StaticItemGumpBase
{
    private static readonly int[] AnimationItemIds =
    [
        0x483E, 0x4841, 0x4844, 0x4847, 0x484A, 0x484D, 0x4850, 0x4853, 0x4856,
        0x4859, 0x485C, 0x485F, 0x4862, 0x4865, 0x4868, 0x486B, 0x486E, 0x4871,
        0x4874, 0x4877, 0x487A, 0x487D, 0x4880, 0x4883, 0x48D4, 0x48D8, 0x49CC,
        0x49D0, 0x5675, 0x5683, 0x571C, 0x5722, 0x5728, 0x572D, 0x5732, 0x573E,
        0x981D, 0x9822, 0x9DBD, 0x9DC9, 0x9DCD, 0x9DDA, 0x9DE5, 0x9DE9, 0x9FA1,
        0x9FA4
    ];

    protected override ReadOnlySpan<int> ItemIds => AnimationItemIds;
    protected override string CommandFormat => "{0}M Add Static {1}";
    protected override StaticItemGumpBase CreateInstance(int page) => new AddAnimStPt5Gump(page);

    public static void Initialize()
    {
        CommandSystem.Register("AddAnimStPt5", AccessLevel.GameMaster, AddAnimStPt5_OnCommand);
    }

    [Usage("AddAnimStPt5")]
    [Description("Displays a menu for adding animated statics (Part 5).")]
    private static void AddAnimStPt5_OnCommand(CommandEventArgs e)
    {
        e.Mobile.CloseGump<AddAnimStPt5Gump>();
        e.Mobile.SendGump(new AddAnimStPt5Gump());
    }

    public AddAnimStPt5Gump(int page = 0) : base(page)
    {
    }
}
