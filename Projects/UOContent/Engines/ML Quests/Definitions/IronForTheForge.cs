using System;
using System.Collections.Generic;
using ModernUO.Serialization;
using Server.Engines.MLQuests;
using Server.Engines.MLQuests.Objectives;
using Server.Engines.MLQuests.Rewards;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.MLQuests.Definitions;

public class IronForTheForge : MLQuest
{
    public IronForTheForge()
    {
        Activated = true;
        Title = "Iron for the Forge";

        Description = "Greetings traveler! The local guards have ordered a heavy shipment of armor, and my forge is running completely cold. Bring me 10 Iron Ingots so I can get back to work, and I will compensate you well.";
        RefusalMessage = "I understand. Raw iron is heavy work, but come back if you change your mind.";
        InProgressMessage = "Still looking for that iron? Mine it or buy it from a smithy, then bring 10 ingots back to me.";
        CompletionMessage = "Ah, magnificent quality iron! This will keep the forge roaring today. Here is your pay as promised.";
        CompletionNotice = CompletionNoticeShort;

        // Objective: Collect 10 Iron Ingots
        Objectives.Add(new CollectObjective(10, typeof(IronIngot), "Iron Ingots"));

        // Reward: 500 Gold
        Rewards.Add(new ItemReward("500 Gold", typeof(Gold), 500));
    }

    public override void Generate()
    {
        base.Generate();

        // Registers this quest and links it to QuestBlacksmith
        MLQuestSystem.Register(this, typeof(QuestBlacksmith));
    }
}

[SerializationGenerator(0, false)]
public partial class QuestBlacksmith : BaseCreature, IQuestGiver
{
    // Implementation for IQuestGiver
    public List<MLQuest> MLQuests => MLQuestSystem.FindQuestList(GetType());

    [Constructible]
    public QuestBlacksmith() : base(AIType.AI_Vendor, FightMode.None, 2)
    {
        Title = "the Blacksmith";
        Race = Race.Human;
        Body = 0x190;
        Female = false;
        Hue = Race.RandomSkinHue();

        SetSpeed(0.5, 2.0);
        InitStats(100, 100, 25);

        Utility.AssignRandomHair(this);

        AddItem(new Backpack());
        AddItem(new ShortPants(0x755));
        AddItem(new Shirt(Utility.RandomNeutralHue()));
        AddItem(new FullApron(Utility.RandomNeutralHue()));
        AddItem(new Boots());
        AddItem(new SmithHammer());
    }

    public override bool IsInvulnerable => true;
    public override string DefaultName => "Gareth";
}
