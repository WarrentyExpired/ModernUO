using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
[Flippable(0x2684, 0x2683)]
public partial class HoodedRobe : BaseOuterTorso
{
    [Constructible]
    public HoodedRobe() : this(0)
    {
    }

    [Constructible]
    public HoodedRobe(int hue) : base(0x2684, hue)
    {
        Weight = 3.0;
    }

    public override string DefaultName => "a hooded robe";
}
