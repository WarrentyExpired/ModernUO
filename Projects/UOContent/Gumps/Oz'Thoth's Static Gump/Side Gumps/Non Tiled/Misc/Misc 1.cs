/*
 * Oz'Thoth's Static Tool - Misc Part 1 Gump
 * Modernized for .NET 10 / ModernUO
 */

using System;
using Server.Commands;

namespace Server.Gumps.OzThoth;

/// <summary>
/// Gump for selecting miscellaneous static items (Part 1).
/// </summary>
public sealed class AddMiscPt1Gump : StaticItemGumpBase
{
    private static readonly int[] MiscItemIds =
    [
        2, 3, 4, 5, 525, 526, 540, 570, 571, 
        572, 573, 574, 575, 576, 587, 604, 605, 606, 
        607, 608, 609, 610, 611, 612, 613, 614, 615, 
        616, 617, 618, 619, 620, 621, 622, 623, 624, 
        625, 626, 627, 628, 629, 630, 637, 638, 639, 
        640, 730, 749, 750, 751, 752, 753, 754, 755, 
        756, 882, 883, 884, 885, 886, 887, 888, 889, 
        890, 891, 892, 893, 930, 931, 932, 933, 934, 
        935, 936, 937, 938, 939, 940, 941, 942, 943, 
        944, 945, 946, 995, 996, 997, 998, 999, 1027, 
        1028, 1029, 1030, 1031, 1032, 1033, 1034, 1035, 1036, 
        1037, 1038, 1039, 1040, 1041, 1042, 1043, 1044, 1045, 
        1046, 1047, 1048, 1049, 1050, 1051, 1052, 1053, 1054, 
        1055, 1056, 1057, 1058, 1059, 1060, 1061, 1062, 1063, 
        1064, 1065, 1066, 1067, 1068, 1069, 1070, 1071, 1072, 
        1073, 1074, 1075, 1076, 1077, 1113, 1114, 1115, 1116, 
        1117, 1118, 1119, 1120, 1121, 1122, 1123, 1124, 1125, 
        1126, 1127, 1130, 1131, 1132, 1133, 1134, 1135, 1136, 
        1137, 1138, 1139, 1140, 1141, 1142, 1143, 1144, 1145, 
        1146, 1147, 1148, 1149, 1150, 1151, 1152, 1153, 1154, 
        1155, 1156, 1157, 1158, 1159, 1160, 1161, 1162, 1163, 
        1164, 1165, 1166, 1167, 1326, 1360, 1361, 1362, 1363, 
        1493, 1517, 1518, 1618, 1619, 1620, 1621, 1622, 1623, 
        1624, 1625, 1626, 1627, 1628, 1629, 1653, 1654, 1655, 
        1656, 1657, 1658, 1659, 1660, 1661, 1662, 1663, 1664
    ];

    protected override ReadOnlySpan<int> ItemIds => MiscItemIds;
    protected override string CommandFormat => "{0}M Add Static {1}";
    protected override StaticItemGumpBase CreateInstance(int page) => new AddMiscPt1Gump(page);

    public static void Initialize()
    {
        CommandSystem.Register("AddMiscPt1", AccessLevel.GameMaster, AddMiscPt1_OnCommand);
    }

    [Usage("AddMiscPt1")]
    [Description("Displays a menu for adding miscellaneous statics (Part 1).")]
    private static void AddMiscPt1_OnCommand(CommandEventArgs e)
    {
        e.Mobile.CloseGump<AddMiscPt1Gump>();
        e.Mobile.SendGump(new AddMiscPt1Gump());
    }

    public AddMiscPt1Gump(int page = 0) : base(page)
    {
    }
}
