using System;
using System.Collections.Generic;
using ModernUO.Serialization;
using Server.Engines.MLQuests;
using Server.Engines.MLQuests.Objectives;
using Server.Engines.MLQuests.Rewards;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.MLQuests.Definitions;

// ==========================================
// 1. BOUNTY BOARD ITEM
// ==========================================
[SerializationGenerator(0, false)]
public partial class DungeonBountyBoard : Item, IQuestGiver
{
    // Implementation for IQuestGiver
    public List<MLQuest> MLQuests => MLQuestSystem.FindQuestList(GetType());

    [Constructible]
    public DungeonBountyBoard() : base(0x1E5E) // Bulletin board Graphic ID
    {
        Name = "Dungeon Bounty Board";
        Movable = false;
    }

    public override void OnDoubleClick(Mobile from)
    {
        if (from is PlayerMobile pm)
        {
            if (from.InRange(GetWorldLocation(), 3))
            {
                MLQuestSystem.OnDoubleClick(this, pm);
            }
            else
            {
                from.SendLocalizedMessage(500446); // That is too far away.
            }
        }
    }
}

// ==========================================
// 2. DESTARD BOUNTY: DRAKES
// ==========================================
public class DestardDrakeBounty : MLQuest
{
    public DestardDrakeBounty()
    {
        Activated = true;
        Title = "Bounty: Destard Drakes";

        Description = "Notice to all adventurers: Drake activity in the caverns of Destard has surged! Slay 10 Drakes to thin their numbers and keep the surrounding passes safe.";
        RefusalMessage = "The bounty remains posted if you change your mind.";
        InProgressMessage = "You haven't slain enough Drakes yet. Return once you've dispatched 10 of them in Destard.";
        CompletionMessage = "Excellent work! The realm thanks you for your service. Here is your gold reward.";
        CompletionNotice = CompletionNoticeShort;

        // Objective: Slay 10 Drakes
        Objectives.Add(new KillObjective(10, new[] { typeof(Drake) }, "Drakes"));

        // Reward: 1,200 Gold
        Rewards.Add(new ItemReward("1,200 Gold", typeof(Gold), 1200));
    }

    public override void Generate()
    {
        base.Generate();
        MLQuestSystem.Register(this, typeof(DungeonBountyBoard));
    }
}

// ==========================================
// 3. DECEIT BOUNTY: SKELETAL KNIGHTS
// ==========================================
public class DeceitSkeletalKnightBounty : MLQuest
{
    public DeceitSkeletalKnightBounty()
    {
        Activated = true;
        Title = "Bounty: Deceit Skeletal Knights";

        Description = "Undead forces are amassing within the halls of Deceit. Slay 10 Skeletal Knights and report back to claim your reward.";
        RefusalMessage = "The bounty remains posted on the board.";
        InProgressMessage = "The skeletal knights still roam Deceit. Return when you have destroyed 10 of them.";
        CompletionMessage = "The undead threat has been pushed back! Take your gold reward.";
        CompletionNotice = CompletionNoticeShort;

        // Objective: Slay 10 Skeletal Knights
        Objectives.Add(new KillObjective(10, new[] { typeof(SkeletalKnight) }, "Skeletal Knights"));

        // Reward: 1,500 Gold
        Rewards.Add(new ItemReward("1,500 Gold", typeof(Gold), 1500));
    }

    public override void Generate()
    {
        base.Generate();
        MLQuestSystem.Register(this, typeof(DungeonBountyBoard));
    }
}
