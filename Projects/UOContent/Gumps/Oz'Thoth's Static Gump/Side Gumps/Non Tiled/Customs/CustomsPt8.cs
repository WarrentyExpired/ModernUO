/*
 * Oz'Thoth's Static Tool - Custom Items Part 8 Gump
 * Modernized for .NET 10 / ModernUO
 */

using System;
using Server.Commands;

namespace Server.Gumps.OzThoth;

/// <summary>
/// Gump for selecting custom static items (Part 8).
/// </summary>
public sealed class AddCustomPt8Gump : StaticItemGumpBase
{
    private static readonly int[] CustomItemIds =
    [
        40487, 40488, 40489, 40490, 40491, 40492, 40493, 40494, 40495,
        40496, 40497, 40498, 40499, 40500, 40501, 40502, 40503, 40504,
        40505, 40506, 40507, 40508, 40509, 40510, 40511, 40512, 40513,
        40514, 40515, 40516, 40517, 40518, 40519, 40520, 40521, 40522,
        40523, 40524, 40525, 40526, 40527, 40528, 40529, 40530, 40531,
        40532, 40533, 40534, 40535, 40536, 40537, 40538, 40539, 40540,
        40541, 40542, 40543, 40544, 40545, 40546, 40547, 40548, 40549,
        40550, 40551, 40552, 40553, 40554, 40555, 40556, 40557, 40558,
        40559, 40560, 40561, 40562, 40563, 40564, 40565, 40566, 40567,
        40568, 40569, 40570, 40571, 40572, 40573, 40574, 40575, 40576,
        40577, 40578, 40579, 40580, 40581, 40582, 40583, 40584, 40585,
        40586, 40587, 40588, 40589, 40590, 40591, 40592, 40593, 40594,
        40595, 40596, 40597, 40598, 40599, 40600, 40601, 40602, 40603,
        40604, 40605, 40606, 40607, 40608, 40609, 40610, 40611, 40612,
        40613, 40614, 40615, 40616, 40617, 40618, 40619, 40620, 40621,
        40622, 40623, 40624, 40625, 40626, 40627, 40628, 40629, 40630,
        40631, 40632, 40633, 40634, 40635, 40636, 40637, 40638, 40639,
        40640, 40641, 40642, 40643, 40644, 40645, 40646, 40647, 40648,
        40649, 40650, 40651, 40652, 40653, 40654, 40655, 40656, 40657,
        40658, 40659, 40660, 40661, 40662, 40663, 40664, 40665, 40666
    ];

    protected override ReadOnlySpan<int> ItemIds => CustomItemIds;
    protected override string CommandFormat => "{0}M Add Static {1}";
    protected override StaticItemGumpBase CreateInstance(int page) => new AddCustomPt8Gump(page);

    public static void Initialize()
    {
        CommandSystem.Register("AddCustomPt8", AccessLevel.GameMaster, AddCustomPt8_OnCommand);
    }

    [Usage("AddCustomPt8")]
    [Description("Displays a menu for adding custom statics (Part 8).")]
    private static void AddCustomPt8_OnCommand(CommandEventArgs e)
    {
        e.Mobile.CloseGump<AddCustomPt8Gump>();
        e.Mobile.SendGump(new AddCustomPt8Gump());
    }

    public AddCustomPt8Gump(int page = 0) : base(page)
    {
    }
}
