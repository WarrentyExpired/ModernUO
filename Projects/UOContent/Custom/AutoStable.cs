using Server.Mobiles;

namespace Server.Utilities
{
    public static class AutoStable
    {
        public static void HandleEntry(Mobile m)
        {
            if (m is PlayerMobile pm && pm.Mounted && pm.Mount is Mobile mountPet)
            {
                pm.Mount.Rider = null;
                if (mountPet is BaseCreature bc)
                {
                    bc.Internalize();
                    bc.IsStabled = true;
                    pm.AutoStabled.Add(bc);

                    pm.SendMessage(0x22, "Your mount is waiting safely for you to return.");
                    pm.PlaySound(0x124);
                }
            }
        }

        public static void HandleExit(Mobile m)
        {
            if (m is PlayerMobile pm && pm.AutoStabled != null && pm.AutoStabled.Count > 0)
            {
                BaseCreature mountToClaim = null;

                if (!pm.Alive)
                {
                    return;
                }

                foreach (Mobile pet in pm.AutoStabled)
                {
                    if (pet is BaseMount bm)
                    {
                        mountToClaim = bm;
                        break;
                    }
                }

                if (mountToClaim != null)
                {
                    mountToClaim.ControlMaster = pm;
                    mountToClaim.Controlled = true;
                    mountToClaim.ControlOrder = OrderType.Follow;
                    mountToClaim.MoveToWorld(pm.Location, pm.Map);
                    mountToClaim.IsStabled = false;
                    mountToClaim.ControlOrder = OrderType.Follow;

                    if (!pm.Mounted && mountToClaim is BaseMount bm)
                    {
                        bm.Rider = pm;
                        pm.SendMessage(0x35, "You remount your companion as you return to safety.");
                    }
                    else
                    {
                        pm.SendMessage(0x35, "Your companion returns to your side.");
                    }

                    pm.AutoStabled.Remove(mountToClaim);
                }
            }
        }
    }
}
