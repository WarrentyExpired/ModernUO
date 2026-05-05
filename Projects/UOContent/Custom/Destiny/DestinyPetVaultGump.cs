using System;
using Server.Network;
using Server.Mobiles;
using Server.Utilities;

namespace Server.Gumps
{
    public class DestinyPetVaultGump : DestinyBaseGump
    {
        public DestinyPetVaultGump(PlayerMobile pm) : base(pm, "Pet Vault")
        {
            AddLabel(230, 65, 1152, "Creatures stored here survive the cycle of rebirth.");
            AddLabel(230, 85, 0x481, $"Vaulted Companions: {pm.PetVault.Count} / 3");

            AddImageTiled(220, 110, 650, 2, 0x2424);

            int y = 130;
            for (int i = 0; i < pm.PetVault.Count; i++)
            {
                BaseCreature pet = pm.PetVault[i];

                AddButton(230, y + 5, 4005, 4007, 100 + i, GumpButtonType.Reply, 0);
                AddLabel(270, y, 1152, $"{pet.Name}");
                AddLabel(270, y + 18, 0x481, $"{pet.GetType().Name} | HP: {pet.HitsMax}");

                y += 55;
            }

            if (pm.PetVault.Count < 3)
            {
                AddButton(230, 600, 4005, 4007, 50, GumpButtonType.Reply, 0);
                AddLabel(265, 600, 0x3F, "Vault Active Companion (Target)");
            }
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            if (info.ButtonID == 0) return;
            base.OnResponse(sender, info); // Handles tab switching

            if (info.ButtonID == 50) // The Vaulting Target
            {
                m_Player.SendMessage("Which companion shall be remembered across lives?");
                m_Player.BeginTarget(10, false, Targeting.TargetFlags.None, (from, targeted) =>
                {
                    if (targeted is BaseCreature bc && bc.ControlMaster == m_Player)
                    {
                        PetVaultController.VaultPet(m_Player, bc);
                        m_Player.SendGump(new DestinyPetVaultGump(m_Player));
                    }
                    else
                    {
                        m_Player.SendMessage(0x22, "You can only vault your own bonded creatures.");
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
                m_Player.SendGump(new DestinyPetVaultGump(m_Player));
            }
        }
    }
}
