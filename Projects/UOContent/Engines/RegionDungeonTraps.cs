using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Spells;

namespace Server.Engines.DungeonTraps
{
    public enum TrapDamageType
    {
        Physical,
        Magical
    }

    public abstract class BaseRegionTrap
    {
        public abstract string Name { get; }
        public abstract TrapDamageType DamageType { get; }
        public abstract int MinDamage { get; }
        public abstract int MaxDamage { get; }

        // The master method called when a region step roll succeeds
        public virtual void Trigger(PlayerMobile pm)
        {
            // 1. Detect Hidden Check (Spot and Avoid)
            // Uses CheckSkill to allow for skill gains. 0.0 to 100.0 means the success chance scales exactly with their skill.
            if (pm.Skills[SkillName.DetectHidden].Value > 0 && pm.CheckSkill(SkillName.DetectHidden, 0.0, 100.0))
            {
                if (Settings.S_TrapDetected)
                {
                    pm.SendMessage(0x42, $"You spot a concealed {Name} and carefully step around it.");
                }
                return;
            }

            // 2. Remove Trap Check (Absolute Disarm)
            if (pm.Skills[SkillName.RemoveTrap].Value > 0 && pm.CheckSkill(SkillName.RemoveTrap, 0.0, 100.0))
            {
                if (Settings.S_TrapDetected)
                {
                    pm.SendMessage(0x35, $"You hear a click and instinctively jam the {Name} mechanism before it fires.");
                    pm.PlaySound(0x3A4); // Click sound
                }
                return;
            }

            // 3. Evasion Check: (Dex + Int) * 0.25
            double evasionChance = (pm.Dex + pm.Int) * 0.25;
            if (evasionChance >= Utility.RandomDouble() * 100.0)
            {
                if (Settings.S_TrapDetected)
                {
                    pm.SendMessage(0x59, $"A {Name} triggers, but your quick reflexes allow you to dodge it!");
                    PlayTrapEffect(pm); // Trap goes off, but misses
                }
                return;
            }

            // 4. Mitigation Check (AR for Physical, Magic Resist for Magical)
            PlayTrapEffect(pm);
            bool mitigated = false;

            if (DamageType == TrapDamageType.Physical)
            {
                // Using PhysicalResistance (or VirtualArmor for pre-AoS)
                if (pm.PhysicalResistance >= Utility.RandomDouble() * 100.0)
                {
                    mitigated = true;
                }
            }
            else // Magical
            {
                // Magic Resist check with skill gain opportunity
                if (pm.Skills[SkillName.MagicResist].Value > 0 && pm.CheckSkill(SkillName.MagicResist, 0.0, 100.0))
                {
                    mitigated = true;
                }
            }

            // 5. Resolve Impact
            if (mitigated)
            {
                if (Settings.S_TrapDamageMitigated)
                {
                    pm.SendMessage(0x22, $"Your defenses absorb the {Name}, but your equipment takes a beating!");
                    DamageRandomArmor(pm);
                }
            }
            else
            {
                if (Settings.S_TrapDamageMitigated)
                {
                    pm.SendMessage(0x22, $"You are struck by a {Name}!");
                    ApplyDamage(pm);
                }
            }
        }

        protected virtual void DamageRandomArmor(PlayerMobile pm)
        {
            // Gather all equippable armor the player is wearing
            List<BaseArmor> wornArmor = new List<BaseArmor>();
            foreach (Item item in pm.Items)
            {
                if (item is BaseArmor armor && armor.HitPoints > 0)
                {
                    wornArmor.Add(armor);
                }
            }

            if (wornArmor.Count > 0)
            {
                // Pick a random piece of armor
                BaseArmor hitArmor = wornArmor[Utility.Random(wornArmor.Count)];

                // Calculate 5% to 10% of current hit points
                int damage = Utility.RandomMinMax((int)(hitArmor.HitPoints * 0.05), (int)(hitArmor.HitPoints * 0.10));

                // Ensure it does at least 1 damage, but doesn't drop below 0
                damage = Math.Max(1, damage);
                hitArmor.HitPoints = Math.Max(0, hitArmor.HitPoints - damage);
            }
            else
            {
                // If they are totally naked but magically mitigated it, they take a small stamina penalty instead
                pm.Stam -= 10;
            }
        }

        public abstract void PlayTrapEffect(PlayerMobile pm);

        public virtual void ApplyDamage(PlayerMobile pm)
        {
            int damage = Utility.RandomMinMax(MinDamage, MaxDamage);
            AOS.Damage(pm, damage, DamageType == TrapDamageType.Physical ? 100 : 0, DamageType == TrapDamageType.Physical ? 0 : 100, 0, 0, 0);
        }
    }

    // ==========================================
    // SPECIFIC TRAP DEFINITIONS
    // ==========================================

    public class TripwireTrap : BaseRegionTrap
    {
        public override string Name => "tripwire";
        public override TrapDamageType DamageType => TrapDamageType.Physical;
        public override int MinDamage => 15;
        public override int MaxDamage => 25;

        public override void PlayTrapEffect(PlayerMobile pm)
        {
            pm.PlaySound(0x3E4); // Snapping sound
            Effects.SendLocationEffect(pm.Location, pm.Map, 0x373A, 10, 10); // Dust cloud
        }

        public override void ApplyDamage(PlayerMobile pm)
        {
            base.ApplyDamage(pm);
            pm.Stam -= 15; // Tripwires knock the wind out of you
        }
    }

    public class ExplosionTrap : BaseRegionTrap
    {
        public override string Name => "hidden explosive";
        public override TrapDamageType DamageType => TrapDamageType.Physical;
        public override int MinDamage => 15;
        public override int MaxDamage => 25;

        public override void PlayTrapEffect(PlayerMobile pm)
        {
            pm.PlaySound(0x207); // Explosion sound
            Effects.SendLocationEffect(pm.Location, pm.Map, 0x36BD, 10, 10); // Big explosion visual
        }
    }

    public class FlameTrap : BaseRegionTrap
    {
        public override string Name => "flame jet";
        public override TrapDamageType DamageType => TrapDamageType.Magical;
        public override int MinDamage => 15;
        public override int MaxDamage => 25;

        public override void PlayTrapEffect(PlayerMobile pm)
        {
            pm.PlaySound(0x226); // Fire sound
            Effects.SendLocationParticles(EffectItem.Create(pm.Location, pm.Map, EffectItem.DefaultDuration), 0x3709, 10, 30, 5052); // Flame column
        }
    }

    public class GasTrap : BaseRegionTrap
    {
        public override string Name => "poison gas cloud";
        public override TrapDamageType DamageType => TrapDamageType.Magical;
        public override int MinDamage => 5;
        public override int MaxDamage => 10; // Low initial damage, but applies poison

        public override void PlayTrapEffect(PlayerMobile pm)
        {
            pm.PlaySound(0x230); // Hissing gas sound
            Effects.SendLocationEffect(pm.Location, pm.Map, 0x3728, 10, 10, 63); // Green smoke
        }

        public override void ApplyDamage(PlayerMobile pm)
        {
            base.ApplyDamage(pm);

            // Gas traps apply a poison effect if the player takes damage
            if (!pm.Poisoned)
            {
                pm.ApplyPoison(pm, Poison.Regular);
            }
        }
    }
}
