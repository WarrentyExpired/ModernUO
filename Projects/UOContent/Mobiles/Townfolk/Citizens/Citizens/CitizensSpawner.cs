using System;
using Server;
using Server.Mobiles;
using Server.Items;

namespace Server.Custom.CitizenSpawns
{
    public enum Terrain
    {
        None,
        Dirt,
        Water,
        Cave,
        Rock
    }

    public static class CitizenTerrainList
    {
        private static readonly Terrain[] _terrainLookup = new Terrain[0x4000];

        static CitizenTerrainList()
        {
            foreach (int id in new[] {
                0x008D, 0x008E, 0x008F, 0x0090, 0x0091, 0x0092, 0x0093, 0x0094, 0x0095, 0x0096,
                0x0097, 0x0098, 0x0099, 0x009A, 0x009B, 0x009C, 0x009D, 0x009E, 0x009F, 0x00A0,
                0x00A1, 0x00A2, 0x00A3, 0x00A4, 0x00A5, 0x00A6, 0x00A7, 0x00DC, 0x00DD, 0x00DE,
                0x00DF, 0x00E0, 0x00E1, 0x00E2, 0x00E3, 0x02D0, 0x02D1, 0x02D2, 0x02D3, 0x02D4,
                0x02D5, 0x02D6, 0x02D7, 0x02E5, 0x02E6, 0x02E7, 0x02E8, 0x02E9, 0x02EA, 0x02EB,
                0x02EC, 0x02ED, 0x02EE, 0x02EF, 0x02F0, 0x02F1, 0x02F2, 0x02F3, 0x02F4, 0x02F5,
                0x02F6, 0x02F7, 0x02F8, 0x02F9, 0x02FA, 0x02FB, 0x02FC, 0x02FD, 0x02FE, 0x02FF,
                0x0303, 0x0304, 0x0305, 0x0306, 0x0307, 0x0308, 0x0309, 0x030A, 0x030B, 0x030C,
                0x030D, 0x030E, 0x030F, 0x0310, 0x0311, 0x0312, 0x0313, 0x0314, 0x0315, 0x0316,
                0x0317, 0x0318, 0x0319, 0x031A, 0x031B, 0x031C, 0x031D, 0x031E, 0x031F, 0x06F4,
                0x0777, 0x0778, 0x0779, 0x077A, 0x077B, 0x077C, 0x077D, 0x077E, 0x077F, 0x0780,
                0x0781, 0x0782, 0x0783, 0x0784, 0x0785, 0x0786, 0x0787, 0x0788, 0x0789, 0x078A,
                0x078B, 0x078C, 0x078D, 0x078E, 0x078F, 0x0790, 0x0791, 0x0071, 0x0072, 0x0073,
                0x0074, 0x0075, 0x0076, 0x0077, 0x0078, 0x0079, 0x007A, 0x007B, 0x007C, 0x0082,
                0x0083, 0x0085, 0x0086, 0x0087, 0x0088, 0x0089, 0x008A, 0x008B, 0x008C, 0x00E8,
                0x00E9, 0x00EA, 0x00EB, 0x0141, 0x0142, 0x0143, 0x0144, 0x014C, 0x014D, 0x014E,
                0x014F, 0x0169, 0x016A, 0x016B, 0x016C, 0x016D, 0x016E, 0x016F, 0x0170, 0x0171,
                0x0172, 0x0173, 0x0174, 0x01DC, 0x01DD, 0x01DE, 0x01DF, 0x01E0, 0x01E1, 0x01E2,
                0x01E3, 0x01E4, 0x01E5, 0x01E6, 0x01E7, 0x01EC, 0x01ED, 0x01EE, 0x01EF, 0x0272,
                0x0273, 0x0274, 0x0275, 0x027E, 0x027F, 0x0280, 0x0281, 0x032C, 0x032D, 0x032E,
                0x032F, 0x033D, 0x033E, 0x033F, 0x0340, 0x0345, 0x0346, 0x0347, 0x0348, 0x0349,
                0x034A, 0x034B, 0x034C, 0x0355, 0x0356, 0x0357, 0x0358, 0x0367, 0x0368, 0x0369,
                0x036A, 0x036B, 0x036C, 0x036D, 0x036E, 0x0377, 0x0378, 0x0379, 0x037A, 0x038D,
                0x038E, 0x038F, 0x0390, 0x0395, 0x0396, 0x0397, 0x0398, 0x0399, 0x039A, 0x039B,
                0x039C, 0x03A5, 0x03A6, 0x03A7, 0x03A8, 0x03F6, 0x03F7, 0x03F9, 0x03FA, 0x03FB,
                0x03FC, 0x03FD, 0x03FE, 0x03FF, 0x0400, 0x0401, 0x0402, 0x0403, 0x0404, 0x0405,
                0x0547, 0x0548, 0x0549, 0x054A, 0x054B, 0x054C, 0x054D, 0x054E, 0x0553, 0x0554,
                0x0555, 0x0556, 0x0597, 0x0598, 0x0599, 0x059A, 0x059B, 0x059C, 0x059D, 0x059E,
                0x0623, 0x0624, 0x0625, 0x0626, 0x0627, 0x0628, 0x0629, 0x062A, 0x062B, 0x062C,
                0x062D, 0x062E, 0x062F, 0x0630, 0x0631, 0x0632, 0x0633, 0x0634, 0x0635, 0x0636,
                0x0637, 0x0638, 0x0639, 0x063A, 0x06F3, 0x06F5, 0x06F6, 0x06F7, 0x06F8, 0x06F9,
                0x06FA, 0x03E9, 0x03EA, 0x03EB, 0x03EC, 0x0443
            })
            {
                _terrainLookup[id] = Terrain.Dirt;
            }
        }

        private static readonly Type[] Citizens = new Type[] { typeof(Citizen), typeof(MonsterCitizen) };

        public static Terrain GetTerrain(Map map, Point3D location, int x, int y)
        {
            if (map == Map.Internal || x < 0 || y < 0 || x >= map.Width || y >= map.Height)
                return Terrain.None;

            LandTile lTile = map.Tiles.GetLandTile(x, y);

            if (lTile.ID < _terrainLookup.Length)
            {
                Terrain t = _terrainLookup[lTile.ID];
                if (t != Terrain.None) return t;
            }

            return Terrain.None;
        }

        public static Terrain TestAround(Terrain ground, Map map, int x, int y, int distance)
        {
            Point3D p = new Point3D(9000, 9000, 0);
            Terrain terrain = Terrain.None;
            int tries = 8;

            while (tries > 0)
            {
                tries--;
                int g = Utility.Random(8);

                terrain = g switch
                {
                    0 => GetTerrain(map, p, x - distance, y - distance),
                    1 => GetTerrain(map, p, x, y - distance),
                    2 => GetTerrain(map, p, x + distance, y - distance),
                    3 => GetTerrain(map, p, x - distance, y),
                    4 => GetTerrain(map, p, x + distance, y),
                    5 => GetTerrain(map, p, x - distance, y + distance),
                    6 => GetTerrain(map, p, x, y + distance),
                    7 => GetTerrain(map, p, x + distance, y + distance),
                    _ => Terrain.None
                };

                if (terrain != Terrain.Dirt)
                    return terrain;
            }

            return ground;
        }

        public static Mobile RandomSpawn(Item spawn, Point3D loc, Map map)
        {
            Mobile m = null;
            spawn.MoveToWorld(loc, map);
            int tries = 100;

            Terrain terrain = GetTerrain(map, loc, spawn.X, spawn.Y);

            if (terrain != Terrain.Water && terrain != Terrain.Cave && terrain != Terrain.Dirt && terrain != Terrain.Rock)
                terrain = TestAround(terrain, map, spawn.X, spawn.Y, 15);

            while (m == null && tries > 0)
            {
                tries--;

                m = spawn switch
                {
                    SpawnCitizen => terrain switch
                    {
                        Terrain.Dirt => Construct(Citizens),
                         _ => null
                    },
                    _ => null
                };
            }

            return m;
        }

        public static Mobile Construct(Type[] types)
        {
            if (types.Length > 0)
            {
                Type typeToSpawn = Utility.RandomDouble() < 0.70 ? typeof(Citizen) : typeof(MonsterCitizen);
                try
                {
                    return Activator.CreateInstance(typeToSpawn) as Mobile;
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }
    }
}
