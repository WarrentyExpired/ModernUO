namespace Server.Custom.ExileHunterBestiary;

internal static class BestiaryData
{
    public static void RegisterAll()
    {

        BestiaryRegistry.Register("Skeleton", "Skeleton", "Skeleton", BestiaryCategory.Undead, 2101, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("SkeletalMage", "Skeletal Mage", "SkeletalMage", BestiaryCategory.Undead, 2101, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("SkeletalKnight", "Skeletal Knight", "SkeletalKnight", BestiaryCategory.Undead, 2101, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("PatchworkSkeleton", "Patchwork Skeleton", "PatchworkSkeleton", BestiaryCategory.Undead, 2101, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Zombie", "Zombie", "Zombie", BestiaryCategory.Undead, 1175, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Ghoul", "Ghoul", "Ghoul", BestiaryCategory.Undead, 1175, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Shade", "Shade", "Shade", BestiaryCategory.Undead, 1150, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Spectre", "Spectre", "Spectre", BestiaryCategory.Undead, 1150, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Wraith", "Wraith", "Wraith", BestiaryCategory.Undead, 1150, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Mummy", "Mummy", "Mummy", BestiaryCategory.Undead, 2305, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("PestilentBandage", "Pestilent Bandage", "PestilentBandage", BestiaryCategory.Undead, 1150, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("BoneKnight", "Bone Knight", "BoneKnight", BestiaryCategory.Undead, 2101, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("BoneMagi", "Bone Magi", "BoneMagi", BestiaryCategory.Undead, 2101, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("RottingCorpse", "Rotting Corpse", "RottingCorpse", BestiaryCategory.Undead, 1175, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Lich", "Lich", "Lich", BestiaryCategory.Undead, 1150, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("LichLord", "Lich Lord", "LichLord", BestiaryCategory.Undead, 1153, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("AncientLich", "Ancient Lich", "AncientLich", BestiaryCategory.Undead, 1175, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("SkeletalDragon", "Skeletal Dragon", "SkeletalDragon", BestiaryCategory.Undead, 1175, 0.03);
        BestiaryRegistry.Register("BoneDemon", "Bone Demon", "BoneDemon", BestiaryCategory.Undead, 2101, 0.04);
        BestiaryRegistry.Register("UndeadGargoyle", "Undead Gargoyle", "UndeadGargoyle", BestiaryCategory.Undead, 1175, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("PutridUndeadGargoyle", "Putrid Undead Gargoyle", "PutridUndeadGargoyle", BestiaryCategory.Undead, 1175, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("WailingBanshee", "Wailing Banshee", "WailingBanshee", BestiaryCategory.Undead, 1153, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("GoreFiend", "Gore Fiend", "GoreFiend", BestiaryCategory.Undead, 1175, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("FleshGolem", "Flesh Golem", "FleshGolem", BestiaryCategory.Undead, 1175, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Gibberling", "Gibberling", "Gibberling", BestiaryCategory.Undead, 1175, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Rotworm", "Rotworm", "Rotworm", BestiaryCategory.Undead, 2215, HunterBestiaryEngine.DefaultDropChance);


        BestiaryRegistry.Register("Daemon", "Daemon", "Daemon", BestiaryCategory.Daemons, 1258, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("FireDaemon", "Fire Daemon", "FireDaemon", BestiaryCategory.Daemons, 1260, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("IceFiend", "Ice Fiend", "IceFiend", BestiaryCategory.Daemons, 1152, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Succubus", "Succubus", "Succubus", BestiaryCategory.Daemons, 1166, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Balron", "Balron", "Balron", BestiaryCategory.Daemons, 1255, 0.03);
        BestiaryRegistry.Register("ArcaneDaemon", "Arcane Daemon", "ArcaneDaemon", BestiaryCategory.Daemons, 1165, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("ChaosDaemon", "Chaos Daemon", "ChaosDaemon", BestiaryCategory.Daemons, 1167, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Moloch", "Moloch", "Moloch", BestiaryCategory.Daemons, 1257, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Ravager", "Ravager", "Ravager", BestiaryCategory.Daemons, 1258, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Devourer", "Devourer of Souls", "Devourer", BestiaryCategory.Daemons, 1153, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Imp", "Imp", "Imp", BestiaryCategory.Daemons, 1255, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Gargoyle", "Gargoyle", "Gargoyle", BestiaryCategory.Daemons, 2419, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("StoneGargoyle", "Stone Gargoyle", "StoneGargoyle", BestiaryCategory.Daemons, 2413, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("FireGargoyle", "Fire Gargoyle", "FireGargoyle", BestiaryCategory.Daemons, 1260, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("GargoyleEnforcer", "Gargoyle Enforcer", "GargoyleEnforcer", BestiaryCategory.Daemons, 2419, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("GargoyleDestroyer", "Gargoyle Destroyer", "GargoyleDestroyer", BestiaryCategory.Daemons, 2419, HunterBestiaryEngine.DefaultDropChance);


        BestiaryRegistry.Register("Dragon", "Dragon", "Dragon", BestiaryCategory.Reptiles, 1365, 0.03);
        BestiaryRegistry.Register("Drake", "Drake", "Drake", BestiaryCategory.Reptiles, 1362, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("WhiteWyrm", "White Wyrm", "WhiteWyrm", BestiaryCategory.Reptiles, 1150, 0.03);
        BestiaryRegistry.Register("ShadowWyrm", "Shadow Wyrm", "ShadowWyrm", BestiaryCategory.Reptiles, 1109, 0.03);
        BestiaryRegistry.Register("AncientWyrm", "Ancient Wyrm", "AncientWyrm", BestiaryCategory.Reptiles, 1370, 0.04);
        BestiaryRegistry.Register("GreaterDragon", "Greater Dragon", "GreaterDragon", BestiaryCategory.Reptiles, 1360, 0.04);
        BestiaryRegistry.Register("SwampDragon", "Swamp Dragon", "SwampDragon", BestiaryCategory.Reptiles, 2212, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Hydra", "Hydra", "Hydra", BestiaryCategory.Reptiles, 1368, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("SerpentineDragon", "Serpentine Dragon", "SerpentineDragon", BestiaryCategory.Reptiles, 1160, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Reptalon", "Reptalon", "Reptalon", BestiaryCategory.Reptiles, 1372, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Lizardman", "Lizardman", "Lizardman", BestiaryCategory.Reptiles, 2210, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("OphidianWarrior", "Ophidian Warrior", "OphidianWarrior", BestiaryCategory.Reptiles, 2001, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("OphidianMage", "Ophidian Mage", "OphidianMage", BestiaryCategory.Reptiles, 2002, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("OphidianMatriarch", "Ophidian Matriarch", "OphidianMatriarch", BestiaryCategory.Reptiles, 2003, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Slith", "Slith", "Slith", BestiaryCategory.Reptiles, 2212, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("ToxicSlith", "Toxic Slith", "ToxicSlith", BestiaryCategory.Reptiles, 1267, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("StoneSlith", "Stone Slith", "StoneSlith", BestiaryCategory.Reptiles, 2413, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("LavaLizard", "Lava Lizard", "LavaLizard", BestiaryCategory.Reptiles, 1260, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("LavaSerpent", "Lava Serpent", "LavaSerpent", BestiaryCategory.Reptiles, 1260, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("LavaSnake", "Lava Snake", "LavaSnake", BestiaryCategory.Reptiles, 1260, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("IceSerpent", "Ice Serpent", "IceSerpent", BestiaryCategory.Reptiles, 1152, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("IceSnake", "Ice Snake", "IceSnake", BestiaryCategory.Reptiles, 1152, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("SilverSerpent", "Silver Serpent", "SilverSerpent", BestiaryCategory.Reptiles, 2301, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("GiantSerpent", "Giant Serpent", "GiantSerpent", BestiaryCategory.Reptiles, 2210, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Alligator", "Alligator", "Alligator", BestiaryCategory.Reptiles, 2210, HunterBestiaryEngine.DefaultDropChance);


        BestiaryRegistry.Register("EarthElemental", "Earth Elemental", "EarthElemental", BestiaryCategory.Elementals, 2419, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("AirElemental", "Air Elemental", "AirElemental", BestiaryCategory.Elementals, 1153, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("FireElemental", "Fire Elemental", "FireElemental", BestiaryCategory.Elementals, 1260, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("WaterElemental", "Water Elemental", "WaterElemental", BestiaryCategory.Elementals, 1165, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("PoisonElemental", "Poison Elemental", "PoisonElemental", BestiaryCategory.Elementals, 1267, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("BloodElemental", "Blood Elemental", "BloodElemental", BestiaryCategory.Elementals, 1258, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("SnowElemental", "Snow Elemental", "SnowElemental", BestiaryCategory.Elementals, 1150, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("IceElemental", "Ice Elemental", "IceElemental", BestiaryCategory.Elementals, 1152, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Efreet", "Efreet", "Efreet", BestiaryCategory.Elementals, 1260, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("LavaElemental", "Lava Elemental", "LavaElemental", BestiaryCategory.Elementals, 1260, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("SandVortex", "Sand Vortex", "SandVortex", BestiaryCategory.Elementals, 2419, HunterBestiaryEngine.DefaultDropChance);


        BestiaryRegistry.Register("Ogre", "Ogre", "Ogre", BestiaryCategory.Humanoids, 2215, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("OgreLord", "Ogre Lord", "OgreLord", BestiaryCategory.Humanoids, 2216, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("ArcticOgreLord", "Arctic Ogre Lord", "ArcticOgreLord", BestiaryCategory.Humanoids, 1150, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Ettin", "Ettin", "Ettin", BestiaryCategory.Humanoids, 2214, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Troll", "Troll", "Troll", BestiaryCategory.Humanoids, 2213, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Titan", "Titan", "Titan", BestiaryCategory.Humanoids, 1153, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Cyclops", "Cyclops", "Cyclops", BestiaryCategory.Humanoids, 2217, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Orc", "Orc", "Orc", BestiaryCategory.Humanoids, 2208, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("OrcishLord", "Orcish Lord", "OrcishLord", BestiaryCategory.Humanoids, 2209, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("OrcCaptain", "Orc Captain", "OrcCaptain", BestiaryCategory.Humanoids, 2210, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("OrcishMage", "Orcish Mage", "OrcishMage", BestiaryCategory.Humanoids, 2208, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("OrcBrute", "Orc Brute", "OrcBrute", BestiaryCategory.Humanoids, 2211, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Ratman", "Ratman", "Ratman", BestiaryCategory.Humanoids, 2307, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("RatmanArcher", "Ratman Archer", "RatmanArcher", BestiaryCategory.Humanoids, 2307, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("RatmanMage", "Ratman Mage", "RatmanMage", BestiaryCategory.Humanoids, 2308, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Minotaur", "Minotaur", "Minotaur", BestiaryCategory.Humanoids, 2418, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Centaur", "Centaur", "Centaur", BestiaryCategory.Humanoids, 2215, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Satyr", "Satyr", "Satyr", BestiaryCategory.Humanoids, 2215, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("InsaneDryad", "Insane Dryad", "InsaneDryad", BestiaryCategory.Humanoids, 2210, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Pixie", "Pixie", "Pixie", BestiaryCategory.Humanoids, 1160, HunterBestiaryEngine.DefaultDropChance);


        BestiaryRegistry.Register("GiantSpider", "Giant Spider", "GiantSpider", BestiaryCategory.Arachnids, 2110, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("DreadSpider", "Dread Spider", "DreadSpider", BestiaryCategory.Arachnids, 1175, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("FrostSpider", "Frost Spider", "FrostSpider", BestiaryCategory.Arachnids, 1152, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("GiantBlackWidow", "Giant Black Widow", "GiantBlackWidow", BestiaryCategory.Arachnids, 1109, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("TerathanDrone", "Terathan Drone", "TerathanDrone", BestiaryCategory.Arachnids, 2115, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("TerathanWarrior", "Terathan Warrior", "TerathanWarrior", BestiaryCategory.Arachnids, 2116, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("TerathanMatriarch", "Terathan Matriarch", "TerathanMatriarch", BestiaryCategory.Arachnids, 2117, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Scorpion", "Scorpion", "Scorpion", BestiaryCategory.Arachnids, 2206, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("RuneBeetle", "Rune Beetle", "RuneBeetle", BestiaryCategory.Arachnids, 1165, 0.03);
        BestiaryRegistry.Register("FireBeetle", "Fire Beetle", "FireBeetle", BestiaryCategory.Arachnids, 1260, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Beetle", "Giant Beetle", "Beetle", BestiaryCategory.Arachnids, 2210, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("IronBeetle", "Iron Beetle", "IronBeetle", BestiaryCategory.Arachnids, 2413, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("DeathwatchBeetle", "Deathwatch Beetle", "DeathwatchBeetle", BestiaryCategory.Arachnids, 2215, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("AntLion", "Ant Lion", "AntLion", BestiaryCategory.Arachnids, 2419, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("FireAnt", "Fire Ant", "FireAnt", BestiaryCategory.Arachnids, 1260, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("SkitteringHopper", "Skittering Hopper", "SkitteringHopper", BestiaryCategory.Arachnids, 2210, HunterBestiaryEngine.DefaultDropChance);


        BestiaryRegistry.Register("Gazer", "Gazer", "Gazer", BestiaryCategory.Beasts, 1166, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("ElderGazer", "Elder Gazer", "ElderGazer", BestiaryCategory.Beasts, 1167, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("GazerLarva", "Gazer Larva", "GazerLarva", BestiaryCategory.Beasts, 1166, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("HellHound", "Hell Hound", "HellHound", BestiaryCategory.Beasts, 1258, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("HellCat", "Hell Cat", "HellCat", BestiaryCategory.Beasts, 1258, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("PredatorHellCat", "Predator Hell Cat", "PredatorHellCat", BestiaryCategory.Beasts, 1258, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Nightmare", "Nightmare", "Nightmare", BestiaryCategory.Beasts, 1109, 0.03);
        BestiaryRegistry.Register("CuSidhe", "Cu Sidhe", "CuSidhe", BestiaryCategory.Beasts, 2212, 0.03);
        BestiaryRegistry.Register("Kirin", "Ki-Rin", "Kirin", BestiaryCategory.Beasts, 1160, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Unicorn", "Unicorn", "Unicorn", BestiaryCategory.Beasts, 1150, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Hiryu", "Hiryu", "Hiryu", BestiaryCategory.Beasts, 1365, 0.03);
        BestiaryRegistry.Register("Phoenix", "Phoenix", "Phoenix", BestiaryCategory.Beasts, 1260, 0.04);
        BestiaryRegistry.Register("TimberWolf", "Timber Wolf", "TimberWolf", BestiaryCategory.Beasts, 2215, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("GreyWolf", "Grey Wolf", "GreyWolf", BestiaryCategory.Beasts, 2215, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("WhiteWolf", "White Wolf", "WhiteWolf", BestiaryCategory.Beasts, 1150, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("DireWolf", "Dire Wolf", "DireWolf", BestiaryCategory.Beasts, 2305, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Cougar", "Cougar", "Cougar", BestiaryCategory.Beasts, 2215, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Panther", "Panther", "Panther", BestiaryCategory.Beasts, 1109, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("SnowLeopard", "Snow Leopard", "SnowLeopard", BestiaryCategory.Beasts, 1150, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("GrizzlyBear", "Grizzly Bear", "GrizzlyBear", BestiaryCategory.Beasts, 2215, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("PolarBear", "Polar Bear", "PolarBear", BestiaryCategory.Beasts, 1150, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("BlackBear", "Black Bear", "BlackBear", BestiaryCategory.Beasts, 1109, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Gorilla", "Gorilla", "Gorilla", BestiaryCategory.Beasts, 2210, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Walrus", "Walrus", "Walrus", BestiaryCategory.Beasts, 2419, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("GiantToad", "Giant Toad", "GiantToad", BestiaryCategory.Beasts, 2210, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Bullfrog", "Bullfrog", "BullFrog", BestiaryCategory.Beasts, 2210, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("VampireBat", "Vampire Bat", "VampireBat", BestiaryCategory.Beasts, 1109, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Mongbat", "Mongbat", "Mongbat", BestiaryCategory.Beasts, 2215, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("DarkWisp", "Dark Wisp", "DarkWisp", BestiaryCategory.Beasts, 1109, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("ShadowWisp", "Shadow Wisp", "ShadowWisp", BestiaryCategory.Beasts, 1109, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("GiantIceWorm", "Giant Ice Worm", "GiantIceWorm", BestiaryCategory.Beasts, 1152, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Raptor", "Raptor", "Raptor", BestiaryCategory.Beasts, 2210, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Kepetch", "Kepetch", "Kepetch", BestiaryCategory.Beasts, 2215, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("KepetchAmbusher", "Kepetch Ambusher", "KepetchAmbusher", BestiaryCategory.Beasts, 2215, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("BloodWorm", "Blood Worm", "BloodWorm", BestiaryCategory.Beasts, 1258, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("OsseinRam", "Ossein Ram", "OsseinRam", BestiaryCategory.Beasts, 2101, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Harpy", "Harpy", "Harpy", BestiaryCategory.Beasts, 2215, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("StoneHarpy", "Stone Harpy", "StoneHarpy", BestiaryCategory.Beasts, 2413, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Corpser", "Corpser", "Corpser", BestiaryCategory.Beasts, 2215, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Reaper", "Reaper", "Reaper", BestiaryCategory.Beasts, 2215, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("BogThing", "Bog Thing", "BogThing", BestiaryCategory.Beasts, 2212, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Bogling", "Bogling", "Bogling", BestiaryCategory.Beasts, 2212, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("PlagueBeast", "Plague Beast", "PlagueBeast", BestiaryCategory.Beasts, 1267, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("PlagueBeastLord", "Plague Beast Lord", "PlagueBeastLord", BestiaryCategory.Beasts, 1267, 0.03);
        BestiaryRegistry.Register("Quagmire", "Quagmire", "Quagmire", BestiaryCategory.Beasts, 2212, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Slime", "Slime", "Slime", BestiaryCategory.Beasts, 2210, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("FrostOoze", "Frost Ooze", "FrostOoze", BestiaryCategory.Beasts, 1152, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("SeaSerpent", "Sea Serpent", "SeaSerpent", BestiaryCategory.Beasts, 1165, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("DeepSeaSerpent", "Deep Sea Serpent", "DeepSeaSerpent", BestiaryCategory.Beasts, 1165, HunterBestiaryEngine.DefaultDropChance);
        BestiaryRegistry.Register("Kraken", "Kraken", "Kraken", BestiaryCategory.Beasts, 1165, 0.03);
        BestiaryRegistry.Register("Leviathan", "Leviathan", "Leviathan", BestiaryCategory.Beasts, 1165, 0.05);


        BestiaryRegistry.Register("DemonKnight", "Demon Knight (Dark Father)", "DemonKnight", BestiaryCategory.Bosses, 1161, HunterBestiaryEngine.BossDropChance);
        BestiaryRegistry.Register("DarknightCreeper", "Darknight Creeper", "DarknightCreeper", BestiaryCategory.Bosses, 1161, HunterBestiaryEngine.BossDropChance);
        BestiaryRegistry.Register("FleshRenderer", "Flesh Renderer", "FleshRenderer", BestiaryCategory.Bosses, 1161, HunterBestiaryEngine.BossDropChance);
        BestiaryRegistry.Register("Impaler", "Impaler", "Impaler", BestiaryCategory.Bosses, 1161, HunterBestiaryEngine.BossDropChance);
        BestiaryRegistry.Register("ShadowKnight", "Shadow Knight", "ShadowKnight", BestiaryCategory.Bosses, 1161, HunterBestiaryEngine.BossDropChance);
        BestiaryRegistry.Register("AbysmalHorror", "Abysmal Horror", "AbysmalHorror", BestiaryCategory.Bosses, 1161, HunterBestiaryEngine.BossDropChance);
        BestiaryRegistry.Register("Harrower", "The Harrower", "Harrower", BestiaryCategory.Bosses, 1175, 0.10);
        BestiaryRegistry.Register("SlasherOfVeils", "Slasher of Veils", "SlasherOfVeils", BestiaryCategory.Bosses, 1170, HunterBestiaryEngine.BossDropChance);
        BestiaryRegistry.Register("StygianDragon", "Stygian Dragon", "StygianDragon", BestiaryCategory.Bosses, 1172, HunterBestiaryEngine.BossDropChance);
        BestiaryRegistry.Register("Medusa", "Medusa", "Medusa", BestiaryCategory.Bosses, 1168, HunterBestiaryEngine.BossDropChance);
        BestiaryRegistry.Register("PrimevalLich", "Primeval Lich", "PrimevalLich", BestiaryCategory.Bosses, 1153, HunterBestiaryEngine.BossDropChance);
        BestiaryRegistry.Register("AbyssalInfernal", "Abyssal Infernal", "AbyssalInfernal", BestiaryCategory.Bosses, 1255, HunterBestiaryEngine.BossDropChance);
        BestiaryRegistry.Register("Rikktor", "Rikktor", "Rikktor", BestiaryCategory.Bosses, 1365, HunterBestiaryEngine.BossDropChance);
        BestiaryRegistry.Register("Mephitis", "Mephitis", "Mephitis", BestiaryCategory.Bosses, 1175, HunterBestiaryEngine.BossDropChance);
        BestiaryRegistry.Register("Semidar", "Semidar", "Semidar", BestiaryCategory.Bosses, 1258, HunterBestiaryEngine.BossDropChance);
        BestiaryRegistry.Register("Barracoon", "Barracoon the Piper", "Barracoon", BestiaryCategory.Bosses, 2307, HunterBestiaryEngine.BossDropChance);
        BestiaryRegistry.Register("Neira", "Neira the Necromancer", "Neira", BestiaryCategory.Bosses, 1175, HunterBestiaryEngine.BossDropChance);
        BestiaryRegistry.Register("LordOaks", "Lord Oaks", "LordOaks", BestiaryCategory.Bosses, 2215, HunterBestiaryEngine.BossDropChance);
        BestiaryRegistry.Register("Silvani", "Silvani", "Silvani", BestiaryCategory.Bosses, 1160, HunterBestiaryEngine.BossDropChance);
        BestiaryRegistry.Register("Ilhenir", "Ilhenir the Stained", "Ilhenir", BestiaryCategory.Bosses, 1267, HunterBestiaryEngine.BossDropChance);
        BestiaryRegistry.Register("Serado", "Serado the Scourge", "Serado", BestiaryCategory.Bosses, 1267, HunterBestiaryEngine.BossDropChance);
        BestiaryRegistry.Register("ChiefParoxysmus", "Chief Paroxysmus", "ChiefParoxysmus", BestiaryCategory.Bosses, 1267, HunterBestiaryEngine.BossDropChance);
        BestiaryRegistry.Register("DreadHorn", "Dread Horn", "DreadHorn", BestiaryCategory.Bosses, 1160, HunterBestiaryEngine.BossDropChance);
        BestiaryRegistry.Register("Travesty", "Travesty", "Travesty", BestiaryCategory.Bosses, 1153, HunterBestiaryEngine.BossDropChance);
        BestiaryRegistry.Register("ShimmeringEffusion", "Shimmering Effusion", "ShimmeringEffusion", BestiaryCategory.Bosses, 1152, HunterBestiaryEngine.BossDropChance);
        BestiaryRegistry.Register("MonstrousInterredGrizzle", "Monstrous Interred Grizzle", "MonstrousInterredGrizzle", BestiaryCategory.Bosses, 1175, HunterBestiaryEngine.BossDropChance);
    }
}
