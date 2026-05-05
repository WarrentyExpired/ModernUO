using System;
using System.Collections.Generic;
using Server.Network;
using Server.Mobiles;
using Server.Destiny;
using Server.Items;

namespace Server.Gumps
{
    public class DestinyTemplatesGump : DestinyBaseGump
    {
        public DestinyTemplatesGump(PlayerMobile pm) : base(pm, "Starting Templates")
        {
            AddLabel(220, 70, 1152, "Choose a path for this life.");

            var choices = m_Player.CurrentTemplateChoices;
            int y = 110;

            for (int i = 0; i < choices.Count; i++)
            {
                var template = choices[i];
                AddButton(220, y, 4005, 4007, 100 + i, GumpButtonType.Reply, 0);
                AddLabel(255, y, 0x481, template.Name);
                AddLabel(380, y, 1152, template.Description);
                y += 45;
            }
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            if (info.ButtonID == 0)
                return;

            base.OnResponse(sender, info);

            if (info.ButtonID >= 100 && info.ButtonID <= 104)
            {
                int index = info.ButtonID - 100;
                var choices = m_Player.CurrentTemplateChoices;
                if (index < choices.Count)
                    ApplyTemplate(m_Player, choices[index]);
            }
            if (info.ButtonID < 900)
            {
                m_Player.SendGump(new DestinyTemplatesGump(m_Player));
            }
        }

        private void ApplyTemplate(PlayerMobile pm, DestinyTemplate template)
        {
            if (pm.HasPickedTemplate)
            {
                pm.SendMessage(0x22, "You have already embraced a destiny for this life.");
                return;
            }
            for (int i = 0; i < pm.Skills.Length; ++i)
                pm.Skills[i].Base = 0.0;
            foreach (var entry in template.Skills)
            {
                double finalValue = entry.Value;
                if (pm.TomeUnlockTier1 && pm.SkillTome.ContainsKey(entry.Key))
                {
                    double storedValue = pm.SkillTome[entry.Key];
                    double heritageCap = pm.CurrentTomeStartingCap;
                    double reclaimed = Math.Min(storedValue, heritageCap);
                    finalValue = Math.Max(finalValue, reclaimed);
                }
                pm.Skills[entry.Key].Base = finalValue;
            }
            pm.HasPickedTemplate = true;
            template.GiveStartingLoot?.Invoke(pm);
            pm.SendMessage(0x3F, $"You have embraced the destiny of a {template.Name}.");
            pm.CloseGump<DestinyTemplatesGump>();
        }
    }
}
