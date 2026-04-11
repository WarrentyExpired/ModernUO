using Server;
using Server.Mobiles;

namespace Server.Regions
{
    public class PirateRegion : BaseRegion
    {
        public PirateRegion(string name, Map map, int priority, params Rectangle3D[] area)
            : base(name, map, priority, area)
        {
        }

        public override void OnEnter(Mobile m)
        {
            base.OnEnter(m);

            if (m.Player)
            {
                m.SendMessage(0x22, $"You have entered {this.Name}!");
            }
        }

        public override void OnExit(Mobile m)
        {
            base.OnExit(m);

            if (m.Player)
            {
                m.SendMessage(0x22, $"You have left {this.Name}.");
            }
        }

        public override bool AllowHousing(Mobile from, Point3D p) => false;
    }
}
