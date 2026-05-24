using ModernUO.Serialization;
using Server.Targeting;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class Wool : Item, IDyable
{
    [Constructible]
    public Wool(int amount = 1) : base(0xDF8)
    {
        Stackable = true;
        Amount = amount;
    }

    public override double DefaultWeight => 4.0;

    // Extensible property to handle distinct yields for subclasses safely
    public virtual int YarnMultiplier => 3;

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

    // Left intact for backend stability, but bypassed by the new bulk lambda target handler below
    public virtual void OnSpun(ISpinningWheel wheel, Mobile from, int hue)
    {
        from.AddToBackpack(new DarkYarn(3)
        {
            Hue = hue
        });
        from.SendLocalizedMessage(1010576); // You put the balls of yarn in your backpack.
    }

    private class PickWheelTarget : Target
    {
        private readonly Wool m_Wool;

        public PickWheelTarget(Wool wool) : base(3, false, TargetFlags.None) => m_Wool = wool;

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (m_Wool.Deleted)
            {
                return;
            }

            var wheel = targeted as ISpinningWheel;

            if (wheel == null && targeted is AddonComponent component)
            {
                wheel = component.Addon as ISpinningWheel;
            }

            if (wheel is Item)
            {
                if (!m_Wool.IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                }
                else if (wheel.Spinning)
                {
                    from.SendLocalizedMessage(502656); // That spinning wheel is being used.
                }
                else
                {
                    // 1. Capture stack parameters and the polymorphic multiplier before destroying the item
                    int amountToProcess = m_Wool.Amount;
                    int woolHue = m_Wool.Hue;
                    int multiplier = m_Wool.YarnMultiplier;

                    // 2. Delete the stack immediately to secure resources up front
                    m_Wool.Delete();

                    // 3. Process the bulk conversion inside the wheel's spin callback closure
                    wheel.BeginSpin((w, mobile, h) =>
                    {
                        int totalYarnEarned = multiplier * amountToProcess;

                        mobile.AddToBackpack(new DarkYarn(totalYarnEarned)
                        {
                            Hue = h
                        });

                        // Dynamically adjusts localization messaging based on singular or plural results
                        mobile.SendLocalizedMessage(totalYarnEarned > 1 ? 1010576 : 1010574);
                    }, from, woolHue);
                }
            }
            else
            {
                from.SendLocalizedMessage(502658); // Use that on a spinning wheel.
            }
        }
    }
}

[SerializationGenerator(0, false)]
public partial class TaintedWool : Wool
{
    [Constructible]
    public TaintedWool(int amount = 1) : base(0x101F)
    {
        Stackable = true;
        Amount = amount;
    }

    public override double DefaultWeight => 4.0;

    // Overrides the base multiplier to safely reduce bulk yields to a 1:1 ratio
    public override int YarnMultiplier => 1;

    // Left intact for structural consistency
    public override void OnSpun(ISpinningWheel wheel, Mobile from, int hue)
    {
        from.AddToBackpack(new DarkYarn
        {
            Hue = hue
        });
        from.SendLocalizedMessage(1010574); // You put a ball of yarn in your backpack.
    }
}
