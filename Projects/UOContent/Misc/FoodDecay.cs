using System;
using Server;
using Server.Network;
using Server.Mobiles;
using Server.Regions;

namespace Server.Misc
{
    public class FoodDecayTimer : Timer
    {
        public static void Initialize()
        {
            new FoodDecayTimer().Start();
            new EatDecayTimer().Start();
        }

        public FoodDecayTimer() : base(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5))
        {
        }

        protected override void OnTick()
        {
            FoodDecay();
        }

        public static void FoodDecay()
        {
            foreach (var state in NetState.Instances)
            {
                if (state.Mobile != null)
                {
                    HungerDecay(state.Mobile);
                }
            }
        }

        public static void HungerDecay(Mobile m)
        {
            if (m is PlayerMobile && m.AccessLevel == AccessLevel.Player)
            {
                if (m.Region is HouseRegion || m.Region is GreetingBusinessRegion)
                    return;

                if (m.Hunger >= 1)
                {
                    m.Hunger -= 1;
                    if (m.Hunger == 15)
                        m.SendMessage("Your stomach gives a slight rumble.");
                    else if (m.Hunger == 10)
                        m.SendMessage("You are starting to feel hungry.");
                    else if (m.Hunger == 5)
                        m.SendMessage("You are getting extremely hungry. You should eat soon!");
                }

                if (m.Thirst >= 1)
                {
                    m.Thirst -= 1;
                    if (m.Thirst == 15)
                        m.SendMessage("Your mouth feels a little dry.");
                    else if (m.Thirst == 10)
                        m.SendMessage("You are starting to feel thirsty.");
                    else if (m.Thirst == 5)
                        m.SendMessage("You are getting extremely thirsty. You should drink soon!");
                }
            }
        }
    }

    public class EatDecayTimer : Timer
    {
        public EatDecayTimer() : base(TimeSpan.FromSeconds(11.0), TimeSpan.FromSeconds(11.0))
        {
        }

        protected override void OnTick()
        {
            EatDecay();
        }

        public static void EatDecay()
        {
            foreach (var state in NetState.Instances)
            {
                if (state.Mobile != null)
                {
                    EatDecaying(state.Mobile);
                }
            }
        }

        public static void EatDecaying(Mobile m)
        {
            if (m is PlayerMobile pm && pm.Alive && pm.AccessLevel == AccessLevel.Player)
            {
                if (m.Region is HouseRegion || m.Region is GreetingBusinessRegion)
                    return;

                if (m.Hunger < 5 || m.Thirst < 5)
                {
                    if (m.Hunger < 5)
                    {
                        int hits = 0;

                        if (m.Hunger >= 4) hits = 2;
                        else if (m.Hunger >= 3) hits = 3;
                        else if (m.Hunger >= 2) hits = 4;
                        else if (m.Hunger >= 1) hits = 5;
                        else
                        {
                            hits = 6;
                            m.SendMessage("You are starving to death!");
                            m.LocalOverheadMessage(MessageType.Emote, 1150, true, "I am so hungry!");
                        }

                        if (m.Hits < hits)
                            hits = m.Hits - 1;

                        if (hits > 0)
                            m.Hits -= hits;

                        if (m.Hunger < 3 && m.Hunger > 0)
                        {
                            if (Utility.RandomDouble() < 0.10) m.SendMessage("You are getting very hungry!");
                            if (Utility.RandomMinMax(1, 20) == 1) m.LocalOverheadMessage(MessageType.Emote, 1150, true, "I am getting hungry!");
                        }
                    }

                    if (m.Thirst < 5)
                    {
                        if (m.Thirst >= 4) m.Stam -= 2;
                        else if (m.Thirst >= 3) m.Stam -= 3;
                        else if (m.Thirst >= 2) m.Stam -= 4;
                        else if (m.Thirst >= 1) m.Stam -= 5;
                        else
                        {
                            m.Stam -= 6;
                            m.SendMessage("You are exhausted from thirst!");
                            m.LocalOverheadMessage(MessageType.Emote, 1150, true, "I am so thirsty!");
                        }

                        if (m.Thirst < 3 && m.Thirst > 0)
                        {
                            if (Utility.RandomDouble() < 0.10) m.SendMessage("You are getting exhausted from thirst!");
                            if (Utility.RandomMinMax(1, 20) == 1) m.LocalOverheadMessage(MessageType.Emote, 1150, true, "I am getting thirsty!");
                        }

                        if (m.Stam < 0)
                            m.Stam = 0;
                    }

                    int test = Math.Min(m.Hunger, m.Thirst);

                    if (test >= 4) m.Mana -= 2;
                    else if (test >= 3) m.Mana -= 3;
                    else if (test >= 2) m.Mana -= 4;
                    else if (test >= 1) m.Mana -= 5;
                    else m.Mana -= 6;

                    if (m.Mana < 0)
                        m.Mana = 0;
                }
                else
                {
                    if (m.Hunger >= 15 && m.Hits < m.HitsMax)
                    {
                        m.Hits += 2;
                    }

                    if (m.Thirst >= 15 && m.Stam < m.StamMax)
                    {
                        m.Stam += 2;
                    }

                    if (m.Hunger >= 15 && m.Thirst >= 15 && m.Mana < m.ManaMax)
                    {
                        m.Mana += 2;
                    }
                }
            }
        }
    }
}
