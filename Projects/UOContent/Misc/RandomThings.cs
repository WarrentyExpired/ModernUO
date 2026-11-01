using System;
using Server;
using System.Collections;
using Server.Misc;
using Server.Network;
using Server.Commands;
using Server.Commands.Generic;
using Server.Items;
using Server.Regions;

namespace Server.Misc
{
    class RandomThings
    {
        public static string GetRandomRealDungeon()
        {
            string rDungeon = "Unknown";
            switch( Utility.RandomMinMax( 0, 8 ))
            {
                case 0: rDungeon = "Dungeon Covetous"; break;
                case 1: rDungeon = "Dungeon Despice"; break;
                case 2: rDungeon = "Dungeon Deceit"; break;
                case 3: rDungeon = "Dungeon Destard"; break;
                case 4: rDungeon = "Dungeon Despise"; break;
                case 5: rDungeon = "Dungeon Shame"; break;
                case 6: rDungeon = "Dungeon Hythloth"; break;
                case 7: rDungeon = "Dungeon Terathan Keep"; break;
                case 8: rDungeon = "Dungeon of Ice"; break;
            }
            return rDungeon;
        }

        public static string GetRandomRealTown()
        {
            string rTown = "Unknown";
            switch( Utility.RandomMinMax( 0, 12 ))
            {
                case 0: rTown = "Bastion"; break;
                case 1: rTown = "Mireport"; break;
                case 2: rTown = "Myrish"; break;
                case 3: rTown = "Teareek"; break;
                case 4: rTown = "Landow"; break;
                case 5: rTown = "Rivermouth"; break;
                case 6: rTown = "Wharf's End"; break;
                case 7: rTown = "Coldbarrow"; break;
                case 8: rTown = "Fairwinds"; break;
                case 9: rTown = "Stonehollow"; break;
                case 10: rTown = "Cresthaven"; break;
                case 11: rTown = "Port Grim"; break;
                case 12: rTown = "Maerid"; break;
            }
            return rTown;
        }

        public static string GetRandomFakeDungeon()
        {
            string fDungeon = "Unknown";
            switch( Utility.RandomMinMax( 0, 2))
            {
                case 0: fDungeon = "Scourge Keep"; break;
                case 1: fDungeon = "Eliffs Cave"; break;
                case 2: fDungeon = "Crypts Elbow"; break;
            }
            return fDungeon;
        }

        public static string GetRandomFakeTown()
        {
            string fTown = "Unknown";
            switch( Utility.RandomMinMax( 0, 2))
            {
                case 0: fTown = "Klydeport"; break;
                case 1: fTown = "Longshore"; break;
                case 2: fTown = "Tessburg"; break;
            }
            return fTown;
        }

        public static string GetRandomAdjective()
        {
            string adj = "an odd";
            switch( Utility.RandomMinMax( 0, 6 ))
            {
                case 0: adj = "an odd"; break;
                case 1: adj = "a weirdly crafted"; break;
                case 2: adj = "a suspiciously shiny"; break;
                case 3: adj = "a blood-stained"; break;
                case 4: adj = "a masterfully forged"; break;
                case 5: adj = "an ancient, cursed"; break;
                case 6: adj = "a rusted but reliable"; break;
            }
            return adj;
        }

        public static string GetRandomName()
        {
            string name = "John";
            switch( Utility.RandomMinMax( 0, 7 ))
            {
                case 0: name = "Balthazar"; break;
                case 1: name = "Kaelen"; break;
                case 2: name = "Elias"; break;
                case 3: name = "Gwendolyn"; break;
                case 4: name = "Darius"; break;
                case 5: name = "Seraphina"; break;
                case 6: name = "Vander"; break;
                case 7: name = "Isolde"; break;
            }
            return name;
        }

        public static string GetRandomTitle()
        {
            string title = "King";
            switch( Utility.RandomMinMax( 0, 5 ))
            {
                case 0: title = "King"; break;
                case 1: title = "Queen"; break;
                case 2: title = "Baron"; break;
                case 3: title = "Duke"; break;
                case 4: title = "Lord"; break;
                case 5: title = "Lady"; break;
            }
            return title;
        }
    }
}
