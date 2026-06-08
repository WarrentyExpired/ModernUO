using System;
using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0)]
public abstract partial class FarmableCrop : Item
{
    [SerializableField(0)]
    private bool _picked;

    public FarmableCrop(int itemID) : base(itemID) => Movable = false;

    public abstract Item GetCropObject();
    public abstract int GetPickedID();
    public override void AddNameProperties(IPropertyList list)
    {
    }

    public override bool HandlesOnMovement => true;

    public override void OnMovement(Mobile m, Point3D oldLocation)
    {
        if (m.Player && m.Alive && !_picked && m.InRange(this.Location, 2))
        {
            Timer.DelayCall(TimeSpan.FromMilliseconds(100), () =>
            {
                if (m.InRange(this.Location, 2) && !_picked)
                {
                    OnPicked(m, Location, Map);
                }
            });
        }
        base.OnMovement(m, oldLocation);
    }

    public override bool OnMoveOver(Mobile from)
    {
        if (from != null && from.Alive && from.Player && !_picked)
        {
            OnPicked(from, this.Location, this.Map);
        }
        return base.OnMoveOver(from);
    }

    public override void OnDoubleClick(Mobile from)
    {
        var map = Map;
        var loc = Location;

        if (Parent != null || Movable || IsLockedDown || IsSecure || map == null || map == Map.Internal)
        {
            return;
        }

        if (!from.InRange(loc, 2) || !from.InLOS(this))
        {
            from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
        }
        else if (!_picked)
        {
            OnPicked(from, loc, map);
        }
    }

    public virtual void OnPicked(Mobile from, Point3D loc, Map map)
    {
        ItemID = GetPickedID();

        var spawn = GetCropObject();
        if (spawn != null)
        {
            if (from.Backpack == null || !from.Backpack.CheckHold(from, spawn, true, true))
            {
                spawn.MoveToWorld(loc, map);
                from.SendMessage("Your backpack is full, the crop was placed on the ground.");
            }
            else
            {
                from.AddToBackpack(spawn);
                from.PlaySound(0x13E); // Harvest sound
            }
        }
        _picked = true;
        Unlink();
        Timer.StartTimer(TimeSpan.FromMinutes(1.0), Delete);
    }

    public void Unlink()
    {
        if (Spawner != null)
        {
            Spawner.Remove(this);
            Spawner = null;
        }
    }

    [AfterDeserialization]
    public void AfterDeserialize()
    {
        if (_picked)
        {
            Delete();
        }
    }
}
