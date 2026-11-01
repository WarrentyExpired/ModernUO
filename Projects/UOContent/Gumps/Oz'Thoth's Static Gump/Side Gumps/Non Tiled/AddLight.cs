/*
 * Oz'Thoth's Static Tool - Lights Item Gump
 * Modernized for .NET 10 / ModernUO
 */

using System;
using Server.Commands;

namespace Server.Gumps.OzThoth;

/// <summary>
/// Gump for selecting light static items.
/// </summary>
public sealed class AddLightsStaticGump : StaticItemGumpBase
{
    private static readonly int[] LightItemIds =
    [
        2555, 2556, 2557, 2558, 2559, 2560, 2561,
        2562, 2563, 2564, 2565, 2566, 2567, 2568,
        2569, 2570, 2571, 2572, 2573, 2574, 2575,
        2576, 2577, 2578, 2579, 2580, 2581, 2582,
        2583, 2584, 2585, 2586, 2587, 2588, 2589,
        2594, 2595, 2596, 2597, 2598, 2599, 2600,
        2601, 2842, 2843, 2844, 2845, 2846, 2847,
        2848, 2849, 2850, 2851, 2852, 2853, 2854,
        2855, 2856, 3553, 3554, 3555, 3556, 3557,
        3558, 3559, 3560, 3561, 3562, 5164, 5165,
        5166, 5167, 5168, 5169, 5170, 5171, 5172,
        5173, 5174, 5175, 6227, 6228, 6229, 6230,
        6231, 6232, 6233, 6234, 6570, 6587, 7288,
        7289, 7290, 15764, 15765, 15766, 15767, 15768,
        15769, 15770, 15771
    ];

    protected override ReadOnlySpan<int> ItemIds => LightItemIds;
    protected override string CommandFormat => "{0}M Add Static {1}";
    protected override StaticItemGumpBase CreateInstance(int page) => new AddLightsStaticGump(page);

    public static void Initialize()
    {
        CommandSystem.Register("AddLightsStatic", AccessLevel.GameMaster, AddLightsStatic_OnCommand);
    }

    [Usage("AddLightsStatic")]
    [Description("Displays a menu for adding light statics.")]
    private static void AddLightsStatic_OnCommand(CommandEventArgs e)
    {
        e.Mobile.CloseGump<AddLightsStaticGump>();
        e.Mobile.SendGump(new AddLightsStaticGump());
    }

    public AddLightsStaticGump(int page = 0) : base(page)
    {
    }
}
