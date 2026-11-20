using System;
using ModernUO.Serialization;
using Server.Targeting;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class ArmorEnhancementTool : Item
{
    [SerializableField(0)]
    [SerializedCommandProperty(AccessLevel.GameMaster)]
    private int _usesRemaining;

    [Constructible]
    public ArmorEnhancementTool() : this(5)
    {
    }

    [Constructible]
    public ArmorEnhancementTool(int uses) : base(0x1EB8) // Tinker tool graphic
    {
        Name = "an armor infusion tool";
        Hue = 0x489;
        Weight = 2.0;
        _usesRemaining = uses;
    }

    public override void OnSingleClick(Mobile from)
    {
        base.OnSingleClick(from);
        LabelTo(from, $"Uses remaining: {_usesRemaining}");
    }

    public override void OnDoubleClick(Mobile from)
    {
        if (!IsChildOf(from.Backpack))
        {
            from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            return;
        }

        if (_usesRemaining <= 0)
        {
            from.SendMessage("This tool has no charges remaining.");
            Delete();
            return;
        }

        from.SendMessage(0x59, "Select the colored ingots or leather you wish to infuse into your armor.");
        from.Target = new ResourceTarget(this);
    }

    private static bool TryGetTierRequirements(
        CraftResource resource,
        out ArmorProtectionLevel minProtection,
        out int requiredAmount)
    {
        switch (resource)
        {
            // Tier 1: Regular / Defense+
            case CraftResource.DullCopper:
            case CraftResource.ShadowIron:
            case CraftResource.Copper:
            case CraftResource.SpinedLeather:
                minProtection = ArmorProtectionLevel.Regular;
                requiredAmount = 15;
                return true;

            // Tier 2: Guarding / Hardening+
            case CraftResource.Bronze:
            case CraftResource.Gold:
            case CraftResource.HornedLeather:
                minProtection = ArmorProtectionLevel.Guarding;
                requiredAmount = 25;
                return true;

            // Tier 3: Fortification+
            case CraftResource.Agapite:
            case CraftResource.Verite:
                minProtection = ArmorProtectionLevel.Fortification;
                requiredAmount = 40;
                return true;

            // Tier 4: Invulnerability
            case CraftResource.Valorite:
            case CraftResource.BarbedLeather:
                minProtection = ArmorProtectionLevel.Invulnerability;
                requiredAmount = 50;
                return true;

            default:
                minProtection = ArmorProtectionLevel.Regular;
                requiredAmount = 0;
                return false;
        }
    }

    private static bool IsMetalArmor(BaseArmor armor) =>
        armor.MaterialType is ArmorMaterialType.Plate
            or ArmorMaterialType.Chainmail
            or ArmorMaterialType.Ringmail
            or ArmorMaterialType.Dragon;

    private static bool IsLeatherArmor(BaseArmor armor) =>
        armor.MaterialType is ArmorMaterialType.Leather
            or ArmorMaterialType.Studded;

    private class ResourceTarget : Target
    {
        private readonly ArmorEnhancementTool _tool;

        public ResourceTarget(ArmorEnhancementTool tool) : base(2, false, TargetFlags.None)
        {
            _tool = tool;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (_tool == null || _tool.Deleted || !_tool.IsChildOf(from.Backpack))
            {
                return;
            }

            if (targeted is not Item resItem || !resItem.IsChildOf(from.Backpack))
            {
                from.SendMessage("You must select colored ingots or leather inside your backpack.");
                return;
            }

            CraftResource resource = CraftResources.GetFromType(resItem.GetType());
            CraftResourceType resType = CraftResources.GetType(resource);

            if ((resType != CraftResourceType.Metal && resType != CraftResourceType.Leather) ||
                !TryGetTierRequirements(resource, out var minProtection, out int requiredAmount))
            {
                from.SendMessage("You must select colored metal ingots or colored leather.");
                return;
            }

            if (resItem.Amount < requiredAmount)
            {
                from.SendMessage($"You need at least {requiredAmount} {CraftResources.GetName(resource)} to infuse armor.");
                return;
            }

            from.SendMessage(
                0x59,
                $"Select the armor or shield to infuse with {CraftResources.GetName(resource)} (Requires: {minProtection}+, Cost: {requiredAmount})."
            );
            from.Target = new ArmorTarget(_tool, resItem, resource, resType, minProtection, requiredAmount);
        }
    }

    private class ArmorTarget : Target
    {
        private readonly ArmorEnhancementTool _tool;
        private readonly Item _resItem;
        private readonly CraftResource _resource;
        private readonly CraftResourceType _resType;
        private readonly ArmorProtectionLevel _minProtection;
        private readonly int _requiredAmount;

        public ArmorTarget(
            ArmorEnhancementTool tool,
            Item resItem,
            CraftResource resource,
            CraftResourceType resType,
            ArmorProtectionLevel minProtection,
            int requiredAmount) : base(2, false, TargetFlags.None)
        {
            _tool = tool;
            _resItem = resItem;
            _resource = resource;
            _resType = resType;
            _minProtection = minProtection;
            _requiredAmount = requiredAmount;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (_tool == null || _tool.Deleted || !_tool.IsChildOf(from.Backpack))
            {
                return;
            }

            if (_resItem == null || _resItem.Deleted || !_resItem.IsChildOf(from.Backpack) || _resItem.Amount < _requiredAmount)
            {
                from.SendMessage("You no longer have enough of that resource in your backpack.");
                return;
            }

            if (targeted is not BaseArmor armor || !armor.IsChildOf(from.Backpack))
            {
                from.SendMessage("You must select a piece of armor or a shield in your backpack.");
                return;
            }

            // Prevent overwriting already-colored crafted armor
            if (armor.Resource != CraftResource.None &&
                armor.Resource != CraftResource.Iron &&
                armor.Resource != CraftResource.RegularLeather)
            {
                from.SendMessage("That armor was already forged from a special material and cannot be infused.");
                return;
            }

            if (armor.EnhancedResource == _resource)
            {
                from.SendMessage("That armor is already infused with that resource.");
                return;
            }

            // Check material compatibility (Metal vs Leather)
            bool compatible = _resType == CraftResourceType.Metal
                ? IsMetalArmor(armor)
                : IsLeatherArmor(armor);

            if (!compatible)
            {
                from.SendMessage("That resource type is not compatible with this type of armor.");
                return;
            }

            // Check minimum UOR magical protection tier
            if (armor.ProtectionLevel < _minProtection)
            {
                from.SendMessage($"That armor must be at least {_minProtection} quality to hold {_resource} infusion.");
                return;
            }

            // Consume resources & apply infusion
            _resItem.Consume(_requiredAmount);
            armor.EnhancedResource = _resource;

            from.PlaySound(0x2A);
            from.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
            from.SendMessage(
                0x3F,
                $"You successfully infuse the {CraftResources.GetName(_resource)} magic resistance into the armor!"
            );

            _tool.UsesRemaining--;
            if (_tool.UsesRemaining <= 0)
            {
                from.SendMessage("Your armor infusion tool has worn out and breaks.");
                _tool.Delete();
            }
        }
    }
}
