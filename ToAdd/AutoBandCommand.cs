/**********************************
* Author: Expa (refactored)       *
* Script Name: AutoBandCommand.cs *
* Target: ModernUO (.NET 10/C#14) *
**********************************/
using System;
using ModernUO.Serialization;
using Server;
using Server.Commands;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Scripts.Commands;

public static class AutoBandCommand
{
    public static void Initialize()
    {
        CommandSystem.Register("bas", AccessLevel.Player, OnBandSelf);
        CommandSystem.Register("bat", AccessLevel.Player, OnBandTarget);
    }

    public static void OnBandSelf(CommandEventArgs e)
    {
        if (e.Mobile is not PlayerMobile pm) return;
        Start(pm, pm);
    }

    public static void OnBandTarget(CommandEventArgs e)
        => e.Mobile.BeginTarget(-1, false, TargetFlags.None, OnTarget);

    private static void OnTarget(Mobile from, object targeted)
    {
        if (from is not PlayerMobile pm || targeted is not Mobile target || target.Deleted || !target.Alive)
        {
            from.SendMessage("Invalid target.");
            return;
        }

        Start(pm, target);
    }

    private static void Start(PlayerMobile pm, Mobile target)
    {
        if (pm.AutoBandageTimer is AutoBandageTimer existing && existing.Targets(target))
        {
            StopTimer(pm);
            SendHealthMessage(pm, $"Bandage auto-heal DISABLED{(pm == target ? "" : $" on {target.Name}")}.");
            return;
        }

        StopTimer(pm);

        var band = pm.Backpack?.FindItemByType<Bandage>();
        if (band == null || band.Amount <= 0)
        {
            pm.SendMessage("You have no bandages!");
            return;
        }

        var timer = new AutoBandageTimer(pm, target);
        pm.AutoBandageTimer = timer;
        timer.Start();

        SendHealthMessage(pm, $"Bandage auto-heal ENABLED{(pm == target ? "" : $" on {target.Name}")}. Health: {target.Hits}/{target.HitsMax}");
    }

    internal static void SendHealthMessage(Mobile m, string msg)
    {
        var denom = Math.Max(1, m.HitsMax);
        double p = (double)m.Hits / denom;
        var hue = p < 0.3 ? 33 : p < 0.7 ? 53 : 63;
        m.SendMessage(hue, msg);
    }

    private static void StopTimer(PlayerMobile pm)
    {
        pm.AutoBandageTimer?.Stop();
        pm.AutoBandageTimer = null;
    }
}

[SerializationGenerator(0)]
public partial class AutoBandageTimer : Timer
{
    private const double ResumeAt = 90.0;
    private const double PauseAt = 100.0;
    private static readonly TimeSpan TickInterval = TimeSpan.FromMilliseconds(100);

    private readonly WeakReference<Mobile> _casterRef;
    private readonly WeakReference<Mobile> _targetRef;
    private bool _paused;
    private DateTime _next;

    public AutoBandageTimer(Mobile caster, Mobile target)
        : base(TimeSpan.Zero, TickInterval)
    {
        _casterRef = new(caster);
        _targetRef = new(target);
        _next = Core.Now;
    }

    public bool Targets(Mobile m)
        => m is not null && _targetRef.TryGetTarget(out var t) && t == m;

    protected override void OnTick()
    {
        if (!_casterRef.TryGetTarget(out var caster) || caster.Deleted || !caster.Alive || caster.Map == Map.Internal)
        {
            Stop();
            Cleanup(caster);
            return;
        }

        if (!_targetRef.TryGetTarget(out var target) || target.Deleted || !target.Alive || target.Map == Map.Internal)
        {
            caster.SendMessage("Target is no longer valid. Automatic bandaging stopped.");
            Stop();
            Cleanup(caster);
            return;
        }

        if (BandageContext.GetContext(caster) != null)
            return;

        var denom = Math.Max(1, target.HitsMax);
        double hpPercent = (double)target.Hits / denom * 100.0;

        if (!_paused && hpPercent >= PauseAt)
        {
            _paused = true;
            caster.SendMessage("Bandaging paused. Target's health is full.");
            return;
        }

        if (_paused && hpPercent < ResumeAt)
        {
            _paused = false;
            caster.SendMessage($"Bandaging resumed. Target's health dropped below {ResumeAt}%.");
            _next = Core.Now;
        }

        if (_paused || Core.Now < _next) return;

        var band = caster.Backpack?.FindItemByType<Bandage>();
        if (band == null || band.Amount <= 0)
        {
            caster.SendMessage("Out of bandages. Automatic bandaging stopped.");
            Stop();
            Cleanup(caster);
            return;
        }

        int preHits = target.Hits;
        double delaySec = 5.0 + 0.05 * Math.Max(0, 120 - caster.Dex);
        _next = Core.Now + TimeSpan.FromSeconds(delaySec);

        Bandage.BandageTargetRequest(caster, band, target);
        Timer.DelayCall(TimeSpan.FromSeconds(delaySec * 0.95), () => ShowResult(caster, target, preHits));
    }

    private static void ShowResult(Mobile? caster, Mobile? target, int preHits)
    {
        if (caster == null || target == null) return;
        if (caster.Deleted || !caster.Alive || target.Deleted || !target.Alive) return;

        int healed = target.Hits - preHits;
        string msg = healed > 0
            ? $"Healing {target.Name}: {target.Hits}/{target.HitsMax} HP (+{healed})"
            : $"Healing {target.Name}: {target.Hits}/{target.HitsMax} HP";

        AutoBandCommand.SendHealthMessage(caster, msg);
    }

    private static void Cleanup(Mobile? caster)
    {
        if (caster is PlayerMobile pm) pm.AutoBandageTimer = null;
    }
}