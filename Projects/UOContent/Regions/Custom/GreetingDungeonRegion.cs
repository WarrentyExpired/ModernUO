using Server;
using Server.Mobiles;

namespace Server.Regions
{
    public class GreetingDungeonRegion : DungeonRegion
    {
        public GreetingDungeonRegion(string name, Map map, int priority, params Rectangle3D[] area)
            : base(name, map, priority, area)
        {
        }
        public override bool MountsAllowed => false;
        public override void OnEnter(Mobile m)
        {
            base.OnEnter(m);
            if (m.Player)
            {
                m.SendMessage($"You entered <BASEFONT COLOR='#FF0000'>{this.Name}</BASEFONT>.");
            }
            if (m is PlayerMobile player && player.Mounted )
            {
                if ( player.Mount is BaseMount )
                {
                    Server.Custom.AutoStable.StoreMount(player);
                }
                else
                {
                    BaseMount.Dismount(player);
                    player.SendMessage("Mounts are not permitted in this Dungeon.");
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
