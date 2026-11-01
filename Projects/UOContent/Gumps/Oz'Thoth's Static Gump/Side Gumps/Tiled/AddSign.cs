/*
 * Oz'Thoth's Static Tool - Sign Post Item Gump
 * Modernized for .NET 10 / ModernUO
 */

using System;
using Server.Commands;

namespace Server.Gumps.OzThoth;

/// <summary>
/// Gump for selecting sign static items to tile.
/// </summary>
public sealed class AddSignStaticGump : StaticItemGumpBase
{
    private static readonly int[] SignItemIds =
    [
                                            2965, 2966, 2979, 2980,
        2981, 2982, 2983, 2984, 2985, 2986, 2987, 2988, 2989, 2990,
        2991, 2992, 2993, 2994, 2995, 2996, 2997, 2998, 2999, 3000,
        3001, 3002, 3003, 3004, 3005, 3006, 3007, 3008, 3009, 3010,
        3011, 3012, 3013, 3014, 3015, 3016, 3017, 3018, 3019, 3020,
        3021, 3022, 3023, 3024, 3025, 3026, 3027, 3028, 3029, 3030,
        3031, 3032, 3033, 3034, 3035, 3036, 3037, 3038, 3039, 3040,
        3041, 3042, 3043, 3044, 3045, 3046, 3047, 3048, 3049, 3050,
        3051, 3052, 3053, 3054, 3055, 3056, 3057, 3058, 3059, 3060,
        3061, 3062, 3063, 3064, 3065, 3066, 3067, 3068, 3069, 3070,
        3071, 3072, 3073, 3074, 3075, 3076, 3077, 3078, 3079, 3080,
        3081, 3082, 3083, 3084, 3085, 3086, 3039, 3040, 7976, 7977
    ];

    protected override ReadOnlySpan<int> ItemIds => SignItemIds;
    protected override string CommandFormat => "{0}M Add Static {1}";
    protected override StaticItemGumpBase CreateInstance(int page) => new AddSignStaticGump(page);

    public static void Initialize()
    {
        CommandSystem.Register("AddSignStatic", AccessLevel.GameMaster, AddSignStatic_OnCommand);
    }

    [Usage("AddSignStatic")]
    [Description("Displays a menu for adding sign statics.")]
    private static void AddSignStatic_OnCommand(CommandEventArgs e)
    {
        e.Mobile.CloseGump<AddSignStaticGump>();
        e.Mobile.SendGump(new AddSignStaticGump());
    }

    public AddSignStaticGump(int page = 0) : base(page)
    {
    }
}
