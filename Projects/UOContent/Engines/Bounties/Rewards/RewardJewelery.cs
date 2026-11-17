using System;
using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
public abstract partial class BaseStatJewelry : BaseJewel
{
    public virtual int StrBonus => 0;
    public virtual int DexBonus => 0;
    public virtual int IntBonus => 0;

    protected BaseStatJewelry(int itemID, Layer layer) : base(itemID, layer)
    {
        Weight = 0.1;
    }

    public override bool OnEquip(Mobile from)
    {
        if (!base.OnEquip(from))
        {
            return false;
        }

        ApplyStatMods(from);
        from.CheckStatTimers();
        return true;
    }

    public override void OnRemoved(IEntity parent)
    {
        if (parent is Mobile from)
        {
            RemoveStatMods(from);
            from.CheckStatTimers();
        }

        base.OnRemoved(parent);
    }

    [AfterDeserialization]
    private void AfterDeserialization()
    {
        if (Parent is Mobile from)
        {
            ApplyStatMods(from);
            from.CheckStatTimers();
        }
    }

    private void ApplyStatMods(Mobile from)
    {
        var serial = Serial;

        if (StrBonus != 0)
        {
            from.AddStatMod(new StatMod(StatType.Str, $"{serial}Str", StrBonus, TimeSpan.Zero));
        }

        if (DexBonus != 0)
        {
            from.AddStatMod(new StatMod(StatType.Dex, $"{serial}Dex", DexBonus, TimeSpan.Zero));
        }

        if (IntBonus != 0)
        {
            from.AddStatMod(new StatMod(StatType.Int, $"{serial}Int", IntBonus, TimeSpan.Zero));
        }
    }

    private void RemoveStatMods(Mobile from)
    {
        var serial = Serial;
        from.RemoveStatMod($"{serial}Str");
        from.RemoveStatMod($"{serial}Dex");
        from.RemoveStatMod($"{serial}Int");
    }

    public override void OnSingleClick(Mobile from)
    {
        base.OnSingleClick(from);

        if (StrBonus != 0)
        {
            LabelTo(from, $"Strength Bonus: {StrBonus:+#;-#;0}");
        }

        if (DexBonus != 0)
        {
            LabelTo(from, $"Dexterity Bonus: {DexBonus:+#;-#;0}");
        }

        if (IntBonus != 0)
        {
            LabelTo(from, $"Intelligence Bonus: {IntBonus:+#;-#;0}");
        }
    }

    public override void GetProperties(IPropertyList list)
    {
        base.GetProperties(list);

        if (StrBonus != 0)
        {
            list.Add(1060485, StrBonus.ToString()); // strength bonus ~1_val~
        }

        if (DexBonus != 0)
        {
            list.Add(1060409, DexBonus.ToString()); // dexterity bonus ~1_val~
        }

        if (IntBonus != 0)
        {
            list.Add(1060432, IntBonus.ToString()); // intelligence bonus ~1_val~
        }
    }
}

// ==========================================
// SET 1: DEXER SET (The Hunters)
// ==========================================

[SerializationGenerator(0, false)]
public partial class HuntersRing : BaseStatJewelry
{
    [Constructible]
    public HuntersRing() : base(0x108A, Layer.Ring)
    {
        Name = "Ring of the Hunter";
        Hue = 1157; // Crimson / Blood Red
    }

    public override int StrBonus => 2;
    public override int DexBonus => 3;
}

[SerializationGenerator(0, false)]
public partial class HuntersBracelet : BaseStatJewelry
{
    [Constructible]
    public HuntersBracelet() : base(0x1086, Layer.Bracelet)
    {
        Name = "Bracelet of the Hunter";
        Hue = 1157;
    }

    public override int StrBonus => 1;
    public override int DexBonus => 4;
}

[SerializationGenerator(0, false)]
public partial class HuntersEarrings : BaseStatJewelry
{
    [Constructible]
    public HuntersEarrings() : base(0x1087, Layer.Earrings)
    {
        Name = "Earrings of the Hunter";
        Hue = 1157;
    }

    public override int StrBonus => 2;
    public override int DexBonus => 3;
}

// ==========================================
// SET 2: MAGERY SET (Archmage)
// ==========================================

[SerializationGenerator(0, false)]
public partial class ArchmageRing : BaseStatJewelry
{
    [Constructible]
    public ArchmageRing() : base(0x108A, Layer.Ring)
    {
        Name = "Ring of the Archmage";
        Hue = 1154; // Ice / Arcane Blue
    }

    public override int DexBonus => 2;
    public override int IntBonus => 3;
}

[SerializationGenerator(0, false)]
public partial class ArchmageBracelet : BaseStatJewelry
{
    [Constructible]
    public ArchmageBracelet() : base(0x1086, Layer.Bracelet)
    {
        Name = "Bracelet of the Archmage";
        Hue = 1154;
    }

    public override int StrBonus => 1;
    public override int IntBonus => 4;
}

[SerializationGenerator(0, false)]
public partial class ArchmageEarrings : BaseStatJewelry
{
    [Constructible]
    public ArchmageEarrings() : base(0x1087, Layer.Earrings)
    {
        Name = "Earrings of the Archmage";
        Hue = 1154;
    }

    public override int StrBonus => 2;
    public override int IntBonus => 3;
}
