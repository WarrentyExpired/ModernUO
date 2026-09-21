using System;
using System.Collections.Generic;
using Server.Logging;

namespace Server.Custom.ExileHunterBestiary;

public static class BestiaryRegistry
{
    private static readonly ILogger _logger = LogFactory.GetLogger(typeof(BestiaryRegistry));

    private static Dictionary<string, HunterCreatureEntry> _byId = new(StringComparer.OrdinalIgnoreCase);
    private static Dictionary<Type, HunterCreatureEntry> _byType = [];
    private static Dictionary<BestiaryCategory, List<HunterCreatureEntry>> _byCategory = [];



    private static readonly Dictionary<Type, HunterCreatureEntry> _resolutionCache = [];

    public static int TotalCount => _byId.Count;

    public static IReadOnlyDictionary<BestiaryCategory, List<HunterCreatureEntry>> ByCategory => _byCategory;

    public static void Initialize()
    {
        _byId = new Dictionary<string, HunterCreatureEntry>(StringComparer.OrdinalIgnoreCase);
        _byType = new Dictionary<Type, HunterCreatureEntry>();
        _byCategory = new Dictionary<BestiaryCategory, List<HunterCreatureEntry>>();
        _resolutionCache.Clear();

        foreach (var category in Enum.GetValues<BestiaryCategory>())
        {
            _byCategory[category] = [];
        }

        BestiaryData.RegisterAll();

        _logger.Information(
            "Initialized with {Count} creatures across {Categories} categories.",
            _byId.Count, _byCategory.Count
        );
    }

    public static void Register(
        string id, string displayName, string typeName,
        BestiaryCategory category, int bookHue = 0,
        double dropChance = HunterBestiaryEngine.DefaultDropChance)
    {
        var type = AssemblyHandler.FindTypeByName(typeName);

        if (type == null)
        {
            _logger.Warning("Type '{TypeName}' not found - bestiary entry '{Id}' skipped.", typeName, id);
            return;
        }

        if (_byType.ContainsKey(type))
        {
            _logger.Warning("Duplicate type '{TypeName}' - bestiary entry '{Id}' skipped.", typeName, id);
            return;
        }

        var entry = new HunterCreatureEntry(id, displayName, type, category, bookHue, dropChance);
        _byId[id] = entry;
        _byType[type] = entry;
        _byCategory[category].Add(entry);
    }

    public static bool TryGetById(string id, out HunterCreatureEntry entry) =>
        _byId.TryGetValue(id ?? "", out entry);





    public static HunterCreatureEntry FindEntry(Type creatureType)
    {
        if (creatureType == null)
        {
            return null;
        }

        if (_resolutionCache.TryGetValue(creatureType, out var cached))
        {
            return cached;
        }

        HunterCreatureEntry found = null;
        for (var current = creatureType; current != null && current != typeof(object); current = current.BaseType)
        {
            if (_byType.TryGetValue(current, out found))
            {
                break;
            }
        }

        _resolutionCache[creatureType] = found;
        return found;
    }
}
