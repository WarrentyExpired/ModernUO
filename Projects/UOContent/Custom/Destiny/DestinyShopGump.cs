using System;
using System.Collections.Generic;
using Server.Network;
using Server.Mobiles;
using Server.Destiny;
using Server.Items;

namespace Server.Gumps
{
    public class DestinyShopGump : Gump
    {
        private PlayerMobile m_Player;

        public DestinyShopGump(PlayerMobile from) : base(50, 50)
        {
            m_Player = from;

            AddPage(0);

            // The Main Frame
            AddBackground(0, 0, 900, 730, 9270);
            AddAlphaRegion(10, 10, 880, 710);

            // Sidebar Alpha
            AddAlphaRegion(20, 20, 180, 690);

            AddLabel(45, 35, 1152, "ALTAR OF DESTINY");
            AddLabel(40, 60, 0x480, $"Points: {m_Player.DestinyPoints}");

            // --- TAB BUTTONS ---

            AddButton(30, 120, 4005, 4007, 0, GumpButtonType.Page, 1);
            AddLabel(65, 120, 1152, "Templates");

            AddButton(30, 160, 4005, 4007, 0, GumpButtonType.Page, 2);
            AddLabel(65, 160, 1152, "Core Upgrades");

            AddButton(30, 200, 4005, 4007, 0, GumpButtonType.Page, 3);
            AddLabel(65, 200, 1152, "Legacy Perks");

            AddButton(30, 240, 4005, 4007, 0, GumpButtonType.Page, 4);
            AddLabel(65, 240, 1152, "World Buffs");

            AddButton(30, 670, 4017, 4019, 0, GumpButtonType.Reply, 0);
            AddLabel(65, 670, 33, "Leave Altar");

            // --- PAGE 1: TEMPLATES ---
            AddPage(1);
            AddLabel(220, 35, 0x3F, "Category: Rebirth Templates");
            AddLabel(220, 60, 1152, "Choose a path. This can only be done once per life.");

            var choices = m_Player.CurrentTemplateChoices;
            int tempY = 110;

            for (int i = 0; i < choices.Count; i++)
            {
                var template = choices[i];
                AddButton(220, tempY, 4005, 4007, 100 + i, GumpButtonType.Reply, 0);
                AddLabel(255, tempY, 0x481, template.Name);
                AddLabel(380, tempY, 1152, template.Description);
                tempY += 40;
            }


            // --- PAGE 2: CORE UPGRADES ---
            AddPage(2);
            AddLabel(220, 35, 0x3F, "Category: Core Character Upgrades");
            AddLabel(220, 70, 1152, "Stat Capacity:");
            AddLabel(350, 70, 0x481, $"{m_Player.MaxStatCap}");
            AddLabel(500, 70, 1152, "Skill Capacity:");
            AddLabel(630, 70, 0x481, $"{(m_Player.MaxSkillCap / 10.0):F1}");

            AddImageTiled(220, 95, 650, 2, 0x2424);

            int statCost = GetCurrentCost(500, m_Player.StatCapPurchases);
            int skillCost = GetCurrentCost(500, m_Player.SkillCapPurchases);

            if (m_Player.StatCapPurchases < 25)
                DrawUpgradeEntry(220, 115, 1, $"Expand Stat Vessel (+1) [{m_Player.StatCapPurchases}/25]", statCost);
            else
                AddLabel(255, 115, 0x22, "Expand Stat Vessel (+1) [MAXED]");

            if (m_Player.SkillCapPurchases < 50)
                DrawUpgradeEntry(220, 165, 2, $"Mental Expansion (+10.0) [{m_Player.SkillCapPurchases}/50]", skillCost);
            else
                AddLabel(255, 165, 0x22, "Mental Expansion (+10.0) [MAXED]");

             // --- PAGE 3: LEGACY PERKS ---
            AddPage(3);
            AddLabel(220, 35, 0x3F, "Category: Legacy Perks");

            // --- PAGE 4: WORLD BUFFS ---
            AddPage(4);
            AddLabel(220, 35, 0x3F, "Category: World Buffs");
        }

        private void DrawUpgradeEntry(int x, int y, int buttonID, string name, int cost)
        {
            AddButton(x, y, 4005, 4007, buttonID, GumpButtonType.Reply, 0);
            AddLabel(x + 35, y, 1152, name);
            AddLabel(x + 550, y, 0x480, $"{cost} Pts");
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            if (info.ButtonID == 0) return;

            // Template Selection
            if (info.ButtonID >= 100 && info.ButtonID <= 104)
            {
                int index = info.ButtonID - 100;
                var choices = m_Player.CurrentTemplateChoices;
                if (index < choices.Count)
                {
                    ApplyTemplate(m_Player, choices[index]);
                }
                return;
            }

            switch (info.ButtonID)
            {
                case 1: // Stat Cap
                    int sCost = GetCurrentCost(500, m_Player.StatCapPurchases);
                    if (m_Player.StatCapPurchases < 25 && Purchase(sCost))
                    {
                        m_Player.MaxStatCap += 1;
                        m_Player.StatCapPurchases++;
                    }
                    break;

                case 2: // Skill Cap
                    int skCost = GetCurrentCost(500, m_Player.SkillCapPurchases);
                    if (m_Player.SkillCapPurchases < 50 && Purchase(skCost))
                    {
                        m_Player.MaxSkillCap += 100;
                        m_Player.SkillCapPurchases++;
                    }
                    break;
            }

            m_Player.SendGump(new DestinyShopGump(m_Player));
        }

        private void ApplyTemplate(PlayerMobile pm, DestinyTemplate template)
        {
            if (pm.SkillsTotal > 0)
            {
                pm.SendMessage(0x22, "Your path for this life is already set.");
                return;
            }

            foreach (var entry in template.Skills)
            {
                pm.Skills[entry.Key].Base = entry.Value;
            }

            if (pm.Backpack != null && pm.Backpack.FindItemByType(typeof(TomeOfKnowledge)) == null)
            {
                pm.AddToBackpack(new TomeOfKnowledge());
                pm.SendMessage(0x3F, "A Tome of Previous Knowledge has been placed in your backpack.");
            }

            template.GiveStartingLoot?.Invoke(pm);
            pm.SendMessage(0x3F, $"You have embraced the destiny of a {template.Name}.");
            pm.CloseGump<DestinyShopGump>();
        }

        private int GetCurrentCost(int baseCost, int currentPurchases)
        {
            int priceTier = currentPurchases / 5;
            double multiplier = Math.Pow(1.5, priceTier);
            return (int)(baseCost * multiplier);
        }

        private bool Purchase(int cost)
        {
            if (m_Player.DestinyPoints >= cost)
            {
                m_Player.DestinyPoints -= cost;
                m_Player.TotalPointsSpent += cost;
                m_Player.SendMessage(0x3F, "The Altar hums as your destiny is rewritten.");
                return true;
            }
            m_Player.SendMessage(0x22, "The Altar remains silent. You lack the Destiny Points.");
            return false;
        }
    }
}
