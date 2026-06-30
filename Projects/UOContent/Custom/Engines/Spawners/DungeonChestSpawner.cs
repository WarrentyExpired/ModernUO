using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class DungeonChestSpawner : Item
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public int ChestLevel { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan RespawnTime { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DungeonChest ActiveChest { get; set; }

        private Timer _timer;

        [Constructible]
        public DungeonChestSpawner(int level, double minutes) : base(0x1EA7)
        {
            ChestLevel = level;
            RespawnTime = TimeSpan.FromMinutes(minutes);

            Name = "Chest Spawner";
            Visible = false;
            Movable = false;

            SpawnChest();
        }

        public DungeonChestSpawner(Serial serial) : base(serial) { }

        public override void Serialize(IGenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
            writer.Write(ChestLevel);
            writer.Write(RespawnTime);

            writer.Write((Item)ActiveChest);
        }

        public override void Deserialize(IGenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            ChestLevel = reader.ReadInt();
            RespawnTime = reader.ReadTimeSpan();

            ActiveChest = reader.ReadEntity<Item>() as DungeonChest;

            Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
            {
                if (ActiveChest == null || ActiveChest.Deleted)
                {
                    SpawnChest();
                }
                else
                {
                    StartTimer();
                }
            });
        }

        public void SpawnChest()
        {
            if (Deleted) return;

            if (ActiveChest != null && !ActiveChest.Deleted)
            {
                ActiveChest.Delete();
            }

            ActiveChest = new DungeonChest(ChestLevel);
            ActiveChest.MoveToWorld(Location, Map);

            StartTimer();
        }

        private void StartTimer()
        {
            _timer?.Stop();
            _timer = Timer.DelayCall(RespawnTime, SpawnChest);
        }

        public override void OnDelete()
        {
            if (ActiveChest != null && !ActiveChest.Deleted)
            {
                ActiveChest.Delete();
            }

            _timer?.Stop();

            base.OnDelete();
        }
    }
}
