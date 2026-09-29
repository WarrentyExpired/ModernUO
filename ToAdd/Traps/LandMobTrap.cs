/**********************************
* Author: Expa                    *
* Optimized ModernUO .Net 10/C#14 *
**********************************/
using System;
using ModernUO.Serialization;
using Server;
using Server.Mobiles;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class LandMobTrap : Item
{
    [SerializableField(0)] private int _minSpawn = 3;
    [SerializableField(1)] private int _maxSpawn = 6;
    [SerializableField(2)] private double _triggerDelaySeconds = 0.5;
    [SerializableField(3)] private int _soundID = 0x1C6;
    [SerializableField(4)] private int _particleItemId = 0x373A;
    [SerializableField(5)] private byte _particleCount = 20;
    [SerializableField(6)] private byte _particleSpeed = 10;
    [SerializableField(7)] private byte _flags = 1;

    private const int MaxAllowedSpawn = 12;
    private TimerExecutionToken _spawnToken;

    private bool Armed
    {
        get => (_flags & 1) != 0;
        set => _flags = (byte)(value ? (_flags | 1) : (_flags & ~1));
    }

    private bool BlockMovement
    {
        get => (_flags & 2) != 0;
        set => _flags = (byte)(value ? (_flags | 2) : (_flags & ~2));
    }

    [Constructible]
    public LandMobTrap() : base(0x1B7A)
    {
        Movable = false;
        Hue = 1058;
        Visible = false;
        Name = "land mob trap";
    }

    public override bool HandlesOnMovement => true;

    public override bool OnMoveOver(Mobile m)
    {
        var map = Map;
        if (Deleted || map == null || map == Map.Internal)
            return true;

        if (!Armed || !m.Player || m.AccessLevel > AccessLevel.Player)
            return true;

        foreach (var bc in GetMobilesInRange<BaseCreature>(1))
        {
            if (bc is Skeleton or SkeletalMage or SkeletalKnight)
                return true;
        }

        Armed = false;
        InvalidateProperties();

        var min = Math.Max(1, _minSpawn);
        var max = Math.Clamp(_maxSpawn, min, MaxAllowedSpawn);

        if (_triggerDelaySeconds <= 0.0)
            DoSpawn(m, min, max);
        else
            Timer.StartTimer(TimeSpan.FromSeconds(_triggerDelaySeconds), () => DoSpawn(m, min, max), out _spawnToken);

        return !BlockMovement;
    }

    private void DoSpawn(Mobile trigger, int minSpawn, int maxSpawn)
    {
        try
        {
            var map = Map;
            if (Deleted || map == null || map == Map.Internal)
                return;

            var count = Utility.RandomMinMax(minSpawn, maxSpawn);

            for (var i = 0; i < count; ++i)
            {
                var cr = CreateRandomCreature();
                if (cr is null)
                    continue;

                try
                {
                    if (TryFindSpawnLocation(map, out var loc))
                    {
                        cr.MoveToWorld(loc, map);
                        cr.Combatant = trigger;
                    }
                    else
                    {
                        cr.Delete();
                    }
                }
                catch
                {
                    cr?.Delete();
                }
            }

            Effects.SendLocationParticles(
                EffectItem.Create(Location, Map, EffectItem.DefaultDuration),
                _particleItemId, _particleCount, _particleSpeed, 5044
            );
            Effects.PlaySound(Location, Map, _soundID);
        }
        finally
        {
            Delete();
        }
    }

    private static BaseCreature CreateRandomCreature() =>
        Utility.Random(5) switch
        {
            0 or 1 => new Skeleton(),
            2 or 3 => new SkeletalMage(),
            _ => new SkeletalKnight()
        };

    private bool TryFindSpawnLocation(Map map, out Point3D loc)
    {
        loc = Location;

        for (var i = 0; i < 10; ++i)
        {
            var x = X + Utility.Random(3) - 1;
            var y = Y + Utility.Random(3) - 1;
            var z = map.GetAverageZ(x, y);

            if (map.CanSpawnMobile(new Point2D(x, y), z))
            {
                loc = new Point3D(x, y, z);
                return true;
            }

            if (map.CanFit(x, y, Z, 16, false, false))
            {
                loc = new Point3D(x, y, Z);
                return true;
            }
        }

        return false;
    }

    public override void OnAfterDelete()
    {
        _spawnToken.Cancel();
        base.OnAfterDelete();
    }

    [AfterDeserialization]
    private void AfterDeserialization() { }
}