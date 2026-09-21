using System;
using System.Collections.Generic;
using Server.Custom.ExileHunterBestiary.Hooks;
using Server.Mobiles;

namespace Server.Custom.ExileHunterBestiary;

public static class HunterCombatTracker
{
    private static readonly TimeSpan Expiry = TimeSpan.FromSeconds(60);

    private sealed class Hit
    {
        public BaseCreature Creature;
        public Mobile Master;
        public Point3D Location;
        public Map Map;
        public DateTime When;
    }

    private static readonly Dictionary<Serial, Hit> _hits = [];

    private static bool _sweeping;

    public static int HitCount { get; private set; }

    public static int TrackedCount => _hits.Count;

    public static void Note(Mobile aggressor, Mobile aggressed)
    {
        if (aggressor == null || aggressed is not BaseCreature creature)
        {
            return;
        }

        if (creature.Deleted || creature.Summoned || creature.IsBonded)
        {
            return;
        }

        var master = aggressor is BaseCreature
        {
            Controlled: true,
            ControlMaster: { } controlMaster
        }
            ? controlMaster
            : aggressor;

        if (master is not PlayerMobile)
        {
            return;
        }

        if (!_hits.TryGetValue(creature.Serial, out var hit))
        {
            hit = new Hit();
            _hits[creature.Serial] = hit;
        }

        hit.Creature = creature;
        hit.Master = master;
        hit.Location = creature.Location;
        hit.Map = creature.Map;
        hit.When = DateTime.UtcNow;

        HitCount++;
    }

    public static Mobile GetLastHitter(BaseCreature creature)
    {
        if (creature == null)
        {
            return null;
        }

        if (_hits.TryGetValue(creature.Serial, out var hit) &&
            DateTime.UtcNow - hit.When <= Expiry &&
            hit.Master is { Deleted: false })
        {
            return hit.Master;
        }

        return null;
    }

    public static bool TryGetDeathLocation(
        BaseCreature creature,
        out Point3D location,
        out Map map
    )
    {
        location = Point3D.Zero;
        map = null;

        if (creature == null)
        {
            return false;
        }

        if (_hits.TryGetValue(creature.Serial, out var hit) &&
            DateTime.UtcNow - hit.When <= Expiry &&
            hit.Map != null &&
            hit.Map != Map.Internal)
        {
            location = hit.Location;
            map = hit.Map;

            return true;
        }

        return false;
    }

    public static void Forget(Serial serial)
    {
        if (!_sweeping)
        {
            _hits.Remove(serial);
        }
    }

    public static void Clear()
    {
        _hits.Clear();
        HitCount = 0;
    }

    public static void StartSweeper()
    {
        Timer.DelayCall(
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(1),
            Sweep
        );
    }

    private static void Sweep()
    {
        if (_hits.Count == 0 || _sweeping)
        {
            return;
        }

        _sweeping = true;

        try
        {
            var now = DateTime.UtcNow;
            List<Serial> remove = null;

            foreach (var (serial, hit) in _hits)
            {
                var creature = hit.Creature;

                if (creature == null ||
                    creature.Deleted ||
                    now - hit.When > Expiry)
                {
                    (remove ??= []).Add(serial);
                    continue;
                }

                if (creature.Hits > 0)
                {
                    continue;
                }

                HunterDeathHook.OnCreatureDeath(
                    creature,
                    hit.Master
                );

                (remove ??= []).Add(serial);
            }

            if (remove == null)
            {
                return;
            }

            foreach (var serial in remove)
            {
                _hits.Remove(serial);
            }
        }
        finally
        {
            _sweeping = false;
        }
    }
}