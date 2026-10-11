using System;
using ModernUO.Serialization;

namespace Server.Items;

// ==============================================================
// LEVEL 4: 99% Trapped / 99% Locked (1% Lottery Ticket!)
// ==============================================================
[SerializationGenerator(0, false)]
public partial class TreasureBagLevel4 : BaseTreasureBag
{
    [Constructible]
    public TreasureBagLevel4()
    {
        Locked = Utility.RandomDouble() < 0.95; // 99% Locked (5% Unlocked)
        RequiredSkill = 92;
        LockLevel = RequiredSkill - Utility.Random(1, 10);
        MaxLockLevel = RequiredSkill + Utility.Random(1, 10);

        if (Utility.RandomDouble() < 0.99) // 99% Trapped
        {
            TrapType = TrapType.ExplosionTrap;
            TrapPower = 4 * Utility.Random(10, 25);
        }

        // --- UNIFIED LOOT GENERATION ---
        // Everyone gets this massive loot pool. The 1% just get to open it for free!

        DropItem(new Gold(Utility.Random(500, 900)));

        for (var i = Utility.Random(4) + 1; i > 0; i--)
        {
            var reagents = Loot.RandomReagent();
            reagents.Amount = 12;
            DropItem(reagents);
        }

        for (var i = Utility.Random(4) + 1; i > 0; i--)
        {
            var scroll = Loot.RandomScroll(0, 47, SpellbookType.Regular);
            scroll.Amount = 16;
            DropItem(scroll);
        }

        DropItem(new BlankScroll(Utility.Random(1, 4)));

        for (var i = Utility.Random(4) + 1; i > 0; i--)
        {
            DropItem(Loot.RandomPotion());
        }

        for (var i = Utility.Random(4) + 1; i > 0; i--)
        {
            var gems = Loot.RandomGem();
            gems.Amount = 12;
            DropItem(gems);
        }

        for (var i = Utility.Random(4) + 1; i > 0; i--)
        {
            DropItem(Loot.RandomWand());
        }

        for (var i = Utility.Random(4) + 1; i > 0; i--)
        {
            var item = Loot.RandomArmorOrShieldOrWeapon();

            if (item is BaseWeapon weapon)
            {
                weapon.DamageLevel = (WeaponDamageLevel)Utility.Random(4);
                weapon.AccuracyLevel = (WeaponAccuracyLevel)Utility.Random(4);
                weapon.DurabilityLevel = (WeaponDurabilityLevel)Utility.Random(4);
                weapon.Quality = WeaponQuality.Regular;
            }
            else if (item is BaseArmor armor)
            {
                armor.ProtectionLevel = (ArmorProtectionLevel)Utility.Random(4);
                armor.Durability = (ArmorDurabilityLevel)Utility.Random(4);
                armor.Quality = ArmorQuality.Regular;
            }

            DropItem(item);
        }

        for (var i = Utility.Random(3) + 1; i > 0; i--)
        {
            DropItem(Loot.RandomClothing());
        }

        for (var i = Utility.Random(3) + 1; i > 0; i--)
        {
            DropItem(Loot.RandomJewelry());
        }
    }
}
