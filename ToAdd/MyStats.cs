// UO Wildlands Custom Script
// Adapted for ModernUO
// Licensed under the GNU General Public License v3.0 (GPL-3.0)
// Derived from ServUO Core and Community scripts (Original Authors Unknown)
// Compiled & Modified by: [Feng / UO Wildlands Team]

using System;
using System.Collections.Generic;
using System.Text;
using Server.Accounting;
using Server.Commands;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Gumps;

public class MyStatsCommand
{
    public static void Initialize()
    {
        CommandSystem.Register("mystats", AccessLevel.Player, MyStats_OnCommand);
    }

    [Usage("mystats")]
    [Description("Opens the Player Statistics dashboard.")]
    public static void MyStats_OnCommand(CommandEventArgs e) => PlayerStatsGump.DisplayTo(e.Mobile);
}

public class PlayerStatsGump(Mobile from) : DynamicGump(50, 50)
{
    // Color Constants
    private const int C_YELLOW = 2213;
    private const int C_WHITE = 2036;
    private const int C_RED = 1258;
    private const int C_BLUE = 1264;
    private const int C_GREEN = 1270;
    private const int C_PURPLE = 1276;
    private const int C_GREY = 2401;

    private const int RightCol = 340; // right-hand column X: Magic Ratings / Miscellaneous / Skills

    public override bool Singleton => true;

    public static void DisplayTo(Mobile from)
    {
        if (from is not { Deleted: false, NetState: not null })
        {
            return;
        }

        from.SendGump(new PlayerStatsGump(from));
    }

    // One "label: value" line anywhere in the gump. ValueOffset/ValueHue cover the
    // resistance boxes, whose colored value sits 55px from the name instead of 130px.
    private readonly struct StatRow(int x, int y, string name, string value, int valueHue = C_WHITE, int valueOffset = 130)
    {
        public readonly int X = x;
        public readonly int Y = y;
        public readonly string Name = name;
        public readonly string Value = value;
        public readonly int ValueHue = valueHue;
        public readonly int ValueOffset = valueOffset;
    }

    protected override void BuildLayout(ref DynamicGumpBuilder builder)
    {
        const int width = 640;
        const int height = 850;
        const int attrY = 55;
        const int resY = 175;
        const int colY = 220;

        // Short alias: every AOS property fetch below reads from the same mobile.
        int Aos(AosAttribute attribute) => AosAttributes.GetValue(from, attribute);

        builder.AddPage();
        builder.AddBackground(0, 0, width, height, 9200);
        builder.AddImageTiled(10, 10, width - 20, height - 20, 2624);
        builder.AddAlphaRegion(10, 10, width - 20, height - 20);

        // --- HEADER ---
        builder.AddHtml(0, 25, width, 25, ColorAndCenter("#FFCC00", from.Name));

        builder.AddButton(width - 55, 21, 0xFBE, 0xFBF, 1, GumpButtonType.Reply, 0); // Refresh

        // --- ATTRIBUTES ---
        builder.AddLabel(30, attrY, C_YELLOW, "Attributes");

        StatRow[] stats =
        [
            new(30, attrY + 25,  "Strength",     from.Str.ToString()),
            new(RightCol, attrY + 25, "Hit Points", $"{from.Hits} / {from.HitsMax}"),
            new(30, attrY + 50,  "Dexterity",    from.Dex.ToString()),
            new(RightCol, attrY + 50, "Stamina",  $"{from.Stam} / {from.StamMax}"),
            new(30, attrY + 75,  "Intelligence", from.Int.ToString()),
            new(RightCol, attrY + 75, "Mana",     $"{from.Mana} / {from.ManaMax}"),
        ];

        AddRows(ref builder, stats);

        // --- RESISTANCES ---
        DrawDivider(ref builder, 20, resY - 5, width - 40);
        builder.AddLabel(30, resY + 5, C_YELLOW, "Resistances");

        StatRow[] resists =
        [
            new(130, resY + 5, "Phys",   $"{from.PhysicalResistance}%", C_GREY,   55),
            new(230, resY + 5, "Fire",   $"{from.FireResistance}%",     C_RED,    55),
            new(330, resY + 5, "Cold",   $"{from.ColdResistance}%",     C_BLUE,   55),
            new(430, resY + 5, "Poison", $"{from.PoisonResistance}%",   C_GREEN,  55),
            new(530, resY + 5, "Energy", $"{from.EnergyResistance}%",   C_PURPLE, 55),
        ];

        AddRows(ref builder, resists);

        // --- RATINGS (Combat & Magic) ---
        DrawDivider(ref builder, 20, colY - 5, width - 40);
        builder.AddLabel(30, colY + 5, C_YELLOW, "Combat Ratings");
        builder.AddLabel(RightCol, colY + 5, C_YELLOW, "Magic Ratings");

        double swingSpeed = from.Weapon is BaseWeapon weapon ? weapon.GetDelay(from).TotalSeconds : 0.0;

        // Standard AOS bandage math, clamped to [2.0, 8.0]
        double bandageSeconds = Math.Clamp(Math.Ceiling(11.0 - from.Dex / 10.0), 2.0, 8.0);

        // ModernUO: ammo-cost reduction lives on the equipped quiver (Layer.Cloak),
        // not as a global AOS attribute.
        int lAmmo = from.FindItemOnLayer(Layer.Cloak) is BaseQuiver quiver ? quiver.LowerAmmoCost : 0;

        // TODO: AbsorptionAttributes / SAAbsorptionAttribute is a custom shard system
        // not present in ModernUO. Stubbed at 0 until it's wired back in.
        int dmgEater = 0;

        // TODO: PointsSystem / CleanUpBritannia is a custom shard system
        // not present in ModernUO. Stubbed at 0 until it's wired back in.
        const double cubPoints = 0;

        StatRow[] ratings =
        [
            // Left side: Combat
            new(30, colY + 30,  "HCI",           $"{Aos(AosAttribute.AttackChance)} / 45"),
            new(30, colY + 50,  "DCI",           $"{Aos(AosAttribute.DefendChance)} / 45"),
            new(30, colY + 70,  "DI",            $"{Aos(AosAttribute.WeaponDamage)} / 100"),
            new(30, colY + 90,  "SSI",           $"{Aos(AosAttribute.WeaponSpeed)} / 60"),
            new(30, colY + 130, "Swing Speed",   swingSpeed > 0 ? $"{swingSpeed:F2}s" : "---"),
            new(30, colY + 150, "Bandage Speed", $"{bandageSeconds:F1}s"),
            new(30, colY + 185, "HP Regen",      $"{Aos(AosAttribute.RegenHits)}"),
            new(30, colY + 205, "Stam Regen",    $"{Aos(AosAttribute.RegenStam)}"),
            new(30, colY + 225, "Mana Regen",    $"{Aos(AosAttribute.RegenMana)}"),
            new(30, colY + 265, "HP Increase",   $"{Aos(AosAttribute.BonusHits)} / 25"),
            new(30, colY + 285, "Stam Increase", $"{Aos(AosAttribute.BonusStam)} / 25"),
            new(30, colY + 305, "Mana Increase", $"{Aos(AosAttribute.BonusMana)} / 25"),
            new(30, colY + 345, "Str Bonus",     $"{Aos(AosAttribute.BonusStr)}"),
            new(30, colY + 365, "Dex Bonus",     $"{Aos(AosAttribute.BonusDex)}"),
            new(30, colY + 385, "Int Bonus",     $"{Aos(AosAttribute.BonusInt)}"),
            new(30, colY + 425, "Damage Eater",  $"{dmgEater} / 30"),
            new(30, colY + 445, "Enhance Pots",  $"{Aos(AosAttribute.EnhancePotions)} / 50"),
            new(30, colY + 465, "Lower Ammo",    $"{lAmmo}"),
            new(30, colY + 505, "Followers",     $"{from.Followers} / {from.FollowersMax}"),
            new(30, colY + 525, "Weight",        $"{from.TotalWeight} / {from.MaxWeight}"),

            // Right side: Magic
            new(RightCol, colY + 30,  "SDI",     $"{Aos(AosAttribute.SpellDamage)}%"),
            new(RightCol, colY + 50,  "LRC",     $"{Aos(AosAttribute.LowerRegCost)}%"),
            new(RightCol, colY + 70,  "LMC",     $"{Aos(AosAttribute.LowerManaCost)}%"),
            new(RightCol, colY + 90,  "FC",      $"{Aos(AosAttribute.CastSpeed)} / 4"),
            new(RightCol, colY + 110, "FCR",     $"{Aos(AosAttribute.CastRecovery)} / 6"),

            // Miscellaneous
            new(RightCol, colY + 165, "Luck",       $"{Aos(AosAttribute.Luck)}"),
            new(RightCol, colY + 185, "Fame",       $"{from.Fame}"),
            new(RightCol, colY + 205, "Karma",      $"{from.Karma}"),
            new(RightCol, colY + 225, "Tithing",    $"{(from as PlayerMobile)?.TithingPoints ?? 0}"),
            new(RightCol, colY + 265, "Acct Gold",  $"{(from.Account is Account acct ? acct.GetTotalGold() : 0):N0}"),
            new(RightCol, colY + 285, "CUB Points", $"{cubPoints:N0}"),
        ];

        AddRows(ref builder, ratings);

        builder.AddLabel(RightCol, colY + 140, C_YELLOW, "Miscellaneous");

        // --- SKILL LIST (native client scrollbar, area Y 595..840) ---
        builder.AddLabel(RightCol, colY + 345, C_YELLOW, "Skills");

        var skills = from.Skills;

        // Filters out Human Jack of All Trades (20.0) unless base is invested or gear pushes it higher
        double humanMinimum = from.Race == Race.Human ? 20.0 : 0.0;

        // Collect visible skills and sort alphabetically (gump rebuild only, not a hot path).
        var visibleSkills = new List<Skill>(skills.Length);

        for (int i = 0; i < skills.Length; i++)
        {
            Skill sk = skills[i];

            if (sk.Base > 0 || (sk.Value > sk.Base && sk.Value > humanMinimum))
            {
                visibleSkills.Add(sk);
            }
        }

        visibleSkills.Sort(static (a, b) => string.CompareOrdinal(a.Name, b.Name));

        // One left-aligned block with <BR> separators: names form a straight left edge,
        // values follow the names. (A <DIV> per line would add paragraph spacing from the renderer.)
        var skillHtml = new StringBuilder(2048);

        for (int i = 0; i < visibleSkills.Count; i++)
        {
            Skill sk = visibleSkills[i];

            if (i > 0)
            {
                skillHtml.Append("<BR>");
            }

            skillHtml.Append($"{sk.Name} {sk.Value:F1} / {sk.Cap:F1}");
        }

        // scrollbar is named arg #11 in current ModernUO: AddHtml(x,y,w,h,text,color,size,fontStyle,align,background,scrollbar)
        // Height 225 ends the area at Y=820 so the scrollbar's down-arrow clears the window frame.
        builder.AddHtml(
            RightCol, colY + 375, 280, 225,
            $"<BASEFONT COLOR=#C8C8C8>{skillHtml}</BASEFONT>", scrollbar: true
        );
    }

    public override void OnResponse(NetState sender, in RelayInfo info)
    {
        if (info.ButtonID == 1) // Refresh
        {
            DisplayTo(from);
        }
    }

    // Renders any batch of StatRows — attributes, resistances and ratings all share this loop.
    private static void AddRows(ref DynamicGumpBuilder builder, ReadOnlySpan<StatRow> rows)
    {
        for (int i = 0; i < rows.Length; i++)
        {
            var r = rows[i];
            builder.AddLabel(r.X, r.Y, C_WHITE, r.Name);
            builder.AddLabel(r.X + r.ValueOffset, r.Y, r.ValueHue, r.Value);
        }
    }

    private static string ColorAndCenter(string color, string text) =>
        $"<BASEFONT COLOR={color}><CENTER>{text}</CENTER></BASEFONT>";

    private static void DrawDivider(ref DynamicGumpBuilder builder, int x, int y, int width) =>
        builder.AddImageTiled(x, y, width, 2, 9264);
}
