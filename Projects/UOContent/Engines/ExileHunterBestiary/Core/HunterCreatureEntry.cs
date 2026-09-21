using System;

namespace Server.Custom.ExileHunterBestiary;

public sealed record HunterCreatureEntry(
    string Id,
    string DisplayName,
    Type CreatureType,
    BestiaryCategory Category,
    int BookHue = 0,
    double DropChance = HunterBestiaryEngine.DefaultDropChance);
