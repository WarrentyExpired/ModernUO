/*
 * Oz'Thoth's Static Tool - Custom Items Part 9 Gump
 * Modernized for .NET 10 / ModernUO
 */

using System;
using Server.Commands;

namespace Server.Gumps.OzThoth;

/// <summary>
/// Gump for selecting custom static items (Part 9).
/// </summary>
public sealed class AddCustomPt9Gump : StaticItemGumpBase
{
    private static readonly int[] CustomItemIds =
    [
        40667, 40668, 40669, 40670, 40671, 40672, 40673, 40674, 40675,
        40676, 40677, 40678, 40679, 40680, 40681, 40682, 40683, 40684,
        40685, 40686, 40687, 40688, 40689, 40690, 40691, 40692, 40693,
        40694, 40695, 40696, 40697, 40698, 40699, 40700, 40701, 40702,
        40703, 40704, 40705, 40706, 40707, 40708, 40709, 40710, 40711,
        40712, 40713, 40714, 40715, 40716, 40717, 40718, 40719, 40720,
        40721, 40722, 40723, 40724, 40725, 40726, 40727, 40728, 40729,
        40730, 40731, 40732, 40733, 40734, 40735, 40736, 40737, 40738,
        40739, 40740, 40741, 40742, 40743, 40744, 40745, 40746, 40747,
        40748, 40749, 40750, 40751, 40752, 40753, 40754, 40755, 40756,
        40757, 40758, 40759, 40760, 40761, 40762, 40763, 40764, 40765,
        40766, 40767, 40768, 40769, 40770, 40771, 40772, 40773, 40774,
        40775, 40776, 40777, 40778, 40779, 40780, 40781, 40782, 40783,
        40784, 40787, 40790, 40791, 40792, 40793, 40794, 40795, 40796,
        40797, 40798, 40799, 40800, 40801, 40802, 40803, 40804, 40805,
        40806, 40807, 40808, 40809, 40810, 40811, 40812, 40813, 40814,
        40815, 40816, 40817, 40818, 40819, 40820, 40821, 40822, 40823,
        40824, 40825, 40826, 40827, 40828, 40829, 40830, 40831, 40832,
        40833, 40834, 40835, 40836, 40837, 40838, 40839, 40840, 40841,
        40842, 40843, 40844, 40845, 40846, 40847, 40848, 40849, 40850
    ];

    protected override ReadOnlySpan<int> ItemIds => CustomItemIds;
    protected override string CommandFormat => "{0}M Add Static {1}";
    protected override StaticItemGumpBase CreateInstance(int page) => new AddCustomPt9Gump(page);

    public static void Initialize()
    {
        CommandSystem.Register("AddCustomPt9", AccessLevel.GameMaster, AddCustomPt9_OnCommand);
    }

    [Usage("AddCustomPt9")]
    [Description("Displays a menu for adding custom statics (Part 9).")]
    private static void AddCustomPt9_OnCommand(CommandEventArgs e)
    {
        e.Mobile.CloseGump<AddCustomPt9Gump>();
        e.Mobile.SendGump(new AddCustomPt9Gump());
    }

    public AddCustomPt9Gump(int page = 0) : base(page)
    {
    }
}
