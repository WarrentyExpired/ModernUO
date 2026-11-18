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
        Add(1, new BountyDefinition(1, "Headless Ones in the lands of Veridia.", new[] { typeof(HeadlessOne) }, 5, 0, 5, 500, "Veridia"));
        Add(1, new BountyDefinition(2, "Orcs in the lands of Veridia.", new[] { typeof(Orc) }, 5, 0, 5, 500, "Veridia"));
        Add(1, new BountyDefinition(3, "Ogres in the lands of Veridia.", new[] { typeof(Ogre) }, 5, 0, 5, 500, "Veridia"));
        Add(1, new BountyDefinition(4, "Ettins in the lands of Veridia.", new[] { typeof(Ettin) }, 5, 0, 5, 500, "Veridia"));
        Add(1, new BountyDefinition(5, "Imps in the lands of Veridia.", new[] { typeof(Imp) }, 5, 0, 5, 500, "Veridia"));
        Add(1, new BountyDefinition(6, "Mongbats in the New Player Dungeon.", new[] { typeof(Mongbat) }, 5, 0, 5, 500, "New Player Dungeon"));
        Add(1, new BountyDefinition(7, "Giant Rats in the New Player Dungeon.", new[] { typeof(GiantRat) }, 5, 0, 5, 500, "New Player Dungeon"));
        Add(1, new BountyDefinition(8, "Frail Skeletons in the New Player Dungeon.", new[] { typeof(FrailSkeleton) }, 5, 0, 5, 500, "New Player Dungeon"));
        Add(1, new BountyDefinition(9, "Ratmen in the New Player Dungeon.", new[] { typeof(MangyRatman), typeof(SicklyRatmanMage), typeof(BlindRatmanArcher) }, 5, 0, 5, 500, "New Player Dungeon"));
        Add(1, new BountyDefinition(10, "Orcs in the New Player Dungeon.", new[] { typeof(FeebleOrc), typeof(OrcGrunt), typeof(WeakOrcishMage) }, 5, 0, 5, 500, "New Player Dungeon"));

        // ==========================================
        // TIER 2: Estimable Hunter (Fame 1250) - Rewards: 15 Tokens, 1200g
        // ==========================================
        Add(2, new BountyDefinition(11, "Earth Elementals in the Dungeon Shame", new[] { typeof(EarthElemental) }, 10, 1250, 15, 1200, "Shame"));
        Add(2, new BountyDefinition(12, "Drakes in the Dungeon Destard", new[] { typeof(Drake) }, 10, 1250, 15, 1200, "Destard"));
        Add(2, new BountyDefinition(13, "Lizardmen in the Dungeon Despise", new[] { typeof(Lizardman) }, 10, 1250, 15, 1200, "Despise"));
        Add(2, new BountyDefinition(14, "Ghosts in the Dungeon Deceit", new[] { typeof(Spectre), typeof(Shade), typeof(Wraith), typeof(Ghoul) }, 10, 1250, 15, 1200, "Deceit"));
        Add(2, new BountyDefinition(15, "Harpies in the Dungeon Covetous", new[] { typeof(Harpy) }, 10, 1250, 15, 1200, "Covetous"));
        Add(2, new BountyDefinition(16, "Hell Hounds in the Dungeon Destard", new[] { typeof(HellHound) }, 10, 1250, 15, 1200, "Hythloth"));
        Add(2, new BountyDefinition(17, "Ratmen in the Dungeon of Ice", new[] { typeof(Ratman), typeof(RatmanMage), typeof(RatmanArcher) }, 10, 1250, 15, 1200, "Ice"));

        // ==========================================
        // TIER 3: Great Slayer (Fame 2500) - Rewards: 30 Tokens, 2500g
        // ==========================================
        Add(3, new BountyDefinition(18, "Cyclops in the Dungeon Despise", new[] { typeof(Cyclops) }, 8, 2500, 30, 2500, "Despise"));
        Add(3, new BountyDefinition(19, "Dread Spiders in the Dungeon Covetous", new[] { typeof(DreadSpider) }, 8, 2500, 30, 2500, "Covetous"));
        Add(3, new BountyDefinition(20, "Liches in the Dungeon Deceit", new[] { typeof(Lich) }, 8, 2500, 30, 2500, "Deceit"));
        Add(3, new BountyDefinition(21, "Dragons in the Dungeon Destard", new[] { typeof(Dragon) }, 8, 2500, 30, 2500, "Destard"));
        Add(3, new BountyDefinition(22, "Daemons in the Dungeon Hythloth", new[] { typeof(Daemon) }, 8, 2500, 30, 2500, "Hythloth"));
        Add(3, new BountyDefinition(23, "Arctic Ogre Lords in the Dungeon of Ice", new[] { typeof(ArcticOgreLord) }, 8, 2500, 30, 2500, "Ice"));
        Add(3, new BountyDefinition(24, "Fire Elementals in the Dungeon Shame", new[] { typeof(FireElemental) }, 8, 2500, 30, 2500, "Shame"));

        // ==========================================
        // TIER 4: Illustrious Champion (Fame 5000) - Rewards: 60 Tokens, 5000g
        // ==========================================
        Add(4, new BountyDefinition(25, "Shadow Wyrms in the Dungeon Destard", new[] { typeof(ShadowWyrm) }, 3, 5000, 60, 5000, "Destard"));
        Add(4, new BountyDefinition(26, "Rotting Corpses in the Dungeon Covetous", new[] { typeof(RottingCorpse) }, 3, 5000, 60, 5000, "Covetous"));
        Add(4, new BountyDefinition(27, "Ancient Liches in the Dungeon Deceit", new[] { typeof(AncientLich) }, 3, 5000, 60, 5000, "Deceit"));
        Add(4, new BountyDefinition(28, "Titans in the Dungeon Despise", new[] { typeof(Titan) }, 3, 5000, 60, 5000, "Despise"));
        Add(4, new BountyDefinition(29, "Fire Gaargoyles in the Dungeon Hythloth", new[] { typeof(FireGargoyle) }, 3, 5000, 60, 5000, "Hythloth"));
        Add(4, new BountyDefinition(30, "White Wyrms in the Dungeon of Ice", new[] { typeof(WhiteWyrm) }, 3, 5000, 60, 5000, "Ice"));
        Add(4, new BountyDefinition(31, "Blood Elementals in the Dungeon Shame", new[] { typeof(BloodElemental) }, 3, 5000, 60, 5000, "Shame"));

        // ==========================================
        // TIER 5: Glorious Lord (Fame 10000) - Rewards: 100 Tokens, 10000g
        // ==========================================
        Add(5, new BountyDefinition(32, "Balrons in the Dungeon Hythloth", new[] { typeof(Balron) }, 2, 10000, 100, 10000, "Hythloth"));
        Add(5, new BountyDefinition(33, "Ancient Wyrms in the Dungeon Destard", new[] { typeof(AncientWyrm) }, 2, 10000, 100, 10000, "Destard"));
        Add(5, new BountyDefinition(34, "Ancient Liches in the Dungeon Deceit", new[] { typeof(AncientLich) }, 2, 10000, 100, 10000, "Deceit"));
        Add(5, new BountyDefinition(35, "Poison Elementals in the Dungeon Shame", new[] { typeof(PoisonElemental) }, 2, 10000, 100, 10000, "Shame"));
        Add(5, new BountyDefinition(36, "Skeletal Dragons in the Dungeon Covetous", new[] { typeof(SkeletalDragon) }, 2, 10000, 100, 10000, "Covetous"));
        Add(5, new BountyDefinition(37, "Ice Fiends in the Dugneon of Ice", new[] { typeof(IceFiend) }, 2, 10000, 100, 10000, "Ice"));
        Add(5, new BountyDefinition(38, "Titan Lord in the Dungeon of Despise", new[] { typeof(TitanLord) }, 2, 10000, 100, 10000, "Despise"));
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
