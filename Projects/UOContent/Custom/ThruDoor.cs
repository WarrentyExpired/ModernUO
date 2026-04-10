using System;
using Server;
using Server.Mobiles;
using Server.Items;
using ModernUO.Serialization;

namespace Server.Items
{
    [SerializationGenerator(1, false)] // Version 1, ReadInt() format
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

        [Constructible] // Correct ModernUO spelling
        public ThruDoor() : this(DoorFacing.WestCW)
        {
        }

        [Constructible]
        public ThruDoor(DoorFacing facing) : base(0x6A5, 0x6A5, 0xEA, 0xEA, BaseDoor.GetOffset(facing))
        {
            Name = "a door";
        }

        // Fixes error CS1501: This method is required by the generator for legacy support
        private void Deserialize(IGenericReader reader, int version)
        {
            // If you have no old RunUO saves to migrate, leave this empty.
            // The generator handles version 1+ automatically via attributes.
        }

        public override void Use(Mobile m)
        {
            if (m.InRange(this.Location, 2))
                DoTeleport(m);
            else
                m.SendLocalizedMessage(500295);
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (m.Player)
                DoTeleport(m);

            return true;
        }
        public void DoTeleport(Mobile m)
        {
            // 1. Check if this is a "Return" door (PointDest is 0,0,0)
            if (PointDest == Point3D.Zero && m is PlayerMobile pm && !string.IsNullOrEmpty(pm.CharacterPublicDoor))
            {
                try
                {
                    // Split the saved string: "X#Y#Z#Map#Zone"
                    string[] split = pm.CharacterPublicDoor.Split('#');
                    if (split.Length >= 4)
                    {
                        int x = Convert.ToInt32(split[0]);
                        int y = Convert.ToInt32(split[1]);
                        int z = Convert.ToInt32(split[2]);
                        Map map = Map.Parse(split[3]);
                        // Move to the saved street location
                        BaseCreature.TeleportPets(pm, new Point3D(x, y, z), map);
                        pm.MoveToWorld(new Point3D(x, y, z), map);
                        // Clear the string so it doesn't loop
                        pm.CharacterPublicDoor = "";
                        pm.MarkDirty();
                        Effects.PlaySound(pm.Location, pm.Map, 0x1FE);
                        return; // Exit early since we found the return path
                    }
                }
                catch
                {
                    m.SendMessage("The magic of this door fails to find your way home.");
                }
            }
            // 2. Standard Teleport Logic (Entry Doors)
            if (MapDest == null || MapDest == Map.Internal)
                return;
            // Save location for Rule 0 before leaving
            if (Rules == 0 && m is PlayerMobile rpm)
            {
                rpm.CharacterPublicDoor = $"{m.X}#{m.Y}#{m.Z}#{m.Map.Name}#{this.Name ?? "the Building"}";
                rpm.MarkDirty();
            }
            // Standard Move
            BaseCreature.TeleportPets(m, PointDest, MapDest);
            m.MoveToWorld(PointDest, MapDest);
            Effects.PlaySound(m.Location, m.Map, 0x1FE);
        }
    }
}
