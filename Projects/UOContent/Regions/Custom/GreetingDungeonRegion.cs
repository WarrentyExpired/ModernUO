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

        public override void OnEnter(Mobile m)
        {
            base.OnEnter(m);
                        if (m is PlayerMobile pm)
            {
                if (pm.Mounted)
                {
                    Server.Commands.StableMountCommand.ForceDismountAndStore(pm);
                    pm.SendMessage("Your mount waits outside.");
                }
            }
            if (m.Player)
            {
                m.SendMessage($"You entered <BASEFONT COLOR='#FF0000'>{this.Name}</BASEFONT>.");
            }
        }

        public override void OnExit(Mobile m)
        {
            base.OnExit(m);

            if (m.Player)
            {
                m.SendMessage($"You have left <BASEFONT COLOR='#FF0000'>{this.Name}</BASEFONT>.");
            }
            if (m is PlayerMobile { Alive: true } pm)
            {
                Server.Commands.RetrieveMountCommand.AutoRemountPlayer(pm);
                if (pm.Mounted)
                {
                    pm.SendMessage("Your mount has returned to you.");
                }
            }
        }
    }
}
