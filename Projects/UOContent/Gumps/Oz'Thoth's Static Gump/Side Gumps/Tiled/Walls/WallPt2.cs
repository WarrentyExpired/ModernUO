/*
 * Oz'Thoth's Static Tool - Wall Part 2 Gump
 * Modernized for .NET 10 / ModernUO
 */

using System;
using Server.Commands;

namespace Server.Gumps.OzThoth;

/// <summary>
/// Gump for selecting wall static items (Part 2).
/// </summary>
public sealed class AddWallPt2Gump : StaticItemGumpBase
{
    private static readonly int[] WallItemIds =
    [
        593, 594, 595, 596, 597, 598, 599, 600, 601, 602, 
        603, 631, 632, 633, 634, 635, 636, 641, 642, 657, 
        658, 659, 660, 661, 662, 663, 664, 665, 666, 667, 
        668, 669, 670, 671, 672, 673, 674, 675, 676, 677, 
        678, 679, 680, 681, 682, 683, 684, 685, 686, 693, 
        694, 695, 696, 697, 698, 699, 700, 701, 702, 703, 
        704, 711, 712, 713, 714, 715, 716, 717, 718, 719, 
        720, 721, 722, 723, 724, 725, 726, 727, 728, 729,
        734, 735, 736, 737, 738, 739, 740, 741, 742, 743, 
        744, 745, 746, 747, 748, 761, 762, 763, 764, 765, 
        766, 767, 768, 872, 873, 874, 875, 876, 877, 878, 
        879, 880, 881, 895, 896, 897, 898, 899, 900, 901, 
        902, 903, 904, 905, 906, 907, 908, 909, 910, 911, 
        912, 913, 914, 915, 916, 917, 918, 947, 948, 949, 
        950, 951, 952, 953, 954, 955, 956, 957, 958, 959, 
        960, 961, 962, 963, 964, 965, 966, 967, 968, 969, 
        970, 971, 972, 973, 974, 975, 976, 977, 978, 979, 
        980, 981, 982, 983, 984, 985, 986, 987, 988, 989, 
        990, 991, 992, 993, 994, 8584, 8585, 8586, 8587, 8588, 
        8589, 8590, 8591, 8592, 8593, 8594, 8595, 8596, 8597, 8598, 
        8599, 8613, 8614, 8615, 8616, 8617, 8618, 8619, 8620, 8621, 
        8622, 8623, 8624, 8625, 8626, 8627, 8628, 8629, 8630, 8631, 
        8632, 8637, 8638, 8639, 8640, 8641, 8642, 8643, 8644, 8645, 
        8646, 8647, 8648, 8649, 8650, 8651, 8652, 8653, 8654, 8655, 
        8656, 8657, 8658, 8659, 8660, 8661, 8662, 8663, 8664, 8665, 
        8666, 8667, 8668, 8669, 8670, 8671, 8672, 8673, 8674, 8675, 
        8676, 8677, 8678, 8679, 8680
    ];

    protected override ReadOnlySpan<int> ItemIds => WallItemIds;
    protected override string CommandFormat => "{0}Tile Static {1}";
    protected override StaticItemGumpBase CreateInstance(int page) => new AddWallPt2Gump(page);

    public static void Initialize()
    {
        CommandSystem.Register("AddWallPt2", AccessLevel.GameMaster, AddWallPt2_OnCommand);
    }

    [Usage("AddWallPt2")]
    [Description("Displays a menu for adding wall statics (Part 2).")]
    private static void AddWallPt2_OnCommand(CommandEventArgs e)
    {
        e.Mobile.CloseGump<AddWallPt2Gump>();
        e.Mobile.SendGump(new AddWallPt2Gump());
    }

    public AddWallPt2Gump(int page = 0) : base(page)
    {
    }
}
