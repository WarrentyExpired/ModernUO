/**********************************
* Author: Unknow                  *   
* Update/Fixed/Refactoring: Expa  *
* Script: PandoraChestLogger.cs   *
* Optimized for ModernUO 0.15.4.18*
**********************************/
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Server;
using Server.Items;
using Server.Mobiles;

static class PandoraChestLogger
{
    static readonly ConcurrentQueue<string> Q = new();
    static CancellationTokenSource _cts;
    static Task _worker;
    static string _logFile;
    static long _maxSize;
    static int _flushInterval;
    static readonly object _initLock = new();
    static readonly JsonSerializerOptions _jsonOpts = new() { WriteIndented = false, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public static void Initialize()
    {
        lock (_initLock)
        {
            if (_worker != null && !_worker.IsCompleted) return;
            var baseDir = AppDomain.CurrentDomain.BaseDirectory ?? Environment.CurrentDirectory;
            var logsDir = Path.Combine(baseDir, "Logs");
            try { Directory.CreateDirectory(logsDir); } catch (Exception ex) { try { Console.Error.WriteLine($"PandoraChestLogger.Initialize: CreateDirectory failed: {ex}"); } catch { } }
            _logFile = Path.Combine(logsDir, "PandoraChestSpawns.log");
            var cfg = PandoraChestConfig.Current ?? new PandoraChestConfig();
            _maxSize = cfg.Logging?.MaxFileSizeBytes ?? (10 * 1024 * 1024);
            _flushInterval = Math.Max(50, cfg.Logging?.FlushIntervalMs ?? 500);
            _cts = new CancellationTokenSource();
            _worker = Task.Run(() => Worker(_cts.Token));
        }
    }

    static async Task Worker(CancellationToken token)
    {
        var batch = new List<string>();
        while (!token.IsCancellationRequested)
        {
            try { await Task.Delay(_flushInterval, token).ConfigureAwait(false); } catch { break; }
            batch.Clear();
            while (Q.TryDequeue(out var l)) { batch.Add(l); if (batch.Count >= 1000) break; }
            if (batch.Count == 0) continue;
            try
            {
                RotateIfNeeded();
                if (!string.IsNullOrEmpty(_logFile)) File.AppendAllLines(_logFile, batch, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                try { Console.Error.WriteLine($"PandoraChestLogger.Worker AppendAllLines failed: {ex}"); } catch { }
                foreach (var l in batch) try { Console.WriteLine(l); } catch { }
            }
        }
        var rem = new List<string>();
        while (Q.TryDequeue(out var l)) rem.Add(l);
        if (rem.Count > 0)
        {
            try { RotateIfNeeded(); if (!string.IsNullOrEmpty(_logFile)) File.AppendAllLines(_logFile, rem, Encoding.UTF8); } catch (Exception ex) { try { Console.Error.WriteLine($"PandoraChestLogger.Worker final flush failed: {ex}"); } catch { } }
        }
    }

    static void RotateIfNeeded()
    {
        try
        {
            if (string.IsNullOrEmpty(_logFile)) return;
            var fi = new FileInfo(_logFile);
            if (fi.Exists && fi.Length > _maxSize)
            {
                var dest = _logFile + "." + DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                try { File.Move(_logFile, dest); } catch (Exception ex) { try { Console.Error.WriteLine($"PandoraChestLogger.RotateIfNeeded File.Move failed: {ex}"); } catch { } }
            }
        }
        catch (Exception ex) { try { Console.Error.WriteLine($"PandoraChestLogger.RotateIfNeeded failed: {ex}"); } catch { } }
    }

    static string HashShort(string input)
    {
        if (string.IsNullOrEmpty(input)) return "(null)";
        try
        {
            using var sha = SHA256.Create();
            var h = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            var hex = BitConverter.ToString(h).Replace("-", "").ToLowerInvariant();
            return hex.Length >= 16 ? hex.Substring(0, 16) : hex;
        }
        catch (Exception ex) { try { Console.Error.WriteLine($"PandoraChestLogger.HashShort failed: {ex}"); } catch { } return "(err)"; }
    }

    static void EnqueueStructured(object obj)
    {
        try
        {
            var line = JsonSerializer.Serialize(obj, _jsonOpts);
            Q.Enqueue(line);
        }
        catch (Exception ex)
        {
            try { Console.Error.WriteLine($"PandoraChestLogger.EnqueueStructured failed: {ex}"); } catch { }
        }
    }

    public static void LogInfo(string tag, string context = null, Mobile who = null, object extra = null)
    {
        try
        {
            var entry = new
            {
                timestamp = DateTime.UtcNow,
                level = "INFO",
                tag,
                context,
                player = who?.Name ?? "(null)",
                accountHash = SafeAccountHash(who),
                extra
            };
            EnqueueStructured(entry);
        }
        catch { }
    }

    public static void LogWarn(string tag, string context = null, Mobile who = null, object extra = null)
    {
        try
        {
            var entry = new
            {
                timestamp = DateTime.UtcNow,
                level = "WARN",
                tag,
                context,
                player = who?.Name ?? "(null)",
                accountHash = SafeAccountHash(who),
                extra
            };
            EnqueueStructured(entry);
        }
        catch { }
    }

    public static void LogError(string tag, Exception ex, string context = null, Mobile who = null)
    {
        try
        {
            var entry = new
            {
                timestamp = DateTime.UtcNow,
                level = "ERROR",
                tag,
                context,
                player = who?.Name ?? "(null)",
                accountHash = SafeAccountHash(who),
                exception = ex?.ToString()
            };
            EnqueueStructured(entry);
        }
        catch (Exception inner)
        {
            try { Console.Error.WriteLine($"PandoraChestLogger.LogError failed to enqueue: {inner}. Original: {tag} {context} {ex}"); } catch { }
        }
    }

    static string SafeAccountHash(Mobile who)
    {
        try { var acc = who?.Account?.Username ?? who?.Account?.ToString() ?? "(null)"; return HashShort(acc); } catch { return "(err)"; }
    }

    public static void LogSpawn(Mobile who, Map map, Point3D loc, int level, double spawnChance)
    {
        try
        {
            if (!PandoraChestConfig.Current.Logging.Enabled) return;
        }
        catch { /* best-effort: fall through */ }

        try
        {
            var entry = new
            {
                timestamp = DateTime.UtcNow,
                level = "INFO",
                tag = "SPAWN",
                player = who?.Name ?? "(null)",
                accountHash = SafeAccountHash(who),
                map = map?.ToString(),
                loc = new { x = loc.X, y = loc.Y, z = loc.Z },
                levelNumber = level,
                chance = spawnChance
            };
            EnqueueStructured(entry);
        }
        catch (Exception ex) { LogError("PandoraChestLogger.LogSpawn", ex, null, who); }
    }

    public static void LogContents(Mobile who, int level, IEnumerable<string> items)
    {
        try
        {
            if (!PandoraChestConfig.Current.Logging.Enabled) return;
        }
        catch { }

        try
        {
            var entry = new
            {
                timestamp = DateTime.UtcNow,
                level = "INFO",
                tag = "CONTENTS",
                player = who?.Name ?? "(null)",
                accountHash = SafeAccountHash(who),
                levelNumber = level,
                items = items
            };
            EnqueueStructured(entry);
        }
        catch (Exception ex) { LogError("PandoraChestLogger.LogContents", ex, null, who); }
    }

    public static void LogMagicDrop(Mobile who, int level, Item it)
    {
        try
        {
            if (!PandoraChestConfig.Current.Logging.Enabled) return;
        }
        catch { }

        try
        {
            var entry = new
            {
                timestamp = DateTime.UtcNow,
                level = "INFO",
                tag = "MAGIC",
                player = who?.Name ?? "(null)",
                accountHash = SafeAccountHash(who),
                levelNumber = level,
                itemType = it?.GetType().Name,
                itemName = it?.Name,
                hue = it?.Hue ?? 0
            };
            EnqueueStructured(entry);
        }
        catch (Exception ex) { LogError("PandoraChestLogger.LogMagicDrop", ex, null, who); }
    }

    public static void Shutdown(int waitMs = 3000)
    {
        try
        {
            _cts?.Cancel();
            _worker?.Wait(waitMs);
        }
        catch (Exception ex) { try { Console.Error.WriteLine($"PandoraChestLogger.Shutdown error: {ex}"); } catch { } }
    }
}