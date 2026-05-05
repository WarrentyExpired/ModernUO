using System;
using System.Collections.Generic;
using System.Linq;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Utilities; // Needed for PetVaultController

namespace Server.Gumps
{
    public class KnowledgeGump : Gump
    {
        private PlayerMobile m_Player;

        public KnowledgeGump(PlayerMobile pm) : base(150, 150)
        {
            m_Player = pm;

            // Data formatting
            string unspentPoints = pm.DestinyPoints.ToString("N0"); // Added commas
            string lifetimeEarned = pm.LifetimeDestinyPoints.ToString("N0");
            string totalDeaths = pm.TotalDeaths.ToString();

            var sortedSkills = pm.SkillTome.OrderBy(x => x.Key.ToString()).ToList();

            AddPage(0);
            AddBackground(0, 0, 550, 650, 9270);
            AddAlphaRegion(10, 10, 530, 630);

            // --- HEADER: DESTINY INFO ---
            AddLabel(160, 20, 1152, "TOME OF PREVIOUS KNOWLEDGE");
            AddImageTiled(20, 45, 510, 2, 0x2424);

            AddLabel(30, 55, 0x3F, "Points:");
            AddLabel(90, 55, 0x480, unspentPoints);
            AddLabel(180, 55, 0x3F, "Lifetime:");
            AddLabel(250, 55, 0x480, lifetimeEarned);
            AddLabel(350, 55, 0x3F, "Deaths:");
            AddLabel(410, 55, 0x480, totalDeaths);

            AddLabel(30, 80, 0x3F, "Heritage Cap:");
            AddLabel(130, 80, 0x481, pm.CurrentTomeStartingCap.ToString("F1"));

            AddImageTiled(20, 105, 510, 2, 0x2424);

            // --- PAGE 1: SKILLS 1-30 ---
            AddPage(1);
            RenderSkillList(sortedSkills, 0, 30);
            if (sortedSkills.Count > 30)
            {
                AddButton(480, 600, 4005, 4007, 0, GumpButtonType.Page, 2);
                AddLabel(410, 600, 1152, "Next Page");
            }
            else
            {
                AddButton(480, 600, 4005, 4007, 0, GumpButtonType.Page, 3);
                AddLabel(410, 600, 1152, "Pet Vault");
            }

            // --- PAGE 2: SKILLS 31-58 ---
            AddPage(2);
            RenderSkillList(sortedSkills, 30, 28);
            AddButton(30, 600, 4014, 4016, 0, GumpButtonType.Page, 1);
            AddLabel(65, 600, 1152, "Prev Page");

            AddButton(480, 600, 4005, 4007, 0, GumpButtonType.Page, 3);
            AddLabel(410, 600, 1152, "Pet Vault");

            // --- PAGE 3: PET VAULT ---
            AddPage(3);
            AddLabel(180, 120, 1152, "SOUL ANCHORED COMPANIONS");
            AddLabel(175, 140, 0x481, $"Vault Capacity: {pm.PetVault.Count} / {pm.MaxPetVaultSlots}");

            int petY = 180;
            for (int i = 0; i < pm.PetVault.Count; i++)
            {
                BaseCreature pet = pm.PetVault[i];
                AddButton(40, petY + 5, 4005, 4007, 100 + i, GumpButtonType.Reply, 0);
                AddLabel(80, petY, 1152, pet.Name);
                AddLabel(80, petY + 20, 0x481, $"{pet.GetType().Name} | HP: {pet.HitsMax}");
                petY += 55;
            }

            AddButton(30, 600, 4014, 4016, 0, GumpButtonType.Page, 1);
            AddLabel(65, 600, 1152, "Back to Skills");

            // Global Close Button
            AddButton(235, 600, 4017, 4019, 0, GumpButtonType.Reply, 0);
            AddLabel(270, 600, 33, "Close");
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            if (info.ButtonID == 0) return;

            if (info.ButtonID == 50) // Vaulting
            {
                m_Player.SendMessage(0x3F, "Which companion shall be anchored to your soul?");
                m_Player.BeginTarget(10, false, Targeting.TargetFlags.None, (from, targeted) =>
                {
                    if (targeted is BaseCreature bc && bc.ControlMaster == m_Player)
                    {
                        PetVaultController.VaultPet(m_Player, bc);
                        m_Player.SendGump(new KnowledgeGump(m_Player)); // Refresh
                    }
                    else
                    {
                        m_Player.SendMessage(0x22, "The Tome only recognizes creatures bound to you.");
                    }
                });
            }
            else if (info.ButtonID >= 100) // Claiming
            {
                int index = info.ButtonID - 100;
                if (index < m_Player.PetVault.Count)
                {
                    PetVaultController.ClaimPet(m_Player, m_Player.PetVault[index]);
                }
                m_Player.SendGump(new KnowledgeGump(m_Player));
            }
        }

        private void RenderSkillList(List<KeyValuePair<SkillName, double>> skills, int start, int count)
        {
            if (skills.Count <= start)
            {
                AddLabel(150, 150, 0x22, "No deep knowledge recorded here.");
                return;
            }

            int x = 40;
            int y = 120;
            int itemsOnPage = 0;

            for (int i = start; i < start + count && i < skills.Count; i++)
            {
                if (itemsOnPage == 15) { x = 280; y = 120; }

                string skillName = skills[i].Key.ToString();
                double skillVal = skills[i].Value;

                AddLabel(x, y, 1152, skillName);
                AddLabel(x + 160, y, 0x481, skillVal.ToString("F1"));

                y += 30;
                itemsOnPage++;
            }
        }
    }
}
