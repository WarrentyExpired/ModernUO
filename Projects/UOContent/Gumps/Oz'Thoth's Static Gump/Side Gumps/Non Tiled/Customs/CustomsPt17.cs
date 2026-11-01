/*
 * Oz'Thoth's Static Tool - Custom Items Part 17 Gump
 * Modernized for .NET 10 / ModernUO
 */

using System;
using Server.Commands;

namespace Server.Gumps.OzThoth;

/// <summary>
/// Gump for selecting custom static items (Part 17).
/// </summary>
public sealed class AddCustomPt17Gump : StaticItemGumpBase
{
    private static readonly int[] CustomItemIds =
    [
        42504, 42505, 42506, 42507, 42508, 42509, 42510, 42511, 42512,
        42513, 42514, 42515, 42516, 42517, 42518, 42519, 42520, 42521,
        42522, 42523, 42524, 42525, 42526, 42527, 42528, 42529, 42530,
        42531, 42532, 42533, 42534, 42535, 42536, 42537, 42538, 42539,
        42540, 42541, 42542, 42543, 42544, 42545, 42546, 42547, 42548,
        42549, 42550, 42551, 42552, 42553, 42554, 42555, 42556, 42557,
        42558, 42559, 42560, 42561, 42562, 42563, 42564, 42565, 42573,
        42574, 42575, 42576, 42577, 42578, 42579, 42580, 42582, 42583,
        42584, 42585, 42586, 42587, 42588, 42589, 42590, 42591, 42592,
        42593, 42594, 42595, 42596, 42597, 42598, 42599, 42600, 42601,
        42602, 42603, 42604, 42605, 42606, 42607, 42608, 42609, 42610,
        42611, 42612, 42613, 42614, 42615, 42616, 42617, 42618, 42619,
        42620, 42621, 42622, 42623, 42624, 42625, 42626, 42627, 42628,
        42629, 42630, 42631, 42632, 42633, 42634, 42635, 42636, 42637,
        42638, 42639, 42640, 42641, 42642, 42643, 42644, 42645, 42646,
        42647, 42648, 42649, 42650, 42651, 42652, 42653, 42656, 42657,
        42658, 42659, 42660, 42661, 42662, 42663, 42664, 42665, 42666,
        42667, 42668, 42669, 42670, 42671, 42672, 42673, 42674, 42675,
        42676, 42677, 42678, 42679, 42680, 42681, 42682, 42683, 42684,
        42685, 42686, 42687, 42688, 42689, 42690, 42691, 42692, 42693,
        42694, 42695, 42696, 42697, 42698, 42699, 42700, 42703, 42704,
        42705, 42706, 42707, 42708, 42709, 42710, 42711, 42712, 42713,
        42714, 42715, 42716, 42717, 42719, 42720, 42721, 42722, 42723,
        42724, 42725, 42726, 42727, 42728, 42729, 42730, 42731, 42732,
        42733, 42734, 42735, 42736, 42738, 42739, 42740, 42741, 42742
    ];

    protected override ReadOnlySpan<int> ItemIds => CustomItemIds;
    protected override string CommandFormat => "{0}M Add Static {1}";
    protected override StaticItemGumpBase CreateInstance(int page) => new AddCustomPt17Gump(page);

    public static void Initialize()
    {
        CommandSystem.Register("AddCustomPt17", AccessLevel.GameMaster, AddCustomPt17_OnCommand);
    }

    [Usage("AddCustomPt17")]
    [Description("Displays a menu for adding custom statics (Part 17).")]
    private static void AddCustomPt17_OnCommand(CommandEventArgs e)
    {
        e.Mobile.CloseGump<AddCustomPt17Gump>();
        e.Mobile.SendGump(new AddCustomPt17Gump());
    }

    public AddCustomPt17Gump(int page = 0) : base(page)
    {
    }
}
