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

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class MagnetTrap : Item
{
    private TimerExecutionToken _timerToken;
    private TimerExecutionToken _deleteTimerToken;
    private readonly TimeSpan _autoDelete = TimeSpan.FromSeconds(30);
    private readonly Dictionary<PlayerMobile, (int Count, DateTime LastSeen)> _dropState = new();
    private DateTime _lastCleanup = Core.Now;

    private const int MetalDropChancePercent = 60;
    private const int MaxDropsPerPlayer = 3;
    private const int CleanupIntervalSeconds = 5;
    private const int DropStateStaleSeconds = 10;

    private static readonly HashSet<string> MetalResourceNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Iron", "Steel", "DullCopper", "ShadowIron", "Copper", "Bronze",
        "Gold", "Silver", "Agapite", "Verite", "Valorite"
    };

    [Constructible]
    public MagnetTrap() : base(0x1AC5)
    {
        Movable = false;
        Name = "a magnetic trap";
        Hue = 1341;
        Timer.StartTimer(TimeSpan.Zero, TimeSpan.FromSeconds(1), DoMagneticEffect, out _timerToken);
        StartDeleteTimer();
    }

    private void DoMagneticEffect()
    {
        if (Deleted)
            return;

        Effects.SendLocationParticles(
            EffectItem.Create(Location, Map, EffectItem.DefaultDuration),
            0x376A, 9, 32, 5024, 0, 5024, 0
        );
        Effects.PlaySound(Location, Map, 0x1F0);

        foreach (var m in GetMobilesInRange(1))
        {
            if (m is PlayerMobile pm)
                TryDropRandomItem(pm);
        }

        var now = Core.Now;
        if ((now - _lastCleanup).TotalSeconds >= CleanupIntervalSeconds)
        {
            _lastCleanup = now;
            CleanupDropState(now);
        }
    }

    private void TryDropRandomItem(PlayerMobile player)
    {
        if (player?.Deleted != false)
            return;

        var now = Core.Now;

        if (!_dropState.TryGetValue(player, out var state))
            state = (0, now);

        if (state.Count >= MaxDropsPerPlayer)
            return;

        _dropState[player] = (state.Count, now);

        var preferMetal = Utility.Random(100) < MetalDropChancePercent;
        var dropped = preferMetal
            ? TryDropFrom(player.Items, IsMetal, player) || TryDropFrom(player.Backpack?.Items, IsMetal, player)
            : TryDropFrom(player.Backpack?.Items, IsMetal, player) || TryDropFrom(player.Items, IsMetal, player);

        if (dropped)
            _dropState[player] = (state.Count + 1, now);
    }

    private bool TryDropFrom(IEnumerable<Item> source, Func<Item, bool> predicate, PlayerMobile player)
    {
        if (source is null)
            return false;

        Item candidate = null;
        var count = 0;

        foreach (var item in source)
        {
            if (item?.Movable != true || !CanDropItem(item) || !predicate(item))
                continue;

            count++;
            if (Utility.Random(count) == 0)
                candidate = item;
        }

        return candidate is not null && DropItemToGround(candidate, player);
    }

    private static bool CanDropItem(Item item)
    {
        if (item?.Movable != true)
            return false;

        if (item.LootType == LootType.Blessed || item.LootType == LootType.Newbied)
            return false;

        if (item is LockableContainer lockable && lockable.Locked)
            return false;

        return true;
    }

    private static bool IsMetal(Item item)
    {
        if (item is null)
            return false;

        if (item is BaseWeapon weapon)
            return IsMetalResource(weapon.Resource);

        if (item is BaseArmor armor)
            return IsMetalResource(armor.Resource);

        if (item is BaseShield shield)
            return IsMetalResource(shield.Resource);

        if (item is BaseJewel)
            return true;

        var typeName = item.GetType().Name;
        return typeName.Contains("sword", StringComparison.OrdinalIgnoreCase) ||
               typeName.Contains("axe", StringComparison.OrdinalIgnoreCase) ||
               typeName.Contains("mace", StringComparison.OrdinalIgnoreCase) ||
               typeName.Contains("dagger", StringComparison.OrdinalIgnoreCase) ||
               typeName.Contains("knife", StringComparison.OrdinalIgnoreCase) ||
               typeName.Contains("plate", StringComparison.OrdinalIgnoreCase) ||
               typeName.Contains("chain", StringComparison.OrdinalIgnoreCase) ||
               typeName.Contains("shield", StringComparison.OrdinalIgnoreCase) ||
               typeName.Contains("helmet", StringComparison.OrdinalIgnoreCase) ||
               typeName.Contains("gauntlet", StringComparison.OrdinalIgnoreCase) ||
               typeName.Contains("ring", StringComparison.OrdinalIgnoreCase) ||
               typeName.Contains("necklace", StringComparison.OrdinalIgnoreCase) ||
               typeName.Contains("earrings", StringComparison.OrdinalIgnoreCase) ||
               typeName.Contains("bracelet", StringComparison.OrdinalIgnoreCase) ||
               typeName.Contains("talisman", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsMetalResource(CraftResource resource)
    {
        var resName = CraftResources.GetName(resource);
        return !string.IsNullOrEmpty(resName) && MetalResourceNames.Contains(resName);
    }

    private bool DropItemToGround(Item item, PlayerMobile player)
    {
        if (item?.Movable != true || player is null || !CanDropItem(item))
            return false;

        try
        {
            item.MoveToWorld(player.Location, player.Map);
            player.SendLocalizedMessage(502690);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private void CleanupDropState(DateTime now)
    {
        var keysToRemove = new List<PlayerMobile>();

        foreach (var kv in _dropState)
        {
            var player = kv.Key;
            var lastSeen = kv.Value.LastSeen;
            if (player is null || player.Deleted || (now - lastSeen).TotalSeconds > DropStateStaleSeconds)
                keysToRemove.Add(player);
        }

        foreach (var key in keysToRemove)
            _dropState.Remove(key);
    }

    private void StartDeleteTimer()
    {
        _deleteTimerToken.Cancel();
        Timer.StartTimer(_autoDelete, Delete, out _deleteTimerToken);
    }

    public override void OnDelete()
    {
        _timerToken.Cancel();
        _deleteTimerToken.Cancel();
        _dropState.Clear();
        base.OnDelete();
    }
}