using System;
using System.Collections.Generic;
using Server.Custom.ExileHunterBestiary.Items;
using Server.Items;
using Server.Mobiles;

namespace Server.Custom.ExileHunterBestiary.Hooks;

public static class HunterDeathHook
{
    private static readonly HashSet<Serial> _processedDeaths = [];

    public static string OnCreatureDeath(BaseCreature creature, Mobile killer)
    {
        if (!HunterBestiaryEngine.Enabled)
        {
            return "system disabled";
        }

        if (creature == null)
        {
            return "creature is null";
        }

        if (creature.Summoned || creature.IsBonded)
        {
            return "creature filtered (summoned/bonded)";
        }

        var entry = BestiaryRegistry.FindEntry(creature.GetType());

        if (entry == null)
        {
            return $"NOT REGISTERED: {creature.GetType().FullName}";
        }

        if (killer == null || killer.Deleted)
        {
            killer = creature.LastKiller;
        }

        if (killer == null || killer.Deleted)
        {
            killer = HunterCombatTracker.GetLastHitter(creature);
        }

        if (killer == null || killer.Deleted)
        {
            return $"{entry.Id}: no killer resolved";
        }

        var master = killer;

        if (killer is BaseCreature killerCreature)
        {
            master = killerCreature.GetMaster() ?? killer;
        }

        if (master is not PlayerMobile player)
        {
            HunterCombatTracker.Forget(creature.Serial);
            return $"{entry.Id}: killer is not a player";
        }

        if (!_processedDeaths.Add(creature.Serial))
        {
            return $"{entry.Id}: already processed";
        }

        var location = creature.Location;
        var map = creature.Map;

        if (map == null || map == Map.Internal)
        {
            HunterCombatTracker.TryGetDeathLocation(
                creature,
                out location,
                out map
            );
        }

        HunterCombatTracker.Forget(creature.Serial);

        var profile = HunterBestiaryEngine.GetProfile(player);

        if (profile == null)
        {
            _processedDeaths.Remove(creature.Serial);
            return $"{entry.Id}: profile is null";
        }

        if (profile.HasLearned(entry.Id))
        {
            return $"{entry.Id}: already learned by this player";
        }

        var chance = HunterBestiaryEngine.GlobalDropChance >= 0
            ? HunterBestiaryEngine.GlobalDropChance
            : entry.DropChance;

        chance = Math.Clamp(chance, 0.0, 1.0);

        var pityDrop = profile.RegisterKill(
            entry.Id,
            HunterBestiaryEngine.PityThreshold
        );

        if (!pityDrop && Utility.RandomDouble() >= chance)
        {
            return $"{entry.Id}: rolled above drop chance ({chance:P0})";
        }

        if (map == null || map == Map.Internal)
        {
            _processedDeaths.Remove(creature.Serial);
            return $"{entry.Id}: death location unavailable";
        }

        var treatise = new HunterTreatise(entry.Id);

        var corpse = FindCorpseAt(location, map, creature);

        if (corpse is { Deleted: false })
        {
            corpse.DropItem(treatise);
            return null;
        }

        DropIntoCorpseLater(
            treatise,
            entry.Id,
            creature.Serial,
            location,
            map,
            0
        );

        return null;
    }

    private static void DropIntoCorpseLater(
        HunterTreatise treatise,
        string creatureId,
        Serial creatureSerial,
        Point3D location,
        Map map,
        int attempt
    )
    {
        Timer.DelayCall(
            TimeSpan.FromMilliseconds(100),
            () =>
            {
                if (treatise.Deleted)
                {
                    return;
                }

                var corpse = FindCorpseAt(location, map, null);

                if (corpse is { Deleted: false })
                {
                    corpse.DropItem(treatise);
                    return;
                }

                if (attempt < 10)
                {
                    DropIntoCorpseLater(
                        treatise,
                        creatureId,
                        creatureSerial,
                        location,
                        map,
                        attempt + 1
                    );

                    return;
                }

                treatise.Delete();
                _processedDeaths.Remove(creatureSerial);
            }
        );
    }

    private static Corpse FindCorpseAt(
        Point3D location,
        Map map,
        BaseCreature owner
    )
    {
        if (map == null || map == Map.Internal)
        {
            return null;
        }

        Corpse nearest = null;
        var nearestDistance = int.MaxValue;

        foreach (var item in World.Items.Values)
        {
            if (item is not Corpse corpse ||
                corpse.Deleted ||
                corpse.Map != map)
            {
                continue;
            }

            var corpseLocation = corpse.Location;
            var distance = Math.Max(
                Math.Abs(corpseLocation.X - location.X),
                Math.Abs(corpseLocation.Y - location.Y)
            );

            if (distance > 3)
            {
                continue;
            }

            if (owner != null && corpse.Owner == owner)
            {
                return corpse;
            }

            if (distance < nearestDistance)
            {
                nearest = corpse;
                nearestDistance = distance;
            }
        }

        return nearest;
    }

    public static void ClearProcessedDeath(Serial serial)
    {
        _processedDeaths.Remove(serial);
    }

    public static void ClearProcessedDeaths()
    {
        _processedDeaths.Clear();
    }
}