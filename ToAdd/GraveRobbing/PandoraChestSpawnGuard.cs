/**********************************
* Author: Unknow                  *   
* Update/Fixed/Refactoring: Expa  *
* Script: PandoraChestSpawnGuard.cs*
* Optimized for ModernUO 0.15.4.18 *
***********************************/
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Server;

static class PandoraChestSpawnGuard
{
    static readonly ConcurrentDictionary<string, DateTime> locs = new();
    static readonly ConcurrentDictionary<string, DateTime> accs = new();

    public static void Initialize() => Task.Run(Cleanup);

    static async Task Cleanup()
    {
        while (true)
        {
            try
            {
                await Task.Delay(30000).ConfigureAwait(false);
                var now = DateTime.UtcNow;

                foreach (var kv in locs.ToArray())
                {
                    if ((now - kv.Value).TotalSeconds > 3600) locs.TryRemove(kv.Key, out _);
                }

                foreach (var kv in accs.ToArray())
                {
                    if ((now - kv.Value).TotalSeconds > 3600) accs.TryRemove(kv.Key, out _);
                }
            }
            catch { }
        }
    }

    static string LocKey(Map? m, Point3D p, int bucket = 2)
    {
        if (m == null) return $"(null):{p.X / bucket}:{p.Y / bucket}";
        return $"{m}:{p.X / bucket}:{p.Y / bucket}";
    }

    public static bool CanSpawnAt(Map m, Point3D p, PandoraChestConfig cfg)
    {
        var k = LocKey(m, p);
        if (locs.TryGetValue(k, out var t) && (DateTime.UtcNow - t).TotalSeconds < cfg.Logging.LocationCooldownSeconds) return false;
        return true;
    }

    public static void RecordAt(Map m, Point3D p) => locs[LocKey(m, p)] = DateTime.UtcNow;

    public static bool CanSpawnForAccount(string a, PandoraChestConfig cfg)
    {
        if (string.IsNullOrEmpty(a)) a = "(null)";
        if (accs.TryGetValue(a, out var t) && (DateTime.UtcNow - t).TotalSeconds < cfg.Logging.AccountCooldownSeconds) return false;
        return true;
    }

    public static void RecordAccount(string a) { if (string.IsNullOrEmpty(a)) a = "(null)"; accs[a] = DateTime.UtcNow; }
}