using System;
using ModernUO.Serialization;

namespace Server.Items;

// ==============================================================
// LEVEL 4: 99% Trapped / 99% Locked (1% Lottery Ticket!)
// ==============================================================
[SerializationGenerator(0, false)]
public partial class RandomLootBag : Bag
{
    private static void GetRandomAOSStats(out int attributeCount, out int min, out int max)
    {
        var rnd = Utility.Random(15);

         if (rnd < 1)
        {
            attributeCount = Utility.RandomMinMax(2, 5);
            min = 20;
            max = 70;
        }
        else if (rnd < 3)
        {
            attributeCount = Utility.RandomMinMax(2, 4);
            min = 20;
            max = 50;
        }
        else if (rnd < 6)
        {
            attributeCount = Utility.RandomMinMax(2, 3);
            min = 20;
            max = 40;
        }
        else if (rnd < 10)
        {
            attributeCount = Utility.RandomMinMax(1, 2);
            min = 10;
            max = 30;
        }
        else
        {
            attributeCount = 1;
            min = 10;
            max = 20;
        }
    }

    private static void GetRandomResists(out int phy, out int fire, out int cold, out int poison, out int energy)
    {
        var phyR = Utility.Random(1, 15);
        phy = phyR;
        var fireR = Utility.Random(1, 15);
        fire = fireR;
        var coldR = Utility.Random(1, 15);
        cold = coldR;
        var poisonR = Utility.Random(1, 15);
        poison = poisonR;
        var energyR = Utility.Random(1, 15);
        energy = energyR;
    }

    [Constructible]
    public RandomLootBag()
    {
        for (var i = Utility.Random(10) + 1; i > 0; i--)
        {
            var weapon = Loot.RandomWeapon();
            GetRandomAOSStats(out var attributeCount, out var min, out var max);
            BaseRunicTool.ApplyAttributesTo(weapon, attributeCount, min, max);
            DropItem(weapon);
        }

        for ( var i = Utility.Random(10) + 1; i > 0; i--)
        {
            var armor = Loot.RandomArmorOrShield();
            GetRandomAOSStats(out var attributeCount, out var min, out var max);
            BaseRunicTool.ApplyAttributesTo(armor, attributeCount, min, max);
            GetRandomResists(out var phy, out var fire, out var cold, out var poison, out var energy);
            armor.PhysicalBonus = phy;
            armor.FireBonus = fire;
            armor.ColdBonus = cold;
            armor.PoisonBonus = poison;
            armor.EnergyBonus = energy;
            DropItem(armor);
        }

        for (var i = Utility.Random(10) + 1; i > 0; i--)
        {
            var jewelry = Loot.RandomJewelry();
            GetRandomAOSStats(out var attributeCount, out var min, out var max);
            BaseRunicTool.ApplyAttributesTo(jewelry, attributeCount, min, max);
            DropItem(jewelry);
        }
    }

}
