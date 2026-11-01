/*
 * Oz'Thoth's Static Tool - Custom Items Part 5 Gump
 * Modernized for .NET 10 / ModernUO
 */

using System;
using Server.Commands;

namespace Server.Gumps.OzThoth;

/// <summary>
/// Gump for selecting custom static items (Part 5).
/// </summary>
public sealed class AddCustomPt5Gump : StaticItemGumpBase
{
    private static readonly int[] CustomItemIds =
    [
        39935, 39936, 39937, 39938, 39939, 39940, 39941, 39942, 39943,
        39944, 39945, 39946, 39947, 39948, 39949, 39950, 39951, 39952,
        39953, 39954, 39955, 39956, 39957, 39958, 39959, 39960, 39961,
        39962, 39963, 39964, 39965, 39966, 39967, 39968, 39969, 39970,
        39971, 39972, 39973, 39974, 39975, 39976, 39977, 39978, 39979,
        39980, 39981, 39982, 39983, 39984, 39985, 39986, 39987, 39988,
        39989, 39990, 39991, 39992, 39993, 39994, 39995, 39996, 39997,
        39998, 39999, 40000, 40001, 40002, 40003, 40004, 40005, 40006,
        40007, 40008, 40009, 40010, 40011, 40012, 40013, 40014, 40015,
        40016, 40017, 40018, 40019, 40020, 40021, 40022, 40023, 40024,
        40025, 40026, 40027, 40028, 40029, 40030, 40031, 40032, 40033,
        40034, 40035, 40036, 40037, 40038, 40039, 40040, 40041, 40042,
        40043, 40044, 40045, 40046, 40047, 40048, 40049, 40050, 40051,
        40052, 40053, 40054, 40055, 40056, 40057, 40058, 40059, 40060,
        40061, 40062, 40063, 40064, 40065, 40066, 40067, 40068, 40069,
        40070, 40071, 40072, 40073, 40074, 40075, 40076, 40077, 40078,
        40079, 40080, 40081, 40082, 40083, 40084, 40085, 40086, 40087,
        40088, 40089, 40090, 40091, 40092, 40093, 40094, 40095, 40096,
        40097, 40098, 40099, 40100, 40101, 40102, 40103, 40104, 40105,
        40106, 40107, 40108, 40109, 40110, 40111, 40112, 40113, 40114
    ];

    protected override ReadOnlySpan<int> ItemIds => CustomItemIds;
    protected override string CommandFormat => "{0}M Add Static {1}";
    protected override StaticItemGumpBase CreateInstance(int page) => new AddCustomPt5Gump(page);

    public static void Initialize()
    {
        CommandSystem.Register("AddCustomPt5", AccessLevel.GameMaster, AddCustomPt5_OnCommand);
    }

    [Usage("AddCustomPt5")]
    [Description("Displays a menu for adding custom statics (Part 5).")]
    private static void AddCustomPt5_OnCommand(CommandEventArgs e)
    {
        e.Mobile.CloseGump<AddCustomPt5Gump>();
        e.Mobile.SendGump(new AddCustomPt5Gump());
    }

    public AddCustomPt5Gump(int page = 0) : base(page)
    {
    }
}
