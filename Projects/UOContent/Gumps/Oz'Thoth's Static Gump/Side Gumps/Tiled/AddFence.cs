/*
 * Oz'Thoth's Static Tool - Fence Selector Gump
 * Modernized for .NET 10 / ModernUO
 */

using System;
using Server.Commands;

namespace Server.Gumps.OzThoth;

/// <summary>
/// Gump for selecting fence static items to tile.
/// </summary>
public sealed class AddFenceGump : StaticItemGumpBase
{
    private static readonly int[] FenceItemIds =
    [
        2081, 2082, 2083, 2084, 2085, 2086, 2087, 2088, 2089,
        2090, 2091, 2092, 2093, 2094, 2095, 2096, 2097, 2098,
        2099, 2101, 2102, 2103, 2104, 2105, 2106, 2107,
        2108, 2109, 2110, 2111, 2112, 2113, 2114, 2115, 2116,
        2117, 2118, 2119, 2120, 2121, 2122, 2123, 2124, 2125,
        2126, 2127, 2128, 2129, 2130, 2131, 2132, 2133, 2134,
        2135, 2136, 2137, 2138, 2139, 2140, 2141, 2142, 2143,
        2144, 2145, 2146, 2147, 2148, 2149, 2150, 2151, 2152,
        2153, 2154, 2155, 2156, 2157, 2158, 2159, 2160, 2161,
        2162, 2163, 2164, 2165, 2167, 2168, 2174, 2175, 2176,
        2177, 2178, 2179, 2180, 2181, 2182, 2183, 2184, 2185,
        2186, 2187, 2222, 2223, 2224, 2225, 2226, 2227, 2228,
        2229, 2230, 2231, 2232, 2233, 2234, 2235, 2236, 2237,
        2238, 2239, 2240, 2241, 2242, 2243, 2244, 2245, 2246,
        2247, 2248, 2249, 2250, 2251, 2252, 2253, 2254, 2283,
        2284, 2285, 2286, 2287, 2288, 2289, 2290, 2291, 2292,
        2293, 2294, 2295, 2296, 2297, 2298, 2299, 2300, 20316,
        20317, 20318, 20319, 20320, 20321
    ];

    protected override ReadOnlySpan<int> ItemIds => FenceItemIds;
    protected override string CommandFormat => "{0}Tile Static {1}";
    protected override StaticItemGumpBase CreateInstance(int page) => new AddFenceGump(page);

    public static void Initialize()
    {
        CommandSystem.Register("AddFence", AccessLevel.GameMaster, AddFence_OnCommand);
    }

    [Usage("AddFence")]
    [Description("Displays a menu from which you can interactively add fences.")]
    private static void AddFence_OnCommand(CommandEventArgs e)
    {
        e.Mobile.CloseGump<AddFenceGump>();
        e.Mobile.SendGump(new AddFenceGump());
    }

    public AddFenceGump(int page = 0) : base(page)
    {
    }
}
