using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0)]
public partial class FarmableSpidersSilk : FarmableCrop
{
    [Constructible]
    public FarmableSpidersSilk() : base(0x0CC3)
    {
    }

    public static int GetCropID() => 3981;

    public override Item GetCropObject() => new SpidersSilk();

    public override int GetPickedID() => 3254;
}
