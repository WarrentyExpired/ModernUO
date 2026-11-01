/*
 * Oz'Thoth's Static Tool - Custom Items Part 15 Gump
 * Modernized for .NET 10 / ModernUO
 */

using System;
using Server.Commands;

namespace Server.Gumps.OzThoth;

/// <summary>
/// Gump for selecting custom static items (Part 15).
/// </summary>
public sealed class AddCustomPt15Gump : StaticItemGumpBase
{
    private static readonly int[] CustomItemIds =
    [
        42052, 42053, 42054, 42055, 42056, 42057, 42058, 42059, 42060,
        42061, 42062, 42063, 42064, 42065, 42066, 42067, 42068, 42069,
        42070, 42071, 42072, 42073, 42074, 42075, 42076, 42077, 42078,
        42079, 42080, 42081, 42082, 42083, 42084, 42085, 42086, 42087,
        42088, 42089, 42090, 42091, 42092, 42093, 42094, 42095, 42096,
        42097, 42098, 42099, 42100, 42101, 42102, 42103, 42104, 42105,
        42106, 42107, 42108, 42109, 42110, 42111, 42112, 42113, 42114,
        42115, 42116, 42117, 42118, 42119, 42120, 42121, 42122, 42123,
        42124, 42125, 42126, 42127, 42128, 42129, 42130, 42131, 42132,
        42133, 42134, 42135, 42136, 42137, 42138, 42139, 42140, 42141,
        42142, 42143, 42144, 42145, 42146, 42147, 42148, 42149, 42150,
        42151, 42152, 42153, 42154, 42155, 42156, 42157, 42158, 42159,
        42160, 42161, 42162, 42163, 42164, 42165, 42166, 42167, 42168,
        42169, 42170, 42171, 42172, 42173, 42174, 42175, 42176, 42177,
        42178, 42179, 42180, 42181, 42182, 42183, 42184, 42185, 42186,
        42187, 42188, 42189, 42190, 42191, 42192, 42193, 42194, 42195,
        42196, 42197, 42198, 42199, 42200, 42201, 42202, 42203, 42204,
        42205, 42206, 42207, 42208, 42209, 42210, 42211, 42212, 42213,
        42214, 42215, 42216, 42217, 42218, 42219, 42220, 42221, 42222,
        42223, 42224, 42225, 42226, 42227, 42228, 42229, 42230, 42231,
        42232, 42233, 42234, 42235, 42236, 42237, 42238, 42239, 42240,
        42241, 42242, 42243, 42244, 42245, 42246, 42247, 42248, 42249,
        42250, 42251, 42252, 42253, 42254, 42255, 42256, 42257, 42258,
        42259, 42260, 42261, 42262, 42263, 42264, 42265, 42266, 42267,
        42268, 42269, 42270, 42271, 42272, 42273, 42274, 42275, 42276
    ];

    protected override ReadOnlySpan<int> ItemIds => CustomItemIds;
    protected override string CommandFormat => "{0}M Add Static {1}";
    protected override StaticItemGumpBase CreateInstance(int page) => new AddCustomPt15Gump(page);

    public static void Initialize()
    {
        CommandSystem.Register("AddCustomPt15", AccessLevel.GameMaster, AddCustomPt15_OnCommand);
    }

    [Usage("AddCustomPt15")]
    [Description("Displays a menu for adding custom statics (Part 15).")]
    private static void AddCustomPt15_OnCommand(CommandEventArgs e)
    {
        e.Mobile.CloseGump<AddCustomPt15Gump>();
        e.Mobile.SendGump(new AddCustomPt15Gump());
    }

    public AddCustomPt15Gump(int page = 0) : base(page)
    {
    }
}
