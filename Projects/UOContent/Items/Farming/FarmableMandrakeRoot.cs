using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0)]
public partial class FarmableMandrakeRoot : FarmableCrop
{
    [Constructible]
    public FarmableMandrakeRoot() : base(3974)
    {
    }

    public static int GetCropID() => 3974;

    public override Item GetCropObject() => new MandrakeRoot();

    public override int GetPickedID() => 3254;
}
