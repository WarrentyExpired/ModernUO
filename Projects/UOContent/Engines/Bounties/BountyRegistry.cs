using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;

namespace Server.Engines.Bounties;

// ==========================================
// 1. BOUNTY DEFINITION
// ==========================================
public class BountyDefinition
{
    public int ID { get; }
    public string Title { get; }
    public Type[] TargetTypes { get; }
    public int RequiredKills { get; }
    public int RequiredFame { get; }
    public int RewardTokens { get; }
    public int RewardGold { get; }
    public string RequiredRegion { get; }

    public BountyDefinition(int id, string title, Type[] targetTypes, int requiredKills, int requiredFame, int rewardTokens, int rewardGold, string requiredRegion = null)
    {
        ID = id;
        Title = title;
        TargetTypes = targetTypes;
        RequiredKills = requiredKills;
        RequiredFame = requiredFame;
        RewardTokens = rewardTokens;
        RewardGold = rewardGold;
        RequiredRegion = requiredRegion;
    }

    public bool Matches(Type type)
    {
        foreach (var t in TargetTypes)
        {
            if (t == type || t.IsAssignableFrom(type))
            {
                return true;
            }
        }
        return false;
    }
}

// ==========================================
// 2. TIERED BOUNTY REGISTRY
// ==========================================
public static class BountyRegistry
{
    // Lists mapped by Tier (1 through 5)
    public static Dictionary<int, List<BountyDefinition>> Tiers { get; private set; } = new();

    // A master dictionary to look up any bounty by ID
    public static Dictionary<int, BountyDefinition> AllBounties { get; private set; } = new();

    public static void Initialize()
    {
        // ==========================================
        // Amount, Fame, Tokens, Gold
        // 15, 0, 5, 500
        // ==========================================

        // ==========================================
        // TIER 1: Local Adventurer (Fame 0) - Rewards: 5 Tokens, 500g
        // ==========================================
        Add(1, new BountyDefinition(1, "Covetous Harpies", new[] { typeof(Harpy) }, 15, 0, 5, 500, "Covetous"));
        Add(1, new BountyDefinition(2, "Shame Earth Elementals", new[] { typeof(EarthElemental) }, 10, 0, 5, 500, "Shame"));
        Add(1, new BountyDefinition(3, "Despise Lizardmen", new[] { typeof(Lizardman) }, 15, 0, 5, 500, "Despise"));
        Add(1, new BountyDefinition(4, "Deceit - Skeletons", new[] { typeof(Skeleton) }, 15, 0, 5, 500, "Deceit"));

        // ==========================================
        // TIER 2: Estimable Hunter (Fame 1250) - Rewards: 15 Tokens, 1200g
        // ==========================================
        Add(2, new BountyDefinition(4, "Deceit Skeletal Knights", new[] { typeof(SkeletalKnight) }, 10, 1250, 15, 1200, "Deceit"));
        Add(2, new BountyDefinition(5, "Destard Drakes", new[] { typeof(Drake) }, 10, 1250, 15, 1200, "Destard"));

        // ==========================================
        // TIER 3: Great Slayer (Fame 2500) - Rewards: 30 Tokens, 2500g
        // ==========================================
        Add(3, new BountyDefinition(6, "Despise Titans", new[] { typeof(Titan) }, 8, 2500, 30, 2500, "Despise"));

        // ==========================================
        // TIER 4: Illustrious Champion (Fame 5000) - Rewards: 60 Tokens, 5000g
        // ==========================================
        Add(4, new BountyDefinition(7, "Destard Dragons", new[] { typeof(Dragon) }, 3, 5000, 60, 5000, "Destard"));

        // ==========================================
        // TIER 5: Glorious Lord (Fame 10000) - Rewards: 100 Tokens, 10000g
        // ==========================================
        Add(5, new BountyDefinition(8, "Hythloth Balrons", new[] { typeof(Balron) }, 2, 10000, 100, 10000, "Hythloth"));
    }

    private static void Add(int tier, BountyDefinition bounty)
    {
        if (!Tiers.ContainsKey(tier))
        {
            Tiers[tier] = new List<BountyDefinition>();
        }

        Tiers[tier].Add(bounty);
        AllBounties[bounty.ID] = bounty;
    }

    // Pulls a random bounty from a specific tier pool
    public static BountyDefinition GetRandomBounty(int tier)
    {
        if (!Tiers.ContainsKey(tier)) return null;

        var list = Tiers[tier];
        return list[Utility.Random(list.Count)];
    }
}
