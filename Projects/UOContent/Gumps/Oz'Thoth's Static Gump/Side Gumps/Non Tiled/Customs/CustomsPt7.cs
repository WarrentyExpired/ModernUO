/*
 * Oz'Thoth's Static Tool - Custom Items Part 7 Gump
 * Modernized for .NET 10 / ModernUO
 */

using System;
using Server.Commands;

namespace Server.Gumps.OzThoth;

/// <summary>
/// Gump for selecting custom static items (Part 7).
/// </summary>
public sealed class AddCustomPt7Gump : StaticItemGumpBase
{
    private static readonly int[] CustomItemIds =
    [
        40301, 40302, 40303, 40304, 40305, 40306, 40307, 40308, 40309,
        40310, 40311, 40312, 40313, 40314, 40315, 40316, 40317, 40318,
        40319, 40320, 40321, 40322, 40323, 40324, 40325, 40326, 40327,
        40328, 40329, 40330, 40331, 40332, 40333, 40334, 40335, 40336,
        40337, 40338, 40339, 40340, 40341, 40342, 40343, 40344, 40345,
        40346, 40347, 40348, 40349, 40350, 40351, 40352, 40353, 40354,
        40355, 40356, 40358, 40359, 40360, 40361, 40362, 40363, 40364,
        40365, 40366, 40367, 40368, 40369, 40370, 40371, 40372, 40373,
        40374, 40375, 40376, 40378, 40379, 40381, 40382, 40383, 40384,
        40385, 40386, 40387, 40388, 40389, 40390, 40391, 40392, 40393,
        40394, 40395, 40396, 40397, 40398, 40399, 40400, 40401, 40402,
        40403, 40404, 40405, 40406, 40407, 40408, 40410, 40411, 40412,
        40413, 40414, 40415, 40416, 40417, 40418, 40419, 40420, 40421,
        40422, 40423, 40424, 40425, 40426, 40427, 40428, 40429, 40430,
        40431, 40432, 40433, 40434, 40435, 40436, 40438, 40439, 40440,
        40441, 40442, 40443, 40444, 40445, 40447, 40448, 40449, 40450,
        40451, 40452, 40453, 40454, 40455, 40456, 40457, 40458, 40459,
        40460, 40461, 40462, 40463, 40464, 40465, 40466, 40467, 40468,
        40469, 40470, 40471, 40472, 40473, 40474, 40475, 40476, 40477,
        40478, 40479, 40480, 40481, 40482, 40483, 40484, 40485, 40486
    ];

    protected override ReadOnlySpan<int> ItemIds => CustomItemIds;
    protected override string CommandFormat => "{0}M Add Static {1}";
    protected override StaticItemGumpBase CreateInstance(int page) => new AddCustomPt7Gump(page);

    public static void Initialize()
    {
        CommandSystem.Register("AddCustomPt7", AccessLevel.GameMaster, AddCustomPt7_OnCommand);
    }

    [Usage("AddCustomPt7")]
    [Description("Displays a menu for adding custom statics (Part 7).")]
    private static void AddCustomPt7_OnCommand(CommandEventArgs e)
    {
        e.Mobile.CloseGump<AddCustomPt7Gump>();
        e.Mobile.SendGump(new AddCustomPt7Gump());
    }

    public AddCustomPt7Gump(int page = 0) : base(page)
    {
    }
}
