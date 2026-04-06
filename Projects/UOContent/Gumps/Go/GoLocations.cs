using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Server.Json;
using Server.Logging;

namespace Server.Gumps;

public static class GoLocations
{
    private static readonly ILogger logger = LogFactory.GetLogger(typeof(GoLocations));

    private static LocationTree _world;

    public static LocationTree GetLocations(Map map)
    {
        // Always return the same world tree, regardless of which map 'GetLocations' is called with
        return _world ??= LoadLocations("world", Map.Felucca);
    }

    private static LocationTree LoadLocations(string fileName, Map map)
    {
        var lastBranch = new Dictionary<Mobile, GoCategory>();
        var path = Path.Combine($"Data/Locations/{fileName}.json");

        try
        {
            var root = JsonConfig.Deserialize<GoCategory>(path);
            if (root == null)
            {
                throw new JsonException($"Failed to deserialize {path}.");
            }

            return new LocationTree(map, lastBranch, root);
        }
        catch (Exception e)
        {
            logger.Error(e, "Failed to load file {Path}.", path);
            return new LocationTree(map, lastBranch, new GoCategory { Name = "Error", Categories = [], Locations = [] });
        }
    }
}
