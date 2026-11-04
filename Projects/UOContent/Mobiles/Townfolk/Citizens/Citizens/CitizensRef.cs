using System;
using Server;
using Server.Regions;
using Server.Custom.CitizenSpawns;

namespace Server.Items
{
    public abstract class CitizenSpawner : Item, ISpawner
    {
        private Mobile _trackedMobile;

        public Point3D HomeLocation => Location;
        public int HomeRange => 5;

        public bool UnlinkOnTaming => true;
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

        public CitizenSpawner() : base(0x6519)
        {
            Name = "citizen ref";
            Visible = false;
            Movable = false;

            Timer.DelayCall(TimeSpan.FromSeconds(1.0), ExecuteSpawn);
        }

        public CitizenSpawner(Serial serial) : base(serial)
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

            if (reg.IsPartOf<DungeonRegion>() || reg.IsPartOf<HouseRegion>() ||
               reg.IsPartOf<GreetingBusinessRegion>() ||
               reg.IsPartOf<GreetingDungeonRegion>())
            {
                Delete();
                return;
            }

            _trackedMobile = CitizenTerrainList.RandomSpawn(this, savedLocation, savedMap);

            if (_trackedMobile != null)
            {
                if (_trackedMobile is ISpawnable spawnable)
                {
                    spawnable.Spawner = this;
                }

                int proxyScatterRange = 0;
                Point3D spawnPoint = savedLocation;

                for (int i = 0; i < 15; i++)
                {
                    int x = savedLocation.X + Utility.RandomMinMax(-proxyScatterRange, proxyScatterRange);
                    int y = savedLocation.Y + Utility.RandomMinMax(-proxyScatterRange, proxyScatterRange);
                    int z = savedMap.GetAverageZ(x, y);
                    Point3D testPoint = new Point3D(x, y, z);

                    if (savedMap.CanFit(x, y, z, 16, false, false))
                    {
                        spawnPoint = testPoint;
                        break;
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

    public class SpawnCitizen : CitizenSpawner
    {
        [Constructible]
        public SpawnCitizen() {}

        public SpawnCitizen(Serial serial) : base(serial) {}

        public override void Serialize(IGenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(IGenericReader reader) { base.Deserialize(reader); int version = reader.ReadInt(); }
    }
}
