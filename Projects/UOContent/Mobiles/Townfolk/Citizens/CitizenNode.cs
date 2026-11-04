using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using ModernUO.Serialization;

namespace Server.Items
{
    public enum CitizenNodeType
    {
        Butcher,
        Smith,
        Smelter,
        MeetingSpot
    }

    [SerializationGenerator(0, false)]
    public partial class CitizenNode : Item
    {
        [SerializableField(0, fieldChanged: nameof(OnNodeTypeChanged))]
        [SerializedCommandProperty(AccessLevel.GameMaster)]
        private CitizenNodeType _nodeType;

        [SerializableField(1)]
        private List<Mobile> _workers = new();

        [SerializableField(2)]
        private Item _workstation;

        [SerializableField(3)]
        private DateTime _nextRespawn;

        public override bool HandlesOnMovement => true;

        private void OnNodeTypeChanged(CitizenNodeType oldValue, CitizenNodeType newValue)
        {
            ClearNode();
        }

        [Constructible]
        public CitizenNode() : this(CitizenNodeType.Smith)
        {
        }

        [Constructible]
        public CitizenNode(CitizenNodeType type) : base(0x1BCE)
        {
            Name = "citizen node";
            Movable = false;
            Visible = false;
            _nodeType = type;
            _nextRespawn = DateTime.UtcNow;
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (Deleted || m == null || !m.Player || m.Hidden || m.Map != Map)
                return;

            if (Utility.InRange(m.Location, Location, 15))
            {
                CheckAndSpawn();
            }
        }

        private void CheckAndSpawn()
        {
            if (Deleted || Map == null || Map == Map.Internal)
                return;

            _workers.RemoveAll(w => w == null || w.Deleted || !w.Alive);

            bool workersMissing = _workers.Count == 0;
            bool stationMissing = (_workstation == null || _workstation.Deleted);

            if ((workersMissing || stationMissing) && DateTime.UtcNow >= _nextRespawn)
            {
                SpawnNode();
                _nextRespawn = DateTime.UtcNow + TimeSpan.FromMinutes(10);
            }
        }

        public void SpawnNode()
        {
            ClearNode();

            switch (_nodeType)
            {
                case CitizenNodeType.Smith:
                    _workstation = new AnvilHit();
                    AddWorker(new TradesmanSmith(), GetSingleWorkerSpot());
                    break;

                case CitizenNodeType.Smelter:
                    _workstation = new ForgeHit();
                    AddWorker(new TradesmanSmelter(), GetSingleWorkerSpot());
                    break;

                case CitizenNodeType.MeetingSpot:
                    _workstation = new CampfireHit();
                    SpawnMeetingSpotCitizens();
                    break;
                case CitizenNodeType.Butcher:
                    var butcherMeat = new ButcherHit();
                    _workstation = butcherMeat;
                    AddWorker(new TradesmanButcher(), butcherMeat.GetWorkerPlacement(Location));
                    break;
            }

            if (_workstation != null)
            {
                _workstation.MoveToWorld(Location, Map);
            }
        }

        private (Point3D loc, Direction dir) GetSingleWorkerSpot()
        {
            if (Utility.RandomBool())
            {
                return (new Point3D(X - 1, Y, Z), Direction.East);
            }
            return (new Point3D(X, Y - 1, Z), Direction.South);
        }

        private void AddWorker(Mobile worker, (Point3D loc, Direction dir) spawnData)
        {
            if (worker == null) return;

            worker.MoveToWorld(spawnData.loc, Map);
            worker.Direction = spawnData.dir;
            _workers.Add(worker);
        }

        private void SpawnMeetingSpotCitizens()
        {
            (Point3D loc, Direction dir)[] potentialSpots = new[]
            {
                (new Point3D(X - 2, Y, Z), Direction.East),   // West spot -> Face East
                (new Point3D(X + 2, Y, Z), Direction.West),   // East spot -> Face West
                (new Point3D(X, Y - 2, Z), Direction.South),  // North spot -> Face South
                (new Point3D(X, Y + 2, Z), Direction.North)   // South spot -> Face North
            };

            Utility.Shuffle(potentialSpots);

            int countToSpawn = Utility.RandomMinMax(2, 4);

            for (int i = 0; i < countToSpawn && i < potentialSpots.Length; i++)
            {
                var spot = potentialSpots[i];
                Mobile attendee;

                if (Utility.RandomBool())
                {
                    var gc = new GatheringCitizen();
                    gc.GatherDirection = spot.dir;
                    attendee = gc;
                }
                else
                {
                    var gm = new GatheringMonster();
                    gm.GatherDirection = spot.dir;
                    attendee = gm;
                }

                AddWorker(attendee, spot);
            }
        }

        public void ClearNode()
        {
            foreach (var worker in _workers)
            {
                if (worker != null && !worker.Deleted)
                {
                    worker.Delete();
                }
            }
            _workers.Clear();

            if (_workstation != null && !_workstation.Deleted)
            {
                _workstation.Delete();
            }
            _workstation = null;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster)
            {
                SpawnNode();
                from.SendMessage("Citizen node forcefully respawned.");
            }
        }

        public override void OnDelete()
        {
            ClearNode();
            base.OnDelete();
        }

        public CitizenNode(Serial serial) : base(serial)
        {
        }
    }
}
