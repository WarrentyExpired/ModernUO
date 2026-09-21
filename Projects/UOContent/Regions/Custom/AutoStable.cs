using System.Collections.Generic;
using Server;
using Server.Mobiles;

namespace Server.Custom;

public static class AutoStable
{
    // Changed from private to internal so the persistence class can read/write it
    internal static readonly Dictionary<Serial, BaseMount> _storedMounts = new();

    public static void StoreMount(PlayerMobile player)
    {
        if (player == null || player.Deleted)
        {
            return;
        }

        if (player.Mount is not BaseMount mount || mount.Deleted)
        {
            return;
        }

        _storedMounts[player.Serial] = mount;

        BaseMount.Dismount(player);

        mount.ControlTarget = null;
        mount.ControlOrder = OrderType.Stay;

        mount.Internalize();

        mount.SetControlMaster(null);
        mount.SummonMaster = null;

        mount.IsStabled = true;
        mount.StabledBy = player;
        mount.Loyalty = BaseCreature.MaxLoyalty;

        player.AddStabled(mount);
    }

    public static void RestoreMount(PlayerMobile player)
    {
        if (player == null || player.Deleted)
        {
            return;
        }

        if (!_storedMounts.TryGetValue(player.Serial, out var mount))
        {
            return;
        }

        if (mount == null || mount.Deleted)
        {
            _storedMounts.Remove(player.Serial);
            return;
        }

        if (!player.Alive)
        {
            return;
        }

        if (player.Mounted)
        {
            return;
        }

        if (!mount.IsStabled || mount.StabledBy != player)
        {
            _storedMounts.Remove(player.Serial);
            return;
        }

        if (player.Followers + mount.ControlSlots > player.FollowersMax)
        {
            player.SendLocalizedMessage(1049612, mount.Name);
            return;
        }

        player.RemoveStabled(mount);

        mount.IsStabled = false;
        mount.StabledBy = null;

        mount.SetControlMaster(player);
        mount.ControlTarget = player;
        mount.ControlOrder = OrderType.Follow;
        mount.Loyalty = BaseCreature.MaxLoyalty;

        _storedMounts.Remove(player.Serial);

        mount.MoveToWorld(player.Location, player.Map);
        mount.Rider = player;
    }

    public static bool HasStoredMount(PlayerMobile player)
    {
        if (player == null)
        {
            return false;
        }

        return _storedMounts.TryGetValue(player.Serial, out var mount) &&
               mount != null &&
               !mount.Deleted;
    }

    public static void ForgetMount(PlayerMobile player)
    {
        if (player == null)
        {
            return;
        }

        _storedMounts.Remove(player.Serial);
    }
}

// ============================================================================
// NEW: ModernUO Persistence Hooks to save the dictionary on server restart
// ============================================================================
public class AutoStablePersistence : GenericPersistence
{
    public static AutoStablePersistence Instance { get; private set; }

    // ModernUO automatically looks for Configure() methods to initialize systems
    public static void Configure()
    {
        Instance = new AutoStablePersistence();
    }

    // Tells the server to create a file named "AutoStable.bin" in the Saves/Generic/ folder
    public AutoStablePersistence() : base("AutoStable", 1)
    {
    }

    public override void Serialize(IGenericWriter writer)
    {
        writer.WriteEncodedInt(0); // Script Version

        // Save how many mounts we are tracking
        writer.WriteEncodedInt(AutoStable._storedMounts.Count);

        foreach (var kvp in AutoStable._storedMounts)
        {
            // Cast the Serial's underlying value to a standard int for safe writing
            writer.Write((int)kvp.Key.Value);
            writer.Write(kvp.Value);
        }
    }

    public override void Deserialize(IGenericReader reader)
    {
        var version = reader.ReadEncodedInt();
        var count = reader.ReadEncodedInt();

        for (var i = 0; i < count; i++)
        {
            // Read the int, convert it to an unsigned int, then cast it back to a Serial
            Serial playerSerial = (Serial)(uint)reader.ReadInt();
            var mount = reader.ReadEntity<BaseMount>();

            // If the mount still exists in the world, restore the tracking link
            if (mount != null && !mount.Deleted)
            {
                AutoStable._storedMounts[playerSerial] = mount;
            }
        }
    }
}
