using System;
using System.Collections.Generic;
using Server.Logging;
using Server.Mobiles;

namespace Server.Custom.ExileHunterBestiary.Hooks;

public static class HunterMarkHook
{
    private static readonly ILogger _logger = LogFactory.GetLogger(typeof(HunterMarkHook));

    private sealed class MarkState
    {
        public BaseCreature Creature;
        public Mobile Master;
        public DateTime ExpiresAt;
        public DateTime NextTickAt;
    }

    private static readonly Dictionary<Serial, MarkState> _marks = [];



    private static bool _applyingMark;

    public static TimeSpan MarkDuration { get; private set; } = TimeSpan.FromSeconds(10);
    public static TimeSpan TickInterval { get; private set; } = TimeSpan.FromSeconds(2);
    public static double TickPercentOfMaxHits { get; private set; } = 0.01;

    public static void Configure()
    {
        MarkDuration = HunterBestiaryEngine.GetTimeSpan("exileBestiary.damage.markDuration", TimeSpan.FromSeconds(10));
        TickInterval = HunterBestiaryEngine.GetTimeSpan("exileBestiary.damage.markTickSeconds", TimeSpan.FromSeconds(2));
        TickPercentOfMaxHits = HunterBestiaryEngine.GetDouble("exileBestiary.damage.markTickPercent", 0.01);

        EventSink.AggressiveAction += OnAggressiveAction;
        Timer.DelayCall(TickInterval, TickInterval, Tick);

        _logger.Information(
            "Hunter's Mark enabled: {Duration}s mark, {Interval}s ticks, {Percent:P0} of max hits per tick.",
            MarkDuration, TickInterval, TickPercentOfMaxHits
        );
    }

    private static void OnAggressiveAction(AggressiveActionEventArgs args)
    {
        if (_applyingMark ||
            !HunterBestiaryEngine.Enabled ||
            args.Aggressed is not BaseCreature creature ||
            creature is { Deleted: true } or { Summoned: true } or { IsBonded: true })
        {
            return;
        }

        var entry = BestiaryRegistry.FindEntry(creature.GetType());
        if (entry == null)
        {
            return;
        }

        var master = args.Aggressor is BaseCreature { Controlled: true, ControlMaster: { } controlMaster }
            ? controlMaster
            : args.Aggressor;

        if (master is not PlayerMobile ||
            !HunterBestiaryEngine.TryGetEffectiveBonus(master, creature, out var bonus))
        {
            return;
        }

        var now = DateTime.UtcNow;
        if (!_marks.TryGetValue(creature.Serial, out var state))
        {
            state = new MarkState { Creature = creature };
            _marks[creature.Serial] = state;
        }

        state.Master = master;
        state.ExpiresAt = now + MarkDuration;
        if (state.NextTickAt < now)
        {
            state.NextTickAt = now + TickInterval;
        }
    }

    private static void Tick()
    {
        if (_marks.Count == 0)
        {
            return;
        }

        var now = DateTime.UtcNow;
        List<Serial> expired = null;

        foreach (var (serial, state) in _marks)
        {
            var creature = state.Creature;
            var master = state.Master;

            if (creature.Deleted || creature.Hits <= 0 ||
                master is not { Deleted: false } ||
                now >= state.ExpiresAt ||
                !master.InRange(creature.Location, 16))
            {
                (expired ??= []).Add(serial);
                continue;
            }

            if (now < state.NextTickAt)
            {
                continue;
            }

            state.NextTickAt = now + TickInterval;

            if (!HunterBestiaryEngine.TryGetEffectiveBonus(master, creature, out var bonus))
            {
                (expired ??= []).Add(serial);
                continue;
            }



            var intensity = HunterBestiaryEngine.TotalBonusCap > 0
                ? bonus / HunterBestiaryEngine.TotalBonusCap
                : 0;

            var amount = (int)Math.Max(1, Math.Round(creature.HitsMax * TickPercentOfMaxHits * intensity));

            _applyingMark = true;
            try
            {
                creature.Damage(amount, master);
            }
            finally
            {
                _applyingMark = false;
            }
        }

        if (expired != null)
        {
            foreach (var serial in expired)
            {
                _marks.Remove(serial);
            }
        }
    }
}
