using Server;
using Server.Mobiles;
using Server.Utilities;
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
            AutoStable.HandleEntry(m);
            if (m.Player)
            {
                m.SendMessage(0x35, $"Welcome to {this.Name}.");
            }
        }

        public override void OnExit(Mobile m)
        {
            base.OnExit(m);
            AutoStable.HandleExit(m);
            if (m.Player)
            {
                m.SendMessage(0x35, $"You have left {this.Name}.");
            }
        }
    }
}
