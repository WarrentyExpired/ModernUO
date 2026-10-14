using System;
using Server.Misc;
using Server.Mobiles;

namespace Server
{
    class ValidSettings
    {
		public static double BoneDecay()
		{
			if ( Settings.S_BoneDecay < 1 )
            {
                Settings.S_BoneDecay = 0;
            }
            else if (Settings.S_BoneDecay < 10 )
            {
                Settings.S_BoneDecay = 10;
            }
            else if (Settings.S_BoneDecay > 1440 )
            {
                Settings.S_BoneDecay = 1440;
            }
            return (double)Settings.S_BoneDecay;
        }

		public static double BondDays()
		{
			int days = Settings.S_BondDays;

			if ( days > 30 )
				days = 30;

			if ( days < 0 )
				days = 0;
			return (double)days;
		}
		public static int ExtraStableSlots()
		{
			int stable = Settings.S_Stables;

			if ( stable < 0 )
				stable = 0;

			if ( stable > 20 )
				stable = 20;

			return stable;
		}
        public static double BountyBoardDelay()
        {
            int mins = Settings.S_DungeonBountyBoardDelay;
            if ( mins > 1440 )
                mins = 1440;
            if (mins < 1 )
                mins = 1;
            return (double)mins;
        }
        public static double BountyRefusalDelay()
        {
            int mins = Settings.S_DungeonBountyRefusalDelay;
            if ( mins > 1440 )
                mins = 1440;
            if (mins < 1 )
                mins = 1;
            return (double)mins;
        }
        public static double RegionDungeonTrapsChance()
        {
            int chance = Settings.S_RegionDungeonTrapsChance;
            if (chance > 100)
                chance = 100;
            if (chance < 1 )
                chance = 1;
            return (double)chance;
        }
    }
}
