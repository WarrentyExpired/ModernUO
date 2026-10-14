using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Engines.DungeonTraps;

namespace Server.Regions
{
    public class GreetingDungeonRegion : NoTravelSpellsAllowedRegion
    {
        // 1. Initialize the pool of available traps
        private static readonly BaseRegionTrap[] _dungeonTraps =
        [
            new TripwireTrap(),
            new ExplosionTrap(),
            new FlameTrap(),
            new GasTrap()
        ];

        // 2. Track how many safe steps a player has remaining to prevent trap spam
        private readonly Dictionary<Mobile, int> _safeSteps = new();

        public GreetingDungeonRegion(string name, Map map, int priority, params Rectangle3D[] area)
            : base(name, map, priority, area)
        {
        }

        public override bool MountsAllowed => false;

        public override void OnEnter(Mobile m)
        {
            base.OnEnter(m);
            if (m.Player)
            {
                m.SendMessage($"You entered <BASEFONT COLOR='#FF0000'>{this.Name}</BASEFONT>.");

                // Give players a 10-step grace period right at the entrance
                _safeSteps[m] = 10;
            }

            if (m is PlayerMobile player && player.Mounted)
            {
                if (player.Mount is BaseMount)
                {
                    Server.Custom.AutoStable.StoreMount(player);
                }
                else
                {
                    BaseMount.Dismount(player);
                    player.SendMessage("Mounts are not permitted in this Dungeon.");
                }
            }
        }

        public override void OnExit(Mobile m)
        {
            base.OnExit(m);
            if (m is PlayerMobile player)
            {
                Server.Custom.AutoStable.RestoreMount(player);

                // Clean up the dictionary when they leave to free up memory
                _safeSteps.Remove(player);
            }
        }

        // 3. Evaluate every step taken inside the region
        public override void OnLocationChanged(Mobile m, Point3D oldLocation)
        {
            base.OnLocationChanged(m, oldLocation);

            if (!Settings.S_RegionDungeonTraps)
            {
                return;
            }

            // Only check for traps if they are an alive player who actually moved to a new tile
            if (m is PlayerMobile pm && pm.Alive && pm.Location != oldLocation && pm.AccessLevel == AccessLevel.Player)
            {
                CheckForTraps(pm);
            }
        }

        private void CheckForTraps(PlayerMobile pm)
        {
            // 4. Handle Safe Steps Cooldown
            if (!_safeSteps.TryGetValue(pm, out int safeStepsLeft))
            {
                safeStepsLeft = 0;
            }

            if (safeStepsLeft > 0)
            {
                _safeSteps[pm] = safeStepsLeft - 1;
                return; // Player is still in their grace period
            }

            // 5. Calculate Trigger Chance (Base 5% chance per step)
            // Detect Hidden actively reduces this chance.
            // At 0 DH = 5% chance. At 100 DH = 1% chance.
            double baseChance = ValidSettings.RegionDungeonTrapsChance();
            double detectHidden = pm.Skills[SkillName.DetectHidden].Value;
            double finalChance = Math.Max(1.0, baseChance - (detectHidden / 25.0));

            // 6. Roll for Trap Trigger
            if (Utility.RandomDouble() * 100.0 <= finalChance)
            {
                // Pick a random trap from the pool
                BaseRegionTrap trap = _dungeonTraps[Utility.Random(_dungeonTraps.Length)];

                // Fire the trap logic cascade (Remove Trap -> Dodge -> AR/MR -> Damage)
                trap.Trigger(pm);

                // Reset the player's safe steps to prevent back-to-back trap triggers
                _safeSteps[pm] = Utility.RandomMinMax(8, 12);
            }
        }
    }
}
