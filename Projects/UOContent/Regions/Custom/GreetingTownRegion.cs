using Server;
using Server.Mobiles;

namespace Server.Regions
{
    public class GreetingTownRegion : TownRegion
    {
        public GreetingTownRegion(string name, Map map, int priority, params Rectangle3D[] area)
            : base(name, map, priority, area)
        {
        }

        public override void OnEnter(Mobile m)
        {
            base.OnEnter(m);

            if (m.Player)
            {
                m.SendMessage($"Welcome to <BASEFONT COLOR='#3BB143'>{this.Name}</BASEFONT>.");
            }
        }

        public override void OnExit(Mobile m)
        {
            base.OnExit(m);
        }
    }
}
