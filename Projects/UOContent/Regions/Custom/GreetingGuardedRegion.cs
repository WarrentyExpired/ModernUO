using Server;
using Server.Mobiles;

namespace Server.Regions
{
    public class GreetingGuardedRegion : GuardedRegion
    {
        public GreetingGuardedRegion(string name, Map map, int priority, params Rectangle3D[] area)
            : base(name, map, priority, area)
        {
        }

        public override void OnEnter(Mobile m)
        {
            base.OnEnter(m);

            if (m.Player)
            {
                m.SendMessage(0x59, $"You have entered the protected zone of {this.Name}.");
            }
        }

        public override void OnExit(Mobile m)
        {
            base.OnExit(m);

            if (m.Player)
            {
                m.SendMessage(0x59, $"You are no longer under the protection of {this.Name}.");
            }
        }
    }
}
