using System;
using Server;
using Server.Commands;

namespace Server.Custom
{
    public static class DailyDungeonManager
    {
        public static string ActiveDungeon { get; private set; }

        private static readonly string[] DungeonRoster =
        {
            "the Dungeon Deceit", "the Dungeon Despise", "the Dungeon Destard",
            "the Dungeon Covetous", "the Dungeon Shame", "the Dungeon Hythloth"
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
            if (displayName.StartsWith("the Dungeon "))
            {
                displayName = displayName.Substring(12);
            }

            m.SendMessage(1161, $"*** Dungeon of the Day: {displayName} ***");
            m.SendMessage(259, "Active Bonuses: Increased Skill gains, 50% more gold, 25% more Monster Kill  Points, and a chance for rare items!");
        }
    }
}
