using System;
using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class ThiefPedestalLootBag : Bag
{
    private static int GetWeightedTier()
    {
        var roll = Utility.Random(100);

        if (roll < 25) return 0;      // 25% chance (Rolls 0-24)
        else if (roll < 55) return 1; // 30% chance (Rolls 25-54)
        else if (roll < 75) return 2; // 20% chance (Rolls 55-74)
        else if (roll < 90) return 3; // 15% chance (Rolls 75-89)
        else if (roll < 96) return 4; //  6% chance (Rolls 90-95)
        else if (roll < 99) return 5; //  3% chance (Rolls 96-98)
        else return 6;                //  1% chance (Rolls 99)
    }

    private static void GetRandomLevel(out int attributeCount, out int min, out int max)
    {
        var pwr = GetWeightedTier();

        if (pwr == 1)
        {
            attributeCount = 1;
            min = 10;
            max = 20;
        }
        else if (pwr == 2)
        {
            attributeCount = Utility.RandomMinMax(1, 2);
            min = 20;
            max = 40;
        }
        else if (pwr == 3)
        {
            attributeCount = Utility.RandomMinMax(2, 3);
            min = 30;
            max = 60;
        }
        else if (pwr == 4)
        {
            attributeCount = Utility.RandomMinMax(2, 4);
            min = 40;
            max = 80;
        }
        else if (pwr == 5)
        {
            attributeCount = Utility.RandomMinMax(3, 5);
            min = 60;
            max = 80;
        }
        else if (pwr == 6)
        {
            attributeCount = Utility.RandomMinMax(4, 5);
            min = 60;
            max = 100;
        }
        else
        {
            attributeCount = 1;
            min = 5;
            max = 10;
        }
    }

    private static void GetRandomResists(out int phy, out int fire, out int cold, out int poison, out int energy)
    {
        var pwr = GetWeightedTier();

        if (pwr == 1)
        {
            phy = Utility.RandomMinMax(1, 3);
            fire = Utility.RandomMinMax(1, 3);
            cold = Utility.RandomMinMax(1, 3);
            poison = Utility.RandomMinMax(1, 3);
            energy = Utility.RandomMinMax(1, 3);
        }
        else if (pwr == 2)
        {
            phy = Utility.RandomMinMax(2, 5);
            fire = Utility.RandomMinMax(2, 5);
            cold = Utility.RandomMinMax(2, 5);
            poison = Utility.RandomMinMax(2, 5);
            energy = Utility.RandomMinMax(2, 5);
        }
        else if (pwr == 3)
        {
            phy = Utility.RandomMinMax(4, 7);
            fire = Utility.RandomMinMax(4, 7);
            cold = Utility.RandomMinMax(4, 7);
            poison = Utility.RandomMinMax(4, 7);
            energy = Utility.RandomMinMax(4, 7);
        }
        else if (pwr == 4)
        {
            phy = Utility.RandomMinMax(6, 10);
            fire = Utility.RandomMinMax(6, 10);
            cold = Utility.RandomMinMax(6, 10);
            poison = Utility.RandomMinMax(6, 10);
            energy = Utility.RandomMinMax(6, 10);
        }
        else if (pwr == 5)
        {
            phy = Utility.RandomMinMax(9, 12);
            fire = Utility.RandomMinMax(9, 12);
            cold = Utility.RandomMinMax(9, 12);
            poison = Utility.RandomMinMax(9, 12);
            energy = Utility.RandomMinMax(9, 12);
        }
        else if (pwr == 6)
        {
            phy = Utility.RandomMinMax(12, 15);
            fire = Utility.RandomMinMax(12, 15);
            cold = Utility.RandomMinMax(12, 15);
            poison = Utility.RandomMinMax(12, 15);
            energy = Utility.RandomMinMax(12, 15);
        }
        else
        {
            phy = 0;
            fire = 0;
            cold = 0;
            poison = 0;
            energy = 0;
        }
    }

    [Constructible]
    public ThiefPedestalLootBag()
    {
        int bagTier = GetWeightedTier();

        int weaponCount = Utility.RandomMinMax(0, bagTier);
        for (var i = 0; i < weaponCount; i++)
        {
            var weapon = Loot.RandomWeapon();
            GetRandomLevel(out var attributeCount, out var min, out var max);
            BaseRunicTool.ApplyAttributesTo(weapon, attributeCount, min, max);
            DropItem(weapon);
        }

        int armorCount = Utility.RandomMinMax(0, bagTier);
        for (var i = 0; i < armorCount; i++)
        {
            var armor = Loot.RandomArmorOrShield();
            GetRandomLevel(out var attributeCount, out var min, out var max);
            BaseRunicTool.ApplyAttributesTo(armor, attributeCount, min, max);
            GetRandomResists(out var phy, out var fire, out var cold, out var poison, out var energy);
            armor.PhysicalBonus = phy;
            armor.FireBonus = fire;
            armor.ColdBonus = cold;
            armor.PoisonBonus = poison;
            armor.EnergyBonus = energy;
            DropItem(armor);
        }

        int jewelryCount = Utility.RandomMinMax(0, bagTier);
        for (var i = 0; i < jewelryCount; i++)
        {
            var jewelry = Loot.RandomJewelry();
            GetRandomLevel(out var attributeCount, out var min, out var max);
            BaseRunicTool.ApplyAttributesTo(jewelry, attributeCount, min, max);
            DropItem(jewelry);
        }

        int reagentCount = Utility.RandomMinMax(0, bagTier);
        for (var i = 0; i < reagentCount; i++)
        {
            var reagents = Loot.RandomReagent();
            reagents.Amount = Math.Max(1, GetWeightedTier() * 2);
            DropItem(reagents);
        }
        int necroreagentCount = Utility.RandomMinMax(0, bagTier);
        for (var i = 0; i < necroreagentCount; i++)
        {
            var necroreagents = Loot.RandomNecromancyReagent();
            necroreagents.Amount = Math.Max(1, GetWeightedTier() * 2);
            DropItem(necroreagents);
        }

        int gemCount = Utility.RandomMinMax(0, bagTier);
        for (var i = 0; i < gemCount; i++)
        {
            var gems = Loot.RandomGem();
            gems.Amount = Math.Max(1, bagTier * 2);
            DropItem(gems);
        }

        int mageScrollCount = Utility.RandomMinMax(0, bagTier);
        int mlevel = 0;
        if (bagTier < 3)
        {
           mlevel = 42;
        }
        else
        {
           mlevel = 63;
        }

        for (var i = 0; i < mageScrollCount; i++)
        {
            var scroll = Loot.RandomScroll(0, mlevel, SpellbookType.Regular);
            scroll.Amount = Math.Max(1, bagTier);
            DropItem(scroll);
        }

        int necroScrollCount = Utility.RandomMinMax(0, bagTier);
        int nlevel = 0;
        if (bagTier < 3)
        {
            nlevel = 6;
        }
        else
        {
            nlevel = 15;
        }
        for (var i = 0; i < necroScrollCount; i++)
        {
            var scroll = Loot.RandomScroll(0, nlevel, SpellbookType.Necromancer);
            scroll.Amount = Math.Max(1, bagTier);
            DropItem(scroll);
        }

        for (var i = Math.Max(1, bagTier); i > 0; i--)
        {
            DropItem(Loot.RandomPotion());
        }

        for (var i = Math.Max(1, bagTier); i > 0; i--)
        {
            DropItem(Loot.RandomClothing());
        }

        if (bagTier > 3)
        {
            if (Utility.RandomBool())
            {
                DropItem(new Arrow((bagTier + (Utility.RandomMinMax(10, 20)))));
            }
            else
            {
                DropItem(new Bolt((bagTier + (Utility.RandomMinMax(10, 20)))));
            }
            DropItem(new BlankScroll(Math.Max(1, GetWeightedTier())));
        }

        int minGold = 100 + (bagTier * 100);
        int maxGold = minGold + 99;
        DropItem(new Gold(Utility.RandomMinMax(minGold, maxGold)));
    }
}
