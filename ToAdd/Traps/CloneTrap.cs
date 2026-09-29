/**********************************
* Author: Expa                    *
* Optimized ModernUO .Net 10/C#14 *
**********************************/
using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using ModernUO.Serialization;
using Server.Items;
using Server.Mobiles;

namespace Server.Items
{
    [SerializationGenerator(0, false)]
    public partial class CloneTrap : Item
    {
        private const int GraphicId = 0xA723;
        private const int MoldHue = 2894;

        [Constructible]
        public CloneTrap() : base(GraphicId)
        {
            Name = "a Clone trap";
            Movable = false;
            Visible = false;
            Hue = MoldHue;
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (m?.CanBeDamaged() != true)
                return base.OnMoveOver(m);

            m.FixedParticles(0x36BD, 20, 10, 5044, EffectLayer.Head);
            m.PlaySound(0x307);
            _ = new MirrorClone(m) { Location = m.Location, Map = m.Map };
            Delete();

            return base.OnMoveOver(m);
        }

        private void Deserialize(IGenericReader reader, int version) { }
    }

    [SerializationGenerator(0, false)]
    public partial class MirrorClone : BaseCreature
    {
        private const int CloneHue = 2894;
        private const int TimerMin = 120;
        private const int TimerMax = 300;
        private const double DmgMult = 2.0;
        private const double ResMult = 0.5;

        [SerializableField(0)]
        private Mobile _victim;

        private MirrorCloneTimer _timer;

        public MirrorClone(Mobile victim) : base(AIType.AI_Melee, FightMode.Closest, 10, 1)
        {
            _victim = victim;
            Hue = CloneHue;
            Copy();
            (_timer = new MirrorCloneTimer(this)).Start();
        }

        public override bool AlwaysMurderer => true;

        private void Copy()
        {
            var v = _victim;
            if (v is null)
                return;

            BodyMod = v.Body;
            Name = v.Name;
            Title = v.Title;
            Fame = v.Fame;
            Female = v.Female;
            NameHue = v.NameHue;
            SpeechHue = v.SpeechHue;
            Criminal = v.Criminal;
            Karma = v.Karma;
            Kills = v.Kills;

            RawStr = Math.Clamp((int)(v.RawStr * DmgMult), 3, 225);
            RawDex = v.RawDex;
            RawInt = v.RawInt;

            StatCap = Math.Clamp((int)(v.StatCap * DmgMult), 60, 900);
            SkillsCap = v.SkillsCap;

            Hits = Math.Clamp((int)(v.Hits * DmgMult), 1, 32767);
            Mana = v.Mana;
            Stam = v.Stam;

            ApplyResistanceBalance();
            CopySkills();
            CopyEquipment();
            CopyMount();
        }

        private void ApplyResistanceBalance()
        {
            if (_victim is null)
                return;

            SetResistance(ResistanceType.Physical, Math.Clamp((int)(_victim.PhysicalResistance * ResMult), 0, 100));
            SetResistance(ResistanceType.Fire, Math.Clamp((int)(_victim.FireResistance * ResMult), 0, 100));
            SetResistance(ResistanceType.Cold, Math.Clamp((int)(_victim.ColdResistance * ResMult), 0, 100));
            SetResistance(ResistanceType.Poison, Math.Clamp((int)(_victim.PoisonResistance * ResMult), 0, 100));
            SetResistance(ResistanceType.Energy, Math.Clamp((int)(_victim.EnergyResistance * ResMult), 0, 100));
        }

        private void CopySkills()
        {
            if (_victim is null)
                return;

            var skills = _victim.Skills;
            var len = skills.Length;
            for (int i = 0; i < len; i++)
            {
                Skills[i].Base = skills[i].Base;
                Skills[i].Cap = skills[i].Cap;
            }
        }

        private void CopyEquipment()
        {
            var items = _victim?.Items;
            if (items is null || items.Count == 0)
                return;

            for (int i = 0, cnt = items.Count; i < cnt; i++)
            {
                var item = items[i];
                if (item?.Parent != _victim || item is IMountItem)
                    continue;

                var copy = ItemFactory.CreateInstance(item);
                if (copy is not null)
                {
                    CopyItemProperties(copy, item);
                    AddItem(copy);
                }
            }
        }

        private void CopyMount()
        {
            var mount = _victim?.FindItemOnLayer(Layer.Mount);
            if (mount is not IMountItem)
                return;

            var copy = ItemFactory.CreateInstance(mount);
            if (copy is null)
                return;

            CopyItemProperties(copy, mount);
            AddItem(copy);

            if (copy is IMountItem { Mount: BaseMount baseMount })
                baseMount.Rider = this;
        }

        private static void CopyItemProperties(Item dest, Item src)
        {
            dest.ItemID = src.ItemID;
            dest.Amount = src.Amount;
            dest.Weight = src.Weight;
            dest.Layer = src.Layer;
            dest.Hue = src.Hue;
            dest.Name = src.Name;
            dest.Movable = src.Movable;
            dest.LootType = src.LootType;
        }

        public override void OnDeath(Container c)
        {
            Cleanup();
            base.OnDeath(c);
        }

        public override void OnDelete()
        {
            Cleanup();
            base.OnDelete();
        }

        private void Cleanup()
        {
            _timer?.Stop();
            DismountAndClear();
        }

        private void DismountAndClear()
        {
            if (Mount is BaseMount mount)
            {
                mount.Rider = null;
                mount.Delete();
            }

            var items = Items;
            for (int i = items.Count - 1; i >= 0; i--)
                items[i].Delete();

            Corpse?.Delete();
        }

        private void Deserialize(IGenericReader reader, int version)
        {
            _victim = reader.ReadEntity<Mobile>();
            if (_victim is not null)
                Delete();
        }

        private sealed class MirrorCloneTimer : Timer
        {
            private WeakReference<MirrorClone> _ref;

            public MirrorCloneTimer(MirrorClone clone)
                : base(TimeSpan.FromSeconds(Utility.Random(TimerMin, TimerMax)))
            {
                _ref = new WeakReference<MirrorClone>(clone);
            }

            protected override void OnTick()
            {
                if (!_ref.TryGetTarget(out var clone) || clone.Deleted)
                {
                    Stop();
                    return;
                }

                clone.Warmode = false;
                Effects.SendLocationParticles(
                    EffectItem.Create(clone.Location, clone.Map, EffectItem.DefaultDuration),
                    0x3728, 10, 10, 2023
                );
                clone.Delete();
                Stop();
            }
        }

        private static class ItemFactory
        {
            private static readonly ConcurrentDictionary<Type, Func<Item>> _ctors = new();

            public static Item CreateInstance(Item src)
            {
                if (src is null)
                    return null;

                var type = src.GetType();
                var factory = _ctors.GetOrAdd(type, CreateFactory);

                if (factory is null)
                {
                    try
                    {
                        return new Item(src.ItemID);
                    }
                    catch
                    {
                        return null;
                    }
                }

                try
                {
                    var item = factory();
                    return item;
                }
                catch
                {
                    try
                    {
                        return new Item(src.ItemID);
                    }
                    catch
                    {
                        return null;
                    }
                }
            }

            private static Func<Item> CreateFactory(Type t)
            {
                try
                {
                    var ctor = t.GetConstructor(Type.EmptyTypes);
                    if (ctor == null)
                        return null;

                    var newExp = Expression.New(ctor);
                    var conv = Expression.Convert(newExp, typeof(Item));
                    var lambda = Expression.Lambda<Func<Item>>(conv);
                    return lambda.Compile();
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}