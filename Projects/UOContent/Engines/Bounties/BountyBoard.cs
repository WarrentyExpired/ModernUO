using System;
using ModernUO.Serialization;
using Server.Engines.Bounties;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Items;

[SerializationGenerator(0, false)]
[Flippable(0x1E5E, 0x1E5F)]
public partial class DungeonBountyBoard : Item
{
    [Constructible]
    public DungeonBountyBoard() : base(0x1E5E)
    {
        Name = "Dungeon Bounty Board";
        Movable = false;
    }

    public override void OnDoubleClick(Mobile from)
    {
        if (from is PlayerMobile pm)
        {
            if (from.InRange(Location, 3))
                pm.SendGump(new BountyBoardGump(pm));
            else
                from.SendLocalizedMessage(500446);
        }
    }
}

public class BountyBoardGump : Gump
{
    private readonly PlayerMobile _player;

    public BountyBoardGump(PlayerMobile player) : base(50, 50)
    {
        _player = player;
        BountyProfile profile = BountySystem.GetProfile(player);

        AddPage(0);
        AddBackground(0, 0, 550, 480, 9270);
        AddAlphaRegion(10, 10, 530, 460);
        AddLabel(180, 15, 1152, "DUNGEON BOUNTY BOARD");

        // --- ACTIVE BOUNTY SECTION ---
        AddLabel(20, 50, 0x3F, "--- Your Active Bounty ---");

        if (profile.HasActiveBounty && BountyRegistry.AllBounties.TryGetValue(profile.ActiveBountyID, out var activeBounty))
        {
            AddLabel(20, 75, 1152, activeBounty.Title);
            AddLabel(350, 75, 2100, $"{profile.CurrentKills} / {activeBounty.RequiredKills} Slain");

            if (profile.CurrentKills >= activeBounty.RequiredKills)
            {
                AddButton(20, 100, 4005, 4007, 1000, GumpButtonType.Reply, 0);
                AddLabel(55, 100, 0x44, "CLAIM REWARD");
            }
            else
            {
                int penaltyCost = activeBounty.RewardGold; // Cancellation costs the reward amount
                AddButton(20, 100, 4017, 4019, 1001, GumpButtonType.Reply, 0);
                AddLabel(55, 100, 33, $"Refuse Bounty (Cost: {penaltyCost} Gold)");
            }
        }
        else
        {
            AddLabel(20, 75, 1152, "You have no active bounties.");
        }

        // --- AVAILABLE BOUNTIES SECTION ---
        AddLabel(20, 150, 0x3F, "--- Available Bounties ---");

        if (profile.IsOnCooldown)
        {
            TimeSpan wait = profile.NextAvailableTime - DateTime.UtcNow;
            AddLabel(20, 180, 33, $"The board is empty. Next bounties arrive in {(int)wait.TotalMinutes} minutes.");
        }
        else
        {
            AddLabel(20, 170, 2100, $"Your Current Fame: {player.Fame}");

            int y = 200;
            for (int tier = 1; tier <= 5; tier++)
            {
                // Grab an example from the tier to check fame requirements and rewards
                var example = BountyRegistry.GetRandomBounty(tier);
                if (example == null) continue;

                if (player.Fame >= example.RequiredFame)
                {
                    if (!profile.HasActiveBounty)
                    {
                        // Assigning Button IDs 101 to 105 for Tiers 1-5
                        AddButton(20, y, 4005, 4007, 100 + tier, GumpButtonType.Reply, 0);
                    }
                    AddLabel(55, y, 1152, $"Request Tier {tier} Bounty");
                    AddLabel(320, y, 0x3F, $"Earn {example.RewardTokens} Tokens");
                }
                else
                {
                    AddLabel(55, y, 33, $"Tier {tier} Bounty (Requires {example.RequiredFame} Fame)");
                }
                y += 35;
            }
        }

        AddButton(20, 440, 4017, 4019, 0, GumpButtonType.Reply, 0);
        AddLabel(55, 440, 1152, "Close Board");
    }

    public override void OnResponse(NetState sender, in RelayInfo info)
    {
        if (info.ButtonID == 0) return;

        BountyProfile profile = BountySystem.GetProfile(_player);

        if (info.ButtonID == 1000) // CLAIM REWARD
        {
            if (profile.HasActiveBounty && BountyRegistry.AllBounties.TryGetValue(profile.ActiveBountyID, out var activeBounty))
            {
                if (profile.CurrentKills >= activeBounty.RequiredKills)
                {
                    _player.AddToBackpack(new BountyToken(activeBounty.RewardTokens));
                    _player.AddToBackpack(new Gold(activeBounty.RewardGold));
                    _player.SendMessage(0x44, $"Complete! Received {activeBounty.RewardTokens} Tokens and {activeBounty.RewardGold} Gold.");
                    _player.PlaySound(0x2E6);

                    // Reset Profile and trigger the 60 minute cooldown
                    profile.ActiveBountyID = -1;
                    profile.CurrentKills = 0;
                    profile.NextAvailableTime = DateTime.UtcNow + TimeSpan.FromMinutes(ValidSettings.BountyBoardDelay());
                }
            }
        }
        else if (info.ButtonID == 1001) // CANCEL BOUNTY (Refusal Fee)
        {
            if (BountyRegistry.AllBounties.TryGetValue(profile.ActiveBountyID, out var activeBounty))
            {
                int cost = activeBounty.RewardGold;

                // Attempt to consume gold from their backpack
                if (_player.Backpack != null && _player.Backpack.ConsumeTotal(typeof(Gold), cost))
                {
                    profile.ActiveBountyID = -1;
                    profile.CurrentKills = 0;
                    profile.NextAvailableTime = DateTime.UtcNow + TimeSpan.FromMinutes(ValidSettings.BountyBoardDelay());
                    _player.SendMessage(33, $"You paid the {cost} gold refusal fee and abandoned the bounty.");
                }
                else
                {
                    _player.SendMessage(33, $"You need {cost} gold in your backpack to refuse this bounty!");
                }
            }
        }
        else if (info.ButtonID > 100 && info.ButtonID <= 105) // ACCEPT RANDOM BOUNTY BY TIER
        {
            int selectedTier = info.ButtonID - 100;

            if (!profile.HasActiveBounty && !profile.IsOnCooldown)
            {
                var newBounty = BountyRegistry.GetRandomBounty(selectedTier);

                if (newBounty != null && _player.Fame >= newBounty.RequiredFame)
                {
                    profile.ActiveBountyID = newBounty.ID;
                    profile.CurrentKills = 0;
                    _player.SendMessage(0x3F, $"You have been assigned: {newBounty.Title}");
                    _player.PlaySound(0x04A);
                }
            }
        }

        _player.SendGump(new BountyBoardGump(_player));
    }
}
