using System;
using Server;
using Server.Commands;
using Server.Items;

namespace Server.Custom
{
    public static class DailyDungeonManager
    {
        public static string ActiveDungeon { get; private set; }

        private static readonly string[] DungeonRoster =
        {
            "the Dungeon Deceit", "the Dungeon Despise", "the Dungeon Destard",
            "the Dungeon Covetous", "the Dungeon Shame", "the Dungeon Hythloth",
            "the Dungeon of Ice"
        };

        public static void Initialize()
        {
            CommandSystem.Register("dotd", AccessLevel.Player, DotD_OnCommand);
            CommandSystem.Register("ForceDungeonSwap", AccessLevel.GameMaster, ForceSwap_OnCommand);
            RotateDungeon();
            Timer.DelayCall(TimeSpan.FromHours(24), TimeSpan.FromHours(24), RotateDungeon);
        }

        public static bool IsActive(Region region)
        {
            if (region == null || string.IsNullOrEmpty(ActiveDungeon))
            {
                return false;
            }

            Region current = region;
            while (current != null)
            {
                if (current.Name == ActiveDungeon)
                {
                    return true;
                }

                current = current.Parent;
            }

            if (region.Name != null && region.Name.StartsWith(ActiveDungeon))
            {
                return true;
            }

            return false;
        }
        public static void RotateDungeon()
        {
            string oldDungeon = ActiveDungeon;
            string newDungeon;

            do
            {
                newDungeon = DungeonRoster[Utility.Random(DungeonRoster.Length)];
            }
            while (newDungeon == oldDungeon && DungeonRoster.Length > 1);

            ActiveDungeon = newDungeon;

            World.Broadcast(1161, true, $"The magical energies of the world have shifted. {ActiveDungeon} is now the Dungeon of the Day!");
        }

        private static readonly Func<Server.Item>[] DailyRares = new Func<Server.Item>[]
        {
            // --- Cosmetics & Status ---
            () => new ClothingBlessDeed(),
            () => new SpecialHairDye(),
            () => new SpecialBeardDye(),
            () => new LeatherDyeTub(),
            () => new FurnitureDyeTub(),
            () => new BlackDyeTub(),
            () => new MetallicLeatherDyeTub(),
            () => new RunebookDyeTub(),
            () => new SpecialDyeTub(),
            () => new WhiteClothDyeTub(),
            () => new WhiteLeatherDyeTub(),

            // --- Wealth & Adventure ---
            // Forces a Level 5 (Devious) Treasure Map
            () => new TreasureMap(5, Map.Trammel),
            // A fat bank check for 5,000 gold
            () => new BankCheck(5000),

            // --- Classic UOR Deco Rares ---
            () => new DecoGoldIngot(),
            () => new DecoGoldIngot2(),
            () => new DecoGoldIngots(),
            () => new DecoGoldIngots2(),
            () => new DecoGoldIngots3(),
            () => new DecoGoldIngots4(),
            () => new DecoIronIngot(),
            () => new DecoIronIngot2(),
            () => new DecoIronIngots(),
            () => new DecoIronIngots2(),
            () => new DecoIronIngots3(),
            () => new DecoIronIngots4(),
            () => new DecoIronIngots5(),
            () => new DecoIronIngots6(),
            () => new DecoSilverIngot(),
            () => new DecoSilverIngot2(),
            () => new DecoSilverIngots(),
            () => new DecoSilverIngots2(),
            () => new DecoSilverIngots3(),
            () => new DecoSilverIngots4(),
            () => new DecoSilverIngots5(),
            () => new Bucket(),
            () => new ClosedBarrel(),
            () => new WaterBarrel(),
            () => new DecoCrystalBall(),
            () => new DecoMagicalCrystal(),
            () => new DecoHay(),
            () => new DecoHay2(),
            () => new DecoHorseDung(),
            () => new EmptyToolKit(),
            () => new Lockpicks(),

            // --- Rare Resources ---
            // Drops exactly 50 Valorite Ingots
            () => new ValoriteIngot(50),
            () => new VeriteIngot(50),
            () => new AgapiteIngot(50),
            () => new GoldIngot(50),

            // --- Weaons and Armor ---
            () =>
            {
                var halberd = new Halberd();
                halberd.DamageLevel = WeaponDamageLevel.Vanq;
                halberd.DurabilityLevel = WeaponDurabilityLevel.Indestructible;
                return halberd;
            }

        };

        public static Server.Item GetRandomRare()
        {
            Func<Server.Item> createItem = DailyRares[Utility.Random(DailyRares.Length)];
            return createItem();
        }
        [Usage("ForceDungeonSwap")]
        [Description("Forces the Dungeon of the Day to rotate immediately.")]
        private static void ForceSwap_OnCommand(CommandEventArgs e)
        {
            RotateDungeon();
            e.Mobile.SendMessage("You have forced the daily dungeon to rotate.");
        }

        [Usage("dotd")]
        [Description("Displays the current Dungeon of the Day and its active bonuses.")]
        private static void DotD_OnCommand(CommandEventArgs e)
        {
            Mobile m = e.Mobile;

            if (string.IsNullOrEmpty(ActiveDungeon))
            {
                m.SendMessage(38, "The magical energies are still shifting. There is no active dungeon today.");
                return;
            }

            string displayName = ActiveDungeon;
            /*if (displayName.StartsWith("the Dungeon "))
            {
                displayName = displayName.Substring(12);
            }*/

            m.SendMessage(1161, $"*** Dungeon of the Day: {displayName} ***");
            m.SendMessage(259, "Active Bonuses: Increased Skill gains, 50% more gold, 25% more Monster Kill  Points, and a chance for rare items!");
        }
    }
}
