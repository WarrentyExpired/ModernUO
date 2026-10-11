using System;
using ModernUO.Serialization;
using Server.Items;

namespace Server.Items;

public abstract class BaseTreasureBag : LockableContainer
{
    public BaseTreasureBag() : base(0xE76)
    {
        SetBagAppearance();
        Movable = false;

        // Pick a random hue between 1 and 1000
        Hue = Utility.Random(1, 1000);
    }

    public BaseTreasureBag(Serial serial) : base(serial)
    {
    }

    public override bool Decays => true;
    public override bool IsDecoContainer => false;

    private readonly TimeSpan _decayTime = TimeSpan.FromMinutes(Utility.Random(15, 60));
    public override TimeSpan DecayTime => _decayTime;

    private static readonly (int, int)[] _bagAppearances =
    {
        (0xE76, 0x3D), // Bag
        (0xE75, 0x3C), // Backpack
        (0xE79, 0x41)  // Pouch
    };

    private void SetBagAppearance()
    {
        (ItemID, GumpID) = _bagAppearances.RandomElement();
    }

    public override Rectangle2D Bounds
    {
        get
        {
            if (GumpID == 0x3D) return new Rectangle2D(29, 34, 108, 94); // Bag
            if (GumpID == 0x3C) return new Rectangle2D(44, 65, 142, 94); // Backpack
            return new Rectangle2D(18, 20, 125, 62);                     // Pouch
        }
    }
}
