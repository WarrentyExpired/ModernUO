using ModernUO.Serialization;
using Server.Items;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class BountyToken : Item
{
    [Constructible]
    public BountyToken() : this(1)
    {
    }

    [Constructible]
    public BountyToken(int amount) : base(0x0EED) // Gold coin artwork
    {
        Name = "Bounty Token";
        Hue = 0x482; // Unique vibrant cyan/blue hue
        Stackable = true;
        Amount = amount;
        LootType = LootType.Blessed; // Keeps tokens safe in dungeons upon death
    }
    public override double DefaultWeight => 0.001; // 1,000 tokens per 1 stone
    // public override double DefaultWeight => 0.0; // Weightless
}
