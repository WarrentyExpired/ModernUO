# Sovereign Realm

A custom Ultima Online shard built on [ModernUO](https://github.com/ModernUO/ModernUO), focused on the **Ultima Online: Renaissance (UOR)** era with custom maps and mechanics.
It is in very early stages, but it is playable. A few of the dungeons are spawned. Most of the telepoters have been place.
And yes **AI** helped me!


## Repository Branches
- `uor` *(Default)* — Active, playable Sovereign Realm server codebase
- `aos` — Age of Shadows custom branch **(Abandoned for now)**
- `main` — Vanilla upstream ModernUO mirror (used for pulling engine updates)

## Getting Started
- Coming Soon.
<!--
1. Clone the repository:
   git clone [https://github.com/Warrentyexpired/ModernUO.git](https://github.com/Warrentyexpired/ModernUO.git)
-->

# Sovereign Realm — Custom Features & Changes

## World & Exploration

### Citizens & Living Towns
- **Wandering Citizens:** Human and monster citizens wander around towns, speaking unique lines of dialogue (some true, some false).
- **Smart Spawning:** Citizens spawn gradually to populate towns naturally and are restricted to specific outdoor tiles (dirt, gravel, and stone) so they never spawn inside houses or businesses.
- **Tradesman Citizens:** Dedicated NPCs stationed at work areas (such as standing at anvils crafting armor and weapons) to bring towns to life.

### Portal Gates
- Spawn dynamically across the Overworld and unleash dangerous monsters when a player steps within a set tile radius.
- Automatically despawn after 3 minutes if no combatants remain nearby.

### Dungeon Bosses & Progression
- **Summonable Bosses:** Require a special summoning item dropped by specific monsters within that dungeon.
- **Stat & Skill Crystals:** Dungeon Bosses are the exclusive source of the **Crystal of Knowledge**, **Crystal of Fortitude**, and **Crystal of Insight**, which act like Power Scrolls to raise individual skill caps, stat caps, or total skill caps by **+1** at a time.

### New Player Dungeon
- A large, two-level starter dungeon featuring weaker monsters on Level 1.
- Offers accelerated skill gain rates up to **80.0 real skill**.

### Bounty System & Altar of Vanquishing
- **Bounty Houses:** Towns now feature a Bounty House, Bounty Vendor, and Bounty Board.
- **Fame-Based Bounties:** Higher Fame unlocks higher-tier bounties on the board, rewarding **Bounty Coins** to spend at the Bounty Vendor.
- **Altar of Vanquishing:** Killing monsters awards **Kill Points** scaled to the monster's Fame, which can be spent at the Altar inside any Bounty House.

---

## Combat, Skills & Character Progression

### Innate Weapon Abilities
- Every weapon class (except Wrestling) features **2 innate abilities**.
- **Unlock Requirements:** 50.0 Weapon Skill & Tactics for the first ability; 100.0 Weapon Skill & Tactics for the second.
- **Activation:** 10% chance to trigger on hit with a 5-second cooldown.

### Healing & Bandage Rework
- **Bandages (Heal Over Time):** Bandages now restore small ticks of HP over their duration instead of all at once (any HP ticked is subtracted from the final heal total), and bandages no longer cure poison.
- **Active Healing Skill:** The **Healing** skill can now be activated directly to instantly cure poison and bleed effects, triggering a 15-second skill lock.

### Skill Gain & Stat Bonuses
- **Accelerated Skill Zones:** Dungeons, the New Player Dungeon, and the **Dungeon of the Day** grant faster skill gain rates.
- **Stat-to-Skill Synergy:** Certain skills receive bonus points scaled to your character's stats.
- **Lifetime Character Tracking:** Characters permanently track **Total Gold Looted**, **Total Deaths**, **Spendable Monster Kill Points**, and **Lifetime Monster Kill Points**.

### Hunger, Thirst & Exhaustion
- **Regeneration Scaling:** Maintaining **15+** Hunger and Thirst grants a small passive stat regeneration bonus; dropping to **5 or below** applies a negative stat regeneration penalty.
- **Exhaustion System:** Every **200 points of damage** taken reduces Hunger and Thirst by 1.

---

## Dungeons, Traps & Treasure

### Overhauled Dungeon Trap System
- Floor traps have a **2% chance** to trigger with every step.
- **Detect Hidden** and **Remove Trap** can prevent traps from triggering.
- Dexterity, Intelligence, Physical Resist, and Magic Resist help mitigate or avoid trap damage.

### Dungeon Loot & Thief Pedestals
- **Dungeon Containers:** Dungeon bags are always trapped and contain Level 1–3 loot; dungeon chests are both locked and trapped and contain Level 4–6 loot.
- **Thief Pedestals:** Spawn trapped bags of random loot (Levels 0–6) requiring **Snooping** and **Stealing** to loot. Once the bag is taken—or if the trap is tripped—the pedestal vanishes.

### Treasure Maps & Fishing SOS
- **Treasure Maps:** Updated with a custom gump. Digging up chests now requires an **equipped pickaxe**.
- **SOS Messages:** Display exact `X, Y` coordinates if you have a **Sextant** in your backpack.

---

## Crafting & Gathering Quality-of-Life

### Crafting UI Upgrades
- **Crafting Queue:** Queue up specific items and quantities directly from the crafting menu, accessible via a new button on the main crafting gump.
- **Crafting Skill Guide:** A new button on the main crafting gump opens a guide displaying everything you can craft at your current skill level.

### Gathering & Processing
- **Auto-Harvesting:** **Mining**, **Lumberjacking**, and **Fishing** automatically continue harvesting without needing to repeatedly re-target.
- **Shovels Removed:** Shovels have been removed from the game because they lack an equipped paperdoll animation; all mining and treasure hunting is now performed with an equipped **pickaxe**.
- **Ore & Smelting:** Mined ore only spawns with the large pile graphic, and smelting automatically processes **1 ore at a time** with a skill-gain check on each smelt.
- **Spinning Wheels & Looms:** Spinning wheels process an entire stack of Cotton, Flax, or Wool at once; looms process an entire stack of thread at once.
- **Walk-Over Farming:** Crop plots only require walking over mature crops to harvest them straight into your backpack.

### Quality-of-Life Features
- **Auto-Mount / Dismount:** Players automatically dismount when entering restricted regions (such as dungeons and town businesses) and automatically remount upon leaving.

---

## Commands & Server Configuration

### Player & Admin Commands

| Command | Description |
| :--- | :--- |
| `[Dashboard` | Opens a comprehensive character statistics dashboard. |
| `[Bounties` | Opens a gump displaying your current bounty progress. |
| `[DOTD` | Displays the current **Dungeon of the Day**. |
| `[BandSelf` | Applies a bandage from your backpack (will not restart if a bandage is already ticking). |
| `[BuildWorld` | Opens the updated world-building gump. |
| `[IS <path>` | Imports spawners from a JSON file (e.g., `[IS Data/Spawns/Dungeon/Shame.json`). |
| `[EAS` | Exports all spawners on the current map to `Data/Spawns/Exported/<timestamp>.json`. |
| `[ECN` | Exports all `CitizenNodes` on the current map. |
| *(Spawner)* `FarmSpawner` | Simplified farm plot spawner for quickly setting up farmland. |

### Server Settings
- **`Settings.cs` & `ValidSettings.cs`:** Server administrators can customize shard mechanics in `Settings.cs`, while `ValidSettings.cs` enforces safe minimum and maximum bounds.
