using System;
using System.Collections.Generic;
using System.Linq;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Utilities;

namespace Server.Gumps
{
    public class KnowledgeGump : Gump
    {
        private PlayerMobile m_Player;

        // Inherit directly from Gump, set a fixed starting position
        public KnowledgeGump(PlayerMobile pm) : base(150, 150)
        {
            m_Player = pm;

            var sortedSkills = pm.SkillTome.OrderBy(x => x.Key.ToString()).ToList();

            Closable = true;
            Disposable = true;
            Resizable = false;

            AddPage(0);
            // The Tome Look: A medium-sized parchment window
            AddBackground(0, 0, 550, 650, 9270);
            AddAlphaRegion(10, 10, 530, 630);

            // --- HEADER ---
            AddLabel(160, 20, 1152, "TOME OF PREVIOUS KNOWLEDGE");
            AddImageTiled(20, 45, 510, 2, 0x2424);

            AddLabel(30, 55, 0x3F, "Unspent Points:");
            AddLabel(150, 55, 0x480, pm.DestinyPoints.ToString("N0"));
            AddLabel(300, 55, 0x3F, "Legacy Rank:");
            AddLabel(420, 55, 1152, GetLegacyRank(pm.LifetimeDestinyPoints));

            // --- TABS ---
            AddButton(30, 90, 4005, 4007, 1, GumpButtonType.Page, 1);
            AddLabel(65, 90, 1152, "Memories");

            AddButton(160, 90, 4005, 4007, 2, GumpButtonType.Page, 2);
            AddLabel(195, 90, 1152, "Soul Archive");

            // --- PAGE 1: SKILL MEMORIES ---
            AddPage(1);
            RenderSkillList(sortedSkills);

            // --- PAGE 2: PET VAULT ---
            AddPage(2);
            AddLabel(180, 120, 1152, "SOUL ANCHORED COMPANIONS");
            AddLabel(175, 140, 0x481, $"Vault Capacity: {pm.PetVault.Count} / {pm.MaxPetVaultSlots}");

            int petY = 180;
            if (pm.PetVault.Count == 0)
            {
                AddLabel(160, 250, 0x22, "No souls are currently anchored.");
            }

            for (int i = 0; i < pm.PetVault.Count; i++)
            {
                BaseCreature pet = pm.PetVault[i];
                AddButton(40, petY + 5, 4005, 4007, 100 + i, GumpButtonType.Reply, 0);
                AddLabel(80, petY, 1152, pet.Name);
                AddLabel(80, petY + 20, 0x481, $"{pet.GetType().Name} | HP: {pet.HitsMax}");
                petY += 55;
            }

            AddLabel(40, 580, 0x481, "Manifesting a pet requires available control slots.");
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            if (info.ButtonID == 0) return;

            // Handle Pet Manifesting
            if (info.ButtonID >= 100)
            {
                int index = info.ButtonID - 100;
                if (index < m_Player.PetVault.Count)
                {
                    PetVaultController.ClaimPet(m_Player, m_Player.PetVault[index]);
                }
                m_Player.SendGump(new KnowledgeGump(m_Player));
            }
        }

        private void RenderSkillList(List<KeyValuePair<SkillName, double>> skills)
        {
            int x = 40;
            int y = 130;
            int count = 0;

            foreach (var kvp in skills)
            {
                if (kvp.Value <= 0) continue;

                // Column logic
                if (count == 15) { x = 280; y = 130; }
                if (count >= 30) break; // Cap display at 30 skills

                AddLabel(x, y, 1152, kvp.Key.ToString());
                AddLabel(x + 160, y, 0x481, kvp.Value.ToString("F1"));
                y += 25;
                count++;
            }
        }

        private string GetLegacyRank(long points)
        {
            if (points > 100000) return "Exalted";
            if (points > 50000) return "Legendary";
            if (points > 10000) return "Veteran";
            return "Neophyte";
        }
    }
}
