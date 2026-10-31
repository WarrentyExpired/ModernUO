using System;
using Server;
using Server.Items;
using Server.Network;
using ModernUO.Serialization;

namespace Server.Mobiles
{
    [SerializationGenerator(0, false)]
    public partial class CrystallineTitan : BaseCreature
    {
        private DateTime _NextStanceChange;
        private TitanStance _CurrentStance;

        private enum TitanStance
        {
            Normal,
            DiamondCore,
            Shattered
        }

        [Constructible]
        public CrystallineTitan() : base(AIType.AI_Melee, FightMode.Closest)
        {
            Name = "The Crystalline Titan";
            Body = 1109;
            BaseSoundID = 0x127;

            SetStr(800, 1000);
            SetDex(120, 150);
            SetInt(100, 150);
            SetHits(15000, 20000);

            SetSkill(SkillName.Tactics, 100.0, 120.0);
            SetSkill(SkillName.Wrestling, 100.0, 120.0);
            SetSkill(SkillName.MagicResist, 120.0, 150.0);
            SetSkill(SkillName.Anatomy, 100.0, 120.0);

            Fame = 25000;
            Karma = -25000;
            VirtualArmor = 80;

            // Start in the normal stance to show off the raw graphic
            EnterNormal();

            // Give players 30 seconds of normal combat before the first special phase triggers
            _NextStanceChange = DateTime.UtcNow + TimeSpan.FromSeconds(30);
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Deleted || !Alive || Combatant == null)
                return;

            // Trigger the swap every 30 seconds while fighting
            if (DateTime.UtcNow > _NextStanceChange)
            {
                // Reset the clock immediately so OnThink doesn't loop
                _NextStanceChange = DateTime.UtcNow + TimeSpan.FromSeconds(30);

                if (_CurrentStance == TitanStance.DiamondCore || _CurrentStance == TitanStance.Shattered)
                {
                    SpeechHue = 0x3B2;
                    Say("*The Titan's crystals begin to stabilize...*");

                    // Wait 2 seconds, then execute the stat/hue change
                    Timer.DelayCall(TimeSpan.FromSeconds(2.0), () => { if (Alive && !Deleted) EnterNormal(); });
                }
                else
                {
                    if (Utility.RandomBool())
                    {
                        SpeechHue = 0x47E;
                        Say("*The Titan braces itself as its shell begins to harden!*");

                        Timer.DelayCall(TimeSpan.FromSeconds(2.0), () => { if (Alive && !Deleted) EnterDiamondCore(); });
                    }
                    else
                    {
                        SpeechHue = 0x22;
                        Say("*The Titan's crystals throb with unstable, violent energy!*");

                        Timer.DelayCall(TimeSpan.FromSeconds(2.0), () => { if (Alive && !Deleted) EnterShattered(); });
                    }
                }
            }
        }

        private void EnterNormal()
        {
            _CurrentStance = TitanStance.Normal;
            Hue = 0;

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Energy, 50, 60);
            SetResistance(ResistanceType.Fire, 50, 60);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 60, 70);

            SetDamage(30, 45);

            PlaySound(0x11F); // Deep rumble sound as the hue shifts
        }

        private void EnterDiamondCore()
        {
            _CurrentStance = TitanStance.DiamondCore;
            Hue = 0x47E;

            SetResistance(ResistanceType.Physical, 80, 90);
            SetResistance(ResistanceType.Energy, 70, 80);
            SetResistance(ResistanceType.Fire, 60, 70);
            SetResistance(ResistanceType.Cold, 60, 70);
            SetResistance(ResistanceType.Poison, 100, 100);

            SetDamage(15, 25);

            PlaySound(0x1E5); // Hardening/magic sound as the hue shifts
        }

        private void EnterShattered()
        {
            _CurrentStance = TitanStance.Shattered;
            Hue = 0x86;

            SetResistance(ResistanceType.Physical, 30, 40);
            SetResistance(ResistanceType.Energy, 30, 40);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 30, 40);

            SetDamage(45, 65);

            PlaySound(0x1F8); // Shattering explosion sound as the hue shifts
        }

        // --- Boss Immunities ---
        public override bool AutoDispel => true;
        public override bool BardImmune => true;
        public override bool Uncalmable => true;
        public override Poison PoisonImmune => Poison.Lethal;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.SuperBoss, 2);
        }

        public CrystallineTitan(Serial serial) : base(serial)
        {
        }
    }
}
