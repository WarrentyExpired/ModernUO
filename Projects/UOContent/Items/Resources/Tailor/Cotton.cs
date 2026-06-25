using ModernUO.Serialization;
using Server.Targeting;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class Cotton : Item, IDyable
{
    [Constructible]
    public Cotton(int amount = 1) : base(0xDF9)
    {
        Stackable = true;
        Amount = amount;
    }

    public override double DefaultWeight => 4.0;

    public bool Dye(Mobile from, DyeTub sender)
    {
        if (Deleted)
        {
            return false;
        }

        Hue = sender.DyedHue;

        return true;
    }

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

    // Left intact for structural consistency, but bypassed by the new bulk lambda target handler below
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
        private readonly Cotton m_Cotton;

        public PickWheelTarget(Cotton cotton) : base(3, false, TargetFlags.None) => m_Cotton = cotton;

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (m_Cotton.Deleted)
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

            if (!m_Cotton.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
            else if (wheel.Spinning)
            {
                from.SendLocalizedMessage(502656); // That spinning wheel is being used.
            }
            else
            {
                // 1. Capture the stack size and any custom dyed colors before destroying the item
                int amountToProcess = m_Cotton.Amount;
                int cottonHue = m_Cotton.Hue;

                // 2. Delete the whole stack immediately to secure the resources up front
                m_Cotton.Delete();

                // 3. Process the bulk thread conversion inside an anonymous callback closure
                wheel.BeginSpin((w, mobile, h) =>
                {
                    mobile.AddToBackpack(new SpoolOfThread(6 * amountToProcess)
                    {
                        Hue = h
                    });
                    mobile.SendLocalizedMessage(1010577); // You put the spools of thread in your backpack.
                }, from, cottonHue);
            }
        }
    }
}
