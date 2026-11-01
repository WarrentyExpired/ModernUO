/*
 * Oz'Thoth's Static Tool - Custom Items Part 6 Gump
 * Modernized for .NET 10 / ModernUO
 */

using System;
using Server.Commands;

namespace Server.Gumps.OzThoth;

/// <summary>
/// Gump for selecting custom static items (Part 6).
/// </summary>
public sealed class AddCustomPt6Gump : StaticItemGumpBase
{
    private static readonly int[] CustomItemIds =
    [
        40115, 40116, 40117, 40118, 40119, 40120, 40121, 40122, 40123,
        40124, 40125, 40126, 40127, 40128, 40129, 40130, 40131, 40132,
        40133, 40134, 40135, 40136, 40137, 40138, 40139, 40140, 40141,
        40142, 40143, 40144, 40145, 40146, 40147, 40148, 40149, 40150,
        40151, 40152, 40153, 40154, 40155, 40156, 40157, 40158, 40159,
        40160, 40161, 40162, 40163, 40164, 40165, 40166, 40167, 40168,
        40169, 40170, 40171, 40172, 40173, 40174, 40175, 40176, 40177,
        40178, 40179, 40180, 40181, 40182, 40183, 40184, 40185, 40188,
        40189, 40190, 40191, 40192, 40193, 40194, 40195, 40196, 40197,
        40198, 40199, 40200, 40201, 40202, 40203, 40204, 40205, 40206,
        40207, 40208, 40209, 40210, 40211, 40212, 40213, 40214, 40215,
        40216, 40217, 40218, 40219, 40220, 40221, 40222, 40223, 40224,
        40225, 40226, 40227, 40228, 40229, 40230, 40231, 40232, 40233,
        40234, 40235, 40236, 40237, 40238, 40239, 40240, 40241, 40242,
        40243, 40244, 40245, 40246, 40247, 40248, 40249, 40250, 40251,
        40252, 40253, 40254, 40255, 40256, 40257, 40258, 40259, 40260,
        40261, 40262, 40263, 40265, 40266, 40267, 40268, 40269, 40270,
        40271, 40272, 40273, 40274, 40275, 40276, 40277, 40278, 40280,
        40281, 40282, 40283, 40284, 40285, 40286, 40287, 40288, 40289,
        40290, 40291, 40292, 40294, 40295, 40297, 40298, 40299, 40300
    ];

    protected override ReadOnlySpan<int> ItemIds => CustomItemIds;
    protected override string CommandFormat => "{0}M Add Static {1}";
    protected override StaticItemGumpBase CreateInstance(int page) => new AddCustomPt6Gump(page);

    public static void Initialize()
    {
        CommandSystem.Register("AddCustomPt6", AccessLevel.GameMaster, AddCustomPt6_OnCommand);
    }

    [Usage("AddCustomPt6")]
    [Description("Displays a menu for adding custom statics (Part 6).")]
    private static void AddCustomPt6_OnCommand(CommandEventArgs e)
    {
        e.Mobile.CloseGump<AddCustomPt6Gump>();
        e.Mobile.SendGump(new AddCustomPt6Gump());
    }

    public AddCustomPt6Gump(int page = 0) : base(page)
    {
    }
}
