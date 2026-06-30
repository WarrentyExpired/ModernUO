using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Custom.Spawners
{
    public class AdvancedSpawner : Item
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public int GlobalMaxCount { get; set; } = 5;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Range { get; set; } = 24;

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan RadarInterval { get; set; } = TimeSpan.FromSeconds(5);

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan MinSpawnDelay { get; set; } = TimeSpan.FromMinutes(1);

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan MaxSpawnDelay { get; set; } = TimeSpan.FromMinutes(3);

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan GracePeriod { get; set; } = TimeSpan.FromMinutes(5);

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsPlayerPresent { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Running { get; set; } = true;

        public List<AdvancedSpawnerEntry> Entries { get; set; } = new List<AdvancedSpawnerEntry>();

        private List<Mobile> _activeMobs = new List<Mobile>();
        private Timer _radarTimer;
        private Timer _spawnTimer;
        private DateTime _lastPlayerSeen;

        [Constructible]
        public AdvancedSpawner() : base(0x1EA7)
        {
            Name = "Advanced  Spawner";
            Visible = false;
            Movable = false;

            _lastPlayerSeen = DateTime.UtcNow;
            StartTimers();
        }

        public AdvancedSpawner(Serial serial) : base(serial) { }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster)
            {
                from.CloseGump<AdvancedSpawnerGump>();
                from.SendGump(new AdvancedSpawnerGump(this));
            }
        }

        private void StartTimers()
        {
            _radarTimer?.Stop();
            _radarTimer = Timer.DelayCall(TimeSpan.Zero, RadarInterval, RadarCheck);

            ConfigureNextSpawnTick();
        }

        private void ConfigureNextSpawnTick()
        {
            _spawnTimer?.Stop();

            if (!Running) return;

            long minMs = (long)MinSpawnDelay.TotalMilliseconds;
            long maxMs = (long)MaxSpawnDelay.TotalMilliseconds;
            TimeSpan randomDelay = TimeSpan.FromMilliseconds(Utility.RandomMinMax(minMs, maxMs));

            _spawnTimer = Timer.DelayCall(randomDelay, SpawnTick);
        }

        private void RadarCheck()
        {
            if (Deleted || Map == null || Map == Map.Internal || !Running) return;

            // Maintain integrity of tracked array
            _activeMobs.RemoveAll(m => m == null || m.Deleted || !m.Alive);

            bool wasPlayerPresent = IsPlayerPresent;
            bool playerFound = false;

            foreach (var m in Map.GetMobilesInRange(Location, Range))
            {
                if (m is PlayerMobile)
                {
                    playerFound = true;
                    break;
                }
            }

            if (playerFound)
            {
                IsPlayerPresent = true;
                _lastPlayerSeen = DateTime.UtcNow;

                 if (!wasPlayerPresent && _activeMobs.Count < GlobalMaxCount)
                {
                    ForceBurstSpawn();
                }
            }
            else
            {
                IsPlayerPresent = false;

                if (_activeMobs.Count > 0 && DateTime.UtcNow - _lastPlayerSeen >= GracePeriod)
                {
                    ExecuteDespawnCleanup();
                }
            }
        }

        public void ForceBurstSpawn()
        {
            // Don't burst if the spawner is off or floating in the void
            if (!Running || Map == null || Map == Map.Internal) return;

            _activeMobs.RemoveAll(m => m == null || m.Deleted || !m.Alive);

            if (Entries.Count > 0 && _activeMobs.Count < GlobalMaxCount)
            {
                int missingMobs = GlobalMaxCount - _activeMobs.Count;

                for (int i = 0; i < missingMobs; i++)
                {
                    int previousCount = _activeMobs.Count;

                    ExecuteSpawnCycle();

                    if (_activeMobs.Count == previousCount) break;
                }

                ConfigureNextSpawnTick();
            }
        }

        private void SpawnTick()
        {
            if (Deleted || Map == null || Map == Map.Internal || !Running || !IsPlayerPresent) return;

            _activeMobs.RemoveAll(m => m == null || m.Deleted || !m.Alive);

            if (_activeMobs.Count < GlobalMaxCount)
            {
                ExecuteSpawnCycle();
            }

            ConfigureNextSpawnTick();
        }

        private void ExecuteSpawnCycle()
        {
            if (Entries.Count == 0) return;

            int probabilitySum = 0;

            // Calculate total relative probability weight across eligible entries
            foreach (var entry in Entries)
            {
                if (entry.CurrentSpawnCount < entry.MaxCount)
                {
                    probabilitySum += entry.Probability;
                }
            }

            if (probabilitySum <= 0) return;

            int roll = Utility.RandomMinMax(1, probabilitySum);

            foreach (var entry in Entries)
            {
                if (entry.CurrentSpawnCount >= entry.MaxCount) continue;

                if (roll <= entry.Probability)
                {
                    SpawnEntry(entry);
                    return;
                }

                roll -= entry.Probability;
            }
        }

        private void SpawnEntry(AdvancedSpawnerEntry entry)
        {
            string typeName = entry.MobName;
            if (!typeName.Contains(".")) typeName = "Server.Mobiles." + typeName;

            Type type = Type.GetType(typeName, false, true);
            if (type == null)
            {
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    type = asm.GetType(typeName, false, true);
                    if (type != null) break;
                }
            }

            if (type != null)
            {
                Mobile mob = Activator.CreateInstance(type) as Mobile;
                if (mob != null)
                {
                    Point3D p = GetValidSpawnLocation();
                    mob.MoveToWorld(p, Map);

                    _activeMobs.Add(mob);
                    entry.TrackedMobs.Add(mob);
                }
            }
            else
            {
                Console.WriteLine($"AdvancedSpawner Error: Could not resolve type '{entry.MobName}'");
            }
        }

        private void ExecuteDespawnCleanup()
        {
            for (int i = _activeMobs.Count - 1; i >= 0; i--)
            {
                Mobile m = _activeMobs[i];
                if (m == null || m.Deleted) continue;

                if (m is BaseCreature bc && bc.Controlled)
                {
                    _activeMobs.RemoveAt(i);
                    RemoveFromInternalEntries(m);
                    continue;
                }

                if (m.Combatant != null) continue;

                m.Delete();
                _activeMobs.RemoveAt(i);
                RemoveFromInternalEntries(m);
            }
        }

        private void RemoveFromInternalEntries(Mobile m)
        {
            foreach (var entry in Entries)
            {
                if (entry.TrackedMobs.Contains(m))
                {
                    entry.TrackedMobs.Remove(m);
                    break;
                }
            }
        }

        private Point3D GetValidSpawnLocation()
        {
            for (int i = 0; i < 10; i++)
            {
                int x = Location.X + Utility.RandomMinMax(-Range, Range);
                int y = Location.Y + Utility.RandomMinMax(-Range, Range);
                int z = Map.GetAverageZ(x, y);

                if (Map.CanSpawnMobile(x, y, z)) return new Point3D(x, y, z);
            }
            return Location;
        }

        public override void OnDelete()
        {
            _radarTimer?.Stop();
            _spawnTimer?.Stop();

            for (int i = _activeMobs.Count - 1; i >= 0; i--)
            {
                _activeMobs[i]?.Delete();
            }

            base.OnDelete();
        }

        public override void Serialize(IGenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write(GlobalMaxCount);
            writer.Write(Range);
            writer.Write(RadarInterval);
            writer.Write(MinSpawnDelay);
            writer.Write(MaxSpawnDelay);
            writer.Write(GracePeriod);
            writer.Write(Running);

            writer.Write(Entries.Count);
            foreach (var entry in Entries)
            {
                writer.Write(entry.MobName);
                writer.Write(entry.MaxCount);
                writer.Write(entry.Probability);

                // Tracked entities per row
                entry.TrackedMobs.RemoveAll(m => m == null || m.Deleted || !m.Alive);
                writer.Write(entry.TrackedMobs.Count);
                foreach (var mob in entry.TrackedMobs)
                {
                    writer.Write(mob);
                }
            }
        }

        public override void Deserialize(IGenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            GlobalMaxCount = reader.ReadInt();
            Range = reader.ReadInt();
            RadarInterval = reader.ReadTimeSpan();
            MinSpawnDelay = reader.ReadTimeSpan();
            MaxSpawnDelay = reader.ReadTimeSpan();
            GracePeriod = reader.ReadTimeSpan();
            Running = reader.ReadBool();

            int entryCount = reader.ReadInt();
            for (int i = 0; i < entryCount; i++)
            {
                var entry = new AdvancedSpawnerEntry
                {
                    MobName = reader.ReadString(),
                    MaxCount = reader.ReadInt(),
                    Probability = reader.ReadInt()
                };

                int trackedCount = reader.ReadInt();
                for (int j = 0; j < trackedCount; j++)
                {
                    Mobile m = reader.ReadEntity<Mobile>();
                    if (m != null)
                    {
                        _activeMobs.Add(m);
                        entry.TrackedMobs.Add(m);
                    }
                }
                Entries.Add(entry);
            }

            _lastPlayerSeen = DateTime.UtcNow;
            StartTimers();
        }
    }

    public class AdvancedSpawnerEntry
    {
        public string MobName { get; set; } = "Orc";
        public int MaxCount { get; set; } = 1;
        public int Probability { get; set; } = 100;
        public List<Mobile> TrackedMobs { get; set; } = new List<Mobile>();
        public int CurrentSpawnCount
        {
            get
            {
                TrackedMobs.RemoveAll(m => m == null || m.Deleted || !m.Alive);
                return TrackedMobs.Count;
            }
        }
    }
}
