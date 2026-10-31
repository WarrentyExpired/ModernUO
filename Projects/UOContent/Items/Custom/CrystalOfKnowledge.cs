using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
    public class CrystalOfKnowledge : Item
    {
        private SkillName _Skill;

        [CommandProperty(AccessLevel.GameMaster)]
        public SkillName Skill
        {
            get => _Skill;
            set
            {
                _Skill = value;
                UpdateName();
            }
        }

        [Constructible]
        public CrystalOfKnowledge() : this((SkillName)Utility.Random(SkillInfo.Table.Length))
        {
        }

        [Constructible]
        public CrystalOfKnowledge(SkillName skill) : base(0x1F19)
        {
            _Skill = skill;
            Hue = 0x501;
            Weight = 1.0;
            UpdateName();
        }

        private void UpdateName()
        {
            Name = $"a minor shard of {_Skill}";
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001);
                return;
            }

            Skill targetSkill = from.Skills[_Skill];

            if (targetSkill == null)
                return;

            int maxCap = 120;
            int capIncrease = 1;

            if (targetSkill.Cap >= maxCap)
            {
                from.SendMessage(38, $"Your mastery of {_Skill} is already at its absolute limit.");
                return;
            }

            targetSkill.Cap += capIncrease;

            if (targetSkill.Cap > maxCap)
            {
                targetSkill.Cap = maxCap;
            }

            from.SendMessage(0x501, $"The shard dissolves, permanently increasing your {_Skill} cap by 1.0!");
            from.PlaySound(0x243);
            from.FixedParticles(0x373A, 10, 15, 5012, EffectLayer.Waist);

            Delete();
        }

        public CrystalOfKnowledge(Serial serial) : base(serial)
        {
        }

        // --- ModernUO Manual Serialization ---
        public override void Serialize(IGenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version

            // Save the specific skill ID to the hard drive
            writer.Write((int)_Skill);
        }

        public override void Deserialize(IGenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            // Load the specific skill ID back when the server boots
            _Skill = (SkillName)reader.ReadInt();
        }
    }
}
