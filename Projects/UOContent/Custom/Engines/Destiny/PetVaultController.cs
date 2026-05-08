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

        public static void ClaimPet(PlayerMobile pm, BaseCreature pet)
        {
            if (pm.Followers + pet.ControlSlots > pm.FollowersMax)
            {
                pm.SendMessage(0x22, "You lack the mental focus (Control Slots) to manifest this creature.");
                return;
            }

            pm.PetVault.Remove(pet);
            pet.ControlMaster = pm;
            pet.Controlled = true;
            pet.IsStabled = false;
            pet.MoveToWorld(pm.Location, pm.Map);
            pm.Followers += pet.ControlSlots;

            pm.SendMessage(0x3F, $"{pet.Name} manifests from your memories.");
        }
    }
}
