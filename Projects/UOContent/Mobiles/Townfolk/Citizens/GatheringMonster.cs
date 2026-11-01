using System;
using Server;

namespace Server.Mobiles
{
    public class GatheringMonster : FriendlyTownMonster
    {
        private Direction _GatherDirection;

        [CommandProperty(AccessLevel.GameMaster)]
        public Direction GatherDirection
        {
            get => _GatherDirection;
            set => _GatherDirection = value;
        }

        [Constructible]
        public GatheringMonster() : base()
        {
            CantWalk = true;
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Direction != _GatherDirection)
            {
                Direction = _GatherDirection;
            }
        }

        public GatheringMonster(Serial serial) : base(serial)
        {
        }

        public override void Serialize(IGenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write((int)_GatherDirection);
        }

        public override void Deserialize(IGenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            _GatherDirection = (Direction)reader.ReadInt();
        }
    }
}
