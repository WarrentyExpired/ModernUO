/*
 * Oz'Thoth's Static Tool - Custom Items Part 1 Gump
 * Modernized for .NET 10 / ModernUO
 */

using System;
using Server.Commands;

namespace Server.Gumps.OzThoth;

/// <summary>
/// Gump for selecting custom static items (Part 1).
/// </summary>
public sealed class AddCustomPt1Gump : StaticItemGumpBase
{
    private static readonly int[] CustomItemIds =
    [
        39214, 39215, 39216, 39217, 39218, 39219, 39220, 39221, 39222,
        39223, 39224, 39225, 39226, 39227, 39228, 39229, 39230, 39231,
        39232, 39233, 39234, 39235, 39236, 39237, 39238, 39239, 39240,
        39241, 39242, 39243, 39244, 39245, 39246, 39247, 39248, 39249,
        39250, 39251, 39252, 39253, 39254, 39255, 39256, 39257, 39258,
        39259, 39260, 39261, 39262, 39263, 39264, 39265, 39266, 39267,
        39268, 39269, 39270, 39271, 39272, 39273, 39274, 39275, 39276,
        39277, 39278, 39279, 39280, 39281, 39282, 39283, 39284, 39285,
        39286, 39287, 39288, 39289, 39290, 39291, 39292, 39293, 39294,
        39295, 39296, 39297, 39298, 39299, 39300, 39301, 39302, 39303,
        39304, 39305, 39306, 39307, 39308, 39309, 39310, 39311, 39312,
        39313, 39314, 39315, 39316, 39317, 39318, 39319, 39320, 39321,
        39322, 39323, 39324, 39325, 39326, 39327, 39328, 39329, 39330,
        39331, 39332, 39333, 39334, 39335, 39336, 39337, 39338, 39339,
        39340, 39341, 39342, 39343, 39344, 39345, 39346, 39347, 39348,
        39349, 39350, 39351, 39352, 39353, 39354, 39355, 39356, 39357,
        39358, 39359, 39360, 39361, 39362, 39363, 39364, 39365, 39366,
        39367, 39368, 39369, 39370, 39371, 39372, 39373, 39374, 39375,
        39376, 39377, 39378, 39379, 39380, 39381, 39382, 39383, 39384,
        39385, 39386, 39387, 39388, 39389, 39390, 39391, 39392, 39393
    ];

    protected override ReadOnlySpan<int> ItemIds => CustomItemIds;
    protected override string CommandFormat => "{0}M Add Static {1}";
    protected override StaticItemGumpBase CreateInstance(int page) => new AddCustomPt1Gump(page);

    public static void Initialize()
    {
        CommandSystem.Register("AddCustomPt1", AccessLevel.GameMaster, AddCustomPt1_OnCommand);
    }

    [Usage("AddCustomPt1")]
    [Description("Displays a menu for adding custom statics (Part 1).")]
    private static void AddCustomPt1_OnCommand(CommandEventArgs e)
    {
        e.Mobile.CloseGump<AddCustomPt1Gump>();
        e.Mobile.SendGump(new AddCustomPt1Gump());
    }

    public AddCustomPt1Gump(int page = 0) : base(page)
    {
    }
}
