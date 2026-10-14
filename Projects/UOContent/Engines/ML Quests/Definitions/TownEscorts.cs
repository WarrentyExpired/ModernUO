using Server.Engines.MLQuests.Objectives;
using Server.Engines.MLQuests.Rewards;
using Server.Items;

namespace Server.Engines.MLQuests.Definitions
{
    public class TownEscort : BaseEscort
    {
        // Escort reward
        private static readonly BaseReward m_Reward = new ItemReward("Gold", typeof(Gold), 500);

        public TownEscort(int title, int progress, int destination, string region)
        {
            Activated = true;
            Title = title;
            Description =
                1072287;
            RefusalMessage =
                1072288;
            InProgressMessage = progress;

            Objectives.Add(new EscortObjective(new QuestArea(destination, region)));

            Rewards.Add(m_Reward);
        }
    }

    public class EscortToBastion : TownEscort
    {
        public EscortToBastion()
            : base(1072275, 1072289, 1072227, "the town of Bastion")
        {
        }
    }
}
