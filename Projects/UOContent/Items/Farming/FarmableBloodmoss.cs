using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0)]
public partial class FarmableBloodmoss : FarmableCrop
{
    [Constructible]
    public FarmableBloodmoss() : base(3963)
    {
    }

    public static int GetCropID() => 3963;

    public override Item GetCropObject() => new Bloodmoss();

    public override int GetPickedID() => 3254;
}
