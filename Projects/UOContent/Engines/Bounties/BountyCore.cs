using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Server;
using Server.Commands;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Engines.Bounties;

// Moved BountyDefinition to BountyRegistry.cs

// ==========================================
// 1. PLAYER BOUNTY PROFILE
// ==========================================
public class BountyProfile
{
    public int ActiveBountyID { get; set; } = -1;
    public int CurrentKills { get; set; } = 0;

    // Time-Gate Tracking
    public DateTime NextAvailableTime { get; set; } = DateTime.MinValue;

    public bool HasActiveBounty => ActiveBountyID != -1;
    public bool IsOnCooldown => DateTime.UtcNow < NextAvailableTime;
}

// ==========================================
// 2. JSON SAVE DATA STRUCTURE
// ==========================================
public class BountySaveData
{
    public uint MobileSerial { get; set; }
    public int ActiveBountyID { get; set; }
    public int CurrentKills { get; set; }
    public long NextAvailableTicks { get; set; } // Save DateTime as ticks
}

// ==========================================
// 3. BOUNTY SYSTEM ENGINE
// ==========================================
public static class BountySystem
{
    private static Dictionary<Mobile, BountyProfile> Profiles = new();

    public static void Configure()
    {
        EventSink.WorldSave += delegate { OnSave(); };
    }

    public static void Initialize()
    {
        CommandSystem.Register("Bounties", AccessLevel.Player, new CommandEventHandler(Bounties_OnCommand));

        // Initialize the new external registry
        BountyRegistry.Initialize();
        Load();
    }

    [Usage("Bounties")]
    [Description("Checks your current active dungeon bounty.")]
    private static void Bounties_OnCommand(CommandEventArgs e)
    {
        if (e.Mobile is not PlayerMobile pm) return;
        BountyProfile profile = GetProfile(pm);
        pm.CloseGump<ActiveBountyGump>();
        pm.SendGump(new ActiveBountyGump(pm, profile));
    }

    public static BountyProfile GetProfile(Mobile pm)
    {
        if (!Profiles.TryGetValue(pm, out var profile))
        {
            profile = new BountyProfile();
            Profiles[pm] = profile;
        }
        return profile;
    }

    // --- BULLETPROOF REGION CHECKER ---
    private static bool IsInValidRegion(Mobile creature, Mobile killer, string requiredRegion)
    {
        if (CheckRegionTree(creature.Region, requiredRegion)) return true;
        if (CheckRegionTree(killer.Region, requiredRegion)) return true;
        return false;
    }

    private static bool CheckRegionTree(Region region, string requiredRegion)
    {
        Region current = region;
        while (current != null)
        {
            if (current.Name != null && current.Name.IndexOf(requiredRegion, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
            current = current.Parent;
        }
        return false;
    }

    // --- KILL TRACKING HOOK ---
    public static void OnCreatureDeath(BaseCreature creature)
    {
        if (creature == null) return;
        Mobile source = creature.LastKiller ?? creature.FindMostRecentDamager(true);

        if (source is BaseCreature pet && pet.Controlled && pet.ControlMaster != null)
            source = pet.ControlMaster;

        if (source is not PlayerMobile killer) return;

        BountyProfile profile = GetProfile(killer);

        if (profile.HasActiveBounty && BountyRegistry.AllBounties.TryGetValue(profile.ActiveBountyID, out BountyDefinition bounty))
        {
            if (!string.IsNullOrEmpty(bounty.RequiredRegion))
            {
                if (!IsInValidRegion(creature, killer, bounty.RequiredRegion)) return;
            }

            if (profile.CurrentKills < bounty.RequiredKills && bounty.Matches(creature.GetType()))
            {
                profile.CurrentKills++;
                if (profile.CurrentKills >= bounty.RequiredKills)
                {
                    killer.SendMessage(0x3F, $"[Bounty Complete]: You have slain {profile.CurrentKills}/{bounty.RequiredKills} {bounty.Title}!");
                    killer.PlaySound(0x5B5);
                }
                else
                {
                    killer.SendMessage(0x35, $"[Bounty Progress]: {creature.Name} slain. ({profile.CurrentKills}/{bounty.RequiredKills})");
                }
            }
        }
    }

    // --- DATA PERSISTENCE (JSON) ---
    private static string FilePath => Path.Combine(Core.BaseDirectory, "Data", "DungeonBounties.json");

    private static void OnSave()
    {
        try
        {
            string dir = Path.GetDirectoryName(FilePath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            var list = new List<BountySaveData>();
            foreach (var kvp in Profiles)
            {
                if (kvp.Key != null && !kvp.Key.Deleted && (kvp.Value.HasActiveBounty || kvp.Value.IsOnCooldown))
                {
                    list.Add(new BountySaveData
                    {
                        MobileSerial = kvp.Key.Serial.Value,
                        ActiveBountyID = kvp.Value.ActiveBountyID,
                        CurrentKills = kvp.Value.CurrentKills,
                        NextAvailableTicks = kvp.Value.NextAvailableTime.Ticks
                    });
                }
            }
            File.WriteAllText(FilePath, JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true }));
        }
        catch (Exception e) { Console.WriteLine($"Error saving Bounties: {e.Message}"); }
    }

    private static void Load()
    {
        if (!File.Exists(FilePath)) return;
        try
        {
            var list = JsonSerializer.Deserialize<List<BountySaveData>>(File.ReadAllText(FilePath));
            if (list == null) return;

            foreach (var data in list)
            {
                Mobile m = World.FindMobile((Serial)data.MobileSerial);
                if (m != null && !m.Deleted)
                {
                    Profiles[m] = new BountyProfile
                    {
                        ActiveBountyID = data.ActiveBountyID,
                        CurrentKills = data.CurrentKills,
                        NextAvailableTime = new DateTime(data.NextAvailableTicks, DateTimeKind.Utc)
                    };
                }
            }
        }
        catch (Exception e) { Console.WriteLine($"Error loading Bounties: {e.Message}"); }
    }
}

// ==========================================
// 4. PLAYER STATUS GUMP ([Bounties Command)
// ==========================================
public class ActiveBountyGump : Gump
{
    public ActiveBountyGump(PlayerMobile pm, BountyProfile profile) : base(100, 100)
    {
        AddPage(0);
        AddBackground(0, 0, 350, 220, 9270);
        AddAlphaRegion(10, 10, 330, 200);
        AddLabel(110, 15, 1152, "ACTIVE BOUNTY STATUS");

        if (profile.HasActiveBounty && BountyRegistry.AllBounties.TryGetValue(profile.ActiveBountyID, out BountyDefinition bounty))
        {
            AddLabel(20, 50, 0x3F, "Target:");
            AddLabel(90, 50, 1152, bounty.Title);

            if (!string.IsNullOrEmpty(bounty.RequiredRegion))
            {
                AddLabel(20, 75, 0x3F, "Location:");
                AddLabel(90, 75, 1152, bounty.RequiredRegion);
            }

            AddLabel(20, 100, 0x3F, "Progress:");

            if (profile.CurrentKills >= bounty.RequiredKills)
            {
                AddLabel(90, 100, 0x44, $"{profile.CurrentKills} / {bounty.RequiredKills} - COMPLETE!");
                AddLabel(20, 125, 0x44, "Return to a Bounty Board to claim!");
            }
            else
            {
                AddLabel(90, 100, 2100, $"{profile.CurrentKills} / {bounty.RequiredKills} Slain");
            }
        }
        else
        {
            AddLabel(20, 75, 33, "You do not have an active bounty.");

            if (profile.IsOnCooldown)
            {
                TimeSpan wait = profile.NextAvailableTime - DateTime.UtcNow;
                AddLabel(20, 95, 2100, $"New bounties in: {(int)wait.TotalMinutes} minutes.");
            }
            else
            {
                AddLabel(20, 95, 1152, "Visit a Bounty Board to accept one.");
            }
        }

        AddButton(20, 175, 4017, 4019, 0, GumpButtonType.Reply, 0);
        AddLabel(55, 175, 1152, "Close");
    }
}
