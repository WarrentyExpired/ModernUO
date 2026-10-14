using System;
using ModernUO.Serialization;
using Server.Engines.MLQuests.Objectives;
using Server.Engines.MLQuests.Rewards;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.MLQuests.Definitions;

// This is not a real quest, it is only used as a reference
public class HumanInNeed : MLQuest
{
    public HumanInNeed()
    {
        Title = 1075011;
        Description = 0;
        RefusalMessage = 0;
        InProgressMessage = 0;
    }

    public override bool RecordCompletion => true;

    public static void AwardTo(PlayerMobile pm)
    {
        MLQuestSystem.GetOrCreateContext(pm).SetDoneQuest(MLQuestSystem.FindQuest(typeof(HumanInNeed)));
        pm.SendLocalizedMessage(1074949, "", 0x2A);
    }
}
