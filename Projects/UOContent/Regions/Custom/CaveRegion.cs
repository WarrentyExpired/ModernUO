using Server;
using Server.Mobiles;

namespace Server.Regions
{
    public class CaveRegion : DungeonRegion
    {
        public CaveRegion(string name, Map map, int priority, params Rectangle3D[] area)
            : base(name, map, priority, area)
        {
        }

        public override void OnEnter(Mobile m)
        {
            base.OnEnter(m);

            if (m.Player)
            {
                m.SendMessage(0x47E, $"The air grows damp as you enter {this.Name}.");
            }
        }

        public override void OnExit(Mobile m)
        {
            base.OnExit(m);

            if (m.Player)
            {
                m.SendMessage(0x47E, $"You emerge from the darkness of {this.Name}.");
            }
        }
    }
}
