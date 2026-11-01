/*
 * Oz'Thoth's Static Tool - Custom Items Part 10 Gump
 * Modernized for .NET 10 / ModernUO
 */

using System;
using Server.Commands;

namespace Server.Gumps.OzThoth;

/// <summary>
/// Gump for selecting custom static items (Part 10).
/// </summary>
public sealed class AddCustomPt10Gump : StaticItemGumpBase
{
    private static readonly int[] CustomItemIds =
    [
        40851, 40852, 40853, 40854, 40855, 40856, 40857, 40858, 40859,
        40860, 40861, 40862, 40863, 40864, 40865, 40866, 40867, 40868,
        40869, 40870, 40871, 40872, 40873, 40874, 40875, 40876, 40877,
        40878, 40879, 40880, 40881, 40882, 40883, 40884, 40885, 40886,
        40887, 40888, 40889, 40890, 40891, 40892, 40893, 40894, 40895,
        40896, 40897, 40898, 40899, 40900, 40901, 40902, 40903, 40904,
        40905, 40906, 40907, 40908, 40909, 40910, 40911, 40912, 40913,
        40914, 40915, 40916, 40917, 40918, 40919, 40920, 40921, 40922,
        40923, 40924, 40925, 40926, 40927, 40928, 40929, 40930, 40931,
        40932, 40933, 40934, 40935, 40936, 40937, 40938, 40939, 40940,
        40941, 40942, 40943, 40944, 40945, 40946, 40947, 40948, 40949,
        40950, 40951, 40952, 40953, 40954, 40955, 40956, 40957, 40959,
        40960, 40961, 40962, 40963, 40964, 40965, 40966, 40967, 40968,
        40969, 40970, 40971, 40972, 40973, 40974, 40975, 40976, 40977,
        40978, 40979, 40980, 40981, 40982, 40983, 40984, 40985, 40986,
        40987, 40988, 40989, 40990, 40991, 40992, 40993, 40994, 40995,
        40996, 40997, 40998, 40999, 41000, 41001, 41002, 41003, 41004,
        41005, 41006, 41007, 41008, 41009, 41010, 41011, 41012, 41013,
        41014, 41015, 41016, 41017, 41018, 41019, 41020, 41021, 41022,
        41023, 41024, 41025, 41026, 41027, 41028, 41029, 41030, 41031,
        41032, 41033, 41034, 41035, 41036, 41037, 41038, 41039, 41040,
        41041, 41042, 41043, 41044, 41045, 41046, 41047, 41048, 41049,
        41050, 41051, 41052, 41053, 41054, 41055, 41056, 41057, 41058,
        41059, 41060, 41061, 41062, 41063, 41064, 41065, 41066, 41067,
        41068, 41069, 41070, 41071, 41072, 41073, 41074, 41075, 41076,
        41077, 41078, 41079, 41080, 41081, 41082, 41083, 41092, 41093
    ];

    protected override ReadOnlySpan<int> ItemIds => CustomItemIds;
    protected override string CommandFormat => "{0}M Add Static {1}";
    protected override StaticItemGumpBase CreateInstance(int page) => new AddCustomPt10Gump(page);

    public static void Initialize()
    {
        CommandSystem.Register("AddCustomPt10", AccessLevel.GameMaster, AddCustomPt10_OnCommand);
    }

    [Usage("AddCustomPt10")]
    [Description("Displays a menu for adding custom statics (Part 10).")]
    private static void AddCustomPt10_OnCommand(CommandEventArgs e)
    {
        e.Mobile.CloseGump<AddCustomPt10Gump>();
        e.Mobile.SendGump(new AddCustomPt10Gump());
    }

    public AddCustomPt10Gump(int page = 0) : base(page)
    {
    }
}
