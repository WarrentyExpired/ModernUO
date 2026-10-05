using System;
using Server;
using ModernUO.Serialization;

namespace Server.Items
{
    [SerializationGenerator(0, false)]
    public partial class WaterSkin : BaseBeverage
    {
        [Constructible]
        public WaterSkin() : base(BeverageType.Water)
        {
            Name = "a waterskin";
        }

        public override double DefaultWeight => 1.0;

        public override int MaxQuantity => 15;

        public override bool ShowQuantity => false;

        public override int ComputeItemID()
        {
            return 0x098F;
        }

        public override void GetProperties(IPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, $"Uses remaining\t{Quantity}");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!ValidateUse(from, true))
                return;

            if (TryRefill(from))
                return;

            if (IsEmpty)
            {
                from.SendMessage("It is empty. You need to find a water trough or river to refill it.");
                return;
            }

            Drink(from);
        }

        private bool TryRefill(Mobile from)
        {
            if (Quantity >= MaxQuantity)
                return false;

            bool foundWater = false;

            foreach (Item item in from.GetItemsInRange(2))
            {
                if (item is IWaterSource || item is BaseWaterContainer || (item is AddonComponent ac && ac.Addon is IWaterSource))
                {
                    foundWater = true;
                    break;
                }
            }

            if (!foundWater && from.Map != null && from.Map != Map.Internal)
            {
                for (int x = -2; x <= 2 && !foundWater; x++)
                {
                    for (int y = -2; y <= 2 && !foundWater; y++)
                    {
                        int checkX = from.X + x;
                        int checkY = from.Y + y;

                        LandTile landTile = from.Map.Tiles.GetLandTile(checkX, checkY);
                        if ((TileData.LandTable[landTile.ID & TileData.MaxLandValue].Flags & TileFlag.Wet) != 0)
                        {
                            foundWater = true;
                        }
                        else
                        {
                            foreach (StaticTile staticTile in from.Map.Tiles.GetStaticTiles(checkX, checkY))
                            {
                                if ((TileData.ItemTable[staticTile.ID & TileData.MaxItemValue].Flags & TileFlag.Wet) != 0)
                                {
                                    foundWater = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            if (foundWater)
            {
                Content = BeverageType.Water;
                Quantity = MaxQuantity;

                from.SendLocalizedMessage(1010089); // "You fill the container with water."
                from.PlaySound(0x240); // Standard splashing refill sound
                return true;
            }

            return false;
        }

        private void Drink(Mobile from)
        {
            if (from.Thirst >= 20)
            {
                from.SendMessage("You are not thirsty right now.");
                return;
            }

            from.Thirst += 1;

            from.PlaySound(Utility.RandomList(0x30, 0x2D6)); // Standard drinking sounds
            from.SendMessage("You take a refreshing drink of water.");

            Quantity--;
        }
    }
}
