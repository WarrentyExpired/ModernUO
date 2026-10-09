using System;
using System.Collections.Generic;
using ModernUO.Serialization;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Mobiles;

[SerializationGenerator(0, false)]
public partial class BountyVendor : BaseCreature
{
    [Constructible]
    public BountyVendor() : base(AIType.AI_Vendor, FightMode.None, 2)
    {
        Title = "the Bounty Quartermaster";
        Race = Race.Human;
        Body = 0x190;
        Female = false;
        Hue = Race.RandomSkinHue();

        SetSpeed(0.5, 2.0);
        InitStats(100, 100, 25);

        Utility.AssignRandomHair(this);

        AddItem(new Backpack());
        AddItem(new FancyShirt(0x482));
        AddItem(new LongPants(0x1F0D));
        AddItem(new Boots());
        AddItem(new Cloak(0x482));
    }

    public override bool IsInvulnerable => true;
    public override string DefaultName => "Capt. Vance";

    public override void OnDoubleClick(Mobile from)
    {
        if (from is PlayerMobile pm)
        {
            if (from.InRange(Location, 3))
            {
                pm.SendGump(new BountyRewardGump(pm, this));
            }
            else
            {
                from.SendLocalizedMessage(500446); // That is too far away.
            }
        }
    }
}

// ==========================================
// BOUNTY REWARD SHOP GUMP
// ==========================================
public class BountyRewardGump : Gump
{
    private readonly PlayerMobile _player;
    private readonly BountyVendor _vendor;

    // Define Reward Store Items using factory functions: (Name, TokenCost, CreateItem)
    private static readonly (string Name, int Cost, Func<Item> CreateItem)[] Rewards = new (string, int, Func<Item>)[]
    {
        ("Dungeon Hunter Ring", 25, () => new GoldRing()),
        ("Decorative Dragon Statue", 50, () => new MonsterStatuette(MonsterStatuetteType.Dragon)),
        ("Monster Statue Dyetub", 75, () => new StatuetteDyeTub()),
        ("Bag of Sending", 100, () => new BagOfSending())
    };

    public BountyRewardGump(PlayerMobile player, BountyVendor vendor) : base(50, 50)
    {
        _player = player;
        _vendor = vendor;

        int totalTokens = player.Backpack != null ? player.Backpack.GetAmount(typeof(BountyToken)) : 0;

        AddPage(0);

        // Matching BuildWorld.cs Frame Style
        AddBackground(0, 0, 450, 310, 9270);
        AddAlphaRegion(10, 10, 430, 290);

        // Header Labels
        AddLabel(110, 15, 1152, "BOUNTY QUARTERMASTER STORE");
        AddLabel(20, 40, 0x3F, $"Your Bounty Tokens: {totalTokens}");

        int y = 75;
        for (int i = 0; i < Rewards.Length; i++)
        {
            var reward = Rewards[i];

            // Item Purchase Button & Labels
            AddButton(20, y, 4005, 4007, i + 1, GumpButtonType.Reply, 0);
            AddLabel(55, y, 1152, reward.Name);
            AddLabel(300, y, 0x3F, $"{reward.Cost} Tokens");

            y += 35;
        }

        // Close Button
        AddButton(20, y + 10, 4017, 4019, 0, GumpButtonType.Reply, 0);
        AddLabel(55, y + 10, 1152, "Close Shop");
    }

    public override void OnResponse(NetState sender, in RelayInfo info)
    {
        if (info.ButtonID <= 0 || info.ButtonID > Rewards.Length)
        {
            return;
        }

        int index = info.ButtonID - 1;
        var reward = Rewards[index];

        if (_player.Backpack == null)
        {
            return;
        }

        int tokenCount = _player.Backpack.GetAmount(typeof(BountyToken));

        if (tokenCount >= reward.Cost)
        {
            if (_player.Backpack.ConsumeTotal(typeof(BountyToken), reward.Cost))
            {
                Item newItem = reward.CreateItem();
                _player.AddToBackpack(newItem);

                _player.SendMessage(0x35, $"You purchased {reward.Name} for {reward.Cost} Bounty Tokens!");
            }
        }
        else
        {
            _player.SendMessage(0x22, $"You need {reward.Cost - tokenCount} more Bounty Tokens to buy this item.");
        }

        // Reopen gump to refresh balance
        _player.SendGump(new BountyRewardGump(_player, _vendor));
    }
}
