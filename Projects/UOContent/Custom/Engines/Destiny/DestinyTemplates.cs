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
            GiveStartingLoot = (pm) =>
            {
                pm.AddToBackpack(new Scissors());
                pm.AddToBackpack(new Bedroll());
                pm.AddToBackpack(new Candle());
                loot?.Invoke(pm);
            };
        }

        public static List<DestinyTemplate> AllTemplates = new List<DestinyTemplate>()
        {
            new DestinyTemplate("Warrior", "30 Macing, Tactics, Anatomy, Healing",
                new Dictionary<SkillName, double>{{SkillName.Macing, 30.0}, {SkillName.Tactics, 30.0}, {SkillName.Anatomy, 30.0}, {SkillName.Healing, 30}},
                (pm) => {
                            pm.AddToBackpack(new Club());
                            pm.AddToBackpack(new Buckler());
                            pm.AddToBackpack(new Bandage(100));
                        }),

            new DestinyTemplate("Mage", "30 Magery, EvalInt, Med, Inscription",
                new Dictionary<SkillName, double>{{SkillName.Magery, 30.0}, {SkillName.EvalInt, 30.0}, {SkillName.Meditation, 30.0}, {SkillName.Inscribe, 30.0}},
                (pm) => {
                            pm.AddToBackpack(new Spellbook());
                            pm.AddToBackpack(new BagOfAllReagents());
                            pm.AddToBackpack(new HarmScroll());
                            pm.AddToBackpack(new HealScroll());
                            pm.AddToBackpack(new BlessScroll());
                            pm.AddToBackpack(new BlankScroll(50));
                            pm.AddToBackpack(new ScribesPen());
                        }),

            new DestinyTemplate("Archer", "30 Archery, Tactics, Anatomy, Fletching",
                new Dictionary<SkillName, double>{{SkillName.Archery, 30.0}, {SkillName.Tactics, 30.0}, {SkillName.Anatomy, 30.0}, {SkillName.Fletching, 30.0}},
                (pm) => {
                            pm.AddToBackpack(new Bow());
                            pm.AddToBackpack(new RepeatingCrossbow());
                            pm.AddToBackpack(new Bolt(100));
                            pm.AddToBackpack(new Arrow(100));
                            pm.AddToBackpack(new FletcherTools());
                        }),

            new DestinyTemplate("Bard", "30 Music, Provo, Discord",
                new Dictionary<SkillName, double>{{SkillName.Musicianship, 30.0}, {SkillName.Provocation, 30.0}, {SkillName.Discordance, 30.0}},
                (pm) => {
                            pm.AddToBackpack(new Tambourine());
                            pm.AddToBackpack(new Drums());
                        }),

            new DestinyTemplate("Tamer", "30 Taming, Lore, Vet",
                new Dictionary<SkillName, double>{{SkillName.AnimalTaming, 30.0}, {SkillName.AnimalLore, 30.0}, {SkillName.Veterinary, 30.0}},
                (pm) => {
                            pm.AddToBackpack(new ShepherdsCrook());
                            pm.AddToBackpack(new Bandage(100));
                        }),

            new DestinyTemplate("Rogue", "30 Fencing, Hiding, Stealth, Ninjitsu",
                new Dictionary<SkillName, double>{{SkillName.Fencing, 30.0}, {SkillName.Hiding, 30.0}, {SkillName.Stealth, 30.0}, {SkillName.Ninjitsu, 30}},
                (pm) => {
                            pm.AddToBackpack(new Kryss());
                            pm.AddToBackpack(new BookOfNinjitsu());
                        }),

            new DestinyTemplate("Knight", "30 Swords, Chivalry, Tactics",
                new Dictionary<SkillName, double>{{SkillName.Swords, 30.0}, {SkillName.Chivalry, 30.0}, {SkillName.Tactics, 30}},
                (pm) => {
                            pm.AddToBackpack(new Katana());
                            pm.AddToBackpack(new BookOfChivalry());
                        }),
            new DestinyTemplate("Tailor", "30 Tailor, Tinker",
                new Dictionary<SkillName, double>{{SkillName.Tailoring, 30}, {SkillName.Tinkering, 30}},
                (pm) => {
                            pm.AddToBackpack(new SewingKit());
                            pm.AddToBackpack(new TinkerTools());
                            pm.AddToBackpack(new UncutCloth(50));
                            pm.AddToBackpack(new IronIngot(50));
                        }),
            new DestinyTemplate("Blacksmith", "30 Smithing, Mining, Tinkering",
                new Dictionary<SkillName, double>{{SkillName.Blacksmith, 30}, {SkillName.Mining, 30}, {SkillName.Tinkering, 30}},
                (pm) => {
                            pm.AddToBackpack(new SmithHammer());
                            pm.AddToBackpack(new Pickaxe());
                            pm.AddToBackpack(new TinkerTools());
                            pm.AddToBackpack(new IronIngot(100));
                        }),
        };
    }
}
