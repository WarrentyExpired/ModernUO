using System;
using System.Collections.Generic;
using Server.Network;
using Server.Mobiles;

namespace Server.Gumps
{
    public class DestinySkillsGump : DestinyBaseGump
    {
        private int m_Page;

        public DestinySkillsGump(PlayerMobile pm, int page = 1) : base(pm, "Skill Resonance")
        {
            m_Page = page;

            if (!pm.HasPickedTemplate)
            {
                AddLabel(250, 150, 0x22, "You must embrace a Template before reclaiming memories.");
                AddLabel(250, 175, 1152, "Visit the Templates tab first.");
                return;
            }

            // Header info moved slightly to breathe
            AddLabel(225, 65, 1152, "The following memories have surfaced from your previous lives.");
            AddLabel(225, 85, 0x481, $"Mental Capacity: {(pm.SkillsTotal / 10.0):F1} / {(pm.MaxSkillCap / 10.0):F1}");

            AddImageTiled(220, 110, 650, 2, 0x2424); // Decorative divider

            var available = pm.AvailableResonanceSkills;

            // Filter list
            List<SkillName> showList = new List<SkillName>();
            foreach (SkillName sn in available)
            {
                double possibleGain = Math.Min(pm.SkillTome[sn], pm.CurrentTomeStartingCap);
                if (pm.Skills[sn].Base < possibleGain)
                    showList.Add(sn);
            }

            // 20 items per page (10 per column) to prevent bottom overflow
            int itemsPerPage = 20;
            int totalPages = (int)Math.Ceiling(showList.Count / (double)itemsPerPage);
            if (m_Page > totalPages) m_Page = totalPages;
            if (m_Page < 1) m_Page = 1;

            int startIndex = (m_Page - 1) * itemsPerPage;

            int yStart = 130;
            int xColumn1 = 230;
            int xColumn2 = 560; // Wide gap between columns

            int y = yStart;
            int x = xColumn1;
            int countOnPage = 0;

            for (int i = startIndex; i < showList.Count && countOnPage < itemsPerPage; i++)
            {
                // Switch to second column after 10 items
                if (countOnPage == 10)
                {
                    x = xColumn2;
                    y = yStart;
                }

                SkillName sn = showList[i];
                double possibleGain = Math.Min(pm.SkillTome[sn], pm.CurrentTomeStartingCap);

                AddButton(x, y + 5, 4005, 4007, 1000 + i, GumpButtonType.Reply, 0);
                AddLabel(x + 40, y, 1152, $"{sn}");
                AddLabel(x + 40, y + 18, 0x481, $"Can Reclaim: {possibleGain:F1}");

                y += 52; // Significant vertical spacing
                countOnPage++;
            }

            // --- Footer / Pagination ---
            if (totalPages > 1)
            {
                AddImageTiled(220, 660, 650, 2, 0x2424);

                if (m_Page > 1)
                    AddButton(220, 675, 4014, 4016, 50, GumpButtonType.Reply, 0);

                AddLabel(480, 675, 1152, $"Page {m_Page} of {totalPages}");

                if (m_Page < totalPages)
                    AddButton(840, 675, 4005, 4007, 51, GumpButtonType.Reply, 0);
            }
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            if (info.ButtonID == 0) return;
            base.OnResponse(sender, info);

            if (info.ButtonID == 50) { m_Player.SendGump(new DestinySkillsGump(m_Player, m_Page - 1)); return; }
            if (info.ButtonID == 51) { m_Player.SendGump(new DestinySkillsGump(m_Player, m_Page + 1)); return; }

            if (info.ButtonID >= 1000)
            {
                int index = info.ButtonID - 1000;
                var available = m_Player.AvailableResonanceSkills;

                List<SkillName> showList = new List<SkillName>();
                foreach (SkillName sn in available)
                {
                    double possibleGain = Math.Min(m_Player.SkillTome[sn], m_Player.CurrentTomeStartingCap);
                    if (m_Player.Skills[sn].Base < possibleGain)
                        showList.Add(sn);
                }

                if (index < showList.Count)
                {
                    ReclaimSkill(m_Player, showList[index]);
                }

                m_Player.SendGump(new DestinySkillsGump(m_Player, m_Page));
            }
        }

        private void ReclaimSkill(PlayerMobile pm, SkillName sn)
        {
            double targetValue = Math.Min(pm.SkillTome[sn], pm.CurrentTomeStartingCap);
            double currentVal = pm.Skills[sn].Base;

            int remainingUnits = pm.MaxSkillCap - pm.SkillsTotal;
            if (remainingUnits <= 0)
            {
                pm.SendMessage(0x22, "Your mental capacity (Skill Cap) is full.");
                return;
            }

            double requested = targetValue - currentVal;
            double allowed = remainingUnits / 10.0;
            double gain = Math.Min(requested, allowed);

            if (gain > 0)
            {
                pm.Skills[sn].Base += gain;
                pm.SendMessage(0x3F, $"The memory of {sn} returns. You are now {pm.Skills[sn].Base:F1}.");
            }
        }
    }
}
