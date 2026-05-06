using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Engines.Farming
{
    public static class FarmSpawner
    {
        private static Timer _timer;

        // Thalassa Farm Plots
        private static readonly Rectangle2D[] m_ThalassaPlot1A = { new(1268, 3406, 10, 10), };
        private static readonly Type[] m_ThalassaCrops1A = { typeof(FarmableCotton)};
        private static readonly Rectangle2D[] m_ThalassaPlot1B = { new(1268, 3416, 10, 10), };
        private static readonly Type[] m_ThalassaCrops1B = { typeof(FarmableFlax)};
        private static readonly Rectangle2D[] m_ThalassaPlot1C = { new(1278, 3406, 10, 10), };
        private static readonly Type[] m_ThalassaCrops1C = { typeof(FarmableCabbage)};
        private static readonly Rectangle2D[] m_ThalassaPlot1D = { new(1278, 3416, 10, 10), };
        private static readonly Type[] m_ThalassaCrops1D = { typeof(FarmableCarrot)};

        private static readonly Rectangle2D[] m_ThalassaPlot2A = { new(1330, 3433, 11, 7), };
        private static readonly Type[] m_ThalassaCrops2A = { typeof(FarmableWheat)};
        private static readonly Rectangle2D[] m_ThalassaPlot2B = { new(1330, 3440, 11, 5), };
        private static readonly Type[] m_ThalassaCrops2B = { typeof(FarmableFlax)};

        private static readonly Rectangle2D[] m_ThalassaPlot3A = { new(1349, 3433, 11, 7), };
        private static readonly Type[] m_ThalassaCrops3A = { typeof(FarmableTurnip)};
        private static readonly Rectangle2D[] m_ThalassaPlot3B = { new(1349, 3440, 11, 5), };
        private static readonly Type[] m_ThalassaCrops3B = { typeof(FarmableCotton)};
        public static void Initialize()
        {
            _timer = Timer.DelayCall(TimeSpan.FromMinutes(1.0), TimeSpan.FromMinutes(1.0), OnTick);
        }

        private static void OnTick()
        {
            ProcessRegion(m_ThalassaPlot1A, m_ThalassaCrops1A);
            ProcessRegion(m_ThalassaPlot1B, m_ThalassaCrops1B);
            ProcessRegion(m_ThalassaPlot1C, m_ThalassaCrops1C);
            ProcessRegion(m_ThalassaPlot1D, m_ThalassaCrops1D);

            ProcessRegion(m_ThalassaPlot2A, m_ThalassaCrops2A);
            ProcessRegion(m_ThalassaPlot2B, m_ThalassaCrops2B);

            ProcessRegion(m_ThalassaPlot3A, m_ThalassaCrops3A);
            ProcessRegion(m_ThalassaPlot3B, m_ThalassaCrops3B);
        }

        private static void ProcessRegion(Rectangle2D[] fields, Type[] cropTypes)
        {
            Map map = Map.Felucca;

            foreach (var rect in fields)
            {
                int maxSpawn = (rect.Width * rect.Height) / 9;
                int currentCount = 0;

                foreach (Item item in map.GetItemsInBounds(rect))
                {
                    if (item is FarmableCrop)
                        currentCount++;
                }

                if (currentCount < maxSpawn)
                {
                    Type type = cropTypes[Utility.Random(cropTypes.Length)];
                    Item crop = Activator.CreateInstance(type) as Item;

                    if (crop != null)
                    {
                        Point3D loc = Utility.RandomPointIn(rect, map);
                        loc.Z = map.GetAverageZ(loc.X, loc.Y);

                        crop.MoveToWorld(loc, map);
                    }
                }
            }
        }
    }
}
