using System;
using Server;
using Server.Mobiles;
using ModernUO.Serialization;

namespace Server.Items
{
    public enum TradesmanNodeType
    {
        Smith,
        Smelter
    }

    [SerializationGenerator(0, false)]
    public partial class TradesmanNode : Item
    {
        // The generator automatically creates 'public TradesmanNodeType NodeType { get; set; }'
        // and triggers OnNodeTypeChanged whenever a GM changes it via the [Props gump.
        [SerializableField(0, fieldChanged: nameof(OnNodeTypeChanged))]
        [SerializedCommandProperty(AccessLevel.GameMaster)]
        private TradesmanNodeType _nodeType;

        [SerializableField(1)]
        private Mobile _worker;

        [SerializableField(2)]
        private Item _workstation;

        [SerializableField(3)]
        private DateTime _nextRespawn;

        public override bool HandlesOnMovement => true;

        private void OnNodeTypeChanged(TradesmanNodeType oldValue, TradesmanNodeType newValue)
        {
            ClearNode(); // Instantly reset the scene if a GM changes the type
        }

        [Constructible]
        public TradesmanNode() : this(TradesmanNodeType.Smith)
        {
        }

        [Constructible]
        public TradesmanNode(TradesmanNodeType type) : base(0x1BCE)
        {
            Name = "tradesman node";
            Movable = false;
            Visible = false; // Completely invisible to players
            _nodeType = type;
            _nextRespawn = DateTime.UtcNow;
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            // Ignore deleted items, NPCs, hidden staff, or players on other maps
            if (Deleted || m == null || !m.Player || m.Hidden || m.Map != Map)
                return;

            // Trigger the spawn if a player walks within 15 tiles
            if (Utility.InRange(m.Location, Location, 15))
            {
                CheckAndSpawn();
            }
        }

        private void CheckAndSpawn()
        {
            if (Deleted || Map == null || Map == Map.Internal)
                return;

            bool workerMissing = (_worker == null || _worker.Deleted || !_worker.Alive);
            bool stationMissing = (_workstation == null || _workstation.Deleted);

            // Only spawn if the scene is missing pieces AND the cooldown is finished
            if ((workerMissing || stationMissing) && DateTime.UtcNow >= _nextRespawn)
            {
                SpawnNode();

                // 10-minute cooldown so players can't spam-farm the NPC if they kill them
                _nextRespawn = DateTime.UtcNow + TimeSpan.FromMinutes(10);
            }
        }

        public void SpawnNode()
        {
            ClearNode(); // Clean up any lingering pieces before spawning fresh

            switch (_nodeType)
            {
                case TradesmanNodeType.Smith:
                    _workstation = new AnvilHit();
                    _worker = new TradesmanSmith();
                    break;
                case TradesmanNodeType.Smelter:
                    _workstation = new ForgeHit();
                    _worker = new TradesmanSmelter();
                    break;
            }

            if (_workstation != null)
            {
                // Place workstation exactly on the node
                _workstation.MoveToWorld(Location, Map);
            }

            if (_worker != null)
            {
                if (Utility.RandomBool())
                {
                    // Place West, face East
                    _worker.MoveToWorld(new Point3D(X - 1, Y, Z), Map);
                    _worker.Direction = Direction.East;
                }
                else
                {
                    // Place North, face South
                    _worker.MoveToWorld(new Point3D(X, Y - 1, Z), Map);
                    _worker.Direction = Direction.South;
                }
            }
        }

        public void ClearNode()
        {
            if (_worker != null && !_worker.Deleted)
            {
                _worker.Delete();
            }
            _worker = null;

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

        // Automatic Garbage Collection: Deleting the node deletes the NPC and workstation
        public override void OnDelete()
        {
            ClearNode();
            base.OnDelete();
        }

        public TradesmanNode(Serial serial) : base(serial)
        {
        }
    }
}
