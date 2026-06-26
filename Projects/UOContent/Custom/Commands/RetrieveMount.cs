using System;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Commands
{
    public class RetrieveMountCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("RetrieveMount", AccessLevel.Player, new CommandEventHandler(RetrieveMount_OnCommand));
        }

        [Usage("RetrieveMount")]
        [Description("Automatically retrieves your last stabled mount and remounts you upon exiting a restricted zone.")]
        public static void RetrieveMount_OnCommand(CommandEventArgs e)
        {
            if (e.Mobile is PlayerMobile pm)
            {
                AutoRemountPlayer(pm);
            }
        }

        public static void AutoRemountPlayer(PlayerMobile pm)
        {
            if (pm == null || pm.Deleted || pm.Map == null || pm.Map == Map.Internal || pm.Mounted)
                return;

            if (pm.Stabled == null)
                return;

            BaseCreature mountToRetrieve = null;

            // ModernUO uses a HashSet, so we use a foreach loop to scan it
            foreach (Mobile m in pm.Stabled)
            {
                // Pattern matching makes this super clean
                if (m is BaseCreature { Deleted: false } pet && pet is IMount)
                {
                    mountToRetrieve = pet;
                    break;
                }
            }

            if (mountToRetrieve != null)
            {
                pm.Stabled.Remove(mountToRetrieve);
                mountToRetrieve.SetControlMaster(pm);
                mountToRetrieve.IsStabled = false;
                mountToRetrieve.MoveToWorld(pm.Location, pm.Map);

                if (mountToRetrieve is IMount mount)
                {
                    mount.Rider = pm;
                    //pm.SendMessage("Welcome back! Your mount has been returned to you.");
                }
            }
        }
    }
}
