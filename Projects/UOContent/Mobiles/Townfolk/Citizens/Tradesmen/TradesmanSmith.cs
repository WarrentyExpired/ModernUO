using System;
using Server;
using Server.Items;
using ModernUO.Serialization;

namespace Server.Mobiles
{
    [SerializationGenerator(0, false)]
    public partial class TradesmanSmith : TradesmanCitizen
    {
        private DateTime _NextWork;
        private AnvilHit _MyAnvil;

        [Constructible]
        public TradesmanSmith()
        {
            Title = "the blacksmith";
            CantWalk = true;

            _NextWork = DateTime.UtcNow + TimeSpan.FromSeconds(2);
        }

        public override void DressTradesman()
        {
            AddItem(new LongPants(Utility.RandomNeutralHue()));
            AddItem(new Boots());

            AddItem(new HalfApron(Utility.RandomList(0x906, 0x8A5, 0x89A)));

            Item hammer = new SmithHammer();
            hammer.Movable = false;
            AddItem(hammer);
        }

        public override void SayRandomChatter()
        {
            string[] lines = {
                "This iron is stubborn today.",
                "A good blade takes time and patience.",
                "Hot work, but someone has to do it.",
                "Need your armor patched up?"
            };
            Say(lines[Utility.Random(lines.Length)]);
        }

        public override void OnThink()
        {
            base.OnThink();

            if (DateTime.UtcNow >= _NextWork)
            {
                if (_MyAnvil == null || _MyAnvil.Deleted)
                {
                    foreach (Item item in GetItemsInRange(1))
                    {
                        if (item is AnvilHit anvil)
                        {
                            _MyAnvil = anvil;
                            break;
                        }
                    }
                }

                if (_MyAnvil != null && !_MyAnvil.Deleted)
                {
                    _MyAnvil.Strike(this);
                }

                _NextWork = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(2, 5));
            }
        }

        public TradesmanSmith(Serial serial) : base(serial)
        {
        }
    }
}

namespace Server.Items
{
    [SerializationGenerator(0, false)]
    public partial class AnvilHit : Item
    {
        [SerializableField(0)]
        private Item _ForgedItem;

        private static readonly int[] _ForgedGraphics =
        {
            0x1BEF, // Iron Ingot
            0x1B73, // Buckler
            0x1BEF, // Iron Ingot
            0x13B9, // Viking Sword
            0x1BEF,
            0x1401, // Kryss
            0x1BEF,
            0x1B76 // Heater Shield
        };

        [Constructible]
        public AnvilHit() : base(0xFAF)
        {
            Name = "anvil";
            Movable = false;
        }

        public void Strike(Mobile from)
        {
            if (from == null || from.Deleted) return;

            from.Direction = from.GetDirectionTo(Location);

            from.Animate(11, 5, 1, true, false, 0);
            from.PlaySound(0x2A);

            if (_ForgedItem == null || _ForgedItem.Deleted)
            {
                _ForgedItem = new Item(_ForgedGraphics[Utility.Random(_ForgedGraphics.Length)]);
                _ForgedItem.Movable = false;
                _ForgedItem.Name = "a forged item";

                _ForgedItem.MoveToWorld(new Point3D(X, Y, Z + 5), Map);
            }
            else
            {
                _ForgedItem.ItemID = _ForgedGraphics[Utility.Random(_ForgedGraphics.Length)];
            }
        }

        public override void OnDelete()
        {
            if (_ForgedItem != null && !_ForgedItem.Deleted)
            {
                _ForgedItem.Delete();
            }
            base.OnDelete();
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            base.OnLocationChange(oldLocation);
            if (_ForgedItem != null && !_ForgedItem.Deleted)
            {
                _ForgedItem.Location = new Point3D(X, Y, Z + 5);
            }
        }

        public override void OnMapChange()
        {
            base.OnMapChange();
            if (_ForgedItem != null && !_ForgedItem.Deleted)
            {
                _ForgedItem.Map = Map;
            }
        }

        public AnvilHit(Serial serial) : base(serial)
        {
        }
    }
}
