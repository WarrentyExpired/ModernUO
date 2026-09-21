using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using Server.Commands;
using Server.Custom.ExileHunterBestiary.Gumps;
using Server.Custom.ExileHunterBestiary.Hooks;
using Server.Logging;
using Server.Mobiles;

namespace Server.Custom.ExileHunterBestiary;

public static class HunterBestiaryEngine
{
    private static readonly ILogger _logger =
        LogFactory.GetLogger(typeof(HunterBestiaryEngine));

    private static readonly Dictionary<Serial, HunterProfile> _profiles = [];

    public const double DefaultDropChance = 0.02;
    public const double BossDropChance = 0.05;

    public static bool Enabled { get; private set; } = true;

    public static double PerCreatureBonus { get; private set; } = 0.10;

    public static double CategoryMilestoneBonus { get; private set; } = 0.20;

    public static double TotalBonusCap { get; private set; } = 0.25;

    public static int PityThreshold { get; private set; } = 40;

    public static DamageMode DamageMode { get; private set; } = DamageMode.Mark;

    public static double GlobalDropChance { get; private set; } = -1.0;

    public static bool Debug { get; private set; } = true;

    public static void Configure()
    {
        Enabled = GetBool("exileBestiary.enabled", true);
        PerCreatureBonus = GetDouble(
            "exileBestiary.damage.perCreatureBonus",
            0.10
        );
        CategoryMilestoneBonus = GetDouble(
            "exileBestiary.damage.categoryMilestoneBonus",
            0.20
        );
        TotalBonusCap = GetDouble(
            "exileBestiary.damage.totalCap",
            0.25
        );
        PityThreshold = GetInt(
            "exileBestiary.drop.pityThreshold",
            40
        );
        DamageMode = GetEnum(
            "exileBestiary.damage.mode",
            DamageMode.Mark
        );
        GlobalDropChance = GetDouble(
            "exileBestiary.drop.chance",
            -1.0
        );
        Debug = GetBool(
            "exileBestiary.debug",
            true
        );

        _logger.Information(
            "Config ({Source}): enabled={Enabled} mode={Mode} dropChance={DropChance} perCreature={PerCreature} milestone={Milestone} cap={Cap} pity={Pity} debug={Debug}",
            _configSource,
            Enabled,
            DamageMode,
            GlobalDropChance,
            PerCreatureBonus,
            CategoryMilestoneBonus,
            TotalBonusCap,
            PityThreshold,
            Debug
        );

        BestiaryRegistry.Initialize();

        if (!Enabled)
        {
            _logger.Warning(
                "System disabled via configuration (exileBestiary.enabled = false)."
            );

            return;
        }

        EventSink.WorldSave += OnWorldSave;
        EventSink.WorldLoad += OnWorldLoad;

        EventSink.AggressiveAction += static args =>
            HunterCombatTracker.Note(args.Aggressor, args.Aggressed);

        HunterCombatTracker.StartSweeper();

        ReloadProfiles();

        CommandSystem.Register(
            "Bestiary",
            AccessLevel.Player,
            OnBestiaryCommand
        );

        CommandSystem.Register(
            "Bestiario",
            AccessLevel.Player,
            OnBestiaryCommand
        );

        CommandSystem.Register(
            "BestiaryTest",
            AccessLevel.GameMaster,
            OnBestiaryTestCommand
        );

        CommandSystem.Register(
            "BestiaryDiag",
            AccessLevel.GameMaster,
            OnBestiaryDiagCommand
        );

        if (DamageMode == DamageMode.Mark)
        {
            HunterMarkHook.Configure();
        }
    }

    private static void ReloadProfiles()
    {
        _profiles.Clear();

        foreach (var (serial, profile) in HunterProfileStore.Load())
        {
            _profiles[serial] = profile;
        }
    }

    public static HunterProfile GetProfile(Mobile m)
    {
        if (m == null)
        {
            return null;
        }

        if (!_profiles.TryGetValue(m.Serial, out var profile))
        {
            profile = new HunterProfile(m.Serial);
            _profiles[m.Serial] = profile;
        }

        return profile;
    }

    public static bool TryGetProfile(
        Mobile m,
        out HunterProfile profile
    )
    {
        if (m == null)
        {
            profile = null;
            return false;
        }

        return _profiles.TryGetValue(m.Serial, out profile);
    }

    public static bool TryGetEffectiveBonus(
        Mobile attacker,
        BaseCreature victim,
        out double bonus
    )
    {
        bonus = 0;

        if (!Enabled || attacker == null || victim == null)
        {
            return false;
        }

        var entry = BestiaryRegistry.FindEntry(victim.GetType());

        if (entry == null ||
            !_profiles.TryGetValue(attacker.Serial, out var profile))
        {
            return false;
        }

        if (profile.HasLearned(entry.Id))
        {
            bonus += PerCreatureBonus;
        }

        if (BestiaryRegistry.ByCategory.TryGetValue(
                entry.Category,
                out var categoryEntries
            ) &&
            categoryEntries.Count > 0)
        {
            bonus += CategoryMilestoneBonus *
                     profile.GetCategoryMasteredCount(entry.Category) /
                     categoryEntries.Count;
        }

        if (bonus > TotalBonusCap)
        {
            bonus = TotalBonusCap;
        }

        return bonus > 0;
    }

    [Usage("Bestiary")]
    [Description("Opens the Hunter's Bestiary.")]
    private static void OnBestiaryCommand(CommandEventArgs e)
    {
        if (e.Mobile is { Deleted: false } from)
        {
            HunterBestiaryGump.DisplayTo(from);
        }
    }

    private static void OnWorldSave()
    {
        HunterProfileStore.Save(_profiles);
    }

    private static void OnWorldLoad()
    {
        ReloadProfiles();
        HunterCombatTracker.Clear();
        HunterDeathHook.ClearProcessedDeaths();
    }

    private static string _configSource =
        "modernuo.json NOT FOUND - defaults in use";

    private static readonly Dictionary<string, string> _rawSettings =
        LoadRawSettings();

    private static Dictionary<string, string> LoadRawSettings()
    {
        var dict = new Dictionary<string, string>(
            StringComparer.OrdinalIgnoreCase
        );

        var searched = new List<string>();

        try
        {
            string path = null;

            foreach (var candidate in ConfigCandidates(searched))
            {
                if (File.Exists(candidate))
                {
                    path = candidate;
                    break;
                }
            }

            if (path == null)
            {
                _configSource =
                    "modernuo.json NOT FOUND; searched: " +
                    string.Join(" | ", searched);

                return dict;
            }

            _configSource = path;

            using var doc = JsonDocument.Parse(
                File.ReadAllText(path)
            );

            if (!doc.RootElement.TryGetProperty(
                    "settings",
                    out var settings
                ))
            {
                return dict;
            }

            foreach (var prop in settings.EnumerateObject())
            {
                dict[prop.Name] =
                    prop.Value.ValueKind == JsonValueKind.String
                        ? prop.Value.GetString()
                        : prop.Value.ToString();
            }
        }
        catch (Exception ex)
        {
            _configSource =
                "modernuo.json read error: " + ex.Message;
        }

        return dict;
    }

    private static IEnumerable<string> ConfigCandidates(
        List<string> searched
    )
    {
        var roots = new List<string>();

        foreach (var root in new[]
                 {
                     Core.BaseDirectory,
                     Directory.GetCurrentDirectory(),
                     AppContext.BaseDirectory
                 })
        {
            try
            {
                if (string.IsNullOrEmpty(root))
                {
                    continue;
                }

                var full = Path.GetFullPath(root);

                if (!roots.Contains(full))
                {
                    roots.Add(full);
                }
            }
            catch
            {
                // Ignore invalid path.
            }
        }

        var seenDirs = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase
        );

        foreach (var root in roots)
        {
            var dir = root;

            while (!string.IsNullOrEmpty(dir) &&
                   seenDirs.Add(dir))
            {
                searched.Add(dir);

                foreach (var sub in new[]
                         {
                             "",
                             "Distribution",
                             "Configuration",
                             "Config",
                             "Configs",
                             "Server",
                             "Run"
                         })
                {
                    yield return Path.Combine(
                        dir,
                        sub,
                        "modernuo.json"
                    );
                }

                dir = Directory.GetParent(dir)?.FullName;
            }
        }
    }

    public static bool GetBool(
        string key,
        bool defaultValue
    ) =>
        _rawSettings.TryGetValue(key, out var s) &&
        bool.TryParse(s, out var value)
            ? value
            : defaultValue;

    public static int GetInt(
        string key,
        int defaultValue
    ) =>
        _rawSettings.TryGetValue(key, out var s) &&
        int.TryParse(
            s,
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out var value
        )
            ? value
            : defaultValue;

    public static double GetDouble(
        string key,
        double defaultValue
    )
    {
        if (_rawSettings.TryGetValue(key, out var s))
        {
            if (double.TryParse(
                    s,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out var value
                ) ||
                double.TryParse(
                    s,
                    NumberStyles.Float,
                    CultureInfo.CurrentCulture,
                    out value
                ))
            {
                return value;
            }
        }

        return defaultValue;
    }

    public static string GetString(
        string key,
        string defaultValue
    ) =>
        _rawSettings.TryGetValue(key, out var value)
            ? value
            : defaultValue;

    public static TimeSpan GetTimeSpan(
        string key,
        TimeSpan defaultValue
    ) =>
        _rawSettings.TryGetValue(key, out var s) &&
        TimeSpan.TryParse(
            s,
            CultureInfo.InvariantCulture,
            out var value
        )
            ? value
            : defaultValue;

    public static TEnum GetEnum<TEnum>(
        string key,
        TEnum defaultValue
    )
        where TEnum : struct =>
        _rawSettings.TryGetValue(key, out var s) &&
        Enum.TryParse<TEnum>(
            s,
            true,
            out var value
        )
            ? value
            : defaultValue;

    public static void DebugLog(string message)
    {
        if (Debug)
        {
            _logger.Information("{Msg}", message);
        }
    }

    [Usage("BestiaryDiag")]
    [Description("Shows combat-event counters for bestiary hook diagnostics.")]
    private static void OnBestiaryDiagCommand(CommandEventArgs e)
    {
        if (e.Mobile is not { } from)
        {
            return;
        }

        from.SendMessage(
            $"AggressiveAction events seen: " +
            $"{HunterCombatTracker.HitCount}; " +
            $"tracked creatures: " +
            $"{HunterCombatTracker.TrackedCount}."
        );

        from.SendMessage(
            "Hit a mob 2-3 times in combat, then run this again. " +
            "If the counter does not grow, no AggressiveAction event is received."
        );
    }

    [Usage("BestiaryTest")]
    [Description("Runs the treatise drop pipeline on the nearest registered creature.")]
    private static void OnBestiaryTestCommand(CommandEventArgs e)
    {
        if (e.Mobile is not PlayerMobile from)
        {
            return;
        }

        BaseCreature target = null;

        foreach (var bc in from.GetMobilesInRange<BaseCreature>(10))
        {
            if (!bc.Deleted &&
                !bc.Summoned &&
                !bc.IsBonded &&
                bc.Hits > 0 &&
                BestiaryRegistry.FindEntry(bc.GetType()) != null)
            {
                target = bc;
                break;
            }
        }

        if (target == null)
        {
            from.SendMessage(
                "No registered creature within 10 tiles."
            );

            return;
        }

        var entry = BestiaryRegistry.FindEntry(target.GetType());

        var effectiveChance = GlobalDropChance >= 0
            ? GlobalDropChance
            : entry?.DropChance ?? 0;

        from.SendMessage(
            $"Running drop pipeline on {target.GetType().Name} " +
            $"(chance {effectiveChance:P0})..."
        );

        var result = HunterDeathHook.OnCreatureDeath(
            target,
            from
        );

        HunterDeathHook.ClearProcessedDeath(target.Serial);
        HunterCombatTracker.Forget(target.Serial);

        if (result == null)
        {
            from.SendMessage(
                0x44,
                $"DROP EXECUTED at {effectiveChance:P0} chance. " +
                "Check the corpse or the ground."
            );
        }
        else
        {
            from.SendMessage(
                0x22,
                "NO DROP: " + result
            );
        }

        DebugLog(
            $"[Bestiary] TEST on {target.GetType().Name}: " +
            $"{result ?? "DROP executed"}"
        );
    }
}