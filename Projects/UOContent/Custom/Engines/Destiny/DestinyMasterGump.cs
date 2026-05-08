using System;
using System.Collections.Generic;
using Server.Network;
using Server.Mobiles;
using Server.Destiny;
using Server.Items;

namespace Server.Gumps
{
    public class DestinyMasterGump : Gump
    {
        private PlayerMobile m_Player;
        private int m_Page; // 0=Dash, 1=Templates, 2=Core, 3=Skills

        public DestinyMasterGump(PlayerMobile pm, int page) : base(50, 50)
        {
            m_Player = pm;
            m_Page = page;

            Closable = true;
            Disposable = true;
            Resizable = false;

            AddPage(0);

            // Main Frame
            AddBackground(0, 0, 900, 730, 9270);
            AddAlphaRegion(10, 10, 880, 710);

            // Sidebar
            AddAlphaRegion(20, 20, 180, 690);
            AddLabel(45, 35, 1152, "ALTAR OF DESTINY");
            AddLabel(40, 60, 0x480, $"Points: {m_Player.DestinyPoints:N0}");

            // --- TAB BUTTONS ---
            AddButton(30, 120, m_Page == 0 ? 4007 : 4005, 4007, 900, GumpButtonType.Reply, 0);
            AddLabel(65, 120, m_Page == 0 ? 0x3F : 1152, "Dashboard");

            AddButton(30, 160, m_Page == 1 ? 4007 : 4005, 4007, 901, GumpButtonType.Reply, 0);
            AddLabel(65, 160, m_Page == 1 ? 0x3F : 1152, "Templates");

            AddButton(30, 200, m_Page == 2 ? 4007 : 4005, 4007, 902, GumpButtonType.Reply, 0);
            AddLabel(65, 200, m_Page == 2 ? 0x3F : 1152, "Core Upgrades");

            AddButton(30, 240, m_Page == 3 ? 4007 : 4005, 4007, 903, GumpButtonType.Reply, 0);
            AddLabel(65, 240, m_Page == 3 ? 0x3F : 1152, "Skill Resonance");

            // Render Content based on Page
            switch (m_Page)
            {
                case 0: RenderDashboard(); break;
                case 1: RenderTemplates(); break;
                case 2: RenderCore(); break;
                case 3: RenderSkills(); break;
            }
        }

        #region Page Rendering

        private void RenderDashboard()
        {
            int y = 80;
            AddLabel(220, y, 0x3F, "--- Current Essence ---"); y += 30;
            AddLabel(230, y, 1152, "Stat Cap:"); AddLabel(400, y, 0x481, m_Player.MaxStatCap.ToString()); y += 25;
            AddLabel(230, y, 1152, "Skill Cap:"); AddLabel(400, y, 0x481, (m_Player.MaxSkillCap / 10.0).ToString("F1")); y += 45;

            AddLabel(220, y, 0x3F, "--- Lifetime Legacy ---"); y += 30;
            AddLabel(230, y, 1152, "Total Points Earned:"); AddLabel(400, y, 0x480, m_Player.LifetimeDestinyPoints.ToString()); y += 25;
            AddLabel(230, y, 1152, "Total Deaths:"); AddLabel(400, y, 33, m_Player.TotalDeaths.ToString());
        }

        private void RenderTemplates()
        {
            AddLabel(220, 70, 1152, "Choose a path for this life.");
            var choices = m_Player.CurrentTemplateChoices;
            int y = 110;
            for (int i = 0; i < choices.Count; i++)
            {
                AddButton(220, y, 4005, 4007, 100 + i, GumpButtonType.Reply, 0);
                AddLabel(255, y, 0x481, choices[i].Name);
                AddLabel(380, y, 1152, choices[i].Description);
                y += 45;
            }
        }

        private void RenderCore()
        {
            AddLabel(220, 70, 1152, "Stat Cap:"); AddLabel(300, 70, 0x481, $"{m_Player.MaxStatCap}");
            AddLabel(400, 70, 1152, "Skill Cap:"); AddLabel(480, 70, 0x481, $"{(m_Player.MaxSkillCap / 10.0):F1}");

            int startY = 120;
            DrawUpgradeRow(startY, 1, $"Expand Stat Vessel (+1) [{m_Player.StatCapPurchases}/125]", GetCurrentCost(500, m_Player.StatCapPurchases), m_Player.StatCapPurchases < 125);
            startY += 50;
            DrawUpgradeRow(startY, 2, $"Mental Expansion (+10.0) [{m_Player.SkillCapPurchases}/40]", GetCurrentCost(500, m_Player.SkillCapPurchases), m_Player.SkillCapPurchases < 40);
            startY += 70;

            AddImageTiled(220, startY - 10, 650, 2, 0x2424);

            // Heritage
            if (!m_Player.TomeUnlockTier1)
                DrawUpgradeRow(startY, 3, "Unlock Soul Heritage (Base 40.0)", 1000, true);
            else {
                AddLabel(255, startY, 0x3F, "Soul Heritage: UNLOCKED (Base 40.0)");
                startY += 50;
                int hCost = (int)(2000 * Math.Pow(1.75, m_Player.TomeSkillBoost));
                DrawUpgradeRow(startY, 4, $"Increase Heritage Cap to {m_Player.CurrentTomeStartingCap + 5.0:F1}", hCost, m_Player.CurrentTomeStartingCap < 120.0);
            }
            startY += 60;

            // Pet Archive
            int vCost = (m_Player.MaxPetVaultSlots - 1) * 5000;
            DrawUpgradeRow(startY, 5, $"Soul Archive Expansion ({m_Player.MaxPetVaultSlots}/10)", vCost, m_Player.MaxPetVaultSlots < 10);
        }

        private void RenderSkills()
        {
            if (!m_Player.HasPickedTemplate) {
                AddLabel(250, 150, 0x22, "You must embrace a Template before reclaiming memories.");
                return;
            }
            AddLabel(225, 85, 0x481, $"Mental Capacity: {(m_Player.SkillsTotal / 10.0):F1} / {(m_Player.MaxSkillCap / 10.0):F1}");
            AddImageTiled(220, 110, 650, 2, 0x2424);

            var available = m_Player.AvailableResonanceSkills;
            int y = 130;
            for (int i = 0; i < available.Count; i++) {
                SkillName sn = available[i];
                double possible = Math.Min(m_Player.SkillTome[sn], m_Player.CurrentTomeStartingCap);
                if (m_Player.Skills[sn].Base < possible) {
                    AddButton(230, y + 5, 4005, 4007, 300 + i, GumpButtonType.Reply, 0);
                    AddLabel(270, y, 1152, $"{sn}");
                    AddLabel(450, y, 0x481, $"Stored: {m_Player.SkillTome[sn]:F1} (Restorable to {possible:F1})");
                    y += 40;
                }
            }
        }

        private void DrawUpgradeRow(int y, int id, string name, int cost, bool available)
        {
            if (available) {
                AddButton(220, y, 4005, 4007, id, GumpButtonType.Reply, 0);
                AddLabel(255, y, 1152, name);
                AddLabel(750, y, 0x3F, $"{cost:N0} Pts");
            } else {
                AddLabel(255, y, 0x22, name + " [MAXED]");
            }
        }

        #endregion

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            if (info.ButtonID == 0) return;

            // Tab Switching
            if (info.ButtonID >= 900 && info.ButtonID <= 903) {
                m_Player.SendGump(new DestinyMasterGump(m_Player, info.ButtonID - 900));
                return;
            }

            // Core Upgrades
            if (m_Page == 2) {
                switch (info.ButtonID) {
                    case 1: HandlePurchase(GetCurrentCost(500, m_Player.StatCapPurchases), () => {
                                m_Player.MaxStatCap++;
                                m_Player.StatCapPurchases++;
                            }); break;
                    case 2: HandlePurchase(GetCurrentCost(1000, m_Player.SkillCapPurchases), () => {
                                m_Player.MaxSkillCap += 100;
                                m_Player.SkillCapPurchases++;
                            }); break;
                    case 3: HandlePurchase(1000, () => m_Player.TomeUnlockTier1 = true); break;
                    case 4: int hCost = (int)(2000 * Math.Pow(1.75, m_Player.TomeSkillBoost));
                            HandlePurchase(hCost, () => m_Player.TomeSkillBoost++); break;
                    case 5: int vCost = (m_Player.MaxPetVaultSlots - 1) * 5000;
                            HandlePurchase(vCost, () => m_Player.MaxPetVaultSlots++); break;
                }
            }

            // Template Selection
            if (m_Page == 1 && info.ButtonID >= 100) {
                int idx = info.ButtonID - 100;
                if (idx < m_Player.CurrentTemplateChoices.Count)
                    ApplyTemplate(m_Player.CurrentTemplateChoices[idx]);
            }

            // Skill Reclaim
            if (m_Page == 3 && info.ButtonID >= 300) {
                int idx = info.ButtonID - 300;
                var list = m_Player.AvailableResonanceSkills;
                if (idx < list.Count) ReclaimSkill(list[idx]);
            }

            m_Player.SendGump(new DestinyMasterGump(m_Player, m_Page));
        }

        private void HandlePurchase(int cost, Action onSuccess)
        {
            if (m_Player.DestinyPoints >= cost) {
                m_Player.DestinyPoints -= cost;
                onSuccess();
                m_Player.PlaySound(0x1F2);
            } else m_Player.SendMessage(0x22, "Insufficient Destiny Points.");
        }

        private void ApplyTemplate(DestinyTemplate t) // (Logic from your previous TemplateGump)
        {
            if (m_Player.HasPickedTemplate) return;
            foreach (var s in m_Player.Skills) s.Base = 0.0;
            foreach (var entry in t.Skills) m_Player.Skills[entry.Key].Base = entry.Value;
            m_Player.HasPickedTemplate = true;
            t.GiveStartingLoot?.Invoke(m_Player);
        }

        private void ReclaimSkill(SkillName sn) // (Logic from your previous SkillsGump)
        {
            double target = Math.Min(m_Player.SkillTome[sn], m_Player.CurrentTomeStartingCap);
            double gain = Math.Min(target - m_Player.Skills[sn].Base, (m_Player.MaxSkillCap - m_Player.SkillsTotal) / 10.0);
            if (gain > 0) m_Player.Skills[sn].Base += gain;
        }

        private int GetCurrentCost(int baseCost, int currentpurchases)
        {
            int priceTier = currentpurchases / 5;
            return (int)(baseCost * Math.Pow(1.5, priceTier));
        }
    }
}
