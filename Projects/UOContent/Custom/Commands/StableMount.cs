using System;
using System.Collections.Generic;
using System.Reflection; // Required for bypassing the private setter
using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Items;

namespace Server.Commands
{
    public class StableMountCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("StableMount", AccessLevel.Player, new CommandEventHandler(StableMount_OnCommand));
        }

        [Usage("StableMount")]
        [Description("Forces a player off their mount and bypasses stable limits for zone control.")]
        public static void StableMount_OnCommand(CommandEventArgs e)
        {
            if (e.Mobile is PlayerMobile pm)
            {
                ForceDismountAndStore(pm);
            }
        }

        public static void ForceDismountAndStore(PlayerMobile pm)
        {
            if (pm == null || !pm.Mounted)
                return;

            IMount mount = pm.Mount;

            if (mount == null)
                return;

            // Modern C# Pattern Matching
            if (mount is BaseMount baseMount && baseMount is BaseCreature mountCreature)
            {
                baseMount.Rider = null;

                mountCreature.ControlTarget = null;
                mountCreature.ControlOrder = OrderType.Stay;
                mountCreature.Internalize();
                mountCreature.SetControlMaster(null);
                mountCreature.SummonMaster = null;
                mountCreature.IsStabled = true;

                // --- THE FIX: Bypass the private setter using Reflection ---
                if (pm.Stabled == null)
                {
                    var prop = typeof(PlayerMobile).GetProperty("Stabled");
                    prop?.GetSetMethod(true)?.Invoke(pm, new object[] { new HashSet<Mobile>() });
                }

                pm.Stabled.Add(mountCreature);

                //pm.SendMessage("Mounts are not allowed here! Your mount has been tucked away.");
            }
            else
            {
                mount.Rider = null;
                pm.SendMessage("Ethereal mounts are not allowed in this zone.");
            }
        }
    }
}
