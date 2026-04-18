using System;
using Server;
using Server.Mobiles;
using Server.Items;
using ModernUO.Serialization;

namespace Server.Items
{
    [SerializationGenerator(0)]
    public partial class ThruDoor : BaseDoor
    {
        [SerializableField(0)]
        [SerializedCommandProperty(AccessLevel.GameMaster)]
        private Point3D _pointDest;

        [SerializableField(1)]
        [SerializedCommandProperty(AccessLevel.GameMaster)]
        private Map _mapDest;

        [SerializableField(2)]
        [SerializedCommandProperty(AccessLevel.GameMaster)]
        private int _rules;

        [Constructible]
        public ThruDoor() : this(DoorFacing.WestCW) { }

        [Constructible]
        public ThruDoor(int itemID) : base(itemID, itemID, 0xEA, 0xEA, Point3D.Zero) { }

        [Constructible]
        public ThruDoor(DoorFacing facing) : base(0x6A5, 0x6A5, 0xEA, 0xEA, BaseDoor.GetOffset(facing)) { }

        public override void Use(Mobile m)
        {
            if (m.InRange(this.Location, 2))
                DoTeleport(m);
            else
                m.SendLocalizedMessage(500295);
        }

        public virtual void DoTeleport(Mobile m)
        {
            if (m is not PlayerMobile pm) return;

            // Rules 0: Return Path Logic
            if (_rules == 0 && !string.IsNullOrEmpty(pm.CharacterPublicDoor))
            {
                string[] split = pm.CharacterPublicDoor.Split('#');
                if (split.Length >= 4)
                {
                    try
                    {
                        int x = Convert.ToInt32(split[0]);
                        int y = Convert.ToInt32(split[1]);
                        int z = Convert.ToInt32(split[2]);
                        Map map = Map.Parse(split[3]);
                        if (map != null && map != Map.Internal)
                        {
                            pm.MoveToWorld(new Point3D(x, y, z), map);
                            pm.CharacterPublicDoor = "";
                            pm.MarkDirty();
                            Effects.PlaySound(pm.Location, pm.Map, 0xEA);
                            return;
                        }
                    } catch { }
                }
            }

            if (MapDest == null || MapDest == Map.Internal) return;

            if (_rules == 0)
            {
                pm.CharacterPublicDoor = $"{m.X}#{m.Y}#{m.Z}#{m.Map.Name}#{this.Name}";
                pm.MarkDirty();
            }

            BaseCreature.TeleportPets(m, PointDest, MapDest);
            m.MoveToWorld(PointDest, MapDest);
            Effects.PlaySound(m.Location, m.Map, 0xEA);
        }
    }
}
