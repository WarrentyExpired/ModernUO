using Server;
using Server.Mobiles;
namespace Server.Regions
{
    public class GreetingBusinessRegion : TownRegion
    {
        public GreetingBusinessRegion(string name, Map map, int priority, params Rectangle3D[] area)
            : base(name, map, priority, area)
        {
        }

        public override void OnEnter(Mobile m)
        {
            base.OnEnter(m);
            if (m.Player)
            {
                m.SendMessage($"Welcome to <BASEFONT COLOR='#FFD700'>{this.Name}</BASEFONT>.");
            }
        }

        public override void OnExit(Mobile m)
        {
            base.OnExit(m);
            if (m.Player)
            {
                m.SendMessage($"You have left <BASEFONT COLOR='#FFD700'>{this.Name}</BASEFONT>.");
            }
        }
    }
}
