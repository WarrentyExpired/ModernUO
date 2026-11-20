using System;
using System.Collections.Concurrent;
using Server.Mobiles;
using Server.Items;

namespace Server.Combat
{
    public static class InnateWeaponAbilities
    {
        private static readonly ConcurrentDictionary<Mobile, long> _cooldowns = new ConcurrentDictionary<Mobile, long>();

        public static void CheckProc(Mobile attacker, Mobile defender, BaseWeapon weapon)
        {
            if (attacker == null || defender == null || weapon == null || !defender.Alive)
                return;

            if (_cooldowns.TryGetValue(attacker, out long nextAllowedProc))
            {
                if (Core.TickCount < nextAllowedProc)
                    return;
            }

            double wpnSkill = attacker.Skills[weapon.Skill].Base;
            double tactics = attacker.Skills[SkillName.Tactics].Base;

            if (wpnSkill < 69.9 || tactics < 69.9)
                return;

            double procChance = 0.10;

            if (Utility.RandomDouble() >= procChance)
                return;

            _cooldowns[attacker] = Core.TickCount + 5000;

            bool isGrandmaster = (wpnSkill >= 99.9 && tactics >= 99.9);
            bool useTierTwo = isGrandmaster && Utility.RandomBool();

            switch (weapon.Skill)
            {
                case SkillName.Swords:
                    if (useTierTwo) ApplySever(attacker, defender);
                    else ApplyBleed(attacker, defender);
                    break;

                case SkillName.Archery:
                    if (useTierTwo) ApplyHeadshot(attacker, defender);
                    else ApplyHinder(attacker, defender);
                    break;

                case SkillName.Fencing:
                    if (useTierTwo) ApplyQuickStrike(attacker, defender);
                    else ApplyPierce(attacker, defender);
                    break;

                case SkillName.Macing:
                    if (useTierTwo) ApplyShatter(attacker, defender);
                    else ApplyConcussion(attacker, defender);
                    break;
            }
        }

        // --- TIER 1 ABILITIES ---

        private static void ApplyBleed(Mobile attacker, Mobile defender)
        {
            if (defender is BaseCreature { BleedImmune: true})
            {
                attacker.SendMessage("Your target is not effected by the bleed attack!");
                return;
            }

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

        // --- TIER 2 ABILITIES ---

        private static void ApplySever(Mobile attacker, Mobile defender)
        {
            defender.PlaySound(0x23B);
            defender.FixedParticles(0x3779, 1, 15, 9905, 32, 7, EffectLayer.Head);

            int damage = Utility.RandomMinMax(15, 25);
            //AOS.Damage(defender, attacker, damage, 100, 0, 0, 0, 0);
            defender.Damage(damage, attacker);
            attacker.SendMessage(0x21, $"You brutally sever your opponent, dealing extra damage!");
            defender.SendMessage(0x22, "A brutal strike tears into your flesh!");
        }

        private static void ApplyHeadshot(Mobile attacker, Mobile defender)
        {
            defender.PlaySound(0x11C); // Heavy thud
            defender.FixedParticles(0x373A, 1, 15, 9905, 33, 7, EffectLayer.Head);

            int directDamage = Utility.RandomMinMax(20, 30);
            AOS.Damage(defender, attacker, directDamage, true, 100, 0, 0, 0, 0, 0);

            attacker.SendMessage(0x481, $"HEADSHOT! You inflict {directDamage} direct damage!");
            defender.SendMessage(0x22, "An arrow strikes your head, bypassing your armor entirely!");
        }

        private static void ApplyQuickStrike(Mobile attacker, Mobile defender)
        {
            defender.PlaySound(0x2B1); // Fast swish sound
            defender.FixedParticles(0x37B9, 1, 14, 9905, 32, 7, EffectLayer.Waist);

            attacker.NextCombatTime = Core.TickCount;

            attacker.SendMessage(0x42, "You strike with blinding speed, granting an immediate follow-up attack!");
            defender.SendMessage(0x22, "Your opponent strikes with blinding speed!");
        }

        private static void ApplyShatter(Mobile attacker, Mobile defender)
        {
            defender.PlaySound(0x308); // Breaking sound
            defender.FixedParticles(0x375A, 1, 15, 9905, 12, 7, EffectLayer.Waist);

            defender.Stam /= 2;
            int damage = Utility.RandomMinMax(10, 20);
            //AOS.Damage(defender, attacker, damage, 100, 0, 0, 0, 0);
            defender.Damage(damage, attacker);
            attacker.SendMessage(0x35, "You shatter your opponent's guard, decimating their stamina!");
            defender.SendMessage(0x22, "A shattering blow knocks the wind out of you!");
        }
    }
}
