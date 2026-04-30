using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0)]
public partial class FarmableBlackPearl : FarmableCrop
{
    [Constructible]
    public FarmableBlackPearl() : base(3962)
    {
    }

    public static int GetCropID() => 3962;

    public override Item GetCropObject() => new BlackPearl();

    public override int GetPickedID() => 3254;
}
