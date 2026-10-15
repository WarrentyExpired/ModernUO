using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Items;

namespace Server.Commands
{
    public class TrapWipeCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("WipeTraps", AccessLevel.GameMaster, WipeTraps_OnCommand);
        }

        [Usage("WipeTraps")]
        [Description("Deletes all physical floor trap items across the world.")]
        private static void WipeTraps_OnCommand(CommandEventArgs e)
        {
            List<Item> traps = new List<Item>();

            foreach (Item item in World.Items.Values)
            {
                if (item is BaseTrap)
                {
                    traps.Add(item);
                }
            }

            int count = traps.Count;

            for (int i = 0; i < traps.Count; i++)
            {
                traps[i].Delete();
            }

            e.Mobile.SendMessage(0x35, $"Purged {count} physical floor traps from the world.");
        }
    }
}
