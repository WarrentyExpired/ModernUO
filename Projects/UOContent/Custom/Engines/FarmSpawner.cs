using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Engines.Farming
{
    public static class FarmSpawner
    {
        private static Timer _timer;

        private static readonly Rectangle2D[] m_WheatPlot = { new(795, 771, 8, 8), };
        private static readonly Type[] m_WheatCrop = { typeof(FarmableWheat)};

        private static readonly Rectangle2D[] m_CottonPlot = { new(805, 771, 8, 8), };
        private static readonly Type[] m_CottonCrop = { typeof(FarmableCotton)};

        private static readonly Rectangle2D[] m_FlaxPlot = { new(796, 779, 8, 8), };
        private static readonly Type[] m_FlaxCrop = { typeof(FarmableFlax)};

        private static readonly Rectangle2D[] m_PumpkinPlot = { new(805, 779, 7, 8), };
        private static readonly Type[] m_PumpkinCrop = { typeof(FarmablePumpkin)};

        private static readonly Rectangle2D[] m_BloodmossPlot = { new(820, 843, 5, 8), };
        private static readonly Type[] m_BloodmossCrop = { typeof(FarmableBloodmoss)};

        private static readonly Rectangle2D[] m_MandrakeRootPlot = { new(826, 843, 5, 8), };
        private static readonly Type[] m_MandrakeRootCrop = { typeof(FarmableMandrakeRoot)};

        private static readonly Rectangle2D[] m_SpidersSilkPlot = { new( 832, 843, 5, 8), };
        private static readonly Type[] m_SpiderSilkCrop = { typeof(FarmableSpidersSilk)};

        private static readonly Rectangle2D[] m_SulfurousAshPlot = { new( 838, 843, 6, 8), };
        private static readonly Type[] m_SulfurousAshCrop = { typeof(FarmableSulfurousAsh)};

        private static readonly Rectangle2D[] m_GinsengPlot = { new( 820, 852, 5, 8), };
        private static readonly Type[] m_GinsengCrop = {typeof(FarmableGinseng)};

        private static readonly Rectangle2D[] m_BlackPearlPlot = { new( 826, 852, 5, 8), };
        private static readonly Type[] m_BlackPearlCrop = { typeof(FarmableBlackPearl)};

        private static readonly Rectangle2D[] m_GarlicPlot = { new( 832, 852, 5, 8), };
        private static readonly Type[] m_GarlicCrop = { typeof(FarmableGarlic)};

        private static readonly Rectangle2D[] m_NightshadePlot = { new( 838, 852, 6, 8), };
        private static readonly Type[] m_NightshadeCrop = { typeof(FarmableNightshade)};

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
            ProcessRegion(m_BloodmossPlot, m_BloodmossCrop);
            ProcessRegion(m_MandrakeRootPlot, m_MandrakeRootCrop);
            ProcessRegion(m_SpidersSilkPlot, m_SpiderSilkCrop);
            ProcessRegion(m_SulfurousAshPlot, m_SulfurousAshCrop);
            ProcessRegion(m_GinsengPlot, m_GinsengCrop);
            ProcessRegion(m_BlackPearlPlot, m_BlackPearlCrop);
            ProcessRegion(m_GarlicPlot, m_GarlicCrop);
            ProcessRegion(m_NightshadePlot, m_NightshadeCrop);
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
