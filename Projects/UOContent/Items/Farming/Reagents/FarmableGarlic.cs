using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0)]
public partial class FarmableGarlic : FarmableCrop
{
    [Constructible]
    public FarmableGarlic() : base(0xC6F)
    {
    }

    public static int GetCropID() => 3972;

    public override Item GetCropObject() => new Garlic();

    public override int GetPickedID() => 3254;
}
