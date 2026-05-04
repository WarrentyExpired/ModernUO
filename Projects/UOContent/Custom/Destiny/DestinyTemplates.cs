using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;

namespace Server.Destiny
{
    public class DestinyTemplate
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Dictionary<SkillName, double> Skills { get; set; }
        public Action<PlayerMobile> GiveStartingLoot { get; set; }

        public DestinyTemplate(string name, string desc, Dictionary<SkillName, double> skills, Action<PlayerMobile> loot)
        {
            Name = name;
            Description = desc;
            Skills = skills;
            GiveStartingLoot = loot;
        }

        public static List<DestinyTemplate> AllTemplates = new List<DestinyTemplate>()
        {
            new DestinyTemplate("Warrior", "30 Swords, Tactics, Anatomy",
                new Dictionary<SkillName, double>{{SkillName.Swords, 30.0}, {SkillName.Tactics, 30.0}, {SkillName.Anatomy, 30.0}, {SkillName.Healing, 30}},
                (pm) => { pm.AddToBackpack(new Katana()); pm.AddToBackpack(new Buckler()); pm.AddToBackpack(new Bandage(100)); }),

            new DestinyTemplate("Mage", "30 Magery, EvalInt, Med",
                new Dictionary<SkillName, double>{{SkillName.Magery, 30.0}, {SkillName.EvalInt, 30.0}, {SkillName.Meditation, 30.0}},
                (pm) => { pm.AddToBackpack(new Spellbook()); }),

            new DestinyTemplate("Archer", "30 Archery, Tactics, Anatomy",
                new Dictionary<SkillName, double>{{SkillName.Archery, 30.0}, {SkillName.Tactics, 30.0}, {SkillName.Anatomy, 30.0}},
                (pm) => { pm.AddToBackpack(new Bow()); pm.AddToBackpack(new Arrow(100)); }),

            new DestinyTemplate("Bard", "30 Music, Provo, Discord",
                new Dictionary<SkillName, double>{{SkillName.Musicianship, 30.0}, {SkillName.Provocation, 30.0}, {SkillName.Discordance, 30.0}},
                (pm) => { pm.AddToBackpack(new Tambourine()); pm.AddToBackpack(new Drums()); }),

            new DestinyTemplate("Tamer", "30 Taming, Lore, Vet",
                new Dictionary<SkillName, double>{{SkillName.AnimalTaming, 30.0}, {SkillName.AnimalLore, 30.0}, {SkillName.Veterinary, 30.0}},
                (pm) => { pm.AddToBackpack(new ShepherdsCrook()); pm.AddToBackpack(new Bandage(100)); }),

            new DestinyTemplate("Rogue", "30 Fencing, Hiding, Stealth",
                new Dictionary<SkillName, double>{{SkillName.Fencing, 30.0}, {SkillName.Hiding, 30.0}, {SkillName.Stealth, 30.0}},
                (pm) => { pm.AddToBackpack(new Kryss()); }),

            // Add more here easily!
        };
    }
}
