using ModernUO.CodeGeneratedEvents;
using Server.Custom.ExileHunterBestiary.Hooks;
using Server.Mobiles;

namespace Server.Custom.ExileHunterBestiary;

public static class HunterCreatureDeathEvent
{
    [OnEvent(nameof(CreatureEvents.CreatureDeathEvent))]
    public static void OnCreatureDeath(BaseCreature creature)
    {
        if (creature == null)
        {
            return;
        }

        HunterDeathHook.OnCreatureDeath(
            creature,
            creature.LastKiller
        );
    }
}
