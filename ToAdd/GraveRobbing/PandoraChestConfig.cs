/**********************************
* Author: Expa                    *
* Script: PandoraChestConfig.cs   *
* Optimized for ModernUO 0.15.4.18*
**********************************/
using System;
using System.IO;
using System.Linq;
using System.Text.Json;

public class PandoraChestConfig
{
    private static PandoraChestConfig _current;
    private static readonly object _lock = new();

    public static PandoraChestConfig Current
    {
        get
        {
            if (_current == null) Load();
            return _current!;
        }
    }

    public SpawnSection Spawn { get; set; } = new();
    public double[] LevelQualityBase { get; set; } = new double[] { 0.0, 0.10, 0.25, 0.45, 0.85 };
    public GoldRange[] GoldRangeByLevel { get; set; } = new[]
    {
        new GoldRange{ Min = 10, Max = 60},
        new GoldRange{ Min = 30, Max = 120},
        new GoldRange{ Min = 80, Max = 300},
        new GoldRange{ Min = 200, Max = 1200},
        new GoldRange{ Min = 1500, Max = 5000}
    };
    public RareDropSection RareDrop { get; set; } = new RareDropSection();
    public ReagentsSection Reagents { get; set; } = new ReagentsSection();
    public WeightsSection Weights { get; set; } = new WeightsSection();
    public GuardianSection Guardian { get; set; } = new GuardianSection();
    public LoggingSection Logging { get; set; } = new LoggingSection();
    public MagicSection Magic { get; set; } = new MagicSection();
    public TrapSection Trap { get; set; } = new TrapSection();
    public MapMultiplierSection MapMultiplier { get; set; } = new MapMultiplierSection();
    public int[] HueByLevel { get; set; } = new int[] { 0, 0x47E, 0x47E, 0x496, 0x496 };
    public double BaseCursedChance { get; set; } = 0.10;
    public double BaseTrappedChance { get; set; } = 0.12;
    public double BaseLockedChance { get; set; } = 0.65;

    public double[] LevelQualityBaseCached = Array.Empty<double>();
    public GoldRange[] GoldRangeByLevelCached = Array.Empty<GoldRange>();
    public int[] ReagentsMinCached = Array.Empty<int>();
    public int[] ReagentsMaxCached = Array.Empty<int>();
    public int[] HueByLevelCached = Array.Empty<int>();

    public class SpawnSection
    {
        public int Levels { get; set; } = 5;
        public double BaseSpawnChance { get; set; } = 0.08;
        public double MiningFactor { get; set; } = 0.0006;
        public double SpiritSpeakFactor { get; set; } = 0.0008;
        public double NecromancyFactor { get; set; } = 0.0008;
        public double MaxSpawnChance { get; set; } = 0.60;
        public bool AllowIfLoggingDisabled { get; set; } = true;
    }

    public class GoldRange
    {
        public int Min { get; set; }
        public int Max { get; set; }
    }

    public class RareDropSection
    {
        public double BaseChance { get; set; } = 0.02;
    }

    public class ReagentsSection
    {
        public int[] MinPerLevel { get; set; } = new int[] { 1, 2, 3, 4, 6 };
        public int[] MaxPerLevel { get; set; } = new int[] { 2, 4, 6, 8, 12 };
    }

    public class WeightsSection
    {
        public double Mining { get; set; } = 0.6;
        public double SpiritSpeak { get; set; } = 1.2;
        public double Necromancy { get; set; } = 1.2;
        public double Level { get; set; } = 0.1;
    }

    public class GuardianSection
    {
        public double BaseChance { get; set; } = 0.05;
        public double LevelMultiplier { get; set; } = 0.06;
        public double MaxChance { get; set; } = 0.95;
        public double HpMultiplierPerQuality { get; set; } = 1.0;
        public double DamageMultiplierPerQuality { get; set; } = 0.8;
    }

    public class LoggingSection
    {
        public bool Enabled { get; set; } = true;
        public long MaxFileSizeBytes { get; set; } = 10 * 1024 * 1024;
        public int FlushIntervalMs { get; set; } = 500;
        public int LocationCooldownSeconds { get; set; } = 60;
        public int AccountCooldownSeconds { get; set; } = 30;
    }

    public class MagicSection
    {
        public bool Enabled { get; set; } = true;
        public double BaseChanceByLevelMultiplier { get; set; } = 0.02;
        public int DecayMinutesIfDropped { get; set; } = 30;
        public int MaxAffixesByLevelBase { get; set; } = 1;
        public System.Collections.Generic.Dictionary<string, AffixRange> AffixRanges { get; set; } = new()
        {
            { "BonusStr", new AffixRange{ Min = 1, Max = 5 } },
            { "BonusDex", new AffixRange{ Min = 1, Max = 5 } },
            { "BonusHits", new AffixRange{ Min = 1, Max = 10 } },
            { "WeaponDamage", new AffixRange{ Min = 1, Max = 10 } }
        };
        public double TreasureMapChancePerLevel { get; set; } = 0.03;
    }

    public class TrapSection
    {
        public double BaseChancePerLevel { get; set; } = 0.12;
        public System.Collections.Generic.Dictionary<string, int> TypeWeights { get; set; } = new()
        {
            { "Explosion", 30 },
            { "Poison", 40 },
            { "SummonUndead", 30 }
        };
        public double PowerMultiplierPerLevel { get; set; } = 0.6;
        public int ExplosionBaseMin { get; set; } = 8;
        public int ExplosionBaseMax { get; set; } = 16;
        public int SummonPerPower { get; set; } = 1;
    }

    public class AffixRange
    {
        public int Min { get; set; }
        public int Max { get; set; }
    }

    public class MapMultiplierSection
    {
        public double Felucca { get; set; } = 1.15;
        public double Trammel { get; set; } = 1.0;
    }

    public static void Load(string path = null)
    {
        lock (_lock)
        {
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory ?? Environment.CurrentDirectory;
                string cfgPath = path ?? Path.Combine(baseDir, "PandoraChestConfig.json");
                if (!File.Exists(cfgPath))
                {
                    _current = new PandoraChestConfig();
                    _current.FinalizeCache();
                    return;
                }

                string json = File.ReadAllText(cfgPath);
                var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var cfg = JsonSerializer.Deserialize<PandoraChestConfig>(json, opts) ?? new PandoraChestConfig();

                cfg.Spawn ??= new SpawnSection();
                cfg.RareDrop ??= new RareDropSection();
                cfg.Reagents ??= new ReagentsSection();
                cfg.Weights ??= new WeightsSection();
                cfg.Guardian ??= new GuardianSection();
                cfg.Logging ??= new LoggingSection();
                cfg.Magic ??= new MagicSection();
                cfg.Trap ??= new TrapSection();
                cfg.MapMultiplier ??= new MapMultiplierSection();

                if (cfg.LevelQualityBase == null) cfg.LevelQualityBase = Enumerable.Repeat(0.2, cfg.Spawn.Levels).ToArray();

                if (cfg.GoldRangeByLevel == null || cfg.GoldRangeByLevel.Length < cfg.Spawn.Levels)
                {
                    var arr = new GoldRange[cfg.Spawn.Levels];
                    var src = cfg.GoldRangeByLevel ?? Array.Empty<GoldRange>();
                    for (int i = 0; i < arr.Length; i++)
                        arr[i] = (i < src.Length && src[i] != null) ? src[i] : new GoldRange { Min = 10, Max = 60 };
                    cfg.GoldRangeByLevel = arr;
                }

                if (cfg.Reagents.MinPerLevel == null || cfg.Reagents.MinPerLevel.Length < cfg.Spawn.Levels)
                {
                    cfg.Reagents.MinPerLevel = new int[cfg.Spawn.Levels];
                    for (int i = 0; i < cfg.Spawn.Levels; i++) cfg.Reagents.MinPerLevel[i] = Math.Max(1, 1 + i);
                }

                if (cfg.Reagents.MaxPerLevel == null || cfg.Reagents.MaxPerLevel.Length < cfg.Spawn.Levels)
                {
                    cfg.Reagents.MaxPerLevel = new int[cfg.Spawn.Levels];
                    for (int i = 0; i < cfg.Spawn.Levels; i++) cfg.Reagents.MaxPerLevel[i] = Math.Max(cfg.Reagents.MinPerLevel[i], 2 + i * 2);
                }

                ValidateAndClamp(cfg);
                cfg.FinalizeCache();
                _current = cfg;
            }
            catch (Exception ex)
            {
                try { Console.Error.WriteLine($"PandoraChestConfig.Load failed: {ex}"); } catch { }
                _current = new PandoraChestConfig();
                _current.FinalizeCache();
            }
        }
    }

    public static void Reload() => Load();

    private static void ValidateAndClamp(PandoraChestConfig cfg)
    {
        cfg.Spawn ??= new SpawnSection();
        cfg.RareDrop ??= new RareDropSection();
        cfg.Reagents ??= new ReagentsSection();
        cfg.Weights ??= new WeightsSection();
        cfg.Guardian ??= new GuardianSection();
        cfg.Logging ??= new LoggingSection();
        cfg.Magic ??= new MagicSection();
        cfg.Trap ??= new TrapSection();
        cfg.MapMultiplier ??= new MapMultiplierSection();

        cfg.Spawn.BaseSpawnChance = Clamp(cfg.Spawn.BaseSpawnChance, 0.0, 1.0);
        cfg.Spawn.MiningFactor = Math.Max(0.0, cfg.Spawn.MiningFactor);
        cfg.Spawn.SpiritSpeakFactor = Math.Max(0.0, cfg.Spawn.SpiritSpeakFactor);
        cfg.Spawn.NecromancyFactor = Math.Max(0.0, cfg.Spawn.NecromancyFactor);
        cfg.Spawn.MaxSpawnChance = Clamp(cfg.Spawn.MaxSpawnChance, 0.0, 1.0);

        cfg.RareDrop.BaseChance = Clamp(cfg.RareDrop.BaseChance, 0.0, 1.0);
        cfg.BaseCursedChance = Clamp(cfg.BaseCursedChance, 0.0, 1.0);
        cfg.BaseTrappedChance = Clamp(cfg.BaseTrappedChance, 0.0, 1.0);
        cfg.BaseLockedChance = Clamp(cfg.BaseLockedChance, 0.0, 1.0);
        cfg.Guardian.BaseChance = Clamp(cfg.Guardian.BaseChance, 0.0, 1.0);
        cfg.Guardian.MaxChance = Clamp(cfg.Guardian.MaxChance, 0.0, 1.0);

        if (cfg.GoldRangeByLevel != null)
            foreach (var gr in cfg.GoldRangeByLevel)
                if (gr != null)
                {
                    if (gr.Min < 0) gr.Min = 0;
                    if (gr.Max < gr.Min) gr.Max = gr.Min;
                }

        if (cfg.Reagents.MinPerLevel == null || cfg.Reagents.MinPerLevel.Length < cfg.Spawn.Levels)
        {
            cfg.Reagents.MinPerLevel = new int[cfg.Spawn.Levels];
            for (int i = 0; i < cfg.Spawn.Levels; i++) cfg.Reagents.MinPerLevel[i] = Math.Max(1, 1 + i);
        }

        if (cfg.Reagents.MaxPerLevel == null || cfg.Reagents.MaxPerLevel.Length < cfg.Spawn.Levels)
        {
            cfg.Reagents.MaxPerLevel = new int[cfg.Spawn.Levels];
            for (int i = 0; i < cfg.Spawn.Levels; i++) cfg.Reagents.MaxPerLevel[i] = Math.Max(cfg.Reagents.MinPerLevel[i], 2 + i * 2);
        }

        if (cfg.Logging.MaxFileSizeBytes <= 0) cfg.Logging.MaxFileSizeBytes = 10 * 1024 * 1024;
        if (cfg.Logging.FlushIntervalMs < 50) cfg.Logging.FlushIntervalMs = 50;
        if (cfg.Logging.LocationCooldownSeconds < 1) cfg.Logging.LocationCooldownSeconds = 60;
        if (cfg.Logging.AccountCooldownSeconds < 1) cfg.Logging.AccountCooldownSeconds = 30;
        if (cfg.Magic.DecayMinutesIfDropped < 1) cfg.Magic.DecayMinutesIfDropped = 30;
    }

    private static double Clamp(double v, double min, double max) => v < min ? min : (v > max ? max : v);

    public void FinalizeCache()
    {
        LevelQualityBaseCached = LevelQualityBase ?? Array.Empty<double>();
        GoldRangeByLevelCached = GoldRangeByLevel ?? Array.Empty<GoldRange>();
        ReagentsMinCached = Reagents?.MinPerLevel ?? Array.Empty<int>();
        ReagentsMaxCached = Reagents?.MaxPerLevel ?? Array.Empty<int>();
        HueByLevelCached = HueByLevel ?? Array.Empty<int>();
    }
}