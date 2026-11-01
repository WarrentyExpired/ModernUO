/*
 * Oz'Thoth's Static Tool - Rock Item Gump
 * Modernized for .NET 10 / ModernUO
 */

using System;
using Server.Commands;

namespace Server.Gumps.OzThoth;

/// <summary>
/// Gump for selecting rock static items.
/// </summary>
public sealed class AddRockGump : StaticItemGumpBase
{
    private static readonly int[] RockItemIds =
    [
        2272, 2273, 2274, 2275, 2276, 2277, 2278, 2279, 2280,
        2281, 2282, 4943, 4944, 4945, 4946, 4947, 4948, 4949,
        4950, 4951, 4952, 4953, 4954, 4955, 4956, 4957, 4958,
        4959, 4960, 4961, 4962, 4963, 4964, 4965, 4966, 4967,
        4968, 4969, 4970, 4971, 4972, 4973, 6001, 6002, 6003,
        6004, 6005, 6006, 6007, 6008, 6009, 6010, 6011, 6012,
        13345, 13346, 13347, 13348, 13350, 13351, 13352, 13353,
        13356, 13357, 13358, 13359, 13360, 13446,
        13447, 13448, 13449, 13450, 13451, 13452, 13453, 13454, 13455,
        13484, 13485, 13486, 13487, 13488, 13489, 13490, 13491, 13492,
        22024, 22025, 22026, 22027, 22028, 22029, 22030, 22031, 22032,
        22033, 22062, 22063, 22064, 22065, 22066, 22067, 22068, 22069,
        22070
    ];

    protected override ReadOnlySpan<int> ItemIds => RockItemIds;
    protected override string CommandFormat => "{0}M Add Static {1}";
    protected override StaticItemGumpBase CreateInstance(int page) => new AddRockGump(page);

    public static void Initialize()
    {
        CommandSystem.Register("AddRock", AccessLevel.GameMaster, AddRock_OnCommand);
    }

    [Usage("AddRock")]
    [Description("Displays a menu for adding rock statics.")]
    private static void AddRock_OnCommand(CommandEventArgs e)
    {
        e.Mobile.CloseGump<AddRockGump>();
        e.Mobile.SendGump(new AddRockGump());
    }

    public AddRockGump(int page = 0) : base(page)
    {
    }
}
