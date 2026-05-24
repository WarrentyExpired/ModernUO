using ModernUO.Serialization;
using Server.Targeting;

namespace Server.Items;

[SerializationGenerator(0, false)]
[Flippable(0x1A9C, 0x1A9D)]
public partial class Flax : Item
{
    [Constructible]
    public Flax(int amount = 1) : base(0x1A9C)
    {
        Stackable = true;
        Amount = amount;
    }

    public override double DefaultWeight => 1;

    public override int LabelNumber => 1026812; // Flax Bundle

    public override bool CanStackWith(Item dropped) =>
        dropped.Stackable && Stackable &&
        dropped.GetType() == GetType() &&
        dropped.ItemID is 0x1A9C or 0x1A9D &&
        dropped.Hue == Hue &&
        dropped.Name == Name &&
        dropped.Amount + Amount <= 60000 &&
        dropped != this;

    public override void OnDoubleClick(Mobile from)
    {
        if (IsChildOf(from.Backpack))
        {
            from.SendLocalizedMessage(502655); // What spinning wheel do you wish to spin this on?
            from.Target = new PickWheelTarget(this);
        }
        else
        {
            from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
        }
    }

    // Left intact for structural consistency, but bypassed by the new target handler below
    public virtual void OnSpun(ISpinningWheel wheel, Mobile from, int hue)
    {
        from.AddToBackpack(new SpoolOfThread(6)
        {
            Hue = hue
        });
        from.SendLocalizedMessage(1010577); // You put the spools of thread in your backpack.
    }

    private class PickWheelTarget : Target
    {
        private readonly Flax m_Flax;

        public PickWheelTarget(Flax flax) : base(3, false, TargetFlags.None) => m_Flax = flax;

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (m_Flax.Deleted)
            {
                return;
            }

            var wheel = targeted as ISpinningWheel;

            if (wheel == null && targeted is AddonComponent component)
            {
                wheel = component.Addon as ISpinningWheel;
            }

            if (wheel is not Item)
            {
                from.SendLocalizedMessage(502658); // Use that on a spinning wheel.
                return;
            }

            if (!m_Flax.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
            else if (wheel.Spinning)
            {
                from.SendLocalizedMessage(502656); // That spinning wheel is being used.
            }
            else
            {
                // 1. Capture the stack size and color parameters before destroying the item
                int amountToProcess = m_Flax.Amount;
                int flaxHue = m_Flax.Hue;

                // 2. Delete the whole stack immediately to prevent anti-duping/exploits during the spinning delay
                m_Flax.Delete();

                // 3. Use an inline lambda to dynamically multiply the spools of thread yield
                wheel.BeginSpin((w, mobile, h) =>
                {
                    mobile.AddToBackpack(new SpoolOfThread(6 * amountToProcess)
                    {
                        Hue = h
                    });
                    mobile.SendLocalizedMessage(1010577); // You put the spools of thread in your backpack.
                }, from, flaxHue);
            }
        }
    }
}
