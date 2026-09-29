/**********************************
* Author: Unknow                  *   
* Update/Fixed/Refactoring: Expa  *
* Script: GraveRobbingCore.cs     *
* Optimized for ModernUO 0.15.4.18*
**********************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ModernUO.Serialization;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Engines.Harvest;
using Server.Network;

static class GRUtil
{
    public static double GetSkillValue(Mobile m, SkillName s) => m?.Skills?[s]?.Value ?? 0.0;
}

namespace Server.Engines.Harvest
{
    [SerializationGenerator(0, false)]
    public partial class GraveRobbing : HarvestSystem
    {
        static GraveRobbing _system;
        public static GraveRobbing System => _system ??= new GraveRobbing();

        readonly HarvestDefinition _def;
        public HarvestDefinition Definition => _def;

        private GraveRobbing()
        {
            _def = new HarvestDefinition
            {
                BankWidth = 2,
                BankHeight = 2,
                MinTotal = 1,
                MaxTotal = 2,
                MinRespawn = TimeSpan.FromMinutes(20),
                MaxRespawn = TimeSpan.FromMinutes(30),
                Skill = SkillName.Mining,
                MaxRange = 2,
                ConsumedPerHarvest = 1,
                ConsumedPerFeluccaHarvest = 1,
                EffectActions = new[] { 13 },
                EffectSounds = new[] { 0x13E },
                EffectCounts = new[] { 1 },
                StaticTiles = new[] { 0xED3, 0xEDF, 0xEE0, 0xEE1, 0xEE2, 0xEE8 },
                RangedTiles = false,
                NoResourcesMessage = "There is nothing here to dig up.",
                FailMessage = "You dig but find nothing of value.",
                OutOfRangeMessage = 500446,
                TimedOutOfRangeMessage = 503041,
                DoubleHarvestMessage = 503042,
                PackFullMessage = 1010481,
                ToolBrokeMessage = 1044038
            };

            _def.Resources = new[]
            {
                new HarvestResource(60, 10, 150, "You dig up some bones.", typeof(Bone)),
                new HarvestResource(40, 30, 150, "You uncover some gold.", typeof(Gold))
            };

            _def.Veins = new[]
            {
                new HarvestVein(70, 0, _def.Resources[0], null),
                new HarvestVein(30, 0, _def.Resources[1], null)
            };

            Definitions = new[] { _def };

            PandoraChestConfig.Load();
            PandoraChestLogger.Initialize();
            PandoraChestSpawnGuard.Initialize();
        }

        public override bool CheckHarvest(Mobile from, Item tool)
        {
            var region = from?.Region;
            var regionName = region?.Name ?? "(null)";
            var ok = false;

            if (region != null)
            {
                try
                {
                    if (region.IsPartOf("Cemetery"))
                        ok = true;
                }
                catch (Exception ex)
                {
                    PandoraChestLogger.LogError("GraveRobbing.CheckHarvest.IsPartOf", ex, $"RegionName={regionName}", from);
                }

                if (!ok && !string.IsNullOrEmpty(regionName))
                {
                    var rn = regionName.ToLowerInvariant();
                    if (rn.Contains("grave") || rn.Contains("graveyard") || rn.Contains("cemetery"))
                        ok = true;
                }
            }

            if (!ok)
            {
                from?.SendMessage($"You may only rob graves in a cemetery. Region: {regionName}");
                return false;
            }

            if (tool == null)
            {
                from?.SendMessage("You need to use a shovel or pickaxe to dig here.");
                return false;
            }

            return base.CheckHarvest(from, tool);
        }

        public override void OnHarvestFinished(Mobile from, Item tool, HarvestDefinition def, HarvestVein vein, HarvestBank bank, HarvestResource resource, object harvested)
        {
            base.OnHarvestFinished(from, tool, def, vein, bank, resource, harvested);

            GetHarvestDetails(from, tool, harvested, out _, out var map, out var loc, out _);

            if (from != null)
            {
                from.Karma -= Utility.RandomMinMax(50, 150);
            }

            var hour = DateTime.Now.Hour;
            var night = hour < 6 || hour > 20;

            MoonPhase? moon = null;
            if (map != null)
            {
                try
                {
                    moon = Clock.GetMoonPhase(map, loc.X, loc.Y);
                }
                catch (Exception ex)
                {
                    PandoraChestLogger.LogError("GraveRobbing.OnHarvestFinished.GetMoonPhase", ex, $"Map={map},Loc={loc}", from);
                    moon = null;
                }
            }

            double baseSpiritChance = 0.10;
            if (moon == MoonPhase.FullMoon) baseSpiritChance += 0.15;
            if (night) baseSpiritChance += 0.10;

            double spiritSpeak = GRUtil.GetSkillValue(from, SkillName.SpiritSpeak);
            double necromancy = GRUtil.GetSkillValue(from, SkillName.Necromancy);
            double skillReduction = (spiritSpeak + necromancy) / 2000.0;
            double spiritChanceFinal = Math.Max(0.01, baseSpiritChance - skillReduction);

            if (Utility.RandomDouble() < spiritChanceFinal)
            {
                var spirit = new VengefulSpirit();
                if (night)
                {
                    spirit.Hits += 20;
                    spirit.DamageMin += 2;
                    spirit.DamageMax += 2;
                }

                Point3D spawnLoc;
                Map spawnMap;
                if (map != null)
                {
                    spawnLoc = loc;
                    spawnMap = map;
                }
                else
                {
                    spawnLoc = from?.Location ?? Point3D.Zero;
                    spawnMap = from?.Map;
                }

                if (spawnMap != null)
                {
                    try
                    {
                        spirit.MoveToWorld(spawnLoc, spawnMap);
                        spirit.Combatant = from;
                        spirit.Say("You should not have disturbed the dead!");
                    }
                    catch (Exception ex)
                    {
                        PandoraChestLogger.LogError("GraveRobbing.OnHarvestFinished.SpawnSpirit", ex, $"Player={from?.Name},Map={spawnMap},Loc={spawnLoc}", from);
                    }
                }
            }

            if (from != null && Utility.RandomDouble() < 0.25)
            {
                from.SendMessage(0x21, "A grave curse weakens you!");
                try
                {
                    from.AddStatMod(new StatMod(StatType.Str, "GraveCurse", -5, TimeSpan.FromMinutes(5)));
                }
                catch (Exception ex)
                {
                    PandoraChestLogger.LogError("GraveRobbing.OnHarvestFinished.AddStatMod", ex, $"Player={from?.Name}", from);
                }
            }

            TrySpawnPandoraChest(from, loc, map);
        }

        void TrySpawnPandoraChest(Mobile from, Point3D loc, Map map)
        {
            if (from == null || map == null) return;
            var cfg = PandoraChestConfig.Current;
            if (!PandoraChestSpawnGuard.CanSpawnAt(map, loc, cfg)) return;

            string acc = "(null)";
            try
            {
                acc = from?.Account?.Username ?? from?.Account?.ToString() ?? "(null)";
            }
            catch (Exception ex)
            {
                PandoraChestLogger.LogError("GraveRobbing.TrySpawnPandoraChest.GetAccount", ex, $"Player={from?.Name}", from);
                acc = "(null)";
            }

            if (!PandoraChestSpawnGuard.CanSpawnForAccount(acc, cfg)) return;

            double mining = GRUtil.GetSkillValue(from, SkillName.Mining);
            double spiritSpeak = GRUtil.GetSkillValue(from, SkillName.SpiritSpeak);
            double necro = GRUtil.GetSkillValue(from, SkillName.Necromancy);
            double spawn = cfg.Spawn.BaseSpawnChance + mining * cfg.Spawn.MiningFactor + spiritSpeak * cfg.Spawn.SpiritSpeakFactor + necro * cfg.Spawn.NecromancyFactor;
            spawn = Math.Min(spawn, cfg.Spawn.MaxSpawnChance);
            if (Utility.RandomDouble() >= spawn) return;

            int level = DetermineChestLevel(mining, spiritSpeak, necro, cfg);
            var chest = new PandoraChest(level, from);
            try
            {
                chest.MoveToWorld(loc, map);
                chest.Populate(from, map);
            }
            catch (Exception ex)
            {
                PandoraChestLogger.LogError("GraveRobbing.TrySpawnPandoraChest.SpawnChest", ex, $"Player={from?.Name},Map={map},Loc={loc},Level={level}", from);
            }

            PandoraChestLogger.LogSpawn(from, map, loc, level, spawn);
            PandoraChestSpawnGuard.RecordAt(map, loc);
            PandoraChestSpawnGuard.RecordAccount(acc);

            double guardChance = cfg.Guardian.BaseChance + level * cfg.Guardian.LevelMultiplier;
            guardChance = Math.Min(guardChance, cfg.Guardian.MaxChance);
            if (Utility.RandomDouble() < guardChance)
            {
                int count = 1 + (level / 3);
                for (int i = 0; i < count; i++)
                {
                    var g = new VengefulSpirit();
                    g.Hits += (int)(g.Hits * (1.0 + cfg.Guardian.HpMultiplierPerQuality * (level - 1)));
                    g.DamageMin += (int)(cfg.Guardian.DamageMultiplierPerQuality * (level - 1));
                    g.DamageMax += (int)(cfg.Guardian.DamageMultiplierPerQuality * (level - 1) * 2);
                    try
                    {
                        g.MoveToWorld(new Point3D(loc.X + Utility.RandomMinMax(-2, 2), loc.Y + Utility.RandomMinMax(-2, 2), loc.Z), map);
                        g.Combatant = from;
                    }
                    catch (Exception ex)
                    {
                        PandoraChestLogger.LogError("GraveRobbing.TrySpawnPandoraChest.SpawnGuard", ex, $"Player={from?.Name},Map={map},Loc={loc},Level={level}", from);
                    }
                }
            }
        }

        int DetermineChestLevel(double mining, double spirit, double necro, PandoraChestConfig cfg)
        {
            double weighted = mining * cfg.Weights.Mining + spirit * cfg.Weights.SpiritSpeak + necro * cfg.Weights.Necromancy;
            double norm = Math.Min(weighted / 120.0, 1.0);
            var q = cfg.LevelQualityBaseCached;
            int levels = Math.Max(1, Math.Min(cfg.Spawn.Levels, q.Length));
            double[] adj = new double[levels];
            for (int i = 0; i < levels; i++)
            {
                double qq = (i < q.Length) ? q[i] : q.Last();
                adj[i] = Math.Max(0.001, (0.2 + qq) * (1.0 + norm * (i / (double)levels) * 3.0));
            }
            double sum = adj.Sum();
            double r = Utility.RandomDouble() * sum;
            double acc = 0;
            for (int i = 0; i < adj.Length; i++)
            {
                acc += adj[i];
                if (r <= acc) return i + 1;
            }
            return levels;
        }
    }

    public static class GraveRobbingInit { public static void Initialize() { _ = GraveRobbing.System; } }
}

namespace Server.Items
{
    [SerializationGenerator(0, false)]
    [Flippable(0x09AA, 0x0E7D)]
    public partial class PandoraChest : LockableContainer
    {
        int _level;
        bool _isCursed;
        bool _isTrapped;
        int _trapPower;
        Mobile _owner;
        enum TrapType { None, Explosion, Poison, SummonUndead }
        TrapType _trapType = TrapType.None;

        [Constructible] public PandoraChest() : this(1, null) { }
        public PandoraChest(int level, Mobile owner) : base(0x09AA)
        {
            Weight = 5.0;
            LootType = LootType.Regular;
            _owner = owner;

            var cfg = PandoraChestConfig.Current;
            _level = Math.Max(1, Math.Min(level, cfg.Spawn.Levels));
            Name = $"Pandora's Chest [L{_level}] {(_level >= 4 ? "(Jeweled)" : _level >= 3 ? "(Polished)" : "(Simple)")}";
            int hue = (cfg.HueByLevelCached != null && cfg.HueByLevelCached.Length >= _level) ? cfg.HueByLevelCached[_level - 1] : 0;
            if (hue != 0) Hue = hue;

            _isCursed = Utility.RandomDouble() < cfg.BaseCursedChance;
            Locked = Utility.RandomDouble() < cfg.BaseLockedChance;
            _isTrapped = Utility.RandomDouble() < cfg.BaseTrappedChance;
            LockLevel = 10 * _level;
            MaxLockLevel = Math.Max(LockLevel, 120);
            RequiredSkill = 10 * _level;
            _trapPower = _isTrapped ? (10 + 15 * _level) : 0;
            if (_isCursed) Name = $"Cursed {Name}";
        }

        public void Populate(Mobile opener, Map map) => FillWithLoot(_level, opener, map);

        void FillWithLoot(int level, Mobile opener, Map map)
        {
            if (Items.Count > 0) return;

            var cfg = PandoraChestConfig.Current;
            var ranges = cfg.GoldRangeByLevelCached;
            int idx = Math.Max(0, Math.Min(level - 1, ranges.Length - 1));
            int gold = Utility.RandomMinMax(ranges[idx].Min, ranges[idx].Max);
            DropItem(new Gold(gold));
            var dropped = new List<string> { $"Gold:{gold}" };

            int minR = cfg.ReagentsMinCached.Length > idx ? cfg.ReagentsMinCached[idx] : 1;
            int maxR = cfg.ReagentsMaxCached.Length > idx ? cfg.ReagentsMaxCached[idx] : Math.Max(1, minR);
            int reagCount = Utility.RandomMinMax(minR, maxR);

            var reagentStacks = new Dictionary<Type, int>();
            for (int i = 0; i < reagCount; i++)
            {
                var r = ReagentForLevel(level, cfg);
                if (r == null) continue;
                var t = r.GetType();
                reagentStacks.TryGetValue(t, out var cur);
                reagentStacks[t] = cur + (r.Amount > 0 ? r.Amount : 1);
            }

            foreach (var kv in reagentStacks)
            {
                try
                {
                    var inst = (Item)Activator.CreateInstance(kv.Key)!;
                    try { inst.Amount = Math.Max(1, kv.Value); } catch (Exception ex) { PandoraChestLogger.LogError("PandoraChest.FillWithLoot.SetReagentAmount", ex, $"Type={kv.Key.FullName},Level={level}", opener); }
                    DropItem(inst);
                    dropped.Add(inst.GetType().Name + (inst.Amount > 1 ? $"x{inst.Amount}" : ""));
                }
                catch (Exception ex)
                {
                    PandoraChestLogger.LogError("PandoraChest.FillWithLoot.CreateReagentInstance", ex, $"Type={kv.Key.FullName},Level={level}", opener);
                }
            }

            if (level >= 4)
            {
                if (level >= 5)
                {
                    try
                    {
                        var bp = new BlackPearl();
                        try { bp.Amount = 1; } catch (Exception ex) { PandoraChestLogger.LogError("PandoraChest.FillWithLoot.SetBlackPearlAmount", ex, $"Level={level}", opener); }
                        DropItem(bp);
                        dropped.Add("Guaranteed:BlackPearl");
                    }
                    catch (Exception ex)
                    {
                        PandoraChestLogger.LogError("PandoraChest.FillWithLoot.CreateBlackPearl", ex, $"Level={level}", opener);
                    }
                }
                else
                {
                    try
                    {
                        var sa = new SulfurousAsh();
                        try { sa.Amount = 1; } catch (Exception ex) { PandoraChestLogger.LogError("PandoraChest.FillWithLoot.SetSulfurousAshAmount", ex, $"Level={level}", opener); }
                        DropItem(sa);
                        dropped.Add("Guaranteed:SulfurousAsh");
                    }
                    catch (Exception ex)
                    {
                        PandoraChestLogger.LogError("PandoraChest.FillWithLoot.CreateSulfurousAsh", ex, $"Level={level}", opener);
                    }
                }
            }

            if (Utility.Random(100) < Math.Min(95, cfg.Weights.Level * 10 + 30 + level * 8))
            {
                DropItem(new NightSightPotion());
                dropped.Add("NightSightPotion");
            }

            if (Utility.Random(100) < Math.Min(95, 20 + level * 6))
            {
                DropItem(new LesserHealPotion());
                dropped.Add("LesserHealPotion");
            }

            double jewelryChance = Math.Min(95.0, 8 + level * 6 + cfg.Weights.Level * 10);
            bool jewelryDropped = false;
            if (Utility.Random(100) < jewelryChance)
            {
                Item j = (level >= 4) ? MakeJeweledJewelry(level) : (level >= 3) ? MakePolishedJewelry(level) : MakeSimpleJewelry(level);
                if (j != null) { DropItem(j); dropped.Add(j.GetType().Name + ":" + (j.Name ?? "j")); jewelryDropped = true; }
            }

            if (level >= 5 && !jewelryDropped)
            {
                var j = MakeJeweledJewelry(level);
                if (j != null) { DropItem(j); dropped.Add("GuaranteedJeweled:" + (j.Name ?? j.GetType().Name)); }
            }

            if (Utility.Random(100) < (20 + level * 8)) { DropItem(new Bone()); dropped.Add("Bone"); }

            if (Utility.RandomDouble() < cfg.RareDrop.BaseChance * level) { DropItem(new SpecialRareBoneItem()); dropped.Add("SpecialRareBoneItem"); }

            if (Utility.Random(100) < 10 + level * 4) { DropItem(new Lockpick()); dropped.Add("Lockpick"); }

            if (cfg.Magic.Enabled)
            {
                double mapMult = MapMultiplier(map, cfg);
                double luck = opener?.Luck ?? 0;
                double skillSum = (GRUtil.GetSkillValue(opener, SkillName.Mining) + GRUtil.GetSkillValue(opener, SkillName.SpiritSpeak) + GRUtil.GetSkillValue(opener, SkillName.Necromancy)) / 3.0;
                double skillFactor = 1.0 + skillSum / 200.0;
                double chance = cfg.Magic.BaseChanceByLevelMultiplier * level * mapMult * (1.0 + luck / 1000.0) * skillFactor;
                chance = Math.Min(chance, 0.95);
                if (Utility.RandomDouble() < chance)
                {
                    Item? magic = null;
                    try
                    {
                        magic = GenerateMagicItem(level, opener, cfg);
                        if (magic != null)
                        {
                            DropItem(magic);
                            PandoraChestLogger.LogMagicDrop(opener, level, magic);
                            dropped.Add("Magic:" + (magic.Name ?? magic.GetType().Name));
                            int minutes = Math.Max(1, cfg.Magic.DecayMinutesIfDropped);
                            Timer.DelayCall(TimeSpan.FromMinutes(minutes), () =>
                            {
                                try { if (magic != null && !magic.Deleted && magic.Parent == this) magic.Delete(); } catch (Exception ex) { PandoraChestLogger.LogError("PandoraChest.FillWithLoot.MagicDecayDelete", ex, $"Player={opener?.Name},Level={level},Item={magic?.GetType().Name}", opener); }
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        PandoraChestLogger.LogError("PandoraChest.FillWithLoot.GenerateMagicItem", ex, $"Player={opener?.Name},Level={level}", opener);
                    }
                }

                if (Utility.RandomDouble() < cfg.Magic.TreasureMapChancePerLevel * level * MapMultiplier(map, cfg))
                {
                    try { var tm = new TreasureMap(level, map); DropItem(tm); dropped.Add("TreasureMap:L" + level); } catch (Exception ex) { PandoraChestLogger.LogError("PandoraChest.FillWithLoot.TreasureMap", ex, $"Player={opener?.Name},Level={level}", opener); }
                }
            }

            var tcfg = PandoraChestConfig.Current.Trap;
            if (tcfg != null)
            {
                double mapMult = MapMultiplier(map, PandoraChestConfig.Current);
                double spirit = GRUtil.GetSkillValue(_owner, SkillName.SpiritSpeak);
                double necro = GRUtil.GetSkillValue(_owner, SkillName.Necromancy);
                double luck = _owner?.Luck ?? 0;
                double reduction = (spirit + necro) / 300.0 + luck / 2000.0;
                double trapBase = tcfg.BaseChancePerLevel * level * mapMult;
                if (Utility.RandomDouble() < Math.Max(0.02, trapBase - reduction))
                {
                    try
                    {
                        int total = tcfg.TypeWeights.Values.Sum();
                        int r = Utility.Random(total);
                        foreach (var kv in tcfg.TypeWeights)
                        {
                            r -= kv.Value;
                            if (r < 0)
                            {
                                if (Enum.TryParse<TrapType>(kv.Key, out var tt)) _trapType = tt;
                                break;
                            }
                        }
                        _isTrapped = true;
                        _trapPower = 1 + (int)Math.Round(level * tcfg.PowerMultiplierPerLevel);
                    }
                    catch (Exception ex)
                    {
                        PandoraChestLogger.LogError("PandoraChest.FillWithLoot.DetermineTrap", ex, $"Player={_owner?.Name},Level={level}", _owner);
                    }
                }
            }

            PandoraChestLogger.LogContents(opener, _level, dropped);
        }

        static double MapMultiplier(Map m, PandoraChestConfig cfg)
        {
            if (m == null) return 1.0;
            var nm = m.ToString().ToLowerInvariant();
            if (nm.Contains("felucca")) return cfg.MapMultiplier.Felucca;
            if (nm.Contains("trammel")) return cfg.MapMultiplier.Trammel;
            return cfg.MapMultiplier.Trammel;
        }

        static Item? GenerateMagicItem(int level, Mobile opener, PandoraChestConfig cfg)
        {
            try
            {
                double pick = Utility.RandomDouble();
                Item it = (pick < 0.55) ? Loot.RandomWeapon() : (pick < 0.90) ? Loot.RandomArmor() : Loot.RandomJewelry();
                if (it == null) return null;

                it.Name = $"Enchanted {it.GetType().Name} (L{level})";
                if (level >= 4) it.Hue = 0x496;
                else if (level >= 3) it.Hue = 0x47E;

                int baseAff = cfg.Magic.MaxAffixesByLevelBase;
                int extra = level >= 5 ? 1 : 0;
                int affixes = baseAff + extra;

                double luck = opener?.Luck ?? 0;
                double skillSum = (GRUtil.GetSkillValue(opener, SkillName.Mining) + GRUtil.GetSkillValue(opener, SkillName.SpiritSpeak) + GRUtil.GetSkillValue(opener, SkillName.Necromancy)) / 3.0;
                double mult = 1.0 + (level - 1) * 0.15 + luck / 500.0 + skillSum / 200.0;

                var keys = cfg.Magic.AffixRanges?.Keys.ToArray() ?? Array.Empty<string>();
                for (int i = 0; i < affixes && keys.Length > 0; i++)
                {
                    var key = keys[Utility.Random(keys.Length)];
                    var range = cfg.Magic.AffixRanges[key];
                    int raw = Utility.RandomMinMax(range.Min, range.Max);
                    int val = Math.Max(1, (int)Math.Round(raw * mult));
                    ApplyAffixToItem(it, key, val);
                }
                return it;
            }
            catch (Exception ex)
            {
                PandoraChestLogger.LogError("PandoraChest.GenerateMagicItem", ex, $"Level={level},Player={opener?.Name}", opener);
                return null;
            }
        }

        static void ApplyAffixToItem(Item it, string affixName, int value)
        {
            if (it == null) return;
            try
            {
                if (it is BaseWeapon weapon)
                {
                    if (weapon.Attributes != null)
                    {
                        switch (affixName)
                        {
                            case "BonusStr": weapon.Attributes.BonusStr += value; return;
                            case "BonusDex": weapon.Attributes.BonusDex += value; return;
                            case "BonusHits": weapon.Attributes.BonusHits += value; return;
                            case "WeaponDamage": weapon.Attributes.WeaponDamage += value; return;
                        }
                    }

                    var wattrsProp = weapon.GetType().GetProperty("WeaponAttributes", BindingFlags.Public | BindingFlags.Instance);
                    if (wattrsProp != null)
                    {
                        var wattrs = wattrsProp.GetValue(weapon);
                        if (wattrs != null)
                        {
                            var p = wattrs.GetType().GetProperty(affixName, BindingFlags.Public | BindingFlags.Instance);
                            if (p != null && p.PropertyType == typeof(int))
                            {
                                int old = (int)p.GetValue(wattrs);
                                p.SetValue(wattrs, old + value);
                                return;
                            }
                        }
                    }
                }

                if (it is BaseArmor armor)
                {
                    if (armor.Attributes != null)
                    {
                        switch (affixName)
                        {
                            case "BonusStr": armor.Attributes.BonusStr += value; return;
                            case "BonusDex": armor.Attributes.BonusDex += value; return;
                            case "BonusHits": armor.Attributes.BonusHits += value; return;
                        }
                    }

                    var aattrsProp = armor.GetType().GetProperty("ArmorAttributes", BindingFlags.Public | BindingFlags.Instance);
                    if (aattrsProp != null)
                    {
                        var aattrs = aattrsProp.GetValue(armor);
                        if (aattrs != null)
                        {
                            var p = aattrs.GetType().GetProperty(affixName, BindingFlags.Public | BindingFlags.Instance);
                            if (p != null && p.PropertyType == typeof(int))
                            {
                                int old = (int)p.GetValue(aattrs);
                                p.SetValue(aattrs, old + value);
                                return;
                            }
                        }
                    }
                }

                if (it is BaseJewel jewel)
                {
                    if (jewel.Attributes != null)
                    {
                        switch (affixName)
                        {
                            case "BonusStr": jewel.Attributes.BonusStr += value; return;
                            case "BonusDex": jewel.Attributes.BonusDex += value; return;
                            case "BonusHits": jewel.Attributes.BonusHits += value; return;
                            case "WeaponDamage": jewel.Attributes.WeaponDamage += value; return;
                        }
                    }
                }

                var attrsProp = it.GetType().GetProperty("Attributes", BindingFlags.Public | BindingFlags.Instance);
                if (attrsProp != null)
                {
                    var attrsObj = attrsProp.GetValue(it);
                    if (attrsObj != null)
                    {
                        var prop = attrsObj.GetType().GetProperty(affixName, BindingFlags.Public | BindingFlags.Instance);
                        if (prop != null && prop.PropertyType == typeof(int))
                        {
                            int old = (int)prop.GetValue(attrsObj);
                            prop.SetValue(attrsObj, old + value);
                            return;
                        }
                    }
                }

                it.Name = $"{it.Name} [+{value} {affixName}]";
            }
            catch (Exception ex)
            {
                PandoraChestLogger.LogError("PandoraChest.ApplyAffixToItem", ex, $"Item={it?.GetType().FullName},Affix={affixName},Value={value}");
                try { it.Name = $"{it.Name} [+{value} {affixName}]"; } catch { }
            }
        }

        static Item ReagentForLevel(int level, PandoraChestConfig cfg)
        {
            int r = Utility.Random(100);
            if (level >= 5 && r < 25) return new BlackPearl();
            if (level >= 4 && r < 30) return new SulfurousAsh();
            Item[] common = { new Garlic(), new Ginseng(), new MandrakeRoot(), new Bloodmoss() };
            return common[Utility.Random(common.Length)];
        }

        static Item MakeSimpleJewelry(int level) => Utility.RandomBool() ? new GoldRing() : new GoldNecklace();

        static Item MakePolishedJewelry(int level)
        {
            var it = MakeSimpleJewelry(level);
            if (it is GoldRing r) { r.Name = "Polished Gold Ring"; r.Hue = 0x47E; }
            if (it is GoldNecklace n) { n.Name = "Polished Gold Necklace"; n.Hue = 0x47E; }
            return it;
        }

        static Item MakeJeweledJewelry(int level)
        {
            var it = MakeSimpleJewelry(level);
            if (it is GoldRing r) { r.Name = "Jeweled Gold Ring"; r.Hue = 0x496; }
            if (it is GoldNecklace n) { n.Name = "Jeweled Gold Necklace"; n.Hue = 0x496; }
            return it;
        }

        public override bool CheckLocked(Mobile from)
        {
            if (_isCursed && from != null)
            {
                double necro = GRUtil.GetSkillValue(from, SkillName.Necromancy);
                double spirit = GRUtil.GetSkillValue(from, SkillName.SpiritSpeak);
                double resist = (necro + spirit) / 240.0;
                if (Utility.RandomDouble() > resist)
                {
                    from.SendMessage(0x22, "An ancient curse flares as you touch the chest...");
                    try { from.ApplyPoison(from, Poison.Deadly); }
                    catch (Exception ex) { PandoraChestLogger.LogError("PandoraChest.CheckLocked.ApplyPoison", ex, $"Player={from?.Name}", from); }
                }
            }
            return base.CheckLocked(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from == null || from.Deleted) return;
            if (Locked) { base.OnDoubleClick(from); return; }

            if (_isTrapped && _trapType != TrapType.None)
            {
                double trapChance = _trapPower / 100.0;
                double spirit = GRUtil.GetSkillValue(from, SkillName.SpiritSpeak);
                double necro = GRUtil.GetSkillValue(from, SkillName.Necromancy);
                double resist = (spirit + necro) / 240.0;
                trapChance = Math.Max(0.05, trapChance - resist);
                if (Utility.RandomDouble() < trapChance) { TriggerTrap(from); _isTrapped = false; Locked = true; return; }
            }

            if (_isCursed)
            {
                from.SendMessage(0x21, "A dark curse hits you for opening a desecrated chest!");
                try { from.AddStatMod(new StatMod(StatType.Dex, "PandoraCurseDex", -3, TimeSpan.FromMinutes(3))); }
                catch (Exception ex) { PandoraChestLogger.LogError("PandoraChest.OnDoubleClick.AddStatMod.Dex", ex, $"Player={from?.Name}", from); }
                try { from.AddStatMod(new StatMod(StatType.Str, "PandoraCurseStr", -3, TimeSpan.FromMinutes(3))); }
                catch (Exception ex) { PandoraChestLogger.LogError("PandoraChest.OnDoubleClick.AddStatMod.Str", ex, $"Player={from?.Name}", from); }
                if (Utility.RandomDouble() < 0.35) try { from.ApplyPoison(from, Poison.Lesser); } catch (Exception ex) { PandoraChestLogger.LogError("PandoraChest.OnDoubleClick.ApplyPoison", ex, $"Player={from?.Name}", from); }
            }

            base.OnDoubleClick(from);
        }

        void TriggerTrap(Mobile from)
        {
            if (from == null || from.Deleted) return;
            var tcfg = PandoraChestConfig.Current.Trap;
            switch (_trapType)
            {
                case TrapType.Explosion:
                    try
                    {
                        int min = tcfg.ExplosionBaseMin + (_trapPower * 2);
                        int max = tcfg.ExplosionBaseMax + (_trapPower * 4);
                        int dmg = Utility.RandomMinMax(min, max);
                        from.Damage(dmg);
                        foreach (var m in from.GetMobilesInRange(2))
                            if (m != from && m.Alive) try { m.Damage(dmg / 2); } catch (Exception ex) { PandoraChestLogger.LogError("PandoraChest.TriggerTrap.DamageNearby", ex, $"Player={from?.Name},Target={m?.Name}", from); }
                    }
                    catch (Exception ex)
                    {
                        PandoraChestLogger.LogError("PandoraChest.TriggerTrap.Explosion", ex, $"Player={from?.Name},TrapPower={_trapPower},Level={_level}", from);
                    }
                    break;
                case TrapType.Poison:
                    try
                    {
                        var p = _trapPower >= 3 ? Poison.Deadly : (_trapPower == 2 ? Poison.Greater : Poison.Lesser);
                        from.ApplyPoison(from, p);
                    }
                    catch (Exception ex)
                    {
                        PandoraChestLogger.LogError("PandoraChest.TriggerTrap.Poison", ex, $"Player={from?.Name},TrapPower={_trapPower},Level={_level}", from);
                    }
                    break;
                case TrapType.SummonUndead:
                    try
                    {
                        int count = Math.Max(1, PandoraChestConfig.Current.Trap.SummonPerPower * _trapPower);
                        for (int i = 0; i < count; i++)
                        {
                            var z = new VengefulSpirit();
                            z.Hits += 10 * _trapPower;
                            z.DamageMin += _trapPower;
                            z.DamageMax += _trapPower * 2;
                            try { z.MoveToWorld(from.Location, from.Map); z.Combatant = from; } catch (Exception ex) { PandoraChestLogger.LogError("PandoraChest.TriggerTrap.SummonUndead.Move", ex, $"Player={from?.Name}", from); }
                        }
                    }
                    catch (Exception ex)
                    {
                        PandoraChestLogger.LogError("PandoraChest.TriggerTrap.SummonUndead", ex, $"Player={from?.Name},TrapPower={_trapPower},Level={_level}", from);
                    }
                    break;
            }
            try { from.SendMessage(0x22, "A hidden trap explodes as you touch the chest!"); } catch (Exception ex) { PandoraChestLogger.LogError("PandoraChest.TriggerTrap.SendMessage", ex, $"Player={from?.Name}", from); }
        }
    }

    [SerializationGenerator(0, false)]
    public partial class SpecialRareBoneItem : Item { [Constructible] public SpecialRareBoneItem() : base(0x1B) { Name = "Ancient Bone Relic"; Hue = 0x485; Weight = 1.0; } }

    [SerializationGenerator(0, false)]
    public partial class VengefulSpirit : BaseCreature
    {
        [Constructible]
        public VengefulSpirit() : base(AIType.AI_Mage, FightMode.Closest, 10, 1)
        {
            Name = "Vengeful Spirit"; Body = 26; Hue = 0x4001; SetStr(50); SetDex(50); SetInt(100); SetHits(40); SetDamage(8, 12); Fame = 4000; Karma = -4000; VirtualArmor = 30;
        }
        public override bool AlwaysAttackable => true;
        public override bool BleedImmune => true;
    }
}