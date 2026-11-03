using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
    public class CrystalOfKnowledge : Item
    {
        private SkillName _Skill;

        // The curated list of AOS skills. Delete or comment out any skills you don't want dropping!
        private static readonly SkillName[] _ValidSkills = new SkillName[]
        {
            SkillName.Anatomy, SkillName.AnimalLore,
            SkillName.Parry,
            SkillName.Peacemaking,
            SkillName.Discordance,
            SkillName.EvalInt, SkillName.Healing, SkillName.Fishing,
            SkillName.Provocation,
            SkillName.Lockpicking, SkillName.Magery, SkillName.MagicResist, SkillName.Tactics,
            SkillName.Musicianship, SkillName.Archery,
            SkillName.SpiritSpeak, SkillName.Stealing, SkillName.AnimalTaming,
            SkillName.Veterinary,
            SkillName.Swords, SkillName.Macing, SkillName.Fencing, SkillName.Wrestling,
            SkillName.Meditation, SkillName.Stealth,
            SkillName.Necromancy, SkillName.Focus, SkillName.Chivalry
        };

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

        // Now pulls exclusively from the _ValidSkills array
        [Constructible]
        public CrystalOfKnowledge() : this(_ValidSkills[Utility.Random(_ValidSkills.Length)])
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
            Name = $"a crystalline shard of {_Skill}";
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

            from.SendMessage(0x501, $"The crystal dissolves, permanently increasing your {_Skill} cap by 1.0!");
            from.PlaySound(0x243);
            from.FixedParticles(0x373A, 10, 15, 5012, EffectLayer.Waist);

            Delete();
        }

        public CrystalOfKnowledge(Serial serial) : base(serial)
        {
        }

        public override void Serialize(IGenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write((int)_Skill);
        }

        public override void Deserialize(IGenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            _Skill = (SkillName)reader.ReadInt();
        }
    }
}
