using Server;
using Server.Mobiles;

namespace Server.Regions
{
    public class GraveyardRegion : BaseRegion
    {
        public GraveyardRegion(string name, Map map, int priority, params Rectangle3D[] area)
            : base(name, map, priority, area)
        {
        }

        public override void OnEnter(Mobile m)
        {
            base.OnEnter(m);

            if (m.Player)
            {
                m.SendMessage(0x482, $"A cold chill runs down your spine as you enter {this.Name}...");
            }
        }

        public override void OnExit(Mobile m)
        {
            base.OnExit(m);

            if (m.Player)
            {
                m.SendMessage(0x482, $"the air feels warmer as you leave the graves behind.");
            }
        }

        public override bool AllowHousing(Mobile from, Point3D p) => false;
    }
}
