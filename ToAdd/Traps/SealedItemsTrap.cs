/**********************************
* Author: Expa                    *
* Optimized ModernUO .Net 10/C#14 *
**********************************/
using System;
using ModernUO.Serialization;
using Server;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items;

public static class SealedTrapConfig
{
    public const int MinItemsToSeal = 1;
    public const int MaxItemsToSeal = 5;
    public const int TrapItemId = 0x9CDD;
    public const int SealedChestItemId = 0x9AA;
    public const bool DeleteAfterTrigger = true;
    public const string ActivationMessage = "The trap has sealed {0} items in the chest!";
    public const string SealedChestMessage = "This chest is magically sealed! Only the Seal Removal Scroll can open it.";
}

[SerializationGenerator(0, false)]
public partial class ItemSealingTrap : Item
{
    [Constructible]
    public ItemSealingTrap() : base(SealedTrapConfig.TrapItemId)
    {
        Movable = false;
        Visible = false;
        Name = "The Sealing trap";
    }

    public override bool OnMoveOver(Mobile m)
    {
        if (m is not PlayerMobile player || player.AccessLevel > AccessLevel.Player)
            return base.OnMoveOver(m);

        var backpack = player.Backpack;
        if (backpack?.Items.Count == 0)
            return base.OnMoveOver(m);

        var itemsToSeal = Math.Min(Utility.RandomMinMax(SealedTrapConfig.MinItemsToSeal, SealedTrapConfig.MaxItemsToSeal), backpack.Items.Count);
        var chest = new SealedItemsChest();

        for (var i = 0; i < itemsToSeal; i++)
        {
            var idx = Utility.Random(backpack.Items.Count);
            var item = backpack.Items[idx];
            backpack.RemoveItem(item);
            chest.DropItem(item);
        }

        chest.MoveToWorld(Location, Map);
        player.SendMessage(string.Format(SealedTrapConfig.ActivationMessage, itemsToSeal));

        if (SealedTrapConfig.DeleteAfterTrigger)
            Delete();

        return base.OnMoveOver(m);
    }
}

[Furniture]
[SerializationGenerator(0, false)]
public partial class SealedItemsChest : LockableContainer
{
    private bool _sealed = true;

    [CommandProperty(AccessLevel.GameMaster)]
    public bool Sealed
    {
        get => _sealed;
        set
        {
            _sealed = value;
            Locked = value;
            InvalidateProperties();
        }
    }

    [Constructible]
    public SealedItemsChest() : base(SealedTrapConfig.SealedChestItemId)
    {
        Locked = true;
        Name = "A sealed little chest";
        LockLevel = ILockpickable.CannotPick;
        RequiredSkill = 0;
        MaxLockLevel = 0;
    }

    public override void OnDoubleClick(Mobile from)
    {
        if (Sealed)
            from?.SendMessage(SealedTrapConfig.SealedChestMessage);
        else
            base.OnDoubleClick(from);
    }

    public override void GetProperties(IPropertyList list)
    {
        base.GetProperties(list);
        if (_sealed)
            list.Add("<BASEFONT COLOR=#FF0000>Sealed</BASEFONT>");
    }

    public void Unseal()
    {
        Sealed = false;
        LockLevel = ILockpickable.CannotPick;
        Name = "A small chest";
    }

    public override void Open(Mobile from)
    {
        if (_sealed)
            from?.SendMessage(SealedTrapConfig.SealedChestMessage);
        else
            base.Open(from);
    }

    public override bool CheckLocked(Mobile from)
    {
        if (!Locked)
            return false;

        if (_sealed)
        {
            from?.SendMessage(SealedTrapConfig.SealedChestMessage);
            return true;
        }

        return base.CheckLocked(from);
    }
}

[SerializationGenerator(0, false)]
public partial class ScrollOfUnsealing : Item
{
    [Constructible]
    public ScrollOfUnsealing() : base(0x1F4F)
    {
        Name = "The Seal Removal Scroll";
        Weight = 1.0;
    }

    public override void OnDoubleClick(Mobile from)
    {
        from?.Target = new UnsealTarget(this);
        from?.SendMessage("Select a sealed chest.");
    }

    private sealed class UnsealTarget : Target
    {
        private readonly ScrollOfUnsealing _scroll;

        public UnsealTarget(ScrollOfUnsealing scroll) : base(3, false, TargetFlags.None) =>
            _scroll = scroll;

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (_scroll?.Deleted != false)
                return;

            switch (targeted)
            {
                case SealedItemsChest { Sealed: true } chest:
                    chest.Unseal();
                    from?.SendMessage("The seal has been lifted!");
                    _scroll.Delete();
                    break;
                case SealedItemsChest:
                    from?.SendMessage("The chest has already been opened!");
                    break;
                default:
                    from?.SendMessage("It's not a sealed chest!");
                    break;
            }
        }
    }
}