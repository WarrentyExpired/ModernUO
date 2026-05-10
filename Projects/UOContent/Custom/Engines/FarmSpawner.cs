using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Engines.Farming
{
    public static class FarmSpawner
    {
        private static Timer _timer;

        // Thalassa Farm Plots
        private static readonly Rectangle2D[] m_ThalassaPlot1A = { new(669, 1111, 9, 9), };
        private static readonly Type[] m_ThalassaCrops1A = { typeof(FarmableCotton)};

        private static readonly Rectangle2D[] m_ThalassaPlot1B = { new(679, 1111, 9, 9), };
        private static readonly Type[] m_ThalassaCrops1B = { typeof(FarmableFlax)};

        private static readonly Rectangle2D[] m_ThalassaPlot1C = { new(669, 1121, 9, 9), };
        private static readonly Type[] m_ThalassaCrops1C = { typeof(FarmableCarrot)};

        private static readonly Rectangle2D[] m_ThalassaPlot1D = { new(679, 1121, 9, 9), };
        private static readonly Type[] m_ThalassaCrops1D = { typeof(FarmableCabbage)};

        private static readonly Rectangle2D[] m_ThalassaPlot2A = { new(731, 1138, 12, 7), };
        private static readonly Type[] m_ThalassaCrops2A = { typeof(FarmableWheat)};

        private static readonly Rectangle2D[] m_ThalassaPlot2B = { new(731, 1145, 12, 7), };
        private static readonly Type[] m_ThalassaCrops2B = { typeof(FarmableOnion)};

        private static readonly Rectangle2D[] m_ThalassaPlot3A = { new(750, 1138, 12, 7), };
        private static readonly Type[] m_ThalassaCrops3A = { typeof(FarmablePumpkin)};

        private static readonly Rectangle2D[] m_ThalassaPlot3B = { new(750, 1146, 12, 7), };
        private static readonly Type[] m_ThalassaCrops3B = { typeof(FarmableTurnip)};

        //Theomara Plots
        private static readonly Rectangle2D[] m_TheomaraPlot1A = { new(560, 922, 9, 6), };
        private static readonly Type[] m_TheomaraCrops1A = { typeof(FarmableBloodmoss)};

        private static readonly Rectangle2D[] m_TheomaraPlot1B = { new(560, 931, 9, 6), };
        private static readonly Type[] m_TheomaraCrops1B = { typeof(FarmableNightshade)};

        private static readonly Rectangle2D[] m_TheomaraPlot2A = { new(578, 922, 9, 6), };
        private static readonly Type[] m_TheomaraCrops2A = { typeof(FarmableBlackPearl)};

        private static readonly Rectangle2D[] m_TheomaraPlot2B = { new(578, 932, 9, 6), };
        private static readonly Type[] m_TheomaraCrops2B = { typeof(FarmableGarlic)};

        private static readonly Rectangle2D[] m_TheomaraPlot3A = { new(605, 922, 9, 6), };
        private static readonly Type[] m_TheomaraCrops3A = { typeof(FarmableGinseng)};

        private static readonly Rectangle2D[] m_TheomaraPlot3B = { new(605, 933, 9, 6), };
        private static readonly Type[] m_TheomaraCrops3B = { typeof(FarmableMandrakeRoot)};

        private static readonly Rectangle2D[] m_TheomaraPlot4A = { new(623, 922, 9, 6), };
        private static readonly Type[] m_TheomaraCrops4A = { typeof(FarmableSpidersSilk)};

        private static readonly Rectangle2D[] m_TheomaraPlot4B = { new(623, 933, 9, 6), };
        private static readonly Type[] m_TheomaraCrops4B = { typeof(FarmableSulfurousAsh)};

        private static readonly Rectangle2D[] m_TheomaraPlot5 = { new(560, 944, 12, 9), };
        private static readonly Type[] m_TheomaraCrops5 = { typeof(FarmableCotton)};

        private static readonly Rectangle2D[] m_TheomaraPlot6 = { new(578, 944, 12, 9), };
        private static readonly Type[] m_TheomaraCrops6 = { typeof(FarmableFlax)};

        private static readonly Rectangle2D[] m_TheomaraPlot7 = { new(644, 101, 7, 3), };
        private static readonly Type[] m_TheomaraCrops7 = { typeof(FarmableNightshade)};

        public static void Initialize()
        {
            _timer = Timer.DelayCall(TimeSpan.FromMinutes(10.0), TimeSpan.FromMinutes(10.0), OnTick);
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
            ProcessRegion(m_TheomaraPlot1A, m_TheomaraCrops1A);
            ProcessRegion(m_TheomaraPlot1B, m_TheomaraCrops1B);
            ProcessRegion(m_TheomaraPlot2A, m_TheomaraCrops2A);
            ProcessRegion(m_TheomaraPlot2B, m_TheomaraCrops2B);
            ProcessRegion(m_TheomaraPlot3A, m_TheomaraCrops3A);
            ProcessRegion(m_TheomaraPlot3B, m_TheomaraCrops3B);
            ProcessRegion(m_TheomaraPlot4A, m_TheomaraCrops4A);
            ProcessRegion(m_TheomaraPlot4B, m_TheomaraCrops4B);
            ProcessRegion(m_TheomaraPlot5, m_TheomaraCrops5);
            ProcessRegion(m_TheomaraPlot6, m_TheomaraCrops6);
            ProcessRegion(m_TheomaraPlot7, m_TheomaraCrops7);
        }

        private static void ProcessRegion(Rectangle2D[] fields, Type[] cropTypes)
        {
            Map map = Map.Ilshenar;

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
