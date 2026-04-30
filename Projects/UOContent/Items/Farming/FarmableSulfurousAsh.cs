using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0)]
public partial class FarmableSulfurousAsh : FarmableCrop
{
    [Constructible]
    public FarmableSulfurousAsh() : base(3980)
    {
    }

    public static int GetCropID() => 3980;

    public override Item GetCropObject() => new SulfurousAsh();

    public override int GetPickedID() => 3254;
}
