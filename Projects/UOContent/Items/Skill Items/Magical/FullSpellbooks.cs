/*************************************
* Author: Unknow  					 *
* Refactoring: Expa                  *
* Script Name: FullMagerySpellbook   *
* Optimized for ModernUO 0.15.4.18   *
*************************************/
using ModernUO.Serialization;
using Server;
using Server.Items;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class FullMagerySpellbook : Spellbook
{
    [Constructible]
    public FullMagerySpellbook() : base(MaskForBits(64), 0xEFA)
    {
        Layer = Layer.OneHanded;
    }

    public FullMagerySpellbook(Serial serial) : base(serial) { }

    private static ulong MaskForBits(int bits) => bits >= 64 ? ulong.MaxValue : ((1UL << bits) - 1UL);
}

[SerializationGenerator(0, false)]
public partial class FullNecroSpellbook : Spellbook
{
    public override SpellbookType SpellbookType => SpellbookType.Necromancer;
    public override int BookOffset => 100;
    public override int BookCount => Core.SE ? 17 : 16;

    [Constructible]
    public FullNecroSpellbook()
        : base(Core.SE ? MaskForBits(17) : MaskForBits(16), 0x2253)
    {
        Layer = Layer.OneHanded;
    }

    public FullNecroSpellbook(Serial serial) : base(serial) { }

    private static ulong MaskForBits(int bits) => bits >= 64 ? ulong.MaxValue : ((1UL << bits) - 1UL);
}

[SerializationGenerator(0, false)]
public partial class FullChivalrySpellbook : Spellbook
{
    public override SpellbookType SpellbookType => SpellbookType.Paladin;
    public override int BookOffset => 200;
    public override int BookCount => 10;

    [Constructible]
    public FullChivalrySpellbook()
        : base(MaskForBits(10), 0x2252)
    {
        Layer = Layer.OneHanded;
    }

    public FullChivalrySpellbook(Serial serial) : base(serial) { }

    private static ulong MaskForBits(int bits) => bits >= 64 ? ulong.MaxValue : ((1UL << bits) - 1UL);
}

[SerializationGenerator(0, false)]
public partial class FullBushidoSpellbook : Spellbook
{
    public override SpellbookType SpellbookType => SpellbookType.Samurai;
    public override int BookOffset => 400;
    public override int BookCount => 6;

    [Constructible]
    public FullBushidoSpellbook()
        : base(MaskForBits(6), 0x238C)
    {
        Layer = Layer.OneHanded;
    }

    public FullBushidoSpellbook(Serial serial) : base(serial) { }

    private static ulong MaskForBits(int bits) => bits >= 64 ? ulong.MaxValue : ((1UL << bits) - 1UL);
}

[SerializationGenerator(0, false)]
public partial class FullNinjitsuSpellbook : Spellbook
{
    public override SpellbookType SpellbookType => SpellbookType.Ninja;
    public override int BookOffset => 500;
    public override int BookCount => 8;

    [Constructible]
    public FullNinjitsuSpellbook()
        : base(MaskForBits(8), 0x23A0)
    {
        Layer = Layer.OneHanded;
    }

    public FullNinjitsuSpellbook(Serial serial) : base(serial) { }

    private static ulong MaskForBits(int bits) => bits >= 64 ? ulong.MaxValue : ((1UL << bits) - 1UL);
}
