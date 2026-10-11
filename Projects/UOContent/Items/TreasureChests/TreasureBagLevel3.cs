using System;
using ModernUO.Serialization;

namespace Server.Items;

// ==============================================================
// LEVEL 3: 90% Trapped / 90% Locked
// ==============================================================
[SerializationGenerator(0, false)]
public partial class TreasureBagLevel3 : BaseTreasureBag
{
    [Constructible]
    public TreasureBagLevel3()
    {
        Locked = Utility.RandomDouble() < 0.90; // 90% Locked (10% Unlocked)
        RequiredSkill = 84;
        LockLevel = RequiredSkill - Utility.Random(1, 10);
        MaxLockLevel = RequiredSkill + Utility.Random(1, 10);
        var isTrapped = false;

        if (Utility.RandomDouble() < 0.90) // 90% Trapped
        {
            TrapType = TrapType.PoisonTrap;
            TrapPower = 3 * Utility.Random(1, 25);
            isTrapped = true;
        }

        if (isTrapped && Locked)
        {
            DropItem(new Gold(Utility.Random(250, 350)));
            for (var i = Utility.Random(2); i >= 0; i--)
            {
                var reagents = Loot.RandomReagent();
                reagents.Amount = Utility.Random(1, 9);
                DropItem(reagents);
            }

            for (var i = Utility.Random(3); i > 0; i--)
            {
                var scrolls = Loot.RandomScroll(0, 47, SpellbookType.Regular);
                scrolls.Amount = Utility.Random(1, 12);
                DropItem(scrolls);
            }

            for (var i = Utility.Random(3); i > 0; i--)
            {
                DropItem(Loot.RandomPotion());
            }

            for (var i = Utility.Random(3); i > 0; i--)
            {
                var gems = Loot.RandomGem();
                gems.Amount = Utility.Random(1, 9);
                DropItem(gems);
            }

            for (var i = Utility.Random(3); i > 0; i--)
            {
                DropItem(Loot.RandomWand());
            }

            for (var i = Utility.Random(3); i > 0; i--)
            {
                var item = Loot.RandomArmorOrShieldOrWeapon();

                if (item is BaseWeapon weapon)
                {
                    weapon.DamageLevel = (WeaponDamageLevel)Utility.Random(3);
                    weapon.AccuracyLevel = (WeaponAccuracyLevel)Utility.Random(3);
                    weapon.DurabilityLevel = (WeaponDurabilityLevel)Utility.Random(3);
                    weapon.Quality = WeaponQuality.Regular;
                }
                else if (item is BaseArmor armor)
                {
                    armor.ProtectionLevel = (ArmorProtectionLevel)Utility.Random(3);
                    armor.Durability = (ArmorDurabilityLevel)Utility.Random(3);
                    armor.Quality = ArmorQuality.Regular;
                }

                DropItem(item);
            }

            for (var i = Utility.Random(3); i > 0; i--)
            {
                DropItem(Loot.RandomClothing());
            }

            for (var i = Utility.Random(3); i > 0; i--)
            {
                DropItem(Loot.RandomJewelry());
            }
        }

        else if (!isTrapped && Locked)
        {
            DropItem(new Gold(Utility.Random(250, 300)));
            for (var i = Utility.Random(2); i >= 0; i--)
            {
                var reagents = Loot.RandomReagent();
                reagents.Amount = Utility.Random(1, 9);
                DropItem(reagents);
            }

            for (var i = Utility.Random(3); i > 0; i--)
            {
                var scrolls = Loot.RandomScroll(0, 47, SpellbookType.Regular);
                scrolls.Amount = Utility.Random(1, 12);
                DropItem(scrolls);
            }

            for (var i = Utility.Random(3); i > 0; i--)
            {
                DropItem(Loot.RandomPotion());
            }

            for (var i = Utility.Random(3); i > 0; i--)
            {
                var gems = Loot.RandomGem();
                gems.Amount = Utility.Random(1, 9);
                DropItem(gems);
            }

            for (var i = Utility.Random(3); i > 0; i--)
            {
                DropItem(Loot.RandomWand());
            }

            if (Utility.RandomBool())
            {
                for (var i = Utility.Random(3); i > 0; i--)
                {
                    var weapon = Loot.RandomWeapon();
                    weapon.DamageLevel = (WeaponDamageLevel)Utility.Random(3);
                    weapon.AccuracyLevel = (WeaponAccuracyLevel)Utility.Random(3);
                    weapon.DurabilityLevel = (WeaponDurabilityLevel)Utility.Random(3);
                    weapon.Quality = WeaponQuality.Regular;
                    DropItem(weapon);
                }
            }

            if (Utility.RandomBool())
            {
                for (var i = Utility.Random(3); i > 0; i--)
                {
                    var armor = Loot.RandomArmorOrShield();
                    armor.ProtectionLevel = (ArmorProtectionLevel)Utility.Random(3);
                    armor.Durability = (ArmorDurabilityLevel)Utility.Random(3);
                    armor.Quality = ArmorQuality.Regular;
                    DropItem(armor);
                }
            }

            for (var i = Utility.Random(3); i > 0; i--)
            {
                DropItem(Loot.RandomClothing());
            }

            for (var i = Utility.Random(3); i > 0; i--)
            {
                DropItem(Loot.RandomJewelry());
            }
        }

        else if (isTrapped && !Locked)
        {
            DropItem(new Gold(Utility.Random(250, 300)));
            for (var i = Utility.Random(2); i >= 0; i--)
            {
                var reagents = Loot.RandomReagent();
                reagents.Amount = Utility.Random(1, 4);
                DropItem(reagents);
            }

            for (var i = Utility.Random(3); i > 0; i--)
            {
                var scrolls = Loot.RandomScroll(0, 47, SpellbookType.Regular);
                scrolls.Amount = Utility.Random(1, 6);
                DropItem(scrolls);
            }

            for (var i = Utility.Random(3); i > 0; i--)
            {
                DropItem(Loot.RandomPotion());
            }

            for (var i = Utility.Random(3); i > 0; i--)
            {
                var gems = Loot.RandomGem();
                gems.Amount = Utility.Random(1, 4);
                DropItem(gems);
            }

            for (var i = Utility.Random(3); i > 0; i--)
            {
                DropItem(Loot.RandomWand());
            }

            if (Utility.RandomBool())
            {
                if (Utility.RandomBool())
                {
                    for (var i = Utility.Random(3); i > 0; i--)
                    {
                        var weapon = Loot.RandomWeapon();
                        weapon.DamageLevel = (WeaponDamageLevel)Utility.Random(3);
                        weapon.AccuracyLevel = (WeaponAccuracyLevel)Utility.Random(3);
                        weapon.DurabilityLevel = (WeaponDurabilityLevel)Utility.Random(3);
                        weapon.Quality = WeaponQuality.Regular;
                        DropItem(weapon);
                    }
                }

                if (Utility.RandomBool())
                {
                    for (var i = Utility.Random(3); i > 0; i--)
                    {
                        var armor = Loot.RandomArmorOrShield();
                        armor.ProtectionLevel = (ArmorProtectionLevel)Utility.Random(3);
                        armor.Durability = (ArmorDurabilityLevel)Utility.Random(3);
                        armor.Quality = ArmorQuality.Regular;
                        DropItem(armor);
                    }
                }
            }
            for (var i = Utility.Random(3); i > 0; i--)
            {
                DropItem(Loot.RandomClothing());
            }

            for (var i = Utility.Random(3); i > 0; i--)
            {
                DropItem(Loot.RandomJewelry());
            }
        }
        else
        {
            DropItem(new Gold(Utility.Random(175, 225)));
            for (var i = Utility.Random(2); i >= 0; i--)
            {
                var reagents = Loot.RandomReagent();
                reagents.Amount = Utility.Random(1, 4);
                DropItem(reagents);
            }

            for (var i = Utility.Random(3); i > 0; i--)
            {
                var scrolls = Loot.RandomScroll(0, 47, SpellbookType.Regular);
                scrolls.Amount = Utility.Random(1, 6);
                DropItem(scrolls);
            }

            for (var i = Utility.Random(3); i > 0; i--)
            {
                DropItem(Loot.RandomPotion());
            }

            for (var i = Utility.Random(3); i > 0; i--)
            {
                var gems = Loot.RandomGem();
                gems.Amount = Utility.Random(1, 4);
                DropItem(gems);
            }

            for (var i = Utility.Random(3); i > 0; i--)
            {
                DropItem(Loot.RandomWand());
            }
            if (Utility.RandomBool())
            {
                for (var i = Utility.Random(3); i > 0; i--)
                {
                    var armor = Loot.RandomArmorOrShield();
                    armor.ProtectionLevel = (ArmorProtectionLevel)Utility.Random(3);
                    armor.Durability = (ArmorDurabilityLevel)Utility.Random(3);
                    armor.Quality = ArmorQuality.Regular;
                    DropItem(armor);
                }
            }
        }
    }
}
