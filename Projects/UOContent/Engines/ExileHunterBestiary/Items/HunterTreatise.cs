using ModernUO.Serialization;
using Server.Network;

namespace Server.Custom.ExileHunterBestiary.Items;

[SerializationGenerator(0)]
public partial class HunterTreatise : Item
{
    [SerializableField(0)]
    [InvalidateProperties]
    [SerializedCommandProperty(AccessLevel.GameMaster)]
    private string _creatureId;

    public HunterCreatureEntry Entry =>
        BestiaryRegistry.TryGetById(_creatureId, out var entry) ? entry : null;

    [Constructible]
    public HunterTreatise(string creatureId) : base(0x1F5F)
    {
        _creatureId = creatureId;
        Weight = 1.0;
        LootType = LootType.Regular;
        UpdateAppearance(false);
    }

    [Constructible]
    public HunterTreatise() : this("Lich")
    {
    }

    public HunterTreatise(Serial serial) : base(serial)
    {
    }

    [AfterDeserialization]
    private void AfterDeserialization() => UpdateAppearance(false);

    private void UpdateAppearance(bool invalidate)
    {
        ItemID = 0x1F5F;

        if (Entry is { } entry)
        {
            Name = $"Hunter's Treatise: {entry.DisplayName}";
            Hue = entry.BookHue != 0 ? entry.BookHue : 2101;
        }
        else
        {
            Name = $"Hunter's Treatise: {_creatureId}";
            Hue = 2101;
        }

        if (invalidate)
        {
            InvalidateProperties();
        }
    }

    public override void OnDoubleClick(Mobile from)
    {
        if (!IsChildOf(from.Backpack))
        {
            from.SendLocalizedMessage(1042001);
            return;
        }

        var profile = HunterBestiaryEngine.GetProfile(from);
        if (profile == null)
        {
            return;
        }

        var displayName = Entry?.DisplayName ?? _creatureId ?? "Unknown";

        if (profile.HasLearned(_creatureId))
        {
            from.SendMessage(0x22, $"You have already mastered the study of {displayName}. Keep this treatise to trade with other hunters!");
            from.PlaySound(0x5C);
            return;
        }

        if (profile.Learn(_creatureId))
        {
            from.SendMessage(
                0x3F,
                $"You thoroughly study the treatise and permanently unlock +{HunterBestiaryEngine.PerCreatureBonus:P0} damage against {displayName}!"
            );
            from.FixedParticles(0x373A, 10, 15, 5012, EffectLayer.Waist);
            from.PlaySound(0x249);
            Consume();
        }
    }

    public override void GetProperties(IPropertyList list)
    {
        base.GetProperties(list);

        var creatureName = Entry?.DisplayName ?? _creatureId ?? "Unknown";
        var category = Entry?.Category.ToString() ?? "Unknown";

        list.Add(
            1070722,
            $"{$"<BASEFONT COLOR=#D1C4E9>Hunter's Study: +{HunterBestiaryEngine.PerCreatureBonus:P0} vs {creatureName}<BASEFONT COLOR=#FFFFFF>"}"
        );
        list.Add(1070722, $"{$"<BASEFONT COLOR=#ECEFF1>Category: {category}<BASEFONT COLOR=#FFFFFF>"}");
        list.Add(1070722, $"{"<BASEFONT COLOR=#FFE082>Double-click to permanently master<BASEFONT COLOR=#FFFFFF>"}");
    }

    public override void SendPropertiesTo(NetState ns)
    {
        if (ns?.Mobile is not { } from)
        {
            return;
        }

        var list = new ObjectPropertyList(this);
        GetProperties(list);

        var learned = HunterBestiaryEngine.TryGetProfile(from, out var profile) && profile.HasLearned(_creatureId);
        list.Add(
            1070722,
            learned
                ? $"{"<BASEFONT COLOR=#00E676>[Already Learned]<BASEFONT COLOR=#FFFFFF>"}"
                : $"{"<BASEFONT COLOR=#00E5FF>[Not Learned Yet]<BASEFONT COLOR=#FFFFFF>"}"
        );

        list.Terminate();
        ns.Send(list.Buffer);
    }
}
