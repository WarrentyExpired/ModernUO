using System;
using Server;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Items
{
    public class TownGatheringSpot : Item
    {
        private List<Mobile> _Attendees = new List<Mobile>();

        [Constructible]
        public TownGatheringSpot() : base(0xFF8D)
        {
            Name = "Town Gathering Spot";
            Visible = true;
            Movable = false;

            Timer.DelayCall(TimeSpan.FromSeconds(1.0), CheckAndSpawn);
        }

        private void CheckAndSpawn()
        {
            if (Deleted || Map == null || Map == Map.Internal)
                return;

            _Attendees.RemoveAll(m => m == null || m.Deleted || !m.Alive);

            if (_Attendees.Count == 0)
            {
                SpawnGathering();
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster)
            {
                SpawnGathering();
                from.SendMessage("Gathering forcefully respawned and reset!");
            }
        }

        public void SpawnGathering()
        {
            foreach(Mobile m in _Attendees)
            {
                if (m != null && !m.Deleted)
                    m.Delete();
            }
            _Attendees.Clear();

            Mobile east = new GatheringCitizen();
            Mobile south = new GatheringCitizen();
            Mobile west = new GatheringMonster();
            Mobile north = new GatheringMonster();

            PlaceAndFace(east, Location.X + 3, Location.Y, Location.Z, Direction.West);
            PlaceAndFace(west, Location.X - 3, Location.Y, Location.Z, Direction.East);
            PlaceAndFace(south, Location.X, Location.Y + 3, Location.Z, Direction.North);
            PlaceAndFace(north, Location.X, Location.Y - 3, Location.Z, Direction.South);
        }

        private void PlaceAndFace(Mobile m, int x, int y, int z, Direction facing)
        {
            m.MoveToWorld(new Point3D(x, y, z), Map);
            m.CantWalk = true;

            if (m is GatheringCitizen gc)
            {
                gc.GatherDirection = facing;
            }
            else if (m is GatheringMonster gm)
            {
                gm.GatherDirection = facing;
            }

            m.Direction = facing;

            _Attendees.Add(m);
        }

        public override void OnDelete()
        {
            foreach (Mobile m in _Attendees)
            {
                if (m != null && !m.Deleted)
                {
                    m.Delete();
                }
            }

            _Attendees.Clear();
            base.OnDelete();
        }

        public TownGatheringSpot(Serial serial) : base(serial)
        {
        }

        public override void Serialize(IGenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(_Attendees.Count);
            foreach (Mobile m in _Attendees)
            {
                writer.Write(m);
            }
        }

        public override void Deserialize(IGenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                Mobile m = reader.ReadEntity<Mobile>();

                if (m != null && !m.Deleted)
                {
                    _Attendees.Add(m);
                }
            }

            Timer.DelayCall(TimeSpan.FromSeconds(1.0), CheckAndSpawn);
        }
    }
}
