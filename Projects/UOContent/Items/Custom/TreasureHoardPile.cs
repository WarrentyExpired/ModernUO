using System;
using ModernUO.Serialization;
using Server.Mobiles;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class TreasureHoardPile : Item
{
    [SerializableField(0)]
    [SerializedCommandProperty(AccessLevel.GameMaster)]
    private int _pullsRemaining;

    [SerializableField(1)]
    [SerializedCommandProperty(AccessLevel.GameMaster)]
    private int _sourceFame;

    [SerializableField(2)]
    private Mobile _owner;

    [SerializableField(3)]
    private DateTime _ownerLockExpire;

    private Timer _decayTimer;

    [Constructible]
    public TreasureHoardPile(int sourceFame = 10000, Mobile owner = null) : base(0xFFA3)
    {
        Name = GetTitleForFame(sourceFame);
        Movable = false;

        _sourceFame = sourceFame;
        _pullsRemaining = GetInitialPulls(sourceFame);
        _owner = owner;
        _ownerLockExpire = DateTime.UtcNow + TimeSpan.FromSeconds(60);

        StartDecayTimer(TimeSpan.FromMinutes(15));
    }

    private static string GetTitleForFame(int fame) => fame switch
    {
        >= 22000 => "a massive dragon's treasure hoard",
        >= 15000 => "a rich treasure hoard",
        _        => "a scattered treasure hoard"
    };

    private static int GetInitialPulls(int fame) => fame switch
    {
        >= 22000 => Utility.RandomMinMax(5, 8),
        >= 15000 => Utility.RandomMinMax(3, 6),
        _        => Utility.RandomMinMax(2, 4)
    };

    private void StartDecayTimer(TimeSpan delay)
    {
        _decayTimer?.Stop();
        _decayTimer = Timer.DelayCall(delay, Delete);
    }

    [AfterDeserialization]
    private void AfterDeserialization()
    {
        StartDecayTimer(TimeSpan.FromMinutes(15));
    }

    public override void OnAfterDelete()
    {
        _decayTimer?.Stop();
        base.OnAfterDelete();
    }

    public override void OnSingleClick(Mobile from)
    {
        base.OnSingleClick(from);
        LabelTo(from, $"[{_pullsRemaining} {(_pullsRemaining == 1 ? "pull" : "pulls")} remaining]");
    }

    public override void OnDoubleClick(Mobile from)
    {
        if (Deleted || _pullsRemaining <= 0)
        {
            Delete();
            return;
        }

        if (!from.InRange(GetWorldLocation(), 2))
        {
            from.SendLocalizedMessage(500446); // That is too far away.
            return;
        }

        if (!from.CanSee(this) || !from.InLOS(this))
        {
            from.SendLocalizedMessage(500237); // Target can not be seen.
            return;
        }

        // Respect 60-second killer looting rights (party members allowed)
        if (_owner != null && from != _owner && DateTime.UtcNow < _ownerLockExpire && from.AccessLevel < AccessLevel.GameMaster)
        {
            if (from.Party == null || from.Party != _owner.Party)
            {
                from.SendMessage(0x22, "You did not earn the right to loot this hoard yet.");
                return;
            }
        }

        Item reward = GeneratePullReward(_sourceFame);
        if (reward != null)
        {
            from.AddToBackpack(reward);
            from.PlaySound(0x2E6); // Gold coins sound
            from.SendMessage(0x35, $"You pull {reward.Name ?? "an item"} from the treasure hoard!");
        }

        _pullsRemaining--;

        if (_pullsRemaining <= 0)
        {
            from.SendMessage(0x3B2, "You scoop up the last of the treasure hoard.");
            Delete();
        }
    }

    private static Item GeneratePullReward(int fame)
    {
        // Tier 0 = 10k-14.9k, Tier 1 = 15k-21.9k, Tier 2 = 22k+
        int tier = fame >= 22000 ? 2 : fame >= 15000 ? 1 : 0;
        int roll = Utility.Random(100);

        // 35% Gold pile
        if (roll < 35)
        {
            int goldAmount = tier switch
            {
                2 => Utility.RandomMinMax(1500, 3000),
                1 => Utility.RandomMinMax(800, 1500),
                _ => Utility.RandomMinMax(400, 800)
            };
            return new Gold(goldAmount);
        }

        // 25% Magic Weapon (UOR-scaled by fame tier)
        if (roll < 60)
        {
            var weapon = Loot.RandomWeapon();
            if (Core.UOR)
            {
                int minLvl = tier + 1; // Tier 0: Ruin+, Tier 1: Might+, Tier 2: Force+
                int maxLvl = Math.Min(tier + 3, (int)WeaponDamageLevel.Vanq);
                weapon.DamageLevel = (WeaponDamageLevel)Utility.RandomMinMax(minLvl, maxLvl);
                weapon.AccuracyLevel = (WeaponAccuracyLevel)Utility.RandomMinMax(minLvl, Math.Min(tier + 3, (int)WeaponAccuracyLevel.Supremely));
                weapon.DurabilityLevel = (WeaponDurabilityLevel)Utility.RandomMinMax(minLvl, Math.Min(tier + 3, (int)WeaponDurabilityLevel.Indestructible));
                weapon.Identified = false;

                // Small chance at a Slayer weapon on high-fame hoards
                if (tier >= 1 && Utility.Random(10) == 0)
                {
                    weapon.Slayer = BaseRunicTool.GetRandomSlayer();
                }
            }
            return weapon;
        }

        // 20% Magic Armor / Shield (UOR-scaled by fame tier)
        if (roll < 80)
        {
            var armor = Loot.RandomArmorOrShield();
            if (Core.UOR)
            {
                int minLvl = tier + 1;
                int maxLvl = Math.Min(tier + 3, (int)ArmorProtectionLevel.Invulnerability);
                armor.ProtectionLevel = (ArmorProtectionLevel)Utility.RandomMinMax(minLvl, maxLvl);
                armor.Durability = (ArmorDurabilityLevel)Utility.RandomMinMax(minLvl, Math.Min(tier + 3, (int)ArmorDurabilityLevel.Indestructible));
                armor.Identified = false;
            }
            return armor;
        }

        // 10% Bounty Tokens
        if (roll < 90)
        {
            int tokens = tier switch
            {
                2 => Utility.RandomMinMax(15, 30),
                1 => Utility.RandomMinMax(8, 15),
                _ => Utility.RandomMinMax(3, 8)
            };
            return new BountyToken(tokens) { Name = $"{tokens} Bounty Tokens" };
        }

        // 10% Treasure Map or Gems/Reagents bundle
        if (Utility.RandomBool())
        {
            int mapLevel = tier switch
            {
                2 => Utility.RandomMinMax(4, 5),
                1 => Utility.RandomMinMax(3, 4),
                _ => Utility.RandomMinMax(1, 3)
            };
            return new TreasureMap(mapLevel, Map.Felucca);
        }

        var gem = Loot.RandomGem();
        gem.Amount = Utility.RandomMinMax(5, 10 + (tier * 5));
        return gem;
    }

    // Helper called from BaseCreature.OnDeath
    public static void TrySpawnFromCreature(BaseCreature bc, Point3D corpseLoc, Map map, Mobile killer, bool isDailyDungeon = false)
    {
        if (bc == null || bc.Summoned || bc.Controlled || bc.Fame < 10000 || map == null || map == Map.Internal)
        {
            return;
        }

        // Base chance: 3% at 10k Fame up to 12% max (+2% flat bonus in the active Daily Dungeon)
        double chance = Math.Min(0.12, 0.03 + ((bc.Fame - 10000) / 2000.0) * 0.01);
        if (isDailyDungeon)
        {
            chance += 0.02;
        }

        if (Utility.RandomDouble() >= chance)
        {
            return;
        }

        Point3D spawnLoc = corpseLoc;

        for (int i = 0; i < 10; i++)
        {
            int x = corpseLoc.X + Utility.RandomMinMax(-2, 2);
            int y = corpseLoc.Y + Utility.RandomMinMax(-2, 2);
            int z = map.GetAverageZ(x, y);

            if (map.CanFit(x, y, z, 16, false, false))
            {
                spawnLoc = new Point3D(x, y, z);
                break;
            }
        }

        var pile = new TreasureHoardPile(bc.Fame, killer);
        if (isDailyDungeon)
        {
            pile.PullsRemaining += 1;
        }

        pile.MoveToWorld(spawnLoc, map);
        pile.PublicOverheadMessage(MessageType.Regular, 0x35, false, "*A treasure hoard spills onto the ground!*");
    }
}
