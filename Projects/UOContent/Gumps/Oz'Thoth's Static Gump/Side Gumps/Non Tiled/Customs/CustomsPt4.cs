/*
 * Oz'Thoth's Static Tool - Custom Items Part 4 Gump
 * Modernized for .NET 10 / ModernUO
 */

using System;
using Server.Commands;

namespace Server.Gumps.OzThoth;

/// <summary>
/// Gump for selecting custom static items (Part 4).
/// </summary>
public sealed class AddCustomPt4Gump : StaticItemGumpBase
{
    private static readonly int[] CustomItemIds =
    [
        39755, 39756, 39757, 39758, 39759, 39760, 39761, 39762, 39763,
        39764, 39765, 39766, 39767, 39768, 39769, 39770, 39771, 39772,
        39773, 39774, 39775, 39776, 39777, 39778, 39779, 39780, 39781,
        39782, 39783, 39784, 39785, 39786, 39787, 39788, 39789, 39790,
        39791, 39792, 39793, 39794, 39795, 39796, 39797, 39798, 39799,
        39800, 39801, 39802, 39803, 39804, 39805, 39806, 39807, 39808,
        39809, 39810, 39811, 39812, 39813, 39814, 39815, 39816, 39817,
        39818, 39819, 39820, 39821, 39822, 39823, 39824, 39825, 39826,
        39827, 39828, 39829, 39830, 39831, 39832, 39833, 39834, 39835,
        39836, 39837, 39838, 39839, 39840, 39841, 39842, 39843, 39844,
        39845, 39846, 39847, 39848, 39849, 39850, 39851, 39852, 39853,
        39854, 39855, 39856, 39857, 39858, 39859, 39860, 39861, 39862,
        39863, 39864, 39865, 39866, 39867, 39868, 39869, 39870, 39871,
        39872, 39873, 39874, 39875, 39876, 39877, 39878, 39879, 39880,
        39881, 39882, 39883, 39884, 39885, 39886, 39887, 39888, 39889,
        39890, 39891, 39892, 39893, 39894, 39895, 39896, 39897, 39898,
        39899, 39900, 39901, 39902, 39903, 39904, 39905, 39906, 39907,
        39908, 39909, 39910, 39911, 39912, 39913, 39914, 39915, 39916,
        39917, 39918, 39919, 39920, 39921, 39922, 39923, 39924, 39925,
        39926, 39927, 39928, 39929, 39930, 39931, 39932, 39933, 39934
    ];

    protected override ReadOnlySpan<int> ItemIds => CustomItemIds;
    protected override string CommandFormat => "{0}M Add Static {1}";
    protected override StaticItemGumpBase CreateInstance(int page) => new AddCustomPt4Gump(page);

    public static void Initialize()
    {
        CommandSystem.Register("AddCustomPt4", AccessLevel.GameMaster, AddCustomPt4_OnCommand);
    }

    [Usage("AddCustomPt4")]
    [Description("Displays a menu for adding custom statics (Part 4).")]
    private static void AddCustomPt4_OnCommand(CommandEventArgs e)
    {
        e.Mobile.CloseGump<AddCustomPt4Gump>();
        e.Mobile.SendGump(new AddCustomPt4Gump());
    }

    public AddCustomPt4Gump(int page = 0) : base(page)
    {
    }
}
