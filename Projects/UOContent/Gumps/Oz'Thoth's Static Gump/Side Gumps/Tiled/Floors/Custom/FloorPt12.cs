/*
 * Oz'Thoth's Static Tool - Floor Part 12 Gump
 * Modernized for .NET 10 / ModernUO
 */

using System;
using Server.Commands;

namespace Server.Gumps.OzThoth;

/// <summary>
/// Gump for selecting floor statics (Part 12).
/// </summary>
public sealed class AddFloorPt12Gump : StaticItemGumpBase
{
    private static readonly int[] FloorItemIds =
    [
        40051, 40052, 40053, 40054, 40055, 40056, 40057, 40058,
        40059, 40060, 40061, 40062, 40063, 40064, 40065, 40066,
        40067, 40068, 40069, 40070, 40071, 40072, 40073, 40074,
        40075, 40076, 40077, 40078, 40079, 40080, 40081, 40082,
        40083, 40084, 40085, 40086, 40087, 40088, 40089, 40090,
        40091, 40092, 40093, 40094, 40095, 40096, 40097, 40098,
        40099, 40100, 40101, 40102, 40103, 40104, 40105, 40106,
        40107, 40108, 40109, 40110, 40111, 40112, 40113, 40114,
        40115, 40116, 40117, 40118, 40119, 40120, 40121, 40122,
        40123, 40124, 40125, 40126, 40127, 40128, 40129, 40130,
        40131, 40132, 40133, 40134, 40135, 40136, 40137, 40138,
        40139, 40140, 40141, 40142, 40143, 40144, 40145, 40146,
        40147, 40148, 40149, 40150, 40151, 40152, 40153, 40154,
        40155, 40156, 40157, 40158, 40159, 40160, 40161, 40162,
        40163, 40164, 40165, 40166, 40167, 40168, 40169, 40170,
        40171, 40172, 40173, 40174, 40175, 40176, 40177, 40178,
        40179, 40180, 40181, 40182, 40183, 40184, 40185, 42338,
        42339, 42340, 42341, 43002, 43003, 43004, 43005, 43006,
        43007, 43008, 43009, 43010, 43011, 43012, 43013, 43014,
        43015, 43016, 43017, 43019, 43020, 43021, 43022, 43023,
        43024, 43025, 43026, 43027, 43028, 43029, 43030, 43031,
        43032, 43033, 43034, 43036, 43037, 43038, 43039, 43040
    ];

    protected override ReadOnlySpan<int> ItemIds => FloorItemIds;
    protected override string CommandFormat => "{0}Tile Static {1}";
    protected override StaticItemGumpBase CreateInstance(int page) => new AddFloorPt12Gump(page);

    public static void Initialize()
    {
        CommandSystem.Register("AddFloorPt12", AccessLevel.GameMaster, AddFloorPt12_OnCommand);
    }

    [Usage("AddFloorPt12")]
    [Description("Displays a menu for adding floor statics (Part 12).")]
    private static void AddFloorPt12_OnCommand(CommandEventArgs e)
    {
        e.Mobile.CloseGump<AddFloorPt12Gump>();
        e.Mobile.SendGump(new AddFloorPt12Gump());
    }

    public AddFloorPt12Gump(int page = 0) : base(page)
    {
    }
}
