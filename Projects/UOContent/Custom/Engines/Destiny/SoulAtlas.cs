using System;
using ModernUO.Serialization;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Items;

[SerializationGenerator(1, false)]
public partial class SoulAtlas : Item
{
    [SerializableField(0)]
    [SerializedCommandProperty(AccessLevel.GameMaster)]
    private int _unlockedMask = 1; // Default to 1 (Bit 0 active = Shame unlocked!)

    // FIXED: Added the Constructible attribute so you can spawn it via in-game commands!
    [Constructible]
    public SoulAtlas() : base(0x22C5) // Runebook Graphic ID
    {
        Name = "Soul Atlas";
        Hue = 1161;                  // Special custom color glow
        LootType = LootType.Blessed; // Protected from being lost on death
        Movable = true;
    }

    // Helper method you can call later from your dungeon key items to unlock pages!
    public void UnlockDungeon(int dungeonIndex)
    {
        if (dungeonIndex >= 0 && dungeonIndex < 8)
        {
            UnlockedMask |= (1 << dungeonIndex);
        }
    }

    public bool IsDungeonUnlocked(int dungeonIndex)
    {
        if (dungeonIndex < 0 || dungeonIndex >= 8) return false;
        return (UnlockedMask & (1 << dungeonIndex)) != 0;
    }

    public override void OnDoubleClick(Mobile from)
    {
        if (from is PlayerMobile pm)
        {
            if (from.InRange(GetWorldLocation(), 2))
            {
                from.SendGump(new SoulAtlasGump(pm, this));
            }
            else
            {
                from.SendLocalizedMessage(500446); // That is too far away.
            }
        }
    }

    private void Deserialize(IGenericReader reader, int version)
    {
        _unlockedMask = reader.ReadInt();
    }
}
