using System;
using Server.Mobiles;

namespace Server.Custom.ExileHunterBestiary.Hooks;

public static class HunterDamageHook
{
    public static long ApplyCount { get; private set; }

    public static void Apply(BaseCreature victim, Mobile attacker, ref int damage)
    {
        ApplyCount++;

        if (victim == null || attacker == null || damage <= 0)
        {
            return;
        }

        var master = attacker is BaseCreature { Controlled: true, ControlMaster: { } controlMaster }
            ? controlMaster
            : attacker;

        if (master is PlayerMobile &&
            HunterBestiaryEngine.TryGetEffectiveBonus(master, victim, out var bonus))
        {
            var scaled = (int)Math.Round(damage * (1.0 + bonus));
            damage = Math.Max(damage + 1, scaled);
        }

        if (victim.Hits > 0 && victim.Hits <= damage)
        {
            var creature = victim;
            var killer = attacker;

            Timer.DelayCall(TimeSpan.FromMilliseconds(100), () =>
            {
                if (creature.Deleted || creature.Hits > 0)
                {
                    return;
                }

                HunterDeathHook.OnCreatureDeath(creature, killer);
            });
        }
    }
}