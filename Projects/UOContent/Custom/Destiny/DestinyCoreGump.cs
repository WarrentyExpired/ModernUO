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
            if (m_Player.StatCapPurchases < 25)
                DrawUpgradeEntry(220, startY, 1, $"Expand Stat Vessel (+1) [{m_Player.StatCapPurchases}/25]", statCost);
            else
                AddLabel(255, startY, 0x22, "Expand Stat Vessel (+1) [MAXED]");

            startY += 50;

            // Skill Cap Purchase
            int skillCost = GetCurrentCost(500, m_Player.SkillCapPurchases);
            if (m_Player.SkillCapPurchases < 50)
                DrawUpgradeEntry(220, startY, 2, $"Mental Expansion (+10.0) [{m_Player.SkillCapPurchases}/50]", skillCost);
            else
                AddLabel(255, startY, 0x22, "Mental Expansion (+10.0) [MAXED]");

            startY += 70;
            AddImageTiled(220, startY - 10, 650, 2, 0x2424);

            // Heritage Logic
            if (!m_Player.TomeUnlockTier1)
            {
                DrawUpgradeEntry(220, startY, 20, "Unlock Soul Heritage (Start with 40.0)", 5000);
            }
            else
            {
                AddLabel(255, startY, 0x3F, "Soul Heritage: UNLOCKED (Base 40.0)");
                startY += 50;
                int bCost = GetCurrentCost(2000, m_Player.TomeSkillBoost);
                double nextCap = m_Player.CurrentTomeStartingCap + 5.0;

                if (nextCap <= 120.0)
                    DrawUpgradeEntry(220, startY, 21, $"Increase Heritage Cap to {nextCap:F1}", bCost);
                else
                    AddLabel(255, startY, 0x481, "Soul Heritage: MAXED (120.0)");
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
                case 20: // Tome Unlock
                    if (!m_Player.TomeUnlockTier1 && Purchase(1000)) m_Player.TomeUnlockTier1 = true;
                    break;
                case 21: // Tome Boost
                    int bCost = GetCurrentCost(1000, m_Player.TomeSkillBoost);
                    if (m_Player.TomeUnlockTier1 && m_Player.CurrentTomeStartingCap < 120 && Purchase(bCost))
                        m_Player.TomeSkillBoost++;
                    break;
            }
            if (info.ButtonID < 900)
            {
                m_Player.SendGump(new DestinyCoreGump(m_Player));
            }
        }
    }
}
