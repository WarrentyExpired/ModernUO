using System;
using System.Collections.Generic;
using System.Linq;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Gumps
{
    public class KnowledgeGump : Gump
    {
        public KnowledgeGump(PlayerMobile pm) : base(150, 150)
        {
            // Data from DestinyInfoGump.cs
            string unspentPoints = pm.DestinyPoints.ToString();
            string lifetimeEarned = pm.LifetimeDestinyPoints.ToString();
            string totalDeaths = pm.TotalDeaths.ToString();

            // Prepare the skill list
            var sortedSkills = pm.SkillTome.OrderBy(x => x.Key.ToString()).ToList();

            // Page 0: Persistent Elements
            AddPage(0);
            AddBackground(0, 0, 550, 650, 9270); // Matches BuildWorldGump aesthetic
            AddAlphaRegion(10, 10, 530, 630);

            // --- HEADER: DESTINY INFO (Visible on all pages) ---
            AddLabel(160, 20, 1152, "TOME OF PREVIOUS KNOWLEDGE");
            AddImageTiled(20, 45, 510, 2, 0x2424);

            // Row 1 of Stats
            AddLabel(30, 55, 0x3F, "Points:");
            AddLabel(90, 55, 0x480, unspentPoints);
            AddLabel(180, 55, 0x3F, "Total Earned:");
            AddLabel(280, 55, 0x480, lifetimeEarned);
            AddLabel(400, 55, 0x3F, "Deaths:");
            AddLabel(460, 55, 33, totalDeaths);

            // Row 2 of Stats
            AddLabel(30, 80, 1152, "Stat Cap:");
            AddLabel(110, 80, 0x481, pm.MaxStatCap.ToString());
            AddLabel(180, 80, 1152, "Skill Cap:");
            AddLabel(260, 80, 0x481, (pm.MaxSkillCap / 10.0).ToString("F1"));

            AddImageTiled(20, 105, 510, 2, 0x2424);

            // --- PAGE 1: SKILLS 1-30 ---
            AddPage(1);
            RenderSkillList(sortedSkills, 0, 30);

            // Next Page Button
            if (sortedSkills.Count > 30)
            {
                AddButton(480, 600, 4005, 4007, 0, GumpButtonType.Page, 2);
                AddLabel(410, 600, 1152, "Next Page");
            }

            // --- PAGE 2: SKILLS 31-58 ---
            AddPage(2);
            RenderSkillList(sortedSkills, 30, 28); // Remaining 28 skills

            // Previous Page Button
            AddButton(30, 600, 4014, 4016, 0, GumpButtonType.Page, 1);
            AddLabel(65, 600, 1152, "Prev Page");

            // Close Button (Visible on all pages via Page 0 would be easier, but placing here for control)
            AddButton(235, 600, 4017, 4019, 0, GumpButtonType.Reply, 0);
            AddLabel(270, 600, 33, "Close");
        }

        private void RenderSkillList(List<KeyValuePair<SkillName, double>> skills, int start, int count)
        {
            if (skills.Count <= start)
            {
                AddLabel(150, 150, 0x22, "No deep knowledge recorded in this section.");
                return;
            }

            int x = 40;
            int y = 120;
            int itemsOnPage = 0;

            for (int i = start; i < start + count && i < skills.Count; i++)
            {
                // Column logic: 15 per column
                if (itemsOnPage == 15)
                {
                    x = 290;
                    y = 120;
                }

                var entry = skills[i];
                AddLabel(x, y, 1152, entry.Key.ToString());
                AddLabel(x + 160, y, 0x481, entry.Value.ToString("F1"));

                y += 28;
                itemsOnPage++;
            }
        }
    }
}
