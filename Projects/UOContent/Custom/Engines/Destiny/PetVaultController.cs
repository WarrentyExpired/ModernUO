using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Utilities
{
    public static class PetVaultController
    {
        public static void VaultPet(PlayerMobile pm, BaseCreature pet)
        {
            if (pm.PetVault.Count >= pm.MaxPetVaultSlots)
            {
                pet.Delete();
                return;
            }

            pet.ControlMaster = null;
            pet.Controlled = true;
            pet.IsStabled = true;
            pet.Internalize();

            pm.PetVault.Add(pet);
            pm.SendMessage(0x3F, $"{pet.Name} has been anchored to your soul archive.");
        }

        public static bool ClaimPet(PlayerMobile pm, int index)
        {
            // Safety check for the index
            if (index < 0 || index >= pm.PetVault.Count)
                return false;

            BaseCreature pet = pm.PetVault[index];

            if (pm.Followers + pet.ControlSlots > pm.FollowersMax)
            {
                pm.SendMessage(0x22, "You lack the mental focus (Control Slots) to manifest this creature.");
                return false;
            }

            // REMOVE from list first to prevent ghosting/cloning
            pm.PetVault.RemoveAt(index);

            pet.ControlMaster = pm;
            pet.Controlled = true;
            pet.IsStabled = false;

            // Move to player location
            pet.MoveToWorld(pm.Location, pm.Map);
            pm.Followers += pet.ControlSlots;

            pm.SendMessage(0x3F, $"{pet.Name} manifests from your memories.");

            // Optional: Add a visual effect so it looks cool
            pet.FixedEffect(0x376A, 9, 32);

            return true;
        }
    }
}
