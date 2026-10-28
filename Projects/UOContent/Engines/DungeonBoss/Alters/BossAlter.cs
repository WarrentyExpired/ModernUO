using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using ModernUO.Serialization;

namespace Server.Custom.Dungeons
{
    [SerializationGenerator(0)]
    public abstract partial class BossAlter : Item
    {
        private Mobile _ActiveBoss;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile ActiveBoss { get => _ActiveBoss; set => _ActiveBoss = value; }

        public abstract Type SummonItem { get; }
        public abstract Type BossType { get; }

        public BossAlter(int itemID) : base(itemID)
        {
            Movable = false;
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (dropped.GetType() != SummonItem)
            {
                from.SendMessage(33, "The altar rejects your offering. It requires a different catalyst.");
                return false;
            }

            if (_ActiveBoss != null && _ActiveBoss.Alive && !_ActiveBoss.Deleted)
            {
                from.SendMessage(33, "The guardian of this domain is already present!");
                return false;
            }

            dropped.Delete();

            Effects.PlaySound(Location, Map, 0x20F);
            Effects.SendLocationEffect(Location, Map, 0x3728, 10, 10);
            Effects.SendLocationEffect(Location, Map, 0x3709, 10, 30, 1153, 0);

            try
            {
                Mobile boss = (Mobile)Activator.CreateInstance(BossType);
                boss.MoveToWorld(new Point3D(X + 1, Y + 1, Z), Map);
                boss.PlaySound(boss.GetAngerSound());

                _ActiveBoss = boss;
                from.SendMessage(89, "The altar consumes your offering, and a terrible presence enters the room!");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error spawning dungeon boss: {e.Message}");
                from.SendMessage(33, "The altar flickers, but nothing happens.");
            }

            return true;
        }

        public BossAlter(Serial serial) : base(serial)
        {
        }
    }
}
