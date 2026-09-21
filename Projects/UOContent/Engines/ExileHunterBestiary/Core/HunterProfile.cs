using System;
using System.Collections.Generic;

namespace Server.Custom.ExileHunterBestiary;

public sealed class HunterProfile
{
    private readonly Dictionary<string, int> _killCounters = new(StringComparer.OrdinalIgnoreCase);

    public Serial MobileSerial { get; }

    public HashSet<string> LearnedCreatures { get; } = new(StringComparer.OrdinalIgnoreCase);

    public int TotalMastered => LearnedCreatures.Count;

    public HunterProfile(Serial serial) => MobileSerial = serial;

    public bool HasLearned(string creatureId) =>
        !string.IsNullOrEmpty(creatureId) && LearnedCreatures.Contains(creatureId);


    public bool Learn(string creatureId)
    {
        if (string.IsNullOrEmpty(creatureId) || !BestiaryRegistry.TryGetById(creatureId, out _))
        {
            return false;
        }

        return LearnedCreatures.Add(creatureId);
    }

    public int GetCategoryMasteredCount(BestiaryCategory category)
    {
        var count = 0;
        if (BestiaryRegistry.ByCategory.TryGetValue(category, out var list))
        {
            foreach (var entry in list)
            {
                if (LearnedCreatures.Contains(entry.Id))
                {
                    count++;
                }
            }
        }

        return count;
    }





    public int GetKillCount(string creatureId) =>
        _killCounters.TryGetValue(creatureId ?? "", out var kills) ? kills : 0;

    public void SetKillCount(string creatureId, int kills)
    {
        if (!string.IsNullOrEmpty(creatureId) && kills > 0)
        {
            _killCounters[creatureId] = kills;
        }
    }


    public bool RegisterKill(string creatureId, int pityThreshold)
    {
        if (string.IsNullOrEmpty(creatureId) || pityThreshold <= 0)
        {
            return false;
        }

        var kills = GetKillCount(creatureId) + 1;
        if (kills < pityThreshold)
        {
            _killCounters[creatureId] = kills;
            return false;
        }

        _killCounters.Remove(creatureId);
        return true;
    }

    public IEnumerable<KeyValuePair<string, int>> KillCounters => _killCounters;
}
