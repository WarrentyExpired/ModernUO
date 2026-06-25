using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0)]
public partial class FarmableGinseng : FarmableCrop
{
    [Constructible]
    public FarmableGinseng() : base(0x18E9)
    {
    }

    public static int GetCropID() => 3973;

    public override Item GetCropObject() => new Ginseng();

    public override int GetPickedID() => 3254;
}
