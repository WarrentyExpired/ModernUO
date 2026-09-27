using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Engines.Farming
{
    public static class FarmSpawner
    {
        private static Timer _timer;

        private static readonly Rectangle2D[] m_FlaxPlot1 = { new(432, 1853, 10, 9), };
        private static readonly Type[] m_FlaxCrop1 = { typeof(FarmableFlax)};

        private static readonly Rectangle2D[] m_CottonPlot1 = { new(408, 1862, 9, 9), };
        private static readonly Type[] m_CottonCrop1 = { typeof(FarmableCotton)};

        private static readonly Rectangle2D[] m_WheatPlot1 = { new(474, 1839, 6, 9), };
        private static readonly Type[] m_WheatCrop1 = { typeof(FarmableWheat)};

        private static readonly Rectangle2D[] m_TurnipPlot1 = { new(1534, 2904, 5, 5), };
        private static readonly Type[] m_TurnipCrop1 = { typeof(FarmableTurnip)};

        private static readonly Rectangle2D[] m_CabbagePlot1 = { new(1606, 2894, 8, 12), };
        private static readonly Type[] m_CabbageCrop1 = { typeof(FarmableCabbage)};

        private static readonly Rectangle2D[] m_CarrotPlot1 = { new(2170, 2104, 4, 7), };
        private static readonly Type[] m_CarrotCrop1 = { typeof(FarmableCarrot)};

        private static readonly Rectangle2D[] m_OnionPlot1 = { new(2175, 2104, 4, 7), };
        private static readonly Type[] m_OnionCrop1 = { typeof(FarmableOnion)};

        private static readonly Rectangle2D[] m_LettucePlot1 = { new(2170, 2112, 4, 7), };
        private static readonly Type[] m_LettuceCrop1 = { typeof(FarmableLettuce)};

        private static readonly Rectangle2D[] m_PumpkinPlot1 = { new(2175, 2112, 4, 7), };
        private static readonly Type[] m_PumpkinCrop1 = { typeof(FarmablePumpkin)};

        public static void Initialize()
        {
            _timer = Timer.DelayCall(TimeSpan.FromMinutes(1.0), TimeSpan.FromMinutes(1.0), OnTick);
        }

        private static void OnTick()
        {
            ProcessRegion(m_FlaxPlot1, m_FlaxCrop1);
            ProcessRegion(m_CottonPlot1, m_CottonCrop1);
            ProcessRegion(m_WheatPlot1, m_WheatCrop1);
            ProcessRegion(m_TurnipPlot1, m_TurnipCrop1);
            ProcessRegion(m_CabbagePlot1, m_CabbageCrop1);
            ProcessRegion(m_CarrotPlot1, m_CarrotCrop1);
            ProcessRegion(m_OnionPlot1, m_OnionCrop1);
            ProcessRegion(m_LettucePlot1, m_LettuceCrop1);
            ProcessRegion(m_PumpkinPlot1, m_PumpkinCrop1);
        }

        private static void ProcessRegion(Rectangle2D[] fields, Type[] cropTypes)
        {
            Map map = Map.Trammel;

            foreach (var rect in fields)
            {
                int maxSpawn = (rect.Width * rect.Height) / 2;
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
