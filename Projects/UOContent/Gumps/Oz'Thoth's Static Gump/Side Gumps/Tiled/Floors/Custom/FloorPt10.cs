/*
 * Oz'Thoth's Static Tool - Floor Part 10 Gump
 * Modernized for .NET 10 / ModernUO
 */

using System;
using Server.Commands;

namespace Server.Gumps.OzThoth;

/// <summary>
/// Gump for selecting floor statics (Part 10).
/// </summary>
public sealed class AddFloorPt10Gump : StaticItemGumpBase
{
    private static readonly int[] FloorItemIds =
    [
        39693, 39694, 39695, 39696, 39697, 39698, 39699, 39700,
        39701, 39702, 39703, 39704, 39705, 39706, 39707, 39708,
        39709, 39710, 39711, 39712, 39713, 39714, 39715, 39716,
        39717, 39718, 39719, 39720, 39721, 39722, 39723, 39724,
        39725, 39726, 39727, 39728, 39729, 39730, 39731, 39732,
        39733, 39734, 39735, 39736, 39737, 39738, 39739, 39740,
        39741, 39742, 39743, 39744, 39745, 39746, 39747, 39748,
        39749, 39750, 39751, 39752, 39753, 39754, 39755, 39756,
        39757, 39758, 39759, 39760, 39761, 39762, 39763, 39764,
        39765, 39766, 39767, 39768, 39769, 39770, 39771, 39772,
        39773, 39774, 39775, 39776, 39777, 39778, 39779, 39780,
        39781, 39782, 39783, 39784, 39785, 39786, 39787, 39788,
        39789, 39790, 39791, 39792, 39793, 39794, 39795, 39796,
        39797, 39798, 39799, 39800, 39801, 39802, 39803, 39804,
        39805, 39806, 39807, 39808, 39809, 39810, 39811, 39812,
        39813, 39814, 39815, 39816, 39817, 39818, 39819, 39820,
        39821, 39822, 39823, 39824, 39825, 39826, 39827, 39828,
        39829, 39830, 39831, 39832, 39833, 39834, 39835, 39836,
        39837, 39838, 39839, 39840, 39841, 39842, 39843, 39844,
        39845, 39846, 39847, 39848, 39849, 39850, 39851, 39852,
        39853, 39854, 39855, 39856, 39857, 39858, 39859, 39860,
        39861, 39862, 39863, 39864, 39865, 39866, 39867, 39868
    ];

    protected override ReadOnlySpan<int> ItemIds => FloorItemIds;
    protected override string CommandFormat => "{0}Tile Static {1}";
    protected override StaticItemGumpBase CreateInstance(int page) => new AddFloorPt10Gump(page);

    public static void Initialize()
    {
        CommandSystem.Register("AddFloorPt10", AccessLevel.GameMaster, AddFloorPt10_OnCommand);
    }

    [Usage("AddFloorPt10")]
    [Description("Displays a menu for adding floor statics (Part 10).")]
    private static void AddFloorPt10_OnCommand(CommandEventArgs e)
    {
        e.Mobile.CloseGump<AddFloorPt10Gump>();
        e.Mobile.SendGump(new AddFloorPt10Gump());
    }

    public AddFloorPt10Gump(int page = 0) : base(page)
    {
    }
}
