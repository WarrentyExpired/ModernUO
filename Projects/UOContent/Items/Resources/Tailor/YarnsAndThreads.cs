using ModernUO.Serialization;
using Server.Targeting;

namespace Server.Items;

[SerializationGenerator(0, false)]
public abstract partial class BaseClothMaterial : Item, IDyable
{
    public BaseClothMaterial(int itemID, int amount = 1) : base(itemID)
    {
        Stackable = true;
        Amount = amount;
    }

    public override double DefaultWeight => 1.0;

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
            from.SendLocalizedMessage(500366); // Select a loom to use that on.
            from.Target = new PickLoomTarget(this);
        }
        else
        {
            from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
        }
    }

    private class PickLoomTarget : Target
    {
        private readonly BaseClothMaterial m_Material;

        public PickLoomTarget(BaseClothMaterial material) : base(3, false, TargetFlags.None) => m_Material = material;

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (m_Material.Deleted)
            {
                return;
            }

            var loom = targeted as ILoom;

            if (loom == null && targeted is AddonComponent component)
            {
                loom = component.Addon as ILoom;
            }

            if (loom != null)
            {
                if (!m_Material.IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                }
                else
                {
                    // 1. Capture stack values and current loom state before deletion
                    int totalThreads = m_Material.Amount;
                    int currentPhase = loom.Phase;
                    int materialHue = m_Material.Hue;

                    // 2. Combine the loom's current contents with the incoming stack
                    int totalEffectiveThreads = currentPhase + totalThreads;

                    // 3. Calculate bolts earned (5 threads per bolt) and the leftover phase remainder
                    int boltsToMake = totalEffectiveThreads / 5;
                    int newPhase = totalEffectiveThreads % 5;

                    // 4. Safely delete the resource stack up front to prevent any anti-duping movement exploits
                    m_Material.Delete();

                    // 5. Commit the new leftover phase back to the loom structure
                    loom.Phase = newPhase;

                    // 6. Award the bulk bolts of cloth if thresholds were met
                    if (boltsToMake > 0)
                    {
                        var create = new BoltOfCloth
                        {
                            Hue = materialHue,
                            Amount = boltsToMake
                        };

                        from.SendLocalizedMessage(500368); // You create some cloth and put it in your backpack.
                        from.AddToBackpack(create);
                    }

                    // 7. If no bolts were finished but progress was added, send the proper progression message
                    if (boltsToMake == 0 && newPhase > 0)
                    {
                        if (targeted is Item item)
                        {
                            // Matches the original localization offsets (Phase 1 = 1010001, Phase 4 = 1010004)
                            item.SendLocalizedMessageTo(from, 1010000 + newPhase);
                        }
                    }
                }
            }
            else
            {
                from.SendLocalizedMessage(500367); // Try using that on a loom.
            }
        }
    }
}

[SerializationGenerator(0, false)]
public partial class DarkYarn : BaseClothMaterial
{
    [Constructible]
    public DarkYarn(int amount = 1) : base(0xE1D, amount)
    {
    }
}

[SerializationGenerator(0, false)]
public partial class LightYarn : BaseClothMaterial
{
    [Constructible]
    public LightYarn(int amount = 1) : base(0xE1E, amount)
    {
    }
}

[SerializationGenerator(0, false)]
public partial class LightYarnUnraveled : BaseClothMaterial
{
    [Constructible]
    public LightYarnUnraveled(int amount = 1) : base(0xE1F, amount)
    {
    }
}

[SerializationGenerator(0, false)]
public partial class SpoolOfThread : BaseClothMaterial
{
    [Constructible]
    public SpoolOfThread(int amount = 1) : base(0xFA0, amount)
    {
    }
}
