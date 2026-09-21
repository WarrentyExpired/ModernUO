using Server;
using Server.Mobiles;
using Server.Custom;

namespace Server.Regions
{
    public class MountlessTownRegion : TownRegion
    {
        public MountlessTownRegion(string name, Map map, int priority, params Rectangle3D[] area)
            : base(name, map, priority, area)
        {
        }
        public override bool MountsAllowed => false;
        public override void OnEnter(Mobile m)
        {
            base.OnEnter(m);
            if (m is PlayerMobile player )
            {
                if ( player.Mounted )
                {
                    Server.Custom.AutoStable.StoreMount(player);
                }
            }
        }

        public override void OnExit(Mobile m)
        {
            base.OnExit(m);
            if ( m is PlayerMobile player )
            {
                Server.Custom.AutoStable.RestoreMount(player);
            }
        }
    }
}
