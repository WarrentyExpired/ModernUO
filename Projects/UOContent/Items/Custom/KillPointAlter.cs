using System;
using ModernUO.Serialization;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class KillPointAltar : Item
{
    [Constructible]
    public KillPointAltar() : base(0xFFBC)
    {
        Name = "Altar of the Vanquished";
        //Hue = 0x489; // Arcane / Ethereal Hue
        Movable = false;
        Light = LightType.Circle225;
    }

    public override void OnDoubleClick(Mobile from)
    {
        if (from is not PlayerMobile pm)
        {
            return;
        }

        if (!from.InRange(GetWorldLocation(), 3))
        {
            from.SendLocalizedMessage(500446); // That is too far away.
            return;
        }

        pm.SendGump(new KillPointRewardGump(pm, this));
    }
}

// ==========================================
// MONSTER KILL POINTS REWARD GUMP
// ==========================================
public class KillPointRewardGump : Gump
{
    private readonly PlayerMobile _player;
    private readonly KillPointAltar _altar;

    // Rare UOR-style hues for special clothing rewards
    private static readonly int[] RareClothingHues =
    [
        1150, // Ice White
        1153, // Pure White
        1154, // Ice Blue
        1157, // Crimson / Blood Red
        1161, // Blaze
        1175, // Charcoal / True Black
        1266, // Electric Blue
        1272, // Neon Green
        2075, // Abyssal
        2747, // Sapphire
        1281, // Paragon Gold
        2732, // Zento Orchid
        2728, // Blue White
    ];

    // Define Altar Rewards: (Name, KillPointCost, CreateItem)
    private static readonly (string Name, int Cost, Func<Item> CreateItem)[] Rewards =
    [
        // --- SOULSTONES ---
        ("Soulstone Fragment (5 Charges)", 100000, () => new SoulstoneFragment()),
        ("SoulStone (Unlimited Transfers)", 250000, () => new SoulStone()),

        // --- ETHEREAL MOUNTS ---
        ("Ethereal Horse", 500000, () => new EtherealHorse()),
        ("Ethereal Llama", 500000, () => new EtherealLlama()),
        ("Ethereal Ostard", 500000, () => new EtherealOstard()),

        // --- RARE CLOTHING (Random Rare Hue + Blessed) ---
        ("Rare-Hued Sandals (Blessed)", 15000, () => CreateRareClothing(() => new Sandals())),
        ("Rare-Hued Half Apron (Blessed)", 15000, () => CreateRareClothing(() => new HalfApron())),
        ("Rare-Hued Cloak (Blessed)", 20000, () => CreateRareClothing(() => new Cloak())),
        ("Rare-Hued Hooded Robe (Blessed)", 25000, () => CreateRareClothing(() => new HoodedRobe())),
        ("Rare-Hued Tribal / Bear Mask (Blessed)", 30000, () =>
        {
            Item mask = Utility.Random(3) switch
            {
                0 => new BearMask(),
                1 => new DeerMask(),
                _ => new TribalMask()
            };
            mask.Hue = RareClothingHues.RandomElement();
            mask.LootType = LootType.Blessed;
            mask.Name = "a rare-hued ceremonial mask";
            return mask;
        }),
    ];

    private static Item CreateRareClothing(Func<BaseClothing> factory)
    {
        var item = factory();
        item.Hue = RareClothingHues.RandomElement();
        item.LootType = LootType.Blessed;
        return item;
    }

    public KillPointRewardGump(PlayerMobile player, KillPointAltar altar) : base(50, 50)
    {
        _player = player;
        _altar = altar;

        int height = 145 + (Rewards.Length * 35);

        AddPage(0);
        AddBackground(0, 0, 480, height, 9270);
        AddAlphaRegion(10, 10, 460, height - 20);

        AddLabel(135, 15, 2213, "ALTAR OF THE VANQUISHED");
        AddLabel(20, 40, 0x3F, $"Spendable Kill Points: {player.MonsterKillPoints:N0}");
        AddLabel(250, 40, 2401, $"Lifetime KP: {player.LifetimeKillPoints:N0}");

        int y = 75;
        for (int i = 0; i < Rewards.Length; i++)
        {
            var reward = Rewards[i];

            AddButton(20, y, 4005, 4007, i + 1, GumpButtonType.Reply, 0);
            AddLabel(55, y, 1152, reward.Name);
            AddLabel(340, y, 0x3F, $"{reward.Cost:N0} KP");

            y += 35;
        }

        // Close Button
        AddButton(20, y + 10, 4017, 4019, 0, GumpButtonType.Reply, 0);
        AddLabel(55, y + 10, 1152, "Step Away");
    }

    public override void OnResponse(NetState sender, in RelayInfo info)
    {
        if (info.ButtonID <= 0 || info.ButtonID > Rewards.Length)
        {
            return;
        }

        if (!_player.InRange(_altar.GetWorldLocation(), 3))
        {
            _player.SendLocalizedMessage(500446); // That is too far away.
            return;
        }

        int index = info.ButtonID - 1;
        var reward = Rewards[index];

        if (_player.Backpack == null)
        {
            return;
        }

        if (_player.MonsterKillPoints >= reward.Cost)
        {
            _player.MonsterKillPoints -= reward.Cost;

            Item newItem = reward.CreateItem();
            _player.AddToBackpack(newItem);

            _player.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
            _player.PlaySound(0x1F7);
            _player.SendMessage(0x35, $"The altar grants you {reward.Name} for {reward.Cost:N0} Kill Points!");
        }
        else
        {
            _player.SendMessage(0x22, $"You need {reward.Cost - _player.MonsterKillPoints:N0} more Kill Points for that offering.");
        }

        // Reopen gump to refresh balance
        _player.SendGump(new KillPointRewardGump(_player, _altar));
    }
}
