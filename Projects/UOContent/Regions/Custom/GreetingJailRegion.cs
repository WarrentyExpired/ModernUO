using Server;
using Server.Mobiles;

namespace Server.Regions
{
    public class GreetingJailRegion : JailRegion
    {
        public GreetingJailRegion(string name, Map map, int priority, params Rectangle3D[] area)
            : base(name, map, priority, area)
        {
        }
        public override void OnEnter(Mobile m)
        {
            base.OnEnter(m);

            if (m.Player)
            {
                m.SendMessage(0x21, $"You are now imprisoned in {this.Name}. All skills and magic are suppressed.");
            }
        }

        public override void OnExit(Mobile m)
        {
            base.OnExit(m);

            if (m.Player)
            {
                m.SendMessage(0x21, $"You have been released from {this.Name}.");
            }
        }
    }
}
