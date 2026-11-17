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

public class DashboardCommand
{
    public static void Initialize()
    {
        CommandSystem.Register("dashboard", AccessLevel.Player, Dashboard_OnCommand);
    }

    [Usage("dashboard")]
    [Description("Opens the Player Statistics dashboard.")]
    public static void Dashboard_OnCommand(CommandEventArgs e) => PlayerStatsGump.DisplayTo(e.Mobile);
}

public class PlayerStatsGump(Mobile from) : DynamicGump(50, 50)
{
    // Color Constants
    private const int C_YELLOW = 2213;
    private const int C_WHITE = 1152;
    private const int C_RED = 1258;
    private const int C_BLUE = 1264;
    private const int C_GREEN = 1270;
    private const int C_PURPLE = 1276;
    private const int C_GREY = 2401;

    // Column X-Coordinates
    private const int LeftCol = 30;
    private const int RightCol = 340;
    private const int ThirdCol = 650;

    public override bool Singleton => true;

    public static void DisplayTo(Mobile from)
    {
        if (from is not { Deleted: false, NetState: not null })
        {
            return;
        }

        from.SendGump(new PlayerStatsGump(from));
    }

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
        const int width = 960;
        int height = Core.AOS ? 700 : 515;

        const int attrY = 55;
        const int resY = 145;
        const int colY = 205;

        PlayerMobile pm = from as PlayerMobile;

        builder.AddPage();
        builder.AddBackground(0, 0, width, height, 9270);
        builder.AddAlphaRegion(10, 10, width - 20, height - 20);

        // --- HEADER ---
        builder.AddHtml(0, 25, width, 25, ColorAndCenter("#FFCC00", from.Name));

        builder.AddLabel(width - 110, 20, C_YELLOW, "Refresh");
        builder.AddButton(width - 55, 21, 0x15E1, 0x15E5, 1, GumpButtonType.Reply, 0);

        // --- ATTRIBUTES (Shared Across Eras) ---
        builder.AddLabel(LeftCol, attrY, C_YELLOW, "Attributes");

        StatRow[] stats =
        [
            new(LeftCol,  attrY + 25, "Strength",     from.Str.ToString()),
            new(LeftCol,  attrY + 50, "Hit Points",   $"{from.Hits} / {from.HitsMax}"),
            new(RightCol, attrY + 25, "Dexterity",    from.Dex.ToString()),
            new(RightCol, attrY + 50, "Stamina",      $"{from.Stam} / {from.StamMax}"),
            new(ThirdCol, attrY + 25, "Intelligence", from.Int.ToString()),
            new(ThirdCol, attrY + 50, "Mana",         $"{from.Mana} / {from.ManaMax}"),
        ];

        AddRows(ref builder, stats);

        DrawDivider(ref builder, 20, resY - 5, width - 40);

        if (Core.AOS)
        {
            BuildAosLayout(ref builder, pm, width, resY, colY);
        }
        else
        {
            BuildUorLayout(ref builder, pm, width, resY, colY);
        }

        // --- 3rd Column: SKILL LIST (Shared Across Eras) ---
        var skills = from.Skills;
        double humanMinimum = Core.AOS && from.Race == Race.Human ? 20.0 : 0.0;
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

        int skillBoxHeight = Core.AOS ? 430 : 255;
        builder.AddHtml(
            ThirdCol, colY + 30, 280, skillBoxHeight,
            $"<BASEFONT COLOR=#C8C8C8>{skillHtml}</BASEFONT>", scrollbar: true
        );
    }

    private void BuildAosLayout(ref DynamicGumpBuilder builder, PlayerMobile pm, int width, int resY, int colY)
    {
        int Aos(AosAttribute attribute) => AosAttributes.GetValue(from, attribute);

        // --- RESISTANCES ---
        builder.AddLabel(435, resY + 5, C_YELLOW, "Resistances");

        StatRow[] resists =
        [
            new(100, resY + 30, "Phys",   $"{from.PhysicalResistance}%", C_GREY,   55),
            new(260, resY + 30, "Fire",   $"{from.FireResistance}%",     C_RED,    55),
            new(420, resY + 30, "Cold",   $"{from.ColdResistance}%",     C_BLUE,   55),
            new(580, resY + 30, "Poison", $"{from.PoisonResistance}%",   C_GREEN,  55),
            new(740, resY + 30, "Energy", $"{from.EnergyResistance}%",   C_PURPLE, 55),
        ];

        AddRows(ref builder, resists);

        // --- MAIN COLUMNS ---
        DrawDivider(ref builder, 20, colY - 5, width - 40);

        builder.AddLabel(LeftCol, colY + 5, C_YELLOW, "Combat Ratings");
        builder.AddLabel(RightCol, colY + 5, C_YELLOW, "Magic Ratings");
        builder.AddLabel(RightCol, colY + 140, C_YELLOW, "Miscellaneous");
        builder.AddLabel(ThirdCol, colY + 5, C_YELLOW, "Skills");

        double swingSpeed = from.Weapon is BaseWeapon weapon ? weapon.GetDelay(from).TotalSeconds : 0.0;
        double bandageSeconds = 5.0 + 0.5 * ((120.0 - from.Dex) / 10.0);
        int lAmmo = from.FindItemOnLayer(Layer.Cloak) is BaseQuiver quiver ? quiver.LowerAmmoCost : 0;

        StatRow[] ratings =
        [
            // 1st Column: Combat
            new(LeftCol, colY + 30,  "HCI",           $"{Aos(AosAttribute.AttackChance)} / 45"),
            new(LeftCol, colY + 50,  "DCI",           $"{Aos(AosAttribute.DefendChance)} / 45"),
            new(LeftCol, colY + 70,  "DI",            $"{Aos(AosAttribute.WeaponDamage)} / 100"),
            new(LeftCol, colY + 90,  "SSI",           $"{Aos(AosAttribute.WeaponSpeed)} / 60"),
            new(LeftCol, colY + 130, "Swing Speed",   swingSpeed > 0 ? $"{swingSpeed:F2}s" : "---"),
            new(LeftCol, colY + 150, "Bandage Speed", $"{bandageSeconds:F1}s"),
            new(LeftCol, colY + 185, "HP Regen",      $"{Aos(AosAttribute.RegenHits)}"),
            new(LeftCol, colY + 205, "Stam Regen",    $"{Aos(AosAttribute.RegenStam)}"),
            new(LeftCol, colY + 225, "Mana Regen",    $"{Aos(AosAttribute.RegenMana)}"),
            new(LeftCol, colY + 265, "HP Increase",   $"{Aos(AosAttribute.BonusHits)} / 25"),
            new(LeftCol, colY + 285, "Stam Increase", $"{Aos(AosAttribute.BonusStam)} / 25"),
            new(LeftCol, colY + 305, "Mana Increase", $"{Aos(AosAttribute.BonusMana)} / 25"),
            new(LeftCol, colY + 345, "Str Bonus",     $"{Aos(AosAttribute.BonusStr)}"),
            new(LeftCol, colY + 365, "Dex Bonus",     $"{Aos(AosAttribute.BonusDex)}"),
            new(LeftCol, colY + 385, "Int Bonus",     $"{Aos(AosAttribute.BonusInt)}"),
            new(LeftCol, colY + 425, "Enhance Pots",  $"{Aos(AosAttribute.EnhancePotions)} / 50"),
            new(LeftCol, colY + 445, "Lower Ammo",    $"{lAmmo}"),

            // 2nd Column: Magic
            new(RightCol, colY + 30,  "SDI",     $"{Aos(AosAttribute.SpellDamage)}%"),
            new(RightCol, colY + 50,  "LRC",     $"{Aos(AosAttribute.LowerRegCost)}%"),
            new(RightCol, colY + 70,  "LMC",     $"{Aos(AosAttribute.LowerManaCost)}%"),
            new(RightCol, colY + 90,  "FC",      $"{Aos(AosAttribute.CastSpeed)} / 4"),
            new(RightCol, colY + 110, "FCR",     $"{Aos(AosAttribute.CastRecovery)} / 6"),

            // 2nd Column: Miscellaneous
            new(RightCol, colY + 165, "Followers",       $"{from.Followers} / {from.FollowersMax}"),
            new(RightCol, colY + 185, "Weight",          $"{from.TotalWeight} / {from.MaxWeight}"),
            new(RightCol, colY + 205, "Luck",            $"{Aos(AosAttribute.Luck)}"),
            new(RightCol, colY + 225, "Fame",            $"{from.Fame}"),
            new(RightCol, colY + 245, "Karma",           $"{from.Karma}"),
            new(RightCol, colY + 265, "Tithing",         $"{pm?.TithingPoints ?? 0}"),
            new(RightCol, colY + 305, "Player Deaths",   pm != null ? pm.TotalPlayerDeaths.ToString("N0") : "0"),
            new(RightCol, colY + 325, "Monsters Killed", pm != null ? pm.TotalMonstersKilled.ToString("N0") : "0"),
            new(RightCol, colY + 345, "Gold Looted",     pm != null ? pm.TotalGoldLooted.ToString("N0") : "0"),
            new(RightCol, colY + 385, "Spendable KP",    pm != null ? pm.MonsterKillPoints.ToString("N0") : "0"),
            new(RightCol, colY + 405, "Lifetime KP",     pm != null ? pm.LifetimeKillPoints.ToString("N0") : "0"),
        ];

        AddRows(ref builder, ratings);
    }

    private void BuildUorLayout(ref DynamicGumpBuilder builder, PlayerMobile pm, int width, int resY, int colY)
    {
        // --- DEFENSE & OVERVIEW BAR ---
        builder.AddLabel(415, resY + 5, C_YELLOW, "Defense & Overview");

        int ar = (int)Math.Round(from.ArmorRating, MidpointRounding.AwayFromZero);
        double magicResist = from.Skills.MagicResist.Value;
        double totalSkills = from.SkillsTotal / 10.0;
        double skillCap = from.SkillsCap / 10.0;

        StatRow[] overview =
        [
            new(120, resY + 30, "Armor Rating", $"{ar}", C_BLUE, 105),
            new(400, resY + 30, "Magic Resist", $"{magicResist:F1}", C_PURPLE, 105),
            new(680, resY + 30, "Skill Total",  $"{totalSkills:F1} / {skillCap:F1}", C_GREEN, 95),
        ];

        AddRows(ref builder, overview);

        // --- MAIN COLUMNS ---
        DrawDivider(ref builder, 20, colY - 5, width - 40);

        builder.AddLabel(LeftCol, colY + 5, C_YELLOW, "Combat & Utility");
        builder.AddLabel(RightCol, colY + 5, C_YELLOW, "Character & Progression");
        builder.AddLabel(ThirdCol, colY + 5, C_YELLOW, "Skills");

        BaseWeapon weapon = from.Weapon as BaseWeapon;
        double swingSpeed = weapon != null ? weapon.GetDelay(from).TotalSeconds : 0.0;
        string damageRange = weapon != null ? $"{weapon.MinDamage} - {weapon.MaxDamage}" : "---";

        // Classic UOR self-bandage timer: 11.0s - (Dex / 20.0) (minimum 2.0s)
        double selfBandage = Math.Max(2.0, 11.0 - (from.Dex / 20.0));
        double otherBandage = Math.Max(1.5, 4.0 - (from.Dex / 60.0));

        // Check if equipped armor allows active/passive meditation
        bool canMeditate = CheckMeditationAllowed(from);

        // Classic UOR Tactics + Anatomy melee damage % bonus
        double tacticsBonus = (from.Skills.Tactics.Value - 50.0) / 2.0;
        double anatomyBonus = from.Skills.Anatomy.Value / 5.0;
        int totalDamageBonus = (int)Math.Round(tacticsBonus + anatomyBonus);

        StatRow[] uorStats =
        [
            // 1st Column: Combat & Utility
            new(LeftCol, colY + 30,  "Base Damage",   damageRange),
            new(LeftCol, colY + 50,  "Swing Speed",   swingSpeed > 0 ? $"{swingSpeed:F2}s" : "---"),
            new(LeftCol, colY + 70,  "Skill DMG Bonus", $"{totalDamageBonus:+#;-#;0}%"),
            new(LeftCol, colY + 105, "Heal (Self)",   $"{selfBandage:F1}s"),
            new(LeftCol, colY + 125, "Heal (Other)",  $"{otherBandage:F1}s"),
            new(LeftCol, colY + 160, "Meditation",    canMeditate ? "Active" : "Blocked (Armor)", canMeditate ? C_GREEN : C_RED),
            new(LeftCol, colY + 180, "Followers",     $"{from.Followers} / {from.FollowersMax}"),
            new(LeftCol, colY + 200, "Weight",        $"{from.TotalWeight} / {from.MaxWeight}"),

            // 2nd Column: Character & Progression
            new(RightCol, colY + 30,  "Fame",            $"{from.Fame}"),
            new(RightCol, colY + 50,  "Karma",           $"{from.Karma}"),
            new(RightCol, colY + 85,  "Player Deaths",   pm != null ? pm.TotalPlayerDeaths.ToString("N0") : "0"),
            new(RightCol, colY + 105, "Monsters Killed", pm != null ? pm.TotalMonstersKilled.ToString("N0") : "0"),
            new(RightCol, colY + 125, "Gold Looted",     pm != null ? pm.TotalGoldLooted.ToString("N0") : "0"),
            new(RightCol, colY + 160, "Spendable KP",    pm != null ? pm.MonsterKillPoints.ToString("N0") : "0"),
            new(RightCol, colY + 180, "Lifetime KP",     pm != null ? pm.LifetimeKillPoints.ToString("N0") : "0"),
        ];

        AddRows(ref builder, uorStats);
    }

    private static bool CheckMeditationAllowed(Mobile m)
    {
        for (int i = 0; i < m.Items.Count; i++)
        {
            if (m.Items[i] is BaseArmor armor && armor.MeditationAllowance == ArmorMeditationAllowance.None)
            {
                return false;
            }
        }

        return true;
    }

    public override void OnResponse(NetState sender, in RelayInfo info)
    {
        if (info.ButtonID == 1)
        {
            DisplayTo(from);
        }
    }

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
