using System;
using Server;
using Server.Mobiles;
using ModernUO.Serialization;

namespace Server.Items
{
    [SerializationGenerator(0)]
    public partial class CrystalOfFortitude : Item
    {
        [Constructible]
        public CrystalOfFortitude() : base(0x1F19) // Matching crystal graphic
        {
            Name = "a crystalline shard of fortitude";
            Hue = 0x21; // A deep, vibrant red hue to represent physical stats
            Weight = 1.0;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // "That must be in your pack for you to use it."
                return;
            }

            // Standard stats are handled as simple integers (unlike skills which use tenths)
            int maxCap = 250;
            int capIncrease = 1;

            if (from.StatCap >= maxCap)
            {
                from.SendMessage(38, "Your physical form cannot be pushed any further. You are already at the absolute maximum stat cap.");
                return;
            }

            // Apply the permanent stat cap increase
            from.StatCap += capIncrease;

            // Failsafe in case a GM accidentally sets them too high
            if (from.StatCap > maxCap)
            {
                from.StatCap = maxCap;
            }

            // Theatrical effects
            from.SendMessage(0x21, "The crystal shatters, surging through your veins! Your total stat cap has permanently increased by 1.");
            from.PlaySound(0x1F8);
            from.FixedParticles(0x373A, 10, 15, 5012, EffectLayer.Waist);

            // Destroy the item
            Delete();
        }

        public CrystalOfFortitude(Serial serial) : base(serial)
        {
        }
    }
}
