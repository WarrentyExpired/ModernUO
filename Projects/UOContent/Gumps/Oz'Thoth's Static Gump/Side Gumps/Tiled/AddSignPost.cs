/*
 * Oz'Thoth's Static Tool - Sign Post Item Gump
 * Modernized for .NET 10 / ModernUO
 */

using System;
using Server.Commands;

namespace Server.Gumps.OzThoth;

/// <summary>
/// Gump for selecting sign post static items to tile.
/// </summary>
public sealed class AddSignPStaticGump : StaticItemGumpBase
{
    private static readonly int[] SignPostItemIds =
    [
        2967, 2968, 2969, 2970, 2971, 2972, 2973, 2974,
        2975, 2976, 2977, 2978
    ];

    protected override ReadOnlySpan<int> ItemIds => SignPostItemIds;
    protected override string CommandFormat => "{0}M Tile Static {1}";
    protected override StaticItemGumpBase CreateInstance(int page) => new AddSignPStaticGump(page);

    public static void Initialize()
    {
        CommandSystem.Register("AddSignPStatic", AccessLevel.GameMaster, AddSignPStatic_OnCommand);
    }

    [Usage("AddSignPStatic")]
    [Description("Displays a menu for adding sign post statics.")]
    private static void AddSignPStatic_OnCommand(CommandEventArgs e)
    {
        e.Mobile.CloseGump<AddSignPStaticGump>();
        e.Mobile.SendGump(new AddSignPStaticGump());
    }

    public AddSignPStaticGump(int page = 0) : base(page)
    {
    }
}
