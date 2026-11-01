/*
 * Oz'Thoth's Static Tool - Custom Items Part 3 Gump
 * Modernized for .NET 10 / ModernUO
 */

using System;
using Server.Commands;

namespace Server.Gumps.OzThoth;

/// <summary>
/// Gump for selecting custom static items (Part 3).
/// </summary>
public sealed class AddCustomPt3Gump : StaticItemGumpBase
{
    private static readonly int[] CustomItemIds =
    [
        39575, 39576, 39577, 39578, 39579, 39580, 39581, 39582, 39583,
        39584, 39585, 39586, 39587, 39588, 39589, 39590, 39591, 39592,
        39593, 39594, 39595, 39596, 39597, 39598, 39599, 39600, 39601,
        39602, 39603, 39604, 39605, 39606, 39607, 39608, 39609, 39610,
        39611, 39612, 39613, 39614, 39615, 39616, 39617, 39618, 39619,
        39620, 39621, 39622, 39623, 39624, 39625, 39626, 39627, 39628,
        39629, 39630, 39631, 39632, 39633, 39634, 39635, 39636, 39637,
        39638, 39639, 39640, 39641, 39642, 39643, 39644, 39645, 39646,
        39647, 39648, 39649, 39650, 39651, 39652, 39653, 39654, 39655,
        39656, 39657, 39658, 39659, 39660, 39661, 39662, 39663, 39664,
        39665, 39666, 39667, 39668, 39669, 39670, 39671, 39672, 39673,
        39674, 39675, 39676, 39677, 39678, 39679, 39680, 39681, 39682,
        39683, 39684, 39685, 39686, 39687, 39688, 39689, 39690, 39691,
        39692, 39693, 39694, 39695, 39696, 39697, 39698, 39699, 39700,
        39701, 39702, 39703, 39704, 39705, 39706, 39707, 39708, 39709,
        39710, 39711, 39712, 39713, 39714, 39715, 39716, 39717, 39718,
        39719, 39720, 39721, 39722, 39723, 39724, 39725, 39726, 39727,
        39728, 39729, 39730, 39731, 39732, 39733, 39734, 39735, 39736,
        39737, 39738, 39739, 39740, 39741, 39742, 39743, 39744, 39745,
        39746, 39747, 39748, 39749, 39750, 39751, 39752, 39753, 39754
    ];

    protected override ReadOnlySpan<int> ItemIds => CustomItemIds;
    protected override string CommandFormat => "{0}M Add Static {1}";
    protected override StaticItemGumpBase CreateInstance(int page) => new AddCustomPt3Gump(page);

    public static void Initialize()
    {
        CommandSystem.Register("AddCustomPt3", AccessLevel.GameMaster, AddCustomPt3_OnCommand);
    }

    [Usage("AddCustomPt3")]
    [Description("Displays a menu for adding custom statics (Part 3).")]
    private static void AddCustomPt3_OnCommand(CommandEventArgs e)
    {
        e.Mobile.CloseGump<AddCustomPt3Gump>();
        e.Mobile.SendGump(new AddCustomPt3Gump());
    }

    public AddCustomPt3Gump(int page = 0) : base(page)
    {
    }
}
