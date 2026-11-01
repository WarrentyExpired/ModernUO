/*
 * Oz'Thoth's Static Tool - Custom Items Part 20 Gump
 * Modernized for .NET 10 / ModernUO
 */

using System;
using Server.Commands;

namespace Server.Gumps.OzThoth;

/// <summary>
/// Gump for selecting custom static items (Part 20).
/// </summary>
public sealed class AddCustomPt20Gump : StaticItemGumpBase
{
    private static readonly int[] CustomItemIds =
    [
        43189, 43190, 43191, 43192, 43193, 43194, 43195, 43196, 43197,
        43198, 43199, 43200, 43201, 43202, 43203, 43204, 43205, 43206,
        43207, 43208, 43209, 43210, 43211, 43212, 43213, 43214, 43215,
        43216, 43217, 43218, 43219, 43220, 43221, 43222, 43223, 43224,
        43225, 43226, 43227, 43228, 43229, 43230, 43231, 43232, 43233,
        43234, 43235, 43236, 43237, 43238, 43239, 43240, 43241, 43242,
        43243, 43244, 43245, 43246, 43247, 43248, 43249, 43250, 43251,
        43252, 43253, 43254, 43255, 43256, 43257, 43258, 43259, 43260,
        43261, 43262, 43263, 43264, 43265, 43267, 43268, 43269, 43270,
        43271, 43272, 43273, 43274, 43275, 43276, 43277, 43278, 43279,
        43280, 43281, 43282, 43283, 43284, 43285, 43286, 43287, 43288,
        43289, 43290, 43291, 43292, 43293, 43294, 43295, 43296, 43297,
        43298, 43300, 43301, 43302, 43303, 43304, 43305, 43306, 43307,
        43308, 43309, 43310, 43311, 43312, 43313, 43314, 43315, 43316,
        43317, 43318, 43319, 43320, 43321, 43322, 43323, 43324, 43325,
        43326, 43327, 43328, 43329, 43330, 43331, 43332, 43333, 43334,
        43335, 43336, 43337, 43338, 43339, 43340, 43341, 43342, 43343,
        43344, 43345, 43346, 43347, 43348, 43349, 43350, 43351, 43352,
        43353, 43354, 43355, 43356, 43357, 43358, 43359, 43360, 43361,
        43362, 43363, 43364, 43365, 43366, 43367
    ];

    protected override ReadOnlySpan<int> ItemIds => CustomItemIds;
    protected override string CommandFormat => "{0}M Add Static {1}";
    protected override StaticItemGumpBase CreateInstance(int page) => new AddCustomPt20Gump(page);

    public static void Initialize()
    {
        CommandSystem.Register("AddCustomPt20", AccessLevel.GameMaster, AddCustomPt20_OnCommand);
    }

    [Usage("AddCustomPt20")]
    [Description("Displays a menu for adding custom statics (Part 20).")]
    private static void AddCustomPt20_OnCommand(CommandEventArgs e)
    {
        e.Mobile.CloseGump<AddCustomPt20Gump>();
        e.Mobile.SendGump(new AddCustomPt20Gump());
    }

    public AddCustomPt20Gump(int page = 0) : base(page)
    {
    }
}
