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
        public static int S_RegionDungeonTrapsChance = 3; // 1-10 Chance to get a trap check per step
        public static bool S_TrapDetected = false; // Get a message if the trap gets detected
        public static bool S_TrapDamageMitigated = true; // Get a message if the trap damage gets mitigated
        public static bool S_StatInfluence = true; // Do stats give skill points.
        // Not implimented yet
        public static bool S_TreasureBags = false; // Spawn random treasure bags.
    }
}
