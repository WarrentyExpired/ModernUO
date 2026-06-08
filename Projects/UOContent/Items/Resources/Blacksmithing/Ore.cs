using ModernUO.Serialization;
using Server.Engines.Craft;
using Server.Mobiles;
using Server.Targeting;
using System.Collections.Generic;

namespace Server.Items;

[SerializationGenerator(2, false)]
public abstract partial class BaseOre : Item
{
    public BaseOre(CraftResource resource, int amount = 1) : base(0x19B9)
    {
        Stackable = true;
        Amount = amount;
        Hue = CraftResources.GetHue(resource);
        Weight = 1.0;
        _resource = resource;
    }

    [SerializableProperty(0)]
    [CommandProperty(AccessLevel.GameMaster)]
    public CraftResource Resource
    {
        get => _resource;
        set
        {
            if (_resource != value)
            {
                _resource = value;
                Hue = CraftResources.GetHue(value);

                InvalidateProperties();
                this.MarkDirty();
            }
        }
    }

    public override int LabelNumber
    {
        get
        {
            if (_resource is >= CraftResource.DullCopper and <= CraftResource.Valorite)
            {
                return 1042845 + (_resource - CraftResource.DullCopper);
            }

            return 1042853; // iron ore;
        }
    }

    public abstract BaseIngot GetIngot();

    private void Deserialize(IGenericReader reader, int version)
    {
        _resource = (CraftResource)reader.ReadByte();
    }

    public override bool CanStackWith(Item dropped) =>
        base.CanStackWith(dropped) &&
        (dropped as BaseOre)?._resource == _resource;

    public override void AddNameProperty(IPropertyList list)
    {
        if (Amount > 1)
        {
            list.Add(1050039, $"{Amount}\t{1026583:#}"); // ~1_NUMBER~ ~2_ITEMNAME~
        }
        else
        {
            list.Add(1026583); // ore
        }
    }

    public override void GetProperties(IPropertyList list)
    {
        base.GetProperties(list);

        if (!CraftResources.IsStandard(_resource))
        {
            var num = CraftResources.GetLocalizationNumber(_resource);

            if (num > 0)
            {
                list.Add(num);
            }
            else
            {
                list.Add(CraftResources.GetName(_resource));
            }
        }
    }

    public override void OnDoubleClick(Mobile from)
    {
        if (!Movable)
        {
            return;
        }

        if (RootParent is BaseCreature)
        {
            from.SendLocalizedMessage(500447); // That is not accessible[cite: 7]
            return;
        }

        // Verify proximity to a forge
        object forge = FindNearbyForge(from);
        if (forge == null)
        {
            from.SendMessage(0x22, "You must be standing next to a forge to refine raw mineral ore.");
            return;
        }

        if (from.Backpack == null)
            return;

        // FIXED: Create a local list snapshot to decouple enumeration from container modification[cite: 7]
        List<BaseOre> oresToProcess = new List<BaseOre>();

        // 1. Run a clean read-only pass to gather references without altering the container[cite: 7]
        foreach (BaseOre ore in from.Backpack.FindItemsByType<BaseOre>())
        {
            if (ore != null && !ore.Deleted && ore.Amount > 0)
            {
                oresToProcess.Add(ore);
            }
        }

        if (oresToProcess.Count == 0)
        {
            from.SendMessage("You do not have any ore in your backpack to smelt.");
            return;
        }

        int totalIngotsCreated = 0;
        int totalOreBurned = 0;

        // 2. Iterate through our isolated snapshot list where container changes are completely safe![cite: 7]
        foreach (BaseOre ore in oresToProcess)
        {
            if (ore.Deleted || ore.Amount <= 0)
                continue;

            var difficulty = ore._resource switch
            {
                CraftResource.DullCopper => 65.0,
                CraftResource.ShadowIron => 70.0,
                CraftResource.Copper     => 75.0,
                CraftResource.Bronze     => 80.0,
                CraftResource.Gold       => 85.0,
                CraftResource.Agapite    => 90.0,
                CraftResource.Verite     => 95.0,
                CraftResource.Valorite   => 99.0,
                _                        => 50.0
            };

            if (difficulty > 50.0 && difficulty > from.Skills.Mining.Value)
            {
                from.SendMessage(0x22, $"You lack the processing knowledge to smelt {CraftResources.GetName(ore._resource)} (Requires {difficulty} Mining).");
                continue;
            }

            var minSkill = difficulty - 25.0;
            var maxSkill = difficulty + 25.0;

            int totalToProcess = ore.Amount;
            int successCount = 0;
            int failCount = 0;

            // Loop 1-by-1 under the hood for maximum skill gain opportunities
            for (int i = 0; i < totalToProcess; i++)
            {
                if (from.CheckTargetSkill(SkillName.Mining, forge, minSkill, maxSkill))
                {
                    successCount++;
                }
                else
                {
                    failCount++;
                }
            }

            // Safely alter container layouts here without invalidating our local array loop tracking[cite: 7]
            ore.Consume(totalToProcess);

            if (successCount > 0)
            {
                BaseIngot ingot = ore.GetIngot();
                ingot.Amount = successCount;
                from.AddToBackpack(ingot);
                totalIngotsCreated += successCount;
            }

            totalOreBurned += failCount;
        }

        if (totalIngotsCreated > 0 || totalOreBurned > 0)
        {
            from.PlaySound(0x57); // Furnace sound[cite: 7]
            from.SendMessage(0x3F, $"Smelting Complete! Refined {totalIngotsCreated} pristine ingots. Lost {totalOreBurned} chunks to impurities.");
        }
    }

    private object FindNearbyForge(Mobile from)
    {
        // 1. Check for dynamic placed items
        foreach (Item item in from.GetItemsInRange(2))
        {
            if (item.ItemID == 4017 || (item.ItemID >= 6522 && item.ItemID <= 6569) || item.ItemID == 11736 || item.GetType().IsDefined(typeof(ForgeAttribute), false))
            {
                return item;
            }
        }

        // 2. Check the map matrix for static world art
        Map map = from.Map;
        if (map != null)
        {
            for (int x = -2; x <= 2; x++)
            {
                for (int y = -2; y <= 2; y++)
                {
                    var tiles = map.Tiles.GetStaticTiles(from.X + x, from.Y + y);
                    foreach (var tile in tiles)
                    {
                        int id = tile.ID;
                        if (id == 4017 || (id >= 6522 && id <= 6569) || id == 11736)
                        {
                            return from;
                        }
                    }
                }
            }
        }

        return null;
    }
}

// ==========================================
// INDIVIDUAL DISCRETE ELEMENT CONSTRUCTORS
// ==========================================
[SerializationGenerator(0, false)]
public partial class IronOre : BaseOre
{
    [Constructible]
    public IronOre(int amount = 1) : base(CraftResource.Iron, amount) { }
    public override BaseIngot GetIngot() => new IronIngot();
}

[SerializationGenerator(0, false)]
public partial class DullCopperOre : BaseOre
{
    [Constructible]
    public DullCopperOre(int amount = 1) : base(CraftResource.DullCopper, amount) { }
    public override BaseIngot GetIngot() => new DullCopperIngot();
}

[SerializationGenerator(0, false)]
public partial class ShadowIronOre : BaseOre
{
    [Constructible]
    public ShadowIronOre(int amount = 1) : base(CraftResource.ShadowIron, amount) { }
    public override BaseIngot GetIngot() => new ShadowIronIngot();
}

[SerializationGenerator(0, false)]
public partial class CopperOre : BaseOre
{
    [Constructible]
    public CopperOre(int amount = 1) : base(CraftResource.Copper, amount) { }
    public override BaseIngot GetIngot() => new CopperIngot();
}

[SerializationGenerator(0, false)]
public partial class BronzeOre : BaseOre
{
    [Constructible]
    public BronzeOre(int amount = 1) : base(CraftResource.Bronze, amount) { }
    public override BaseIngot GetIngot() => new BronzeIngot();
}

[SerializationGenerator(0, false)]
public partial class GoldOre : BaseOre
{
    [Constructible]
    public GoldOre(int amount = 1) : base(CraftResource.Gold, amount) { }
    public override BaseIngot GetIngot() => new GoldIngot();
}

[SerializationGenerator(0, false)]
public partial class AgapiteOre : BaseOre
{
    [Constructible]
    public AgapiteOre(int amount = 1) : base(CraftResource.Agapite, amount) { }
    public override BaseIngot GetIngot() => new AgapiteIngot();
}

[SerializationGenerator(0, false)]
public partial class VeriteOre : BaseOre
{
    [Constructible]
    public VeriteOre(int amount = 1) : base(CraftResource.Verite, amount) { }
    public override BaseIngot GetIngot() => new VeriteIngot();
}

[SerializationGenerator(0, false)]
public partial class ValoriteOre : BaseOre
{
    [Constructible]
    public ValoriteOre(int amount = 1) : base(CraftResource.Valorite, amount) { }
    public override BaseIngot GetIngot() => new ValoriteIngot();
}
