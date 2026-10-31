using System;
using Server;
using Server.Mobiles;
using ModernUO.Serialization;

namespace Server.Items
{
    [SerializationGenerator(0)]
    public partial class CrystalOfInsight : Item
    {
        [Constructible]
        public CrystalOfInsight() : base(0x1F19) // A cool crystal/gem graphic
        {
            Name = "a crystalline shard of insight";
            Hue = 0x47E; // Matches the Diamond Core stance hue
            Weight = 1.0;
        }

        public override void OnDoubleClick(Mobile from)
        {
            // Security check: Must be in the player's main backpack to use
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // "That must be in your pack for you to use it."
                return;
            }

            // In UO integer logic, 720.0 is 7200, and 1.0 is 10
            int maxCap = 7200;
            int capIncrease = 10;

            if (from.SkillsCap >= maxCap)
            {
                from.SendMessage(38, "Your mind cannot expand any further. You are already at the absolute maximum skill cap.");
                return;
            }

            // Apply the permanent increase
            from.SkillsCap += capIncrease;

            // Failsafe: If they somehow used a +1.0 item while at 719.5, hard-cap it at exactly 720.0
            if (from.SkillsCap > maxCap)
            {
                from.SkillsCap = maxCap;
            }

            // Theatrical effects to make the reward feel powerful
            from.SendMessage(0x47E, "The crystal shatters in your hands, expanding your mind! Your total skill cap has permanently increased by 1.0.");
            from.PlaySound(0x1F8); // Shatter sound (same as the Titan's stance shift)
            from.FixedParticles(0x373A, 10, 15, 5012, EffectLayer.Waist); // Sparkle effect around the player

            // Destroy the item now that it has been consumed
            Delete();
        }

        public CrystalOfInsight(Serial serial) : base(serial)
        {
        }
    }
}
