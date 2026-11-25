namespace Server
{
    public static class Settings
    {
        public static bool S_SaveOnCharacterLogout = true; // Saves on Logout
        public static int S_BoneDecay = 1440; // 10-1440 minutes till bones decay
        public static bool S_LineOfSight = false; // Mobs are hidden in Dungeons
        public static int S_BondDays = 1; // 1-30 Days to bond with pet
        public static int S_Stables = 3; // 0-20 Extra stable slots
        public static int S_DungeonBountyBoardDelay = 60; // 1-1440 minutes delay between bountie quests
        public static int S_DungeonBountyRefusalDelay = 10; // 1-1440 minutes delay between refusing a quest
        public static bool S_RegionDungeonTraps = true; // Step activated dungeon traps
        public static int S_RegionDungeonTrapsChance = 2; // 1-10 Chance to get a trap check per step
        public static bool S_TrapDetected = false; // Get a message if the trap gets detected
        public static bool S_TrapDamageMitigated = true; // Get a message if the trap damage gets mitigated
        public static bool S_StatInfluence = true; // Do stats give skill points.
        public static bool S_InnatesPvP = true; // Allow innate weapon abilities in PvP
        public static bool S_MonsterInnates = true; // Allow monsters/NPCs to proc innate weapon abilities

        // Pre-AOS Melee / Ranged Damage Scalars (UOR default was 0.50 across all three)
        public static double S_PvPMeleeDamageScalar = 0.50;
        public static double S_MonsterMeleeDamageScalar = 0.85;
        public static double S_CreatureVsCreatureDamageScalar = 0.75;

        // Caps effective Str used for monster melee damage scaling
        // (e.g. 250 Str = +50% max instead of +160%-240% on Dragons/Balrons)
        public static int S_MonsterStrDamageCap = 250;

        // Not implimented yet
        public static bool S_TreasureBags = false; // Spawn random treasure bags.
    }
}
