using System;
using System.Collections.Generic;
using Server.Network;
using Server.Mobiles;
using Server.Destiny;
using Server.Items;

namespace Server.Gumps
{
    public class DestinyCoreGump : DestinyBaseGump
    {
        public DestinyCoreGump(PlayerMobile pm) : base(pm, "Core Upgrades")
        {
            // Current Stats Display
            AddLabel(220, 70, 1152, "Stat Cap:");
            AddLabel(300, 70, 0x481, $"{m_Player.MaxStatCap}");
            AddLabel(400, 70, 1152, "Skill Cap:");
            AddLabel(480, 70, 0x481, $"{(m_Player.MaxSkillCap / 10.0):F1}");

            int startY = 120;

            // Stat Cap Purchase
            int statCost = GetCurrentCost(500, m_Player.StatCapPurchases);
            if (m_Player.StatCapPurchases < 125)
                DrawUpgradeEntry(220, startY, 1, $"Expand Stat Vessel (+1) [{m_Player.StatCapPurchases}/125]", statCost);
            else
                AddLabel(255, startY, 0x22, "Expand Stat Vessel (+1) [MAXED]");

            startY += 50;

            // Skill Cap Purchase
            int skillCost = GetCurrentCost(500, m_Player.SkillCapPurchases);
            if (m_Player.SkillCapPurchases < 40)
                DrawUpgradeEntry(220, startY, 2, $"Mental Expansion (+10.0) [{m_Player.SkillCapPurchases}/40]", skillCost);
            else
                AddLabel(255, startY, 0x22, "Mental Expansion (+10.0) [MAXED]");

            startY += 70;
            AddImageTiled(220, startY - 10, 650, 2, 0x2424);

            // Heritage Logic
            if (!m_Player.TomeUnlockTier1)
            {
                DrawUpgradeEntry(220, startY, 20, "Unlock Soul Heritage (Start with 40.0)", 1000);
            }
            else
            {
                AddLabel(255, startY, 0x3F, "Soul Heritage: UNLOCKED (Base 40.0)");
                startY += 50;
                int hCost = GetHeritageCost(1000, m_Player.TomeSkillBoost);
                double nextCap = m_Player.CurrentTomeStartingCap + 5.0;

                if (nextCap <= 120.0)
                    DrawUpgradeEntry(220, startY, 21, $"Increase Heritage Cap to {nextCap:F1}", hCost);
                else
                    AddLabel(255, startY, 0x481, "Soul Heritage: MAXED (120.0)");
            }

            startY += 60; // Increment for the next section

            // Pet vault slot upgrade
            int vaultCost = GetVaultUpgradeCost(m_Player.MaxPetVaultSlots);
            bool isMaxed = m_Player.MaxPetVaultSlots >= 10;

            // Fixed: Changed 'y' to 'startY'
            AddLabel(230, startY, 1152, "Soul Archive Expansion");
            AddLabel(230, startY + 20, 0x481, $"Current Slots: {m_Player.MaxPetVaultSlots} / 10");

            if (!isMaxed)
            {
                AddButton(550, startY + 5, 4005, 4007, 22, GumpButtonType.Reply, 0);
                AddLabel(590, startY + 5, 0x3F, $"{vaultCost:N0} Points");
            }
            else
            {
                AddLabel(550, startY + 5, 0x44, "MAXED");
            }
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            if (info.ButtonID == 0)
                return;

            base.OnResponse(sender, info); // Handles tabs

            switch (info.ButtonID)
            {
                case 1: // Stat Cap
                    int sCost = GetCurrentCost(500, m_Player.StatCapPurchases);
                    if (m_Player.StatCapPurchases < 125 && Purchase(sCost))
                    {
                        m_Player.MaxStatCap += 1;
                        m_Player.StatCapPurchases++;
                    }
                    break;

                case 2: // Skill Cap
                    int skCost = GetCurrentCost(500, m_Player.SkillCapPurchases);
                    if (m_Player.SkillCapPurchases < 40 && Purchase(skCost))
                    {
                        m_Player.MaxSkillCap += 100;
                        m_Player.SkillCapPurchases++;
                    }
                    break;

                case 20: // Tome Unlock
                    if (!m_Player.TomeUnlockTier1 && Purchase(1000))
                        m_Player.TomeUnlockTier1 = true;
                    break;

                case 21: // Heritage Cap
                    int heritageCost = GetHeritageCost(2000, m_Player.TomeSkillBoost);
                    if (m_Player.TomeUnlockTier1 && m_Player.CurrentTomeStartingCap < 120.0 && Purchase(heritageCost))
                    {
                        m_Player.TomeSkillBoost++;
                        m_Player.SendMessage(0x3F, "Your soul's capacity for ancient memories has expanded.");
                    }
                    break;

                case 22: // Soul Archive Expansion
                    int cost = GetVaultUpgradeCost(m_Player.MaxPetVaultSlots);
                    if (m_Player.MaxPetVaultSlots >= 10)
                    {
                        m_Player.SendMessage(0x22, "Your soul can hold no more companions.");
                    }
                    else if (m_Player.DestinyPoints < cost)
                    {
                        m_Player.SendMessage(0x22, "You do not have enough Destiny Points to expand your soul.");
                    }
                    else
                    {
                        m_Player.DestinyPoints -= cost;
                        m_Player.MaxPetVaultSlots += 1;
                        m_Player.FixedEffect(0x375A, 10, 15);
                        m_Player.PlaySound(0x1F2);
                        m_Player.SendMessage(0x3F, $"Your Soul Archive has expanded to {m_Player.MaxPetVaultSlots} slots!");
                    }
                    break;
            } // <--- FIXED: Added missing closing brace for the switch statement

            if (info.ButtonID < 900)
            {
                m_Player.SendGump(new DestinyCoreGump(m_Player));
            }
        }

        private int GetHeritageCost(int baseCost, int boostLevel)
        {
            return (int)(baseCost * Math.Pow(1.75, boostLevel));
        }

        public static int GetVaultUpgradeCost(int currentSlots)
        {
            int upgradeLevel = currentSlots - 1;
            return upgradeLevel * 5000;
        }
    }
}
