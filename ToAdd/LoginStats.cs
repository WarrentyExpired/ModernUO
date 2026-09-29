/*********************************
* Author: Anna                   *
* Fixed and Refactoring: Expa    *
* Optimized ModernUO .Net10/C#14 *
**********************************/
using System;
using System.Linq;
using System.Diagnostics;
using Server;
using Server.Mobiles;
using Server.Network;

namespace Server.Misc;

public static class LoginStats
{
    static readonly TimeSpan ReadyDelay = TimeSpan.FromSeconds(1);
    static readonly TimeSpan InvulDuration = TimeSpan.FromSeconds(10);
    const int HuePersonal = 0x35, HueGlobalLoginLogout = 0x3B2, HueDisconnected = 0x22;
    const int MaxReadyAttempts = 10;

    public static void Initialize()
    {
        EventSink.Connected += OnConnected;
        EventSink.BeforeDisconnected += OnBeforeDisconnected;
        EventSink.Logout += OnDisconnect;
        EventSink.Disconnected += OnDisconnect;
        Trace.WriteLine("[LoginStats] initialized.");
    }

    static void OnConnected(Mobile m)
    {
        if (m is PlayerMobile pm && pm.AccessLevel == AccessLevel.Player)
            Timer.StartTimer(ReadyDelay, () => HandleLogin(pm, 0));
    }

    static void OnBeforeDisconnected(Mobile m)
    {
        if (m is PlayerMobile pm)
            try { pm.NetState?.Disconnect("Connection lost"); } catch { }
    }

    static void OnDisconnect(Mobile m)
    {
        if (m is PlayerMobile pm)
        {
            World.Broadcast(HueDisconnected, false, $"{pm.Name} has disconnected.");
            Trace.WriteLine($"[LoginStats] Disconnected: {pm.Name}");
        }
    }

    static void HandleLogin(PlayerMobile pm, int attempt)
    {
        if (pm == null || pm.Deleted) return;
        if ((pm.NetState == null || pm.SessionStart == default) && attempt < MaxReadyAttempts)
        {
            Timer.StartTimer(ReadyDelay, () => HandleLogin(pm, attempt + 1)); return;
        }
        if (pm.NetState == null || pm.SessionStart == default) return;

        var gt = pm.GameTime;
        if (gt.TotalDays > 10000) gt = TimeSpan.Zero;
        pm.SendMessage(HuePersonal, $"Welcome back {pm.Name}! Total play time: {FormatPlaytime(gt)}");

        Timer.StartTimer(TimeSpan.FromMilliseconds(500), () =>
        {
            if (pm == null || pm.Deleted) return;
            var text = $"{pm.Name} has logged in.";
            foreach (var ns in NetState.Instances.ToArray())
            {
                var mob = ns?.Mobile;
                if (mob == null || mob == pm) continue;
                try { mob.SendMessage(HueGlobalLoginLogout, text); } catch { }
            }
        });

        pm.Hidden = true; pm.Blessed = true;
        Timer.StartTimer(InvulDuration, () => { if (pm != null && !pm.Deleted) { pm.Hidden = false; pm.Blessed = false; } });
        Trace.WriteLine($"[LoginStats] Login: {pm.Name}");
    }

    static string FormatPlaytime(TimeSpan ts) => $"{ts.Days}d {ts.Hours}h {ts.Minutes}m";
}