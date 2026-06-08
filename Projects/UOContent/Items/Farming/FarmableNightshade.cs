using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0)]
public partial class FarmableNightshade : FarmableCrop
{
    [Constructible]
    public FarmableNightshade() : base(3976)
    {
    }

    public static int GetCropID() => 3976;

    public override Item GetCropObject() => new Nightshade();

    public override int GetPickedID() => 3254;
}
