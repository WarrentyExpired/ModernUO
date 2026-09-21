using System;
using System.Collections.Generic;
using Server.Gumps;
using Server.Network;

namespace Server.Custom.ExileHunterBestiary.Gumps;

public sealed class HunterBestiaryGump : DynamicGump
{
    private const int EntriesPerPage = 10;

    private const int ButtonClose = 0;
    private const int ButtonNext = 10;
    private const int ButtonPrev = 11;
    private const int ButtonToggleFilter = 12;
    private const int ButtonSearch = 13;
    private const int TextSearch = 1;
    private const int ButtonCategoryBase = 100;
    private const int ButtonCategoryLimit = 150;

    private readonly Mobile _player;
    private readonly BestiaryCategory _category;
    private readonly int _page;
    private readonly string _search;
    private readonly bool _hideMastered;

    public override bool Singleton => true;

    private HunterBestiaryGump(Mobile player, BestiaryCategory category, int page, string search, bool hideMastered)
        : base(100, 40)
    {
        _player = player;
        _category = category;
        _page = page;
        _search = search ?? "";
        _hideMastered = hideMastered;
    }

    public static void DisplayTo(
        Mobile from, BestiaryCategory category = BestiaryCategory.Undead,
        int page = 0, string search = "", bool hideMastered = false)
    {
        if (from is not { Deleted: false } || !HunterBestiaryEngine.Enabled)
        {
            return;
        }

        from.SendGump(new HunterBestiaryGump(from, category, page, search, hideMastered));
    }

    protected override void BuildLayout(ref DynamicGumpBuilder builder)
    {
        var profile = HunterBestiaryEngine.TryGetProfile(_player, out var p) ? p : null;
        var totalMastered = profile?.TotalMastered ?? 0;
        var grandTotal = BestiaryRegistry.TotalCount;
        var grandPct = grandTotal > 0 ? totalMastered * 100.0 / grandTotal : 0;

        builder.AddPage();
        builder.AddBackground(0, 0, 660, 560, 9270);
        builder.AddAlphaRegion(10, 10, 640, 540);
        builder.AddBackground(14, 14, 632, 532, 9200);
        builder.AddAlphaRegion(18, 18, 624, 524);


        builder.AddItem(25, 20, 0x2252, 1153);
        builder.AddHtml(85, 20, 310, 24, "<BASEFONT COLOR=#FFFFFF SIZE=5>The Grand Hunter's Bestiary</BASEFONT>");
        builder.AddHtml(85, 46, 310, 20, "<BASEFONT COLOR=#D1C4E9 SIZE=3>Studied Treatises & Anatomical Lore</BASEFONT>");
        builder.AddHtml(
            400, 24, 240, 22,
            $"<BASEFONT COLOR=#ECEFF1>Overall: <BASEFONT COLOR=#00FFCC>{totalMastered}/{grandTotal}</BASEFONT> ({grandPct:F0}%)</BASEFONT>"
        );
        builder.AddHtml(
            400, 46, 240, 20,
            $"<BASEFONT COLOR=#FFE082>Cap +{HunterBestiaryEngine.TotalBonusCap:P0}"
            + $" - {(HunterBestiaryEngine.DamageMode == DamageMode.Mark ? "Hunter's Mark" : "Multiplier")}</BASEFONT>"
        );

        builder.AddImageTiled(20, 72, 620, 2, 9354);
        builder.AddImageTiled(195, 78, 2, 428, 9354);


        var catY = 82;
        foreach (var cat in Enum.GetValues<BestiaryCategory>())
        {
            var selected = cat == _category;
            var catTotal = BestiaryRegistry.ByCategory[cat].Count;
            var catMastered = profile?.GetCategoryMasteredCount(cat) ?? 0;

            builder.AddButton(24, catY + 4, selected ? 4006 : 4005, 4007, ButtonCategoryBase + (int)cat);

            var catColor = selected ? "#00FFCC" : catMastered == catTotal && catTotal > 0 ? "#69F0AE" : "#ECEFF1";
            builder.AddHtml(
                60, catY + 5, 130, 20,
                $"<BASEFONT COLOR={catColor}>{cat}</BASEFONT> <BASEFONT COLOR=#90A4AE>({catMastered}/{catTotal})</BASEFONT>"
            );

            catY += 34;
        }


        builder.AddButton(24, catY + 8, _hideMastered ? 4006 : 4005, 4007, ButtonToggleFilter);
        builder.AddHtml(
            60, catY + 9, 130, 20,
            $"<BASEFONT COLOR={(_hideMastered ? "#00FFCC" : "#ECEFF1")}>Hide mastered</BASEFONT>"
        );


        var catList = BestiaryRegistry.ByCategory[_category];
        var catLearned = profile?.GetCategoryMasteredCount(_category) ?? 0;
        var catPct = catList.Count > 0 ? catLearned * 100.0 / catList.Count : 0;

        builder.AddHtml(205, 82, 430, 24, $"<BASEFONT COLOR=#FFFFFF SIZE=4>{_category} Studies</BASEFONT>");
        builder.AddHtml(
            205, 106, 430, 20,
            $"<BASEFONT COLOR=#ECEFF1>Mastery: <BASEFONT COLOR=#00FFCC>{catLearned} / {catList.Count}</BASEFONT> ({catPct:F0}%)"
            + $" - milestone bonus up to +{HunterBestiaryEngine.CategoryMilestoneBonus:P0}</BASEFONT>"
        );


        builder.AddTextEntry(205, 132, 300, 22, 0x480, TextSearch, _search);
        builder.AddButton(512, 132, 4005, 4007, ButtonSearch);
        builder.AddHtml(548, 134, 80, 20, "<BASEFONT COLOR=#ECEFF1>Search</BASEFONT>");

        builder.AddImageTiled(200, 160, 435, 2, 9354);


        var filtered = new List<HunterCreatureEntry>(catList.Count);
        foreach (var entry in catList)
        {
            var learned = profile != null && profile.HasLearned(entry.Id);
            if (_hideMastered && learned)
            {
                continue;
            }

            if (_search.Length > 0 &&
                entry.DisplayName.Contains(_search, StringComparison.OrdinalIgnoreCase) == false)
            {
                continue;
            }

            filtered.Add(entry);
        }

        filtered.Sort(static (a, b) => string.CompareOrdinal(a.DisplayName, b.DisplayName));

        var maxPages = Math.Max(1, (filtered.Count + EntriesPerPage - 1) / EntriesPerPage);
        var page = Math.Clamp(_page, 0, maxPages - 1);
        var start = page * EntriesPerPage;
        var end = Math.Min(start + EntriesPerPage, filtered.Count);

        var categoryFraction = catList.Count > 0 ? (double)catLearned / catList.Count : 0;

        var entryY = 170;
        for (var i = start; i < end; i++)
        {
            var creature = filtered[i];
            var learned = profile != null && profile.HasLearned(creature.Id);


            var bonus = (learned ? HunterBestiaryEngine.PerCreatureBonus : 0.0)
                        + HunterBestiaryEngine.CategoryMilestoneBonus * categoryFraction;
            bonus = Math.Min(bonus, HunterBestiaryEngine.TotalBonusCap);

            builder.AddBackground(200, entryY, 435, 32, learned ? 9200 : 9300);
            builder.AddAlphaRegion(202, entryY + 2, 431, 28);
            builder.AddImage(207, entryY + 5, learned ? 4017 : 4020);

            var nameColor = learned ? "#FFFFFF" : "#B0BEC5";
            builder.AddHtml(250, entryY + 6, 185, 20, $"<BASEFONT COLOR={nameColor}>{creature.DisplayName}</BASEFONT>");
            builder.AddHtml(
                440, entryY + 6, 190, 20,
                learned
                    ? $"<BASEFONT COLOR=#69F0AE>Mastered - +{HunterBestiaryEngine.PerCreatureBonus:P0} (+{bonus:P0} total)</BASEFONT>"
                    : $"<BASEFONT COLOR=#78909C>Pending Treatise - +{bonus:P0} milestone</BASEFONT>"
            );

            entryY += 36;
        }


        builder.AddImageTiled(20, 520, 620, 2, 9354);

        builder.AddButton(30, 528, 4017, 4019, ButtonClose);
        builder.AddHtml(65, 530, 50, 20, "<BASEFONT COLOR=#FFFFFF>Close</BASEFONT>");

        if (page > 0)
        {
            builder.AddButton(200, 528, 4014, 4016, ButtonPrev);
            builder.AddHtml(236, 530, 60, 20, "<BASEFONT COLOR=#ECEFF1>Previous</BASEFONT>");
        }

        builder.AddHtml(360, 530, 120, 20, $"<BASEFONT COLOR=#B0BEC5>Page {page + 1} of {maxPages}</BASEFONT>");

        if (page < maxPages - 1)
        {
            builder.AddButton(560, 528, 4005, 4007, ButtonNext);
            builder.AddHtml(590, 530, 40, 20, "<BASEFONT COLOR=#ECEFF1>Next</BASEFONT>");
        }
    }

    public override void OnResponse(NetState sender, in RelayInfo info)
    {
        if (sender.Mobile is not { } from)
        {
            return;
        }

        var search = info.GetTextEntry(TextSearch)?.Trim() ?? _search;

        switch (info.ButtonID)
        {
            case ButtonClose:
                return;
            case ButtonNext:
                DisplayTo(from, _category, _page + 1, search, _hideMastered);
                return;
            case ButtonPrev:
                DisplayTo(from, _category, Math.Max(0, _page - 1), search, _hideMastered);
                return;
            case ButtonToggleFilter:
                DisplayTo(from, _category, 0, search, !_hideMastered);
                return;
            case ButtonSearch:
                DisplayTo(from, _category, 0, search, _hideMastered);
                return;
            default:
            {
                if (info.ButtonID is >= ButtonCategoryBase and < ButtonCategoryLimit)
                {
                    DisplayTo(from, (BestiaryCategory)(info.ButtonID - ButtonCategoryBase), 0, search, _hideMastered);
                }

                return;
            }
        }
    }
}
