using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using System.Linq;

namespace Server.Combat
{
    public static class DestinyDeathEngine
    {
        public static void HandleDeath(PlayerMobile pm)
        {
            if (pm == null)
                return;

            pm.HasPickedTemplate = false;
            pm.RecordKnowledge();
            pm.TotalDeaths++;
            pm.GenerateTemplateChoices();
            pm.GenerateResonanceSkills();

            pm.RawStr = 60;
            pm.RawDex = 10;
            pm.RawInt = 10;

            for (int i = 0; i < pm.Skills.Length; ++i)
            {
                Skill sk = pm.Skills[i];
                sk.Base = 0;

                if (sk.SkillName == SkillName.Focus || sk.SkillName == SkillName.Meditation)
                {
                    sk.SetLockNoRelay(SkillLock.Locked);
                }
                else
                {
                    sk.SetLockNoRelay(SkillLock.Up);
                }
                sk.Update();
            }

            Point3D hallOfDestinies = new Point3D(15, 1174, 0);
            pm.MoveToWorld(hallOfDestinies, Map.Trammel);

            Timer.DelayCall(TimeSpan.FromSeconds(3.5), () =>
            {
                if (!pm.Alive)
                {
                    pm.Resurrect();
                }
            });
        }

        public static void HandleResurrection(PlayerMobile pm)
        {
            if (pm == null)
                return;

            pm.Hits = pm.HitsMax;
            pm.Stam = pm.StamMax;
            pm.Mana = pm.ManaMax;

            // Clear the Player's backpack
            if (pm.Backpack != null)
            {
                for (int i = pm.Backpack.Items.Count - 1; i >= 0; i--)
                {
                    Item item = pm.Backpack.Items[i];
                    if (item == pm.StashBox || item is StashContainer)
                    {
                        continue;
                    }
                    if (!item.Insured)
                    {
                        item.Delete();
                    }
                }
            }

            // Clear the player's bank box
            BankBox bank = pm.FindBankNoCreate();
            if (bank != null)
            {
                for (int i = bank.Items.Count - 1; i >= 0; i--)
                {
                    Item item = bank.Items[i];
                    if (item == pm.StashBox || item is StashContainer)
                    {
                        continue;
                    }
                    item.Delete();
                }
            }

            // Clear the player's current unbonded pets and vault active ones
            if (pm.AllFollowers != null && pm.AllFollowers.Count > 0)
            {
                Mobile[] activeFollowers = pm.AllFollowers.ToArray();
                int followVault = 0;
                int followDelete = 0;

                foreach (Mobile m in activeFollowers)
                {
                    if (m is BaseCreature bc)
                    {
                        if (bc.Summoned)
                        {
                            bc.Delete();
                            continue;
                        }

                        // FIXED: Added an explicit check to make sure the vault doesn't already have this pet reference saved!
                        if (bc.IsBonded && pm.PetVault.Count < pm.MaxPetVaultSlots)
                        {
                            if (!pm.PetVault.Contains(bc))
                            {
                                Server.Utilities.PetVaultController.VaultPet(pm, bc);
                                followVault++;
                            }
                        }
                        else
                        {
                            // Safety Check: Only delete if it wasn't successfully vaulted or already in the vault
                            if (!pm.PetVault.Contains(bc))
                            {
                                bc.Delete();
                                followDelete++;
                            }
                        }
                    }
                }

                if (followVault > 0)
                {
                    pm.SendMessage(0x3F, $"{followVault} bonded companion{(followVault > 1 ? "s have" : " has")} been recovered and sent to your Soul Archive.");
                }
                if (followDelete > 0)
                {
                    pm.SendMessage(0x22, $"{followDelete} companion{(followDelete > 1 ? "s" : "")} could not be saved and perished eternally.");
                }
            }

            // Clear stabled storage
            if (pm.Stabled != null && pm.Stabled.Count > 0)
            {
                Mobile[] stableArray = pm.Stabled.ToArray();
                int vaultedCount = 0;
                int deletedCount = 0;

                foreach (Mobile m in stableArray)
                {
                    if (m is BaseCreature bc)
                    {
                        // FIXED: Added an explicit check here as well to intercept double-processed overlapping pets!
                        if (bc.IsBonded && pm.PetVault.Count < pm.MaxPetVaultSlots)
                        {
                            if (!pm.PetVault.Contains(bc))
                            {
                                Server.Utilities.PetVaultController.VaultPet(pm, bc);
                                vaultedCount++;
                            }
                        }
                        else
                        {
                            if (!pm.PetVault.Contains(bc))
                            {
                                bc.Delete();
                                deletedCount++;
                            }
                        }
                    }
                }

                pm.Stabled.Clear();

                if (vaultedCount > 0)
                {
                    pm.SendMessage(0x3F, $"{vaultedCount} stabled bonded companion{(vaultedCount > 1 ? "s have" : " has")} been recovered and sent to your Soul Archive.");
                }
                if (deletedCount > 0)
                {
                    pm.SendMessage(0x22, $"{deletedCount} stabled companion{(deletedCount > 1 ? "s" : "")} could not be saved and perished eternally.");
                }
            }

            pm.Followers = 0;

            if (pm.Backpack != null && pm.Backpack.FindItemByType(typeof(Server.Items.TomeOfKnowledge)) == null)
            {
                pm.Backpack.DropItem(new Server.Items.TomeOfKnowledge());
            }
        }
    }
}
