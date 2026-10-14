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
    public class EscortToMirePort : TownEscort
    {
        public EscortToMirePort()
            : base(1072276, 1072290, 1072228, "the town of Mireport")
        {
        }
    }
    public class EscortToFairwinds : TownEscort
    {
        public EscortToFairwinds()
            : base(1072277, 1072291, 1072229, "the town of Fairwinds")
        {
        }
    }
    public class EscortToCresthaven : TownEscort
    {
        public EscortToCresthaven()
            : base(1072278, 1072292, 1072230, "the town of Cresthaven")
        {
        }
    }
    public class EscortToLandow : TownEscort
    {
        public EscortToLandow()
            : base(1072279, 1072293, 1072231, "the town of Landow")
        {
        }
    }
    public class EscortToEastRivermouth : TownEscort
    {
        public EscortToEastRivermouth()
            : base(1072280, 1072294, 1072232, "East Rivermouth")
        {
        }
    }
    public class EscortToWestRivermouth : TownEscort
    {
        public EscortToWestRivermouth()
            : base(1072281, 1072295, 1072233, "West Rivermouth")
        {
        }
    }
    public class EscortToStonehollow : TownEscort
    {
        public EscortToStonehollow()
            : base(1072282, 1072296, 1072234, "the town of Stonehollow")
        {
        }
    }
    public class EscortToColdbarrow : TownEscort
    {
        public EscortToColdbarrow()
            : base(1072283, 1072297, 1072235, "the town of Coldbarrow")
        {
        }
    }
    public class EscortToWharfsEnd : TownEscort
    {
        public EscortToWharfsEnd()
            : base(1072284, 1072298, 1072236, "the town of Wharf's End")
        {
        }
    }
    public class EscortToLandowDock : TownEscort
    {
        public EscortToLandowDock()
            : base(1072285, 1072299, 1072237, "the Landow Docks")
        {
        }
    }
    public class EscortToEndPointDock : TownEscort
    {
        public EscortToEndPointDock()
            : base(1072286, 1072300, 1072238, "the End Point Dock")
        {
        }
    }
}
