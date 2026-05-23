using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Engines.Farming
{
    public static class FarmSpawner
    {
        private static Timer _timer;

        private static readonly Rectangle2D[] m_FlaxPlot = { new(5558, 730, 7, 5), };
        private static readonly Type[] m_FlaxCrop = { typeof(FarmableFlax)};

        private static readonly Rectangle2D[] m_CottonPlot = { new(5558, 735, 7, 5), };
        private static readonly Type[] m_CottonCrop = { typeof(FarmableCotton)};

        private static readonly Rectangle2D[] m_WheatPlot = { new(5567, 730, 7, 5), };
        private static readonly Type[] m_WheatCrop = { typeof(FarmableWheat)};

        private static readonly Rectangle2D[] m_PumpkinPlot = { new(5567, 735, 7, 5), };
        private static readonly Type[] m_PumpkinCrop = { typeof(FarmablePumpkin)};

        public static void Initialize()
        {
            _timer = Timer.DelayCall(TimeSpan.FromMinutes(1.0), TimeSpan.FromMinutes(1.0), OnTick);
        }

        private static void OnTick()
        {
            ProcessRegion(m_FlaxPlot, m_FlaxCrop);
            ProcessRegion(m_CottonPlot, m_CottonCrop);
            ProcessRegion(m_PumpkinPlot, m_PumpkinCrop);
            ProcessRegion(m_WheatPlot, m_WheatCrop);
        }

        private static void ProcessRegion(Rectangle2D[] fields, Type[] cropTypes)
        {
            Map map = Map.Trammel;

            foreach (var rect in fields)
            {
                // CHANGE THIS LINE:
                // / 9 is very sparse. / 2 or / 3 will fill the field significantly more.
                int maxSpawn = (rect.Width * rect.Height) / 2;
                int currentCount = 0;

                foreach (Item item in map.GetItemsInBounds(rect))
                {
                    if (item is FarmableCrop)
                        currentCount++;
                }

                if (currentCount < maxSpawn)
                {
                    // We calculate how many we need to add to reach the new density
                    int toSpawn = maxSpawn - currentCount;

                    for (int i = 0; i < toSpawn; i++)
                    {
                        Type type = cropTypes[Utility.Random(cropTypes.Length)];
                        Item crop = Activator.CreateInstance(type) as Item;

                        if (crop != null)
                        {
                            Point3D loc = Utility.RandomPointIn(rect, map);
                            loc.Z = map.GetAverageZ(loc.X, loc.Y);

                            // Final check to make sure we don't stack crops on the exact same tile
                            bool occupied = false;
                            foreach (Item existing in map.GetItemsInRange(loc, 0))
                            {
                                if (existing is FarmableCrop)
                                {
                                    occupied = true;
                                    break;
                                }
                            }

                            if (!occupied)
                                crop.MoveToWorld(loc, map);
                            else
                                crop.Delete(); // Clean up if the random spot was taken
                        }
                    }
                }
            }
        }
    }
}
