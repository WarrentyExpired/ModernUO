/**********************************
* Author: Expa                    *
* Optimized ModernUO .Net 10/C#14 *
**********************************/
using System;
using System.Collections.Generic;
using ModernUO.Serialization;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items;

static class CurseConfig
{
    public const int ScrollId = 0x1F2E;
    public const int ScrollHue = 0x58;
    public const int TrapId = 0x1B7B;
    public const int Range = 1;
    public const int SoundRemove = 0x1F7;
    public const int SoundTrap = 0x1E1;
    public const int PartRemove = 0x373A;
    public const int PartTrap = 0x374B;
    public const string TrapName = "Hidden Curse Trap";
    public static readonly TimeSpan Cooldown = TimeSpan.FromMinutes(5);
    public const bool ShowWhileCooling = true;
    public const string Suffix = " [The Cursed!]";
    public const string MsgNotInPack = "Scroll must be in backpack.";
    public const string MsgPrompt = "Choose cursed item.";
    public const string MsgNotCursed = "Not cursed.";
    public const string MsgSelect = "Select an item.";
    public const string MsgRemovedPack = "Curse removed (backpack).";
    public const string MsgRemovedGround = "Curse removed (ground).";
    public const string MsgReload = "Trap reloading!";
    public const string MsgCursed = "Item cursed and moved.";
}

[SerializationGenerator(0, false)]
public partial class CurseRemovalScroll : Item
{
    [Constructible]
    public CurseRemovalScroll() : base(CurseConfig.ScrollId) =>
        Name = "Curse Removal Scroll";

    public CurseRemovalScroll(Serial s) : base(s) { }

    public override void OnDoubleClick(Mobile from)
    {
        if (from?.Backpack is null || !this.IsChildOf(from.Backpack))
        {
            from?.SendMessage(CurseConfig.MsgNotInPack);
            return;
        }

        from.Target = new T(this);
        from.SendMessage(CurseConfig.MsgPrompt);
    }

    private sealed class T : Target
    {
        private readonly CurseRemovalScroll _s;

        public T(CurseRemovalScroll s) : base(3, false, TargetFlags.None) => _s = s;

        protected override void OnTarget(Mobile from, object target)
        {
            if (_s?.Deleted != false || from?.Deleted != false)
                return;

            if (target is not Item it)
            {
                from.SendMessage(CurseConfig.MsgSelect);
                return;
            }

            var isCursed = it.LootType == LootType.Cursed ||
                (it.Name?.EndsWith(CurseConfig.Suffix, StringComparison.Ordinal) == true);

            if (!isCursed)
            {
                from.SendMessage(CurseConfig.MsgNotCursed);
                return;
            }

            it.LootType = LootType.Regular;
            it.Movable = true;

            if (!string.IsNullOrEmpty(it.Name) && it.Name.EndsWith(CurseConfig.Suffix, StringComparison.Ordinal))
            {
                var baseName = it.Name[..^CurseConfig.Suffix.Length];
                it.Name = string.IsNullOrEmpty(baseName) ? null : baseName;
            }

            from.PlaySound(CurseConfig.SoundRemove);
            from.FixedParticles(CurseConfig.PartRemove, 10, 15, 5018, EffectLayer.Waist);
            from.SendMessage(it.IsChildOf(from.Backpack) ? CurseConfig.MsgRemovedPack : CurseConfig.MsgRemovedGround);
            _s.Delete();
        }
    }
}

[SerializationGenerator(0, false)]
public partial class CurseTrap : BaseTrap
{
    private DateTime _next = DateTime.Now;

    [Constructible]
    public CurseTrap() : base(CurseConfig.TrapId) => InitTrap();

    public CurseTrap(Serial s) : base(s) => InitTrap();

    private void InitTrap()
    {
        Name = CurseConfig.TrapName;
        Movable = false;
        Visible = false;
    }

    public override bool PassivelyTriggered => true;
    public override TimeSpan PassiveTriggerDelay => TimeSpan.Zero;
    public override int PassiveTriggerRange => CurseConfig.Range;
    public override TimeSpan ResetDelay => TimeSpan.Zero;

    public override void OnTrigger(Mobile from)
    {
        if (from?.Alive != true || !from.Player)
            return;

        if (Core.Now < _next)
        {
            from.SendMessage(CurseConfig.MsgReload);
            return;
        }

        var item = SelectTarget(from);
        if (item == null)
            return;

        var oldName = item.Name;
        var oldLoot = item.LootType;
        var oldMovable = item.Movable;

        try
        {
            if (item.Parent == from)
                from.RemoveItem(item);

            item.LootType = LootType.Regular;
            item.Movable = false;

            if (!(item.Name?.EndsWith(CurseConfig.Suffix) == true))
            {
                var baseName = oldName ?? item.ItemData.Name ?? item.GetType().Name;
                item.Name = baseName + CurseConfig.Suffix;
            }

            if (!item.IsChildOf(from.Backpack) && !TryPlaceInBackpack(from, item))
                item.MoveToWorld(from.Location, from.Map);

            from.PlaySound(CurseConfig.SoundTrap);
            from.FixedParticles(CurseConfig.PartTrap, 10, 10, 5044, EffectLayer.Head);
            from.SendMessage(CurseConfig.MsgCursed);
            _next = Core.Now + CurseConfig.Cooldown;
        }
        catch
        {
            if (item?.Deleted == false)
            {
                item.LootType = oldLoot;
                item.Movable = oldMovable;
                item.Name = oldName;
            }
        }
    }

    private static bool TryPlaceInBackpack(Mobile to, Item it)
    {
        if (to?.Backpack is null || it.IsChildOf(to.Backpack))
            return false;

        try
        {
            return to.Backpack.TryDropItem(to, it, false);
        }
        catch
        {
            try
            {
                to.Backpack.DropItem(it);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    private static Item SelectTarget(Mobile m)
    {
        if (m == null)
            return null;

        double totalWeight = 0.0;
        Item firstCandidate = null;

        foreach (var itm in m.Items)
        {
            if (itm?.Deleted != false)
                continue;

            var inEquip = itm.Parent == m;
            var inPack = m.Backpack != null && itm.IsChildOf(m.Backpack);

            if (!inEquip && !inPack)
                continue;

            if (itm.LootType == LootType.Cursed || !itm.Movable)
                continue;

            if (itm.Layer is Layer.Backpack or Layer.Hair or Layer.FacialHair)
                continue;

            var weight = CalculateWeight(itm);
            if (weight <= 0.0)
                continue;

            if (firstCandidate == null)
                firstCandidate = itm;

            totalWeight += weight;
        }

        if (totalWeight <= 0.0)
            return firstCandidate;

        var roll = Utility.RandomDouble() * totalWeight;
        var accumulated = 0.0;

        foreach (var itm in m.Items)
        {
            if (itm?.Deleted != false)
                continue;

            var inEquip = itm.Parent == m;
            var inPack = m.Backpack != null && itm.IsChildOf(m.Backpack);

            if (!inEquip && !inPack)
                continue;

            if (itm.LootType == LootType.Cursed || !itm.Movable)
                continue;

            if (itm.Layer is Layer.Backpack or Layer.Hair or Layer.FacialHair)
                continue;

            var weight = CalculateWeight(itm);
            if (weight <= 0.0)
                continue;

            accumulated += weight;
            if (accumulated >= roll)
                return itm;
        }

        return firstCandidate;
    }

    private static double CalculateWeight(Item it)
    {
        var baseWeight = it.Amount > 1 ? Math.Min(it.Amount, 100) : 1.0;

        return it switch
        {
            Gold g => baseWeight + Math.Min(g.Amount, 1000) / 10.0,
            Bandage => baseWeight + 50,
            Arrow => baseWeight + 50,
            Bolt => baseWeight + 50,
            SpellScroll => baseWeight + 40,
            BasePotion => baseWeight + 40,
            BaseReagent => baseWeight + 40,
            Spellbook => baseWeight + 60,
            Runebook => baseWeight + 80,
            _ => GetByTypeName(it, baseWeight)
        };
    }

    private static double GetByTypeName(Item it, double baseWeight)
    {
        var typeName = it.GetType().Name;

        if (typeName.Contains("scroll", StringComparison.OrdinalIgnoreCase))
            return baseWeight + 20;

        if (typeName.Contains("book", StringComparison.OrdinalIgnoreCase) && it is not Spellbook)
            return baseWeight + 15;

        return baseWeight;
    }
}