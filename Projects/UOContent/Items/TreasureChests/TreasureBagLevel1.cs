using System;
using ModernUO.Serialization;

namespace Server.Items;
// ==============================================================
// LEVEL 1: 50% Trapped / 50% Locked
// ==============================================================
[SerializationGenerator(0, false)]
public partial class TreasureBagLevel1 : BaseTreasureBag
{
    [Constructible]
    public TreasureBagLevel1()
    {
        var isTrapped = false;

        Locked = Utility.RandomDouble() < 0.50; // 50% Locked
        RequiredSkill = 57;
        LockLevel = RequiredSkill - Utility.Random(1, 10);
        MaxLockLevel = RequiredSkill + Utility.Random(1, 10);

        if (Utility.RandomDouble() < 0.50) // 50% Trapped
        {
            TrapType = TrapType.DartTrap;
            TrapPower = Utility.Random(1, 25);
            isTrapped = true;
        }

        if (isTrapped && Locked)
        {
            DropItem(new Gold(Utility.Random(30, 100)));
            if (Utility.RandomBool())
            {
                var gems = Loot.RandomGem();
                gems.Amount = Utility.Random(1, 3);
                DropItem(gems);
            }
            if (Utility.RandomBool()) DropItem(Loot.RandomWeapon());
            if (Utility.RandomBool()) DropItem(Loot.RandomArmorOrShield());
            if (Utility.RandomBool()) DropItem(Loot.RandomClothing());
            if (Utility.RandomBool()) DropItem(Loot.RandomJewelry());
        }
        else if (isTrapped && !Locked)
        {
            DropItem(new Gold(Utility.Random(30, 80)));
            if (Utility.RandomBool())
            {
                var gems = Loot.RandomGem();
                gems.Amount = Utility.Random(1, 3);
                DropItem(gems);
            }
            if (Utility.RandomBool()) DropItem(Loot.RandomWeapon());
            if (Utility.RandomBool()) DropItem(Loot.RandomClothing());
        }
        else if (!isTrapped && Locked)
        {
            DropItem(new Gold(Utility.Random(30, 80)));
            if (Utility.RandomBool())
            {
                var gems = Loot.RandomGem();
                gems.Amount = Utility.Random(1, 3);
                DropItem(gems);
            }
            if (Utility.RandomBool()) DropItem(Loot.RandomArmor());
        }
        else
        {
            DropItem(new Gold(Utility.Random(30, 50)));
            if (Utility.RandomBool())
            {
                var gems = Loot.RandomGem();
                gems.Amount = 1;
                DropItem(gems);
            }
        }
    }
}
