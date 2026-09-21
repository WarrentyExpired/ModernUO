using ModernUO.Serialization;
using Server.Custom.ExileHunterBestiary.Gumps;
using Server.Network;

namespace Server.Custom.ExileHunterBestiary.Items;

[SerializationGenerator(0)]
public partial class HunterBestiary : Item
{
    public const int DefaultBestiaryHue = 1153;

    [Constructible]
    public HunterBestiary() : base(0x2252)
    {
        Name = "The Grand Hunter's Bestiary";
        Hue = DefaultBestiaryHue;
        Weight = 2.0;
        LootType = LootType.Blessed;
    }

    public HunterBestiary(Serial serial) : base(serial)
    {
    }

    public override void OnDoubleClick(Mobile from)
    {
        if (!IsChildOf(from.Backpack))
        {
            from.SendLocalizedMessage(1042001);
            return;
        }

        HunterBestiaryGump.DisplayTo(from);
    }

    public override void GetProperties(IPropertyList list)
    {
        base.GetProperties(list);

        list.Add(1070722, $"{"<BASEFONT COLOR=#D1C4E9>Grand Grimoire of Monster Lore<BASEFONT COLOR=#FFFFFF>"}");
        list.Add(1070722, $"{"<BASEFONT COLOR=#ECEFF1>Tracks mastered treatises and combat bonuses<BASEFONT COLOR=#FFFFFF>"}");
        list.Add(1070722, $"{"<BASEFONT COLOR=#FFE082>Double-click to open Bestiary<BASEFONT COLOR=#FFFFFF>"}");
    }

    public override void SendPropertiesTo(NetState ns)
    {
        if (ns?.Mobile is not { } from)
        {
            return;
        }

        var list = new ObjectPropertyList(this);
        GetProperties(list);

        var mastered = HunterBestiaryEngine.TryGetProfile(from, out var profile) ? profile.TotalMastered : 0;
        var total = BestiaryRegistry.TotalCount;

        list.Add(
            1070722,
            $"{$"<BASEFONT COLOR=#00FFCC>Mastery Progress: {mastered} / {total} Creatures<BASEFONT COLOR=#FFFFFF>"}"
        );

        list.Terminate();
        ns.Send(list.Buffer);
    }
}
