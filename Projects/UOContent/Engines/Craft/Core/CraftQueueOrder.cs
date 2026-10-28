using System;

namespace Server.Engines.Craft;

public class CraftQueueOrder
{
    public CraftItem Item { get; }
    public Type ResourceType { get; }
    public int AmountRequested { get; set; }
    public int AmountCompleted { get; set; }
    public int AmountExceptional { get; set; }
    public int AmountFailed { get; set; }
    public bool IsCompleted { get; set; }

    public CraftQueueOrder(CraftItem item, Type resourceType, int amountRequested)
    {
        Item = item;
        ResourceType = resourceType;
        AmountRequested = amountRequested;
    }

    /// <summary>
    /// Returns true if the order is explicitly flagged complete or has reached its target count.
    /// </summary>
    public bool IsFinished => IsCompleted || AmountCompleted >= AmountRequested;
}
