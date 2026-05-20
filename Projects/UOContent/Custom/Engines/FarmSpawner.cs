using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Engines.Farming
{
    public static class FarmSpawner
    {
        private static Timer _timer;

        private static readonly Rectangle2D[] m_NightshadePlot = { new(5465, 527, 6, 6), };
        private static readonly Type[] m_NightshadeCrop = { typeof(FarmableNightshade)};

        private static readonly Rectangle2D[] m_MandrakeRootPlot = { new(5465, 536, 6, 6), };
        private static readonly Type[] m_MandrakeRootCrop = { typeof(FarmableMandrakeRoot)};

        private static readonly Rectangle2D[] m_BlackPearlPlot = { new(5465, 545, 6, 6), };
        private static readonly Type[] m_BlackPearlCrop = { typeof(FarmableBlackPearl)};

        private static readonly Rectangle2D[] m_SulfurousAshPlot = { new(5465, 554, 6, 6), };
        private static readonly Type[] m_SulfurousAshCrop = { typeof(FarmableSulfurousAsh)};

        private static readonly Rectangle2D[] m_GarlicPlot = { new(5483, 554, 6, 6), };
        private static readonly Type[] m_GarlicCrop = { typeof(FarmableGarlic)};

        private static readonly Rectangle2D[] m_SpidersSilkPlot = { new(5483, 545, 6, 6), };
        private static readonly Type[] m_SpidersSilkCrop = { typeof(FarmableSpidersSilk)};

        private static readonly Rectangle2D[] m_GinsengPlot = { new(5483, 536, 6, 6), };
        private static readonly Type[] m_GinsengCrop = { typeof(FarmableGinseng)};

        private static readonly Rectangle2D[] m_BloodmossPlot = { new(5483, 527, 6, 6), };
        private static readonly Type[] m_BloodmossCrop = { typeof(FarmableBloodmoss)};

        private static readonly Rectangle2D[] m_FlaxPlot = { new(5474, 536, 6, 6), };
        private static readonly Type[] m_FlaxCrop = { typeof(FarmableFlax)};

        private static readonly Rectangle2D[] m_CottonPlot = { new(5474, 545, 6, 6), };
        private static readonly Type[] m_CottonCrop = { typeof(FarmableCotton)};

        private static readonly Rectangle2D[] m_PumpkinPlot = { new(5474, 554, 6, 6), };
        private static readonly Type[] m_PumpkinCrop = { typeof(FarmablePumpkin)};

        public static void Initialize()
        {
            _timer = Timer.DelayCall(TimeSpan.FromMinutes(5.0), TimeSpan.FromMinutes(5.0), OnTick);
        }

        private static void OnTick()
        {
            ProcessRegion(m_NightshadePlot, m_NightshadeCrop);
            ProcessRegion(m_MandrakeRootPlot, m_MandrakeRootCrop);
            ProcessRegion(m_BlackPearlPlot, m_BlackPearlCrop);
            ProcessRegion(m_SulfurousAshPlot, m_SulfurousAshCrop);
            ProcessRegion(m_GarlicPlot, m_GarlicCrop);
            ProcessRegion(m_SpidersSilkPlot, m_SpidersSilkCrop);
            ProcessRegion(m_GinsengPlot, m_GinsengCrop);
            ProcessRegion(m_BlackPearlPlot, m_BlackPearlCrop);
            ProcessRegion(m_BloodmossPlot, m_BloodmossCrop);
            ProcessRegion(m_FlaxPlot, m_FlaxCrop);
            ProcessRegion(m_CottonPlot, m_CottonCrop);
            ProcessRegion(m_PumpkinPlot, m_PumpkinCrop);
        }

        private static void ProcessRegion(Rectangle2D[] fields, Type[] cropTypes)
        {
            Map map = Map.Trammel;

            foreach (var rect in fields)
            {
                // CHANGE THIS LINE:
                // / 9 is very sparse. / 2 or / 3 will fill the field significantly more.
                int maxSpawn = (rect.Width * rect.Height) / 4;
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
