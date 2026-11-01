/*
 * Oz'Thoth's Static Tool - Custom Items Part 2 Gump
 * Modernized for .NET 10 / ModernUO
 */

using System;
using Server.Commands;

namespace Server.Gumps.OzThoth;

/// <summary>
/// Gump for selecting custom static items (Part 2).
/// </summary>
public sealed class AddCustomPt2Gump : StaticItemGumpBase
{
    private static readonly int[] CustomItemIds =
    [
        39394, 39395, 39396, 39397, 39398, 39399, 39400, 39401, 39402,
        39403, 39404, 39405, 39406, 39407, 39408, 39409, 39410, 39411,
        39412, 39413, 39414, 39415, 39416, 39417, 39418, 39419, 39420,
        39421, 39422, 39423, 39424, 39425, 39426, 39427, 39428, 39429,
        39430, 39431, 39432, 39433, 39434, 39435, 39436, 39437, 39438,
        39439, 39440, 39441, 39442, 39443, 39444, 39445, 39446, 39447,
        39448, 39449, 39450, 39451, 39452, 39453, 39454, 39455, 39456,
        39457, 39458, 39459, 39460, 39461, 39462, 39463, 39464, 39465,
        39466, 39467, 39468, 39469, 39470, 39471, 39472, 39473, 39474,
        39475, 39476, 39477, 39478, 39479, 39480, 39481, 39482, 39483,
        39484, 39485, 39486, 39487, 39488, 39489, 39490, 39491, 39492,
        39493, 39494, 39495, 39496, 39497, 39498, 39499, 39500, 39501,
        39502, 39503, 39504, 39505, 39506, 39507, 39508, 39509, 39510,
        39511, 39512, 39513, 39514, 39515, 39517, 39518, 39519, 39520,
        39521, 39522, 39523, 39524, 39525, 39526, 39527, 39528, 39529,
        39530, 39531, 39532, 39533, 39534, 39535, 39536, 39537, 39538,
        39539, 39540, 39541, 39542, 39543, 39544, 39545, 39546, 39547,
        39548, 39549, 39550, 39551, 39552, 39553, 39554, 39555, 39556,
        39557, 39558, 39559, 39560, 39561, 39562, 39563, 39564, 39565,
        39566, 39567, 39568, 39569, 39570, 39571, 39572, 39573, 39574
    ];

    protected override ReadOnlySpan<int> ItemIds => CustomItemIds;
    protected override string CommandFormat => "{0}M Add Static {1}";
    protected override StaticItemGumpBase CreateInstance(int page) => new AddCustomPt2Gump(page);

    public static void Initialize()
    {
        CommandSystem.Register("AddCustomPt2", AccessLevel.GameMaster, AddCustomPt2_OnCommand);
    }

    [Usage("AddCustomPt2")]
    [Description("Displays a menu for adding custom statics (Part 2).")]
    private static void AddCustomPt2_OnCommand(CommandEventArgs e)
    {
        e.Mobile.CloseGump<AddCustomPt2Gump>();
        e.Mobile.SendGump(new AddCustomPt2Gump());
    }

    public AddCustomPt2Gump(int page = 0) : base(page)
    {
    }
}
