using Server;
using Server.Mobiles;

namespace Server.Regions
{
    public class GreetingGraveyardRegion : DungeonRegion
    {
        public GreetingGraveyardRegion(string name, Map map, int priority, params Rectangle3D[] area)
            : base(name, map, priority, area)
        {
        }

        public override void OnEnter(Mobile m)
        {
            base.OnEnter(m);

            // If we are moving from the Parent Town into this Graveyard,
            // the Town's OnExit should already be suppressed by the Parent/Child JSON setting.
            if (m.Player)
            {
                // A ghostly blue/grey color for that "unsettling" vibe
                m.SendMessage(0x47F, $"The air turns deathly cold as you enter {this.Name}.");
            }
        }

        public override void OnExit(Mobile m)
        {
            base.OnExit(m);

            if (m.Player)
            {
                m.SendMessage(0x47F, $"The chill leaves your bones as you exit {this.Name}.");
            }
        }
    }
}
