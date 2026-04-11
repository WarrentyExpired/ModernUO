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
                m.SendMessage(0x22, $"Warning: You have entered the territory of {this.Name}!");
            }
        }

        public override void OnExit(Mobile m)
        {
            base.OnExit(m);

            if (m.Player)
            {
                m.SendMessage(0x22, $"The sails of {this.Name} fade into the distance.");
            }
        }

        public override bool AllowHousing(Mobile from, Point3D p) => false;
    }
}
