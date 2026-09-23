using System;
using Server;
using Server.Regions;
using Server.Custom.DynamicSpawns;

namespace Server.Items
{
    public abstract class BaseProxySpawner : Item, ISpawner
    {
        private Mobile _trackedMobile;

        public bool UnlinkOnTaming => true;
        public Point3D HomeLocation => Location;
        public int HomeRange => 5;

        public Guid Guid { get; } = Guid.NewGuid();
        public int WalkingRange => HomeRange;
        public Region Region => Region.Find(Location, Map);
        public Rectangle3D SpawnBounds => new Rectangle3D(Location.X - HomeRange, Location.Y - HomeRange, -128, HomeRange * 2, HomeRange * 2, 256);
        public bool ReturnOnDeactivate => false;
        public bool Running { get; set; } = true;

        public Point3D GetSpawnPosition(ISpawnable spawnable, Map map)
        {
            return Location;
        }

        public void Respawn()
        {
            // Proxy spawners do not handle their own respawns.
            // When the monster dies, the proxy deletes itself,
            // and the parent Master Spawner drops a new proxy.
        }

        public bool IsInSpawnBounds(Point3D loc)
        {
            return Utility.InRange(Location, loc, WalkingRange);
        }

        public void Remove(ISpawnable spawn)
        {
            if (!Deleted)
                Delete();
        }

        public BaseProxySpawner() : base(0x6519)
        {
            Name = "spawn ref";
            Visible = false;
            Movable = false;

            Timer.DelayCall(TimeSpan.FromSeconds(1.0), ExecuteSpawn);
        }

        public BaseProxySpawner(Serial serial) : base(serial)
        {
        }

        public override void Serialize(IGenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
            writer.Write(_trackedMobile);
        }

        public override void Deserialize(IGenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            _trackedMobile = reader.ReadEntity<Mobile>();

            if (_trackedMobile != null && !_trackedMobile.Deleted && _trackedMobile.Alive)
            {
                if (_trackedMobile is ISpawnable spawnable)
                {
                    spawnable.Spawner = this;
                }
            }
            else
            {
                Timer.DelayCall(TimeSpan.FromSeconds(1.0), Delete);
            }
        }

        private void ExecuteSpawn()
        {
            if (Deleted || Map == null || Map == Map.Internal)
                return;

            Point3D savedLocation = Location;
            Map savedMap = Map;

            Region reg = Region.Find(savedLocation, savedMap);
            if (reg.IsPartOf<TownRegion>() || reg.IsPartOf<GuardedRegion>() ||
                reg.IsPartOf<DungeonRegion>() || reg.IsPartOf<HouseRegion>() ||
                reg.IsPartOf<GreetingTownRegion>() || reg.IsPartOf<GreetingBusinessRegion>() ||
                reg.IsPartOf<GreetingDungeonRegion>()
            )
            {
                Delete();
                return;
            }

            _trackedMobile = SpawnList.RandomSpawn(this, savedLocation, savedMap);

            if (_trackedMobile != null)
            {
                if (_trackedMobile is ISpawnable spawnable)
                {
                    spawnable.Spawner = this;
                }

                int scatterRange = 25;

                foreach (Item item in savedMap.GetItemsInRange(savedLocation, 0))
                {
                    if (item is ISpawner spawner && item != this)
                    {
                        scatterRange = spawner.WalkingRange;
                        break;
                    }
                }

                Point3D spawnPoint = savedLocation;

                for (int i = 0; i < 15; i++)
                {
                    int x = savedLocation.X + Utility.RandomMinMax(-scatterRange, scatterRange);
                    int y = savedLocation.Y + Utility.RandomMinMax(-scatterRange, scatterRange);
                    int z = savedMap.GetAverageZ(x, y);
                    Point3D testPoint = new(x, y, z);

                    if (this is SpawnWater)
                    {
                        if (SpawnList.GetTerrain(savedMap, testPoint, x, y) == Terrain.Water)
                        {
                            spawnPoint = testPoint;
                            break;
                        }
                    }
                    else
                    {
                        if (savedMap.CanFit(x, y, z, 16, false, false))
                        {
                            spawnPoint = testPoint;
                            break;
                        }
                    }
                }

                _trackedMobile.MoveToWorld(spawnPoint, savedMap);
                Internalize();
            }
            else
            {
                Delete();
            }
        }
    }

    public class SpawnWater : BaseProxySpawner
    {
        [Constructible] public SpawnWater() {}
        public SpawnWater(Serial serial) : base(serial) {}
        public override void Serialize(IGenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(IGenericReader reader) { base.Deserialize(reader); int version = reader.ReadInt(); }
    }

    public class SpawnCave : BaseProxySpawner
    {
        [Constructible] public SpawnCave() {}
        public SpawnCave(Serial serial) : base(serial) {}
        public override void Serialize(IGenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(IGenericReader reader) { base.Deserialize(reader); int version = reader.ReadInt(); }
    }

    public class SpawnDirt : BaseProxySpawner
    {
        [Constructible] public SpawnDirt() {}
        public SpawnDirt(Serial serial) : base(serial) {}
        public override void Serialize(IGenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(IGenericReader reader) { base.Deserialize(reader); int version = reader.ReadInt(); }
    }

    public class SpawnForest : BaseProxySpawner
    {
        [Constructible] public SpawnForest() {}
        public SpawnForest(Serial serial) : base(serial) {}
        public override void Serialize(IGenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(IGenericReader reader) { base.Deserialize(reader); int version = reader.ReadInt(); }
    }

    public class SpawnGrass : BaseProxySpawner
    {
        [Constructible] public SpawnGrass() {}
        public SpawnGrass(Serial serial) : base(serial) {}
        public override void Serialize(IGenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(IGenericReader reader) { base.Deserialize(reader); int version = reader.ReadInt(); }
    }

    public class SpawnJungle : BaseProxySpawner
    {
        [Constructible] public SpawnJungle() {}
        public SpawnJungle(Serial serial) : base(serial) {}
        public override void Serialize(IGenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(IGenericReader reader) { base.Deserialize(reader); int version = reader.ReadInt(); }
    }

    public class SpawnRock : BaseProxySpawner
    {
        [Constructible] public SpawnRock() {}
        public SpawnRock(Serial serial) : base(serial) {}
        public override void Serialize(IGenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(IGenericReader reader) { base.Deserialize(reader); int version = reader.ReadInt(); }
    }

    public class SpawnSand : BaseProxySpawner
    {
        [Constructible] public SpawnSand() {}
        public SpawnSand(Serial serial) : base(serial) {}
        public override void Serialize(IGenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(IGenericReader reader) { base.Deserialize(reader); int version = reader.ReadInt(); }
    }

    public class SpawnSwamp : BaseProxySpawner
    {
        [Constructible] public SpawnSwamp() {}
        public SpawnSwamp(Serial serial) : base(serial) {}
        public override void Serialize(IGenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(IGenericReader reader) { base.Deserialize(reader); int version = reader.ReadInt(); }
    }

    public class SpawnSnow : BaseProxySpawner
    {
        [Constructible] public SpawnSnow() {}
        public SpawnSnow(Serial serial) : base(serial) {}
        public override void Serialize(IGenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(IGenericReader reader) { base.Deserialize(reader); int version = reader.ReadInt(); }
    }

    public class SpawnCritters : BaseProxySpawner
    {
        [Constructible] public SpawnCritters() {}
        public SpawnCritters(Serial serial) : base(serial) {}
        public override void Serialize(IGenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(IGenericReader reader) { base.Deserialize(reader); int version = reader.ReadInt(); }
    }
}
