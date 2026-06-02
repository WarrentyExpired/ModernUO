using System;
using System.Collections.Generic;
using System.Linq;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Utilities;
using Server.Items;
using Server.Targeting;

namespace Server.Gumps
{
    public class KnowledgeGump : Gump
    {
        private PlayerMobile m_Player;

        // Made public so the confirmation gump can read the same configuration data cleanly
        public static readonly string[] DungeonNames = new string[8]
        {
            "SHAME (TIER 1)",
            "DECEIT (TIER 2)",
            "COVETOUS (TIER 3)",
            "HYTHLOTH (TIER 4)",
            "DESPISE (TIER 5)",
            "DESTARD (TIER 6)",
            "WRONG (TIER 7)",
            "THE VOID FINALE (TIER 8)"
        };

        public static readonly Point3D[] DungeonCoords = new Point3D[8]
        {
            new Point3D(5141, 1761, 0),  // Shame
            new Point3D(4111, 434, 5),   // Deceit
            new Point3D(2498, 921, 0),   // Covetous
            new Point3D(4721, 3816, 0),  // Hythloth
            new Point3D(1301, 1080, 0),  // Despise
            new Point3D(1176, 2640, 0),  // Destard
            new Point3D(2043, 224, 0),   // Wrong
            new Point3D(1361, 895, 0)    // Finale
        };

        public KnowledgeGump(PlayerMobile pm) : base(150, 150)
        {
            m_Player = pm;

            var sortedSkills = pm.SkillTome.OrderBy(x => x.Key.ToString()).ToList();

            Closable = true;
            Disposable = true;
            Resizable = false;

            AddPage(0);
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

            AddButton(290, 90, 4005, 4007, 3, GumpButtonType.Page, 3);
            AddLabel(325, 90, 1152, "Soul Atlas");

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

            // --- PAGE 3: INTEGRATED SOUL ATLAS ---
            AddPage(3);
            AddLabel(190, 120, 1152, "WORLD PROGRESSION ARCHIVE");

            int atlasY = 170;
            for (int i = 0; i < 8; i++)
            {
                bool isUnlocked = (m_Player.SoulAtlasMask & (1 << i)) != 0;

                if (isUnlocked)
                {
                    AddButton(40, atlasY + 2, 4005, 4007, 200 + i, GumpButtonType.Reply, 0);
                    AddLabel(85, atlasY, 0x480, DungeonNames[i]);
                    AddLabel(420, atlasY, 0x3F, "[READY]");
                }
                else
                {
                    AddImage(40, atlasY + 4, 4017);
                    AddLabel(85, atlasY, 0x384, DungeonNames[i]);

                    if (i == 0)
                    {
                        AddLabel(230, atlasY, 0x22, "See the Guardian of Passage to Unlock.");
                    }
                    else
                    {
                        AddLabel(420, atlasY, 0x22, "[LOCKED]");
                    }
                }
                atlasY += 45;
            }

            AddButton(40, 575, 4005, 4007, 500, GumpButtonType.Reply, 0);
            AddLabel(75, 575, 0x481, "Shatter Sealed Chapters (Target Core Unlock Item)");
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            if (info.ButtonID == 0) return;

            // Handle Pet Manifesting
            if (info.ButtonID >= 100 && info.ButtonID < 200)
            {
                int index = info.ButtonID - 100;
                if (PetVaultController.ClaimPet(m_Player, index))
                {
                    m_Player.SendGump(new KnowledgeGump(m_Player));
                }
                else
                {
                    m_Player.SendGump(new KnowledgeGump(m_Player));
                }
                return;
            }

            // FIXED: Intercepts the click and routes it to the Warning Confirmation Box instead of instant travel!
            if (info.ButtonID >= 200 && info.ButtonID < 208)
            {
                int index = info.ButtonID - 200;
                bool isUnlocked = (m_Player.SoulAtlasMask & (1 << index)) != 0;

                if (isUnlocked)
                {
                    m_Player.CloseGump<DungeonConfirmGump>();
                    m_Player.SendGump(new DungeonConfirmGump(m_Player, index));
                }
                else
                {
                    m_Player.SendMessage(0x22, "That progressive destination is currently sealed.");
                    m_Player.SendGump(new KnowledgeGump(m_Player));
                }
                return;
            }

            // Handle Item Targeting Sync Trigger Click
            if (info.ButtonID == 500)
            {
                m_Player.SendMessage("Select the dungeon essence or unlock item inside your backpack.");
                m_Player.Target = new SoulAtlasUnlockTarget();
                return;
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

                if (count == 15) { x = 280; y = 130; }
                if (count >= 30) break;

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

        private class SoulAtlasUnlockTarget : Target
        {
            public SoulAtlasUnlockTarget() : base(2, false, TargetFlags.None) { }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (from is not PlayerMobile pm)
                    return;

                if (targeted is not Item item || !item.IsChildOf(pm.Backpack) || item.Deleted)
                {
                    pm.SendMessage(0x22, "The targeted item must reside directly inside your backpack.");
                    pm.SendGump(new KnowledgeGump(pm));
                    return;
                }

                if (item is PassageEssence)
                {
                    if ((pm.SoulAtlasMask & 1) != 0)
                    {
                        pm.SendMessage(0x481, "The chapters detailing Shame have already been permanently unsealed.");
                    }
                    else
                    {
                        item.Delete();
                        pm.SoulAtlasMask |= 1;

                        pm.PlaySound(0x244);
                        pm.SendMessage(0x3F, "The Essence dissolves into your grimoire. Shame is now permanently unlocked!");
                    }
                }
                else
                {
                    pm.SendMessage(0x22, "That item does not possess a progressive matrix signature compatible with this Tome.");
                }

                pm.SendGump(new KnowledgeGump(pm));
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
            {
                if (from is PlayerMobile pm)
                    pm.SendGump(new KnowledgeGump(pm));
            }
        }
    }

    // ADDED: The permanent roguelite warning prompt
    public class DungeonConfirmGump : Gump
    {
        private readonly PlayerMobile m_Player;
        private readonly int m_Index;

        public DungeonConfirmGump(PlayerMobile pm, int index) : base(220, 250)
        {
            m_Player = pm;
            m_Index = index;

            Closable = false;
            Disposable = true;

            AddPage(0);
            // Compact wooden warning stone plate look
            AddBackground(0, 0, 420, 240, 9270);
            AddAlphaRegion(10, 10, 400, 220);

            AddLabel(155, 25, 0x22, "ASTRAL WARNING");
            AddImageTiled(20, 50, 380, 2, 0x2424);

            // Formatted layout alert
            string text = $"<BASEFONT COLOR=#FFFFFF>You are attempting to bridge a rift directly to</BASEFONT> <BASEFONT COLOR=#FFF000>{KnowledgeGump.DungeonNames[m_Index]}</BASEFONT>.<br><br><BASEFONT COLOR=#FFFFFF>Be warned: Once enter you can not leave!</BASEFONT><BASEFONT COLOR=#FF3333> The only escape from this layout is through death.</BASEFONT><br><br><BASEFONT COLOR=#FFFFFF>Do you accept this destiny?</BASEFONT>";
            AddHtml(30, 65, 360, 110, text, false, false);

            // Confirmation Actions
            AddButton(60, 185, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddLabel(95, 185, 0x480, "Accept");

            AddButton(240, 185, 4005, 4007, 2, GumpButtonType.Reply, 0);
            AddLabel(275, 185, 0x35, "Flee");
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            if (info.ButtonID == 1) // Clicked Accept
            {
                bool isUnlocked = (m_Player.SoulAtlasMask & (1 << m_Index)) != 0;

                if (isUnlocked)
                {
                    // Execute dimensional jump
                    m_Player.FixedEffect(0x3709, 10, 30);
                    m_Player.PlaySound(0x1FC);

                    m_Player.MoveToWorld(KnowledgeGump.DungeonCoords[m_Index], Map.Trammel);

                    m_Player.FixedEffect(0x3709, 10, 30);
                    m_Player.PlaySound(0x1FC);

                    m_Player.SendMessage(0x3F, $"The Soul Atlas safely delivers you to your fate inside {KnowledgeGump.DungeonNames[m_Index]}.");
                }
            }
            else // Clicked Flee or closed window
            {
                m_Player.SendMessage("You pull your awareness back from the precipice.");
                m_Player.SendGump(new KnowledgeGump(m_Player));
            }
        }
    }
}
