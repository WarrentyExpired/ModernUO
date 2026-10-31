using System;
using Server;
using Server.Items;
using ModernUO.Serialization;

namespace Server.Mobiles
{
    [SerializationGenerator(0, false)]
    public partial class Citizen : BaseCreature
    {
        // Unsaved variable so the timer resets naturally on server boot
        private DateTime _NextChatter;

        // Add as many random rumors or phrases as you want here!
        private static readonly string[] _ChatterLines = new string[]
        {
            "Lovely weather we're having today.",
            "I heard the dungeons are more dangerous than usual...",
            "Just another quiet day in town.",
            "I need to visit the provisioner soon.",
            "Have you seen my lost dog?",
            "The guards seem on edge lately."
        };

        [Constructible]
        public Citizen() : base(AIType.AI_Animal, FightMode.None)
        {
            SetSpeed(0.2, 0.4);
            Title = "the citizen";
            Hue = Race.Human.RandomSkinHue();
            SpeechHue = 48;
            //Blessed = true;

         if (Female = Utility.RandomBool())
        {
            Body = 0x191;
            Name = NameList.RandomName("female");
            AddItem(new FancyDress(Utility.RandomDyedHue()));
            Title = "the actress";
        }
        else
        {
            Body = 0x190;
            Name = NameList.RandomName("male");
            AddItem(new LongPants(Utility.RandomNeutralHue()));
            AddItem(new FancyShirt(Utility.RandomDyedHue()));
            Title = "the actor";
        }

            AddItem(new Shirt(Utility.RandomNeutralHue()));
            AddItem(new Boots(Utility.RandomNeutralHue()));

            _NextChatter = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(5, 15));
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Deleted)
                return;

            if (DateTime.UtcNow > _NextChatter)
            {
                // Only talk if there is a player nearby to hear it (saves server CPU)
                bool playerNearby = false;
                foreach (Mobile m in GetMobilesInRange(8))
                {
                    if (m != null && m.Player && m.Alive)
                    {
                        playerNearby = true;
                        break;
                    }
                }

                if (playerNearby)
                {
                    string line = _ChatterLines[Utility.Random(_ChatterLines.Length)];
                    Say(line);
                }

                // Reset the timer (30 to 60 seconds between random barks)
                _NextChatter = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(30, 60));
            }
        }

        public Citizen(Serial serial) : base(serial)
        {
        }
    }
}
