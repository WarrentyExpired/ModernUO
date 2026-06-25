using System;
using Server.Mobiles;
using Server.Items;

namespace Server.Combat
{
    public static class InnateWeaponAbilities
    {
        public static void CheckProc(Mobile attacker, Mobile defender, BaseWeapon weapon)
        {
            if (attacker == null || defender == null || weapon == null || !defender.Alive)
                return;
            double anatomy = attacker.Skills[SkillName.Anatomy].Value;

            double procChance = 0.10 + ((anatomy / 120.0) * 0.15);

            if (Utility.RandomDouble() >= procChance)
                return;

            switch (weapon.Skill)
            {
                case SkillName.Swords:
                    ApplyBleed(attacker, defender);
                    break;

                case SkillName.Archery:
                    ApplyHinder(attacker, defender);
                    break;

                case SkillName.Fencing:
                    ApplyPierce(attacker, defender);
                    break;

                case SkillName.Macing:
                    ApplyConcussion(attacker, defender);
                    break;
            }
        }

        private static void ApplyBleed(Mobile attacker, Mobile defender)
        {
            defender.PlaySound(0x133);

            if (typeof(BaseWeapon).Assembly.GetType("Server.Items.BleedAttack") != null)
            {
                BleedAttack.BeginBleed(defender, attacker);
            }

            attacker.SendMessage(0x3F, "Your blade inflicts a bleeding wound!");
            defender.SendMessage(0x22, "You are bleeding severely!");
        }

        private static void ApplyHinder(Mobile attacker, Mobile defender)
        {
            defender.PlaySound(0x204);
            defender.FixedEffect(0x376A, 9, 32);

            defender.Freeze(TimeSpan.FromSeconds(3.0));

            attacker.SendMessage(0x481, "Your arrow pins the target in place!");
            defender.SendMessage(0x22, "You have been pinned and cannot move!");
        }

        private static void ApplyPierce(Mobile attacker, Mobile defender)
        {
            defender.PlaySound(0x56);
            int directDamage = (int)(attacker.Skills[SkillName.Tactics].Value * 0.2) + Utility.RandomMinMax(2, 5);
            AOS.Damage(defender, attacker, directDamage, true, 100, 0, 0, 0, 0, 0);
            attacker.SendMessage(0x42, $"Your precise thrust pierces through their armor for {directDamage} direct damage!");
            defender.SendMessage(0x22, $"You are pierced for {directDamage} direct damage, bypassing your armor!");
        }

        private static void ApplyConcussion(Mobile attacker, Mobile defender)
        {
            defender.PlaySound(0x213);

            int manaDrain = Utility.RandomMinMax(10, 15);
            int stamDrain = Utility.RandomMinMax(10, 15);
            defender.Mana -= manaDrain;
            defender.Stam -= stamDrain;

            attacker.SendMessage(0x35, $"Your crushing blow drained {manaDrain} mana and {stamDrain} stamina!");
        }
    }
}
