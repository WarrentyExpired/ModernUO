using System;
using ModernUO.Serialization;

namespace Server.Items;
// ==============================================================
// LEVEL 2: 75% Trapped / 75% Locked
// ==============================================================
[SerializationGenerator(0, false)]
public partial class TreasureBagLevel2 : BaseTreasureBag
{
    [Constructible]
    public TreasureBagLevel2()
    {
        Locked = Utility.RandomDouble() < 0.75; // 75% Locked (25% Unlocked)
        RequiredSkill = 72;
        LockLevel = RequiredSkill - Utility.Random(1, 10);
        MaxLockLevel = RequiredSkill + Utility.Random(1, 10);
        var isTrapped = false;

        if (Utility.RandomDouble() < 0.75) // 75% Trapped
        {
            TrapType = TrapType.ExplosionTrap;
            TrapPower = 2 * Utility.Random(1, 25);
            isTrapped = true;
        }

        if (Locked && isTrapped)
        {
            DropItem(new Gold(Utility.Random(80, 150)));
            for (var i = Utility.Random(3); i > 0; i--)
            {
                var reagents = Loot.RandomReagent();
                reagents.Amount = Utility.Random(1, 2);
                DropItem(reagents);
            }
            if (Utility.RandomBool())
            {
                var scrolls = Loot.RandomScroll(0, 39, SpellbookType.Regular);
                scrolls.Amount = Utility.Random(1, 8);
                DropItem(scrolls);
            }
            if (Utility.RandomBool())
            {
                var gems = Loot.RandomGem();
                gems.Amount = Utility.Random(1, 6);
                DropItem(gems);
            }
            if (Utility.RandomBool()) DropItem(Loot.RandomPotion());
        }
        else if (isTrapped && !Locked)
        {
            DropItem(new Gold(Utility.Random(80, 120)));
            for (var i = Utility.Random(3); i > 0; i--)
            {
                var reagents = Loot.RandomReagent();
                reagents.Amount = Utility.Random(1, 2);
                DropItem(reagents);
            }
            if (Utility.RandomBool())
            {
                var scrolls = Loot.RandomScroll(0, 39, SpellbookType.Regular);
                scrolls.Amount = Utility.Random(1, 4);
                DropItem(scrolls);
            }
            if (Utility.RandomBool())
            {
                var gems = Loot.RandomGem();
                gems.Amount = Utility.Random(1, 3);
                DropItem(gems);
            }
            if (Utility.RandomBool()) DropItem(Loot.RandomPotion());
        }
        else if (!isTrapped && Locked)
        {
            DropItem(new Gold(Utility.Random(80, 120)));
            for (var i = Utility.Random(3); i > 0; i--)
            {
                var reagents = Loot.RandomReagent();
                reagents.Amount = Utility.Random(1, 2);
                DropItem(reagents);
            }
            if (Utility.RandomBool())
            {
                var scrolls = Loot.RandomScroll(0, 39, SpellbookType.Regular);
                scrolls.Amount = Utility.Random(1, 4);
                DropItem(scrolls);
            }
            if (Utility.RandomBool())
            {
                var gems = Loot.RandomGem();
                gems.Amount = Utility.Random(1, 3);
                DropItem(gems);
            }
            if (Utility.RandomBool()) DropItem(Loot.RandomPotion());
        }
        else
        {
            DropItem(new Gold(Utility.Random(80, 100)));
            var gems = Loot.RandomGem();
            gems.Amount = Utility.Random(1, 3);
            DropItem(gems);
        }
    }
}
