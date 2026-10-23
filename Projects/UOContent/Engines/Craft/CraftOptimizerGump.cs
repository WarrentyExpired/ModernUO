using System;
using System.Collections.Generic;
using Server.Gumps;
using Server.Items;
using Server.Network;

namespace Server.Engines.Craft;

public class CraftOptimizerGump : DynamicGump
{
    private const int LabelHue = 0x480;
    private const int LabelColor = 0x7FFF;
    private readonly Mobile _from;
    private readonly CraftSystem _craftSystem;
    private readonly BaseTool _tool;
    private readonly List<OptimizerEntry> _eligibleItems = new();

    private class OptimizerEntry
    {
        public CraftItem Item { get; init; }
        public double Chance { get; init; }
    }

    public override bool Singleton => true;

    public CraftOptimizerGump(Mobile from, CraftSystem craftSystem, BaseTool tool) : base(40, 40)
    {
        _from = from;
        _craftSystem = craftSystem;
        _tool = tool;
        PopulateEligibleItems();
    }

    private void PopulateEligibleItems()
    {
        var context = _craftSystem.GetContext(_from);

        foreach (var group in _craftSystem.CraftGroups)
        {
            foreach (var craftItem in group.CraftItems)
            {
                // Pull definitions matching the player's current sub-material selection (e.g., Dull Copper vs Iron)
                var res = craftItem.UseSubRes2 ? _craftSystem.CraftSubRes2 : _craftSystem.CraftSubRes;
                var resIndex = -1;

                if (context != null)
                {
                    resIndex = craftItem.UseSubRes2 ? context.LastResourceIndex2 : context.LastResourceIndex;
                }

                // Run the exact success engine calculation used by the item details page
                var chance = craftItem.GetSuccessChance(
                    _from,
                    resIndex > -1 ? res.GetAt(resIndex).ItemType : null,
                    _craftSystem,
                    false,
                    out _
                );

                // OPTIMIZER FILTER: Only track items that are possible to make (>0%) but still give gains (<100%)[cite: 9]
                if (chance > 0.0 && chance < 1.0)
                {
                    _eligibleItems.Add(new OptimizerEntry { Item = craftItem, Chance = chance });
                }
            }
        }

        // Sort by lowest success chance up to highest so the best difficulty options appear first
        _eligibleItems.Sort((a, b) => a.Chance.CompareTo(b.Chance));
    }

    protected override void BuildLayout(ref DynamicGumpBuilder builder)
    {
        builder.AddPage();
        builder.AddBackground(0, 0, 530, 437, 5054);
        builder.AddImageTiled(10, 10, 510, 22, 2624);
        builder.AddImageTiled(10, 37, 510, 300, 2624);
        builder.AddImageTiled(10, 342, 510, 85, 2624);
        builder.AddAlphaRegion(10, 10, 510, 417);

        // Header Title
        if (_craftSystem.GumpTitle.Number > 0)
            builder.AddHtmlLocalized(15, 12, 510, 20, _craftSystem.GumpTitle.Number, LabelColor);
        else
            builder.AddLabel(15, 12, LabelHue, $"{_craftSystem.GumpTitle.String} - SKILL GAIN GUIDE");

        // Column Titles
        builder.AddHtmlLocalized(60, 42, 250, 22, 1044053, LabelColor); // ITEM[cite: 9]
        builder.AddLabel(370, 42, LabelHue, "Success Chance");

        // Back Button
        builder.AddButton(15, 395, 4014, 4016, 0);
        builder.AddHtmlLocalized(50, 397, 150, 18, 1044150, LabelColor); // BACK[cite: 9]

        if (_eligibleItems.Count == 0)
        {
            builder.AddLabel(40, 120, LabelHue, "No optimal skill gain items found for your current level.");
            return;
        }

        // Paginate items 10 per view layer[cite: 8]
        for (var i = 0; i < _eligibleItems.Count; i++)
        {
            var index = i % 10;
            var entry = _eligibleItems[i];

            if (index == 0)
            {
                if (i > 0)
                {
                    builder.AddButton(485, 345, 4005, 4007, 0, GumpButtonType.Page, i / 10 + 1); // NEXT[cite: 8]
                }

                builder.AddPage(i / 10 + 1);

                if (i > 0)
                {
                    builder.AddButton(455, 345, 4014, 4015, 0, GumpButtonType.Page, i / 10); // PREV[cite: 8]
                }
            }

            // Draw selection button (Small blue detail button style)[cite: 8]
            builder.AddButton(20, 70 + index * 24, 4011, 4012, i + 1);

            if (entry.Item.NameNumber > 0)
                builder.AddHtmlLocalized(55, 72 + index * 24, 280, 18, entry.Item.NameNumber, LabelColor);
            else
                builder.AddLabel(55, 72 + index * 24, LabelHue, entry.Item.NameString);

            builder.AddLabel(370, 72 + index * 24, LabelHue, $"{Math.Clamp(entry.Chance, 0, 1) * 100:F1}%");
        }
    }

    public override void OnResponse(NetState sender, in RelayInfo info)
    {
        if (info.ButtonID == 0)
        {
            // Return to main menu safely[cite: 9]
            _from.SendGump(new CraftGump(_from, _craftSystem, _tool, null));
            return;
        }

        var index = info.ButtonID - 1;

        if (index >= 0 && index < _eligibleItems.Count)
        {
            var entry = _eligibleItems[index];

            // Forward the player straight to the item details menu so they can immediately craft it!
            _from.SendGump(new CraftGumpItem(_from, _craftSystem, entry.Item, _tool));
        }
    }
}
