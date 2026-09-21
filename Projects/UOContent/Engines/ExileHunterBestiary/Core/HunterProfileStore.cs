using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Server.Logging;

namespace Server.Custom.ExileHunterBestiary;

public static class HunterProfileStore
{
    private static readonly ILogger _logger = LogFactory.GetLogger(typeof(HunterProfileStore));

    private static string SavePath => Path.Combine(Core.BaseDirectory, "Data", "ExileHunterProfiles.xml");

    public static void Save(IReadOnlyDictionary<Serial, HunterProfile> profiles)
    {
        try
        {
            var dir = Path.GetDirectoryName(SavePath);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var doc = new XmlDocument();
            var root = doc.CreateElement("HunterProfiles");
            doc.AppendChild(root);

            foreach (var (serial, profile) in profiles)
            {
                var hasCounters = false;
                foreach (var (_, kills) in profile.KillCounters)
                {
                    if (kills > 0)
                    {
                        hasCounters = true;
                        break;
                    }
                }

                if (profile.TotalMastered == 0 && !hasCounters)
                {
                    continue;
                }

                var pElem = doc.CreateElement("Profile");
                pElem.SetAttribute("serial", serial.Value.ToString());

                foreach (var id in profile.LearnedCreatures)
                {
                    var cElem = doc.CreateElement("Creature");
                    cElem.SetAttribute("id", id);

                    var kills = profile.GetKillCount(id);
                    if (kills > 0)
                    {
                        cElem.SetAttribute("kills", kills.ToString());
                    }

                    pElem.AppendChild(cElem);
                }

                foreach (var (id, kills) in profile.KillCounters)
                {
                    if (kills <= 0 || profile.HasLearned(id))
                    {
                        continue;
                    }

                    var cElem = doc.CreateElement("Creature");
                    cElem.SetAttribute("id", id);
                    cElem.SetAttribute("kills", kills.ToString());
                    pElem.AppendChild(cElem);
                }

                root.AppendChild(pElem);
            }

            doc.Save(SavePath);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error saving bestiary profiles.");
        }
    }

    public static Dictionary<Serial, HunterProfile> Load()
    {
        var profiles = new Dictionary<Serial, HunterProfile>();

        try
        {
            if (!File.Exists(SavePath))
            {
                return profiles;
            }

            var doc = new XmlDocument();
            doc.Load(SavePath);

            var list = doc.SelectNodes("//HunterProfiles/Profile");
            if (list == null)
            {
                return profiles;
            }

            foreach (XmlElement pElem in list)
            {
                if (!int.TryParse(pElem.GetAttribute("serial"), out var sVal))
                {
                    continue;
                }

                var profile = new HunterProfile((Serial)(uint)sVal);
                var hasData = false;

                var cList = pElem.SelectNodes("Creature");
                if (cList != null)
                {
                    foreach (XmlElement cElem in cList)
                    {
                        var id = cElem.GetAttribute("id");


                        if (!BestiaryRegistry.TryGetById(id, out _))
                        {
                            continue;
                        }

                        profile.LearnedCreatures.Add(id);

                        if (int.TryParse(cElem.GetAttribute("kills"), out var kills) && kills > 0)
                        {
                            profile.SetKillCount(id, kills);
                        }

                        hasData = true;
                    }
                }

                if (hasData)
                {
                    profiles[profile.MobileSerial] = profile;
                }
            }

            _logger.Information("Loaded {Count} bestiary profiles.", profiles.Count);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error loading bestiary profiles.");
        }

        return profiles;
    }
}
