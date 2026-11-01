/*
 * Oz'Thoth's Static Tool - Cooking/Food Item Gump
 * Modernized for .NET 10 / ModernUO
 */

using System;
using Server.Commands;

namespace Server.Gumps.OzThoth;

/// <summary>
/// Gump for selecting cooking/food static items.
/// </summary>
public sealed class AddCookingStaticGump : StaticItemGumpBase
{
    private static readonly int[] CookingItemIds =
    [
        539, 2416, 2417, 2418, 2419, 2420, 2421, 2422,
        2423, 2424, 2425, 2426, 2427, 2428, 2429, 2430,
        2431, 2444, 2445, 2446, 2448, 2449, 2450, 2451,
        2452, 2453, 2454, 2455, 2456, 2457, 2458, 2459,
        2460, 2461, 2462, 2463, 2464, 2465, 2466, 2467,
        2468, 2469, 2470, 2471, 2476, 2477, 2478, 2479,
        2480, 2481, 2482, 2483, 2484, 2485, 2486, 2487,
        2488, 2489, 2490, 2491, 2492, 2493, 2494, 2495,
        2496, 2497, 2498, 2499, 2500, 2501, 2502, 2503,
        2504, 2505, 2506, 2507, 2508, 2509, 2510, 2511,
        2512, 2513, 2514, 2515, 2516, 2517, 2518, 2519,
        2520, 2521, 2522, 2523, 2524, 2525, 2526, 2527,
        2528, 2529, 2530, 2531, 2532, 2533, 2534, 2535,
        2536, 2537, 2538, 2539, 2540, 2541, 2542, 2543,
        2544, 2545, 2546, 2547, 2548, 2549, 2550, 2551,
        2552, 2553, 2554, 3164, 3165, 3172, 3173, 3174,
        3175, 3178, 3179, 3180, 3181, 3182, 3184, 3185,
        3186, 3187, 3188, 3189, 3191, 3192, 3193, 3194,
        3195, 3196, 3199, 3200, 3201, 3202, 3354, 3542,
        3543, 3544, 3545, 4086, 4087, 4088, 4089, 4090,
        4091, 4092, 4093, 4094, 4095, 4096, 4097, 4098,
        4099, 4100, 4101, 4102, 4103, 4104, 4105, 4153,
        4154, 4155, 4156, 4157, 4158, 4159, 4160, 4161,
        4162, 4163, 4164, 4165, 4166, 4227, 5628, 5629,
        5630, 5631, 5632, 5633, 5634, 5635, 5636, 5637,
        5638, 5639, 5640, 5641, 5642, 5643, 5644, 5917,
        5918, 5919, 5920, 5921, 5922, 5923, 5924, 5925,
        5926, 5927, 5928, 5929, 5930, 5931, 5932, 5933,
        7701, 7702, 7703, 7704, 7705, 7706, 7707, 7708,
        7709, 7710, 7711, 7811, 7812, 7813, 7814, 7815,
        7816, 7817, 7818, 7819, 7820, 7821, 7822, 7823,
        7824, 7825, 7826, 7827, 7828, 7829, 7830, 7831,
        7832, 7833, 9448, 9449, 10302, 10303, 10304, 10305,
        10315, 10316, 10317, 10318, 10319, 10320, 12247, 12248,
        15641, 15642, 15643, 15644, 18060, 18061, 18062, 18063,
        18064, 18077, 18078, 18079, 18080, 18081
    ];

    protected override ReadOnlySpan<int> ItemIds => CookingItemIds;
    protected override string CommandFormat => "{0}M Add Static {1}";
    protected override StaticItemGumpBase CreateInstance(int page) => new AddCookingStaticGump(page);

    public static void Initialize()
    {
        CommandSystem.Register("AddCookingStatic", AccessLevel.GameMaster, AddCookingStatic_OnCommand);
    }

    [Usage("AddCookingStatic")]
    [Description("Displays a menu for adding cooking/food statics.")]
    private static void AddCookingStatic_OnCommand(CommandEventArgs e)
    {
        e.Mobile.CloseGump<AddCookingStaticGump>();
        e.Mobile.SendGump(new AddCookingStaticGump());
    }

    public AddCookingStaticGump(int page = 0) : base(page)
    {
    }
}
