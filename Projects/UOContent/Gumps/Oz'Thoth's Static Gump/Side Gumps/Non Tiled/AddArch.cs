/*
 * Oz'Thoth's Static Tool - Arch Item Gump
 * Modernized for .NET 10 / ModernUO
 */

using System;
using Server.Commands;

namespace Server.Gumps.OzThoth;

/// <summary>
/// Gump for selecting arch static items.
/// </summary>
public sealed class AddArchGump : StaticItemGumpBase
{
    private static readonly int[] ArchItemIds =
    [
        40, 41, 42, 43, 44,
        69, 70, 71, 72, 73, 109, 110, 111, 112, 113,
        114, 115, 116, 117, 118, 119, 120, 121, 122, 123,
        124, 125, 126, 127, 134, 135, 136, 137, 138, 139,
        198, 205, 206, 207, 208, 209, 210, 211, 212, 213,
        214, 215, 216, 217, 218, 219, 220, 221, 222, 223,
        224, 225, 226, 227, 228, 229, 230, 231, 232, 233,
        234, 235, 236, 237, 238, 239, 240, 241, 242, 243,
        244, 245, 246, 247, 274, 275, 276, 277, 278, 279,
        280, 281, 282, 316, 317, 318, 319, 320, 321, 322,
        323, 324, 325, 326, 327, 328, 329, 330, 331, 364,
        365, 366, 367, 368, 469, 470, 471, 472, 473, 474,
        475, 476, 477, 478, 479, 549, 581, 582, 583, 584, 585,
        586, 688, 689, 690, 691, 692, 919, 920, 921, 922,
        923, 924, 925, 926, 927, 928, 1080, 1081, 1082, 1083,
        1084, 1085, 1086, 1087, 1088, 1089, 1090, 1091, 1092, 1093,
        1094, 1095, 1096, 1097, 1098, 1099, 1100, 1101, 1102, 1103,
        1104, 1105, 1106, 1107, 10716, 10717, 10718, 10719, 10720, 10721,
        10722, 10723, 10724, 10725, 11177, 11178, 11179, 11180, 11181, 11203,
        11204, 11205, 11206, 11711, 11712, 11713, 11714, 12059, 12060, 12061,
        12062, 13838, 13839, 13840, 13841, 13842, 16726, 16727, 16728, 16729,
        16740, 16741, 16748, 16749, 16750, 16751, 16752, 16753, 16754, 16755,
        16783, 16800, 16803, 16804, 30705, 30706, 30707, 30708, 30709
    ];

    protected override ReadOnlySpan<int> ItemIds => ArchItemIds;
    protected override string CommandFormat => "{0}M Add Static {1}";
    protected override StaticItemGumpBase CreateInstance(int page) => new AddArchGump(page);

    public static void Initialize()
    {
        CommandSystem.Register("AddArch", AccessLevel.GameMaster, AddArch_OnCommand);
    }

    [Usage("AddArch")]
    [Description("Displays a menu for adding arch statics.")]
    private static void AddArch_OnCommand(CommandEventArgs e)
    {
        e.Mobile.CloseGump<AddArchGump>();
        e.Mobile.SendGump(new AddArchGump());
    }

    public AddArchGump(int page = 0) : base(page)
    {
    }
}
