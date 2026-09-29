/***********************************
* Author: Zik (12/21/2013)         *
* Refactoring: Expa (05/08/2026)   *
* Optimized for ModernUO 0.15.6.48 *
***********************************/
using ModernUO.Serialization;
using Server;
using Server.Mobiles;
using System;
using System.Runtime.CompilerServices;

namespace Server.Items;

public static class TrainingConfig
{
    public const int MaxDistance = 1;
    public const int AntiMacroDelayMs = 5000;
    public const double MaxSkill = 100.0;
    public const double MaxSkillDifficulty = 100.0;
}

public abstract class SkillTrainingBoxBase : LockableContainer
{
    protected SkillTrainingBoxBase(int itemID) : base(itemID)
    {
        RequiredSkill = 0;
        Movable = false;
    }

    protected SkillTrainingBoxBase(Serial serial) : base(serial)
    {
    }

    private sealed class TrainTimeRecord
    {
        public DateTime LastTime { get; set; }
    }

    private static readonly ConditionalWeakTable<Mobile, TrainTimeRecord> _lastTrainTime = new();
    private static readonly object _syncLock = new();

    protected bool ValidateTraining(Mobile from)
    {
        if (!from.Alive)
        {
            from.SendMessage("You are dead.");
            return false;
        }

        if (!InRange(from.Location, TrainingConfig.MaxDistance))
        {
            from.SendMessage("You are too far away.");
            return false;
        }

        return CheckAntiMacro(from);
    }

    private static bool CheckAntiMacro(Mobile from)
    {
        lock (_syncLock)
        {
            var now = DateTime.UtcNow;

            if (_lastTrainTime.TryGetValue(from, out var record))
            {
                if ((now - record.LastTime).TotalMilliseconds < TrainingConfig.AntiMacroDelayMs)
                    return false;
            }

            return true;
        }
    }

    protected static void UpdateTrainingTime(Mobile from)
    {
        lock (_syncLock)
        {
            if (_lastTrainTime.TryGetValue(from, out var record))
                record.LastTime = DateTime.UtcNow;
            else
                _lastTrainTime.Add(from, new TrainTimeRecord { LastTime = DateTime.UtcNow });
        }
    }
}

[SerializationGenerator(1, false)]
[Flippable(0xE86, 0xE85)]
public sealed partial class LockpickTrainingBox : SkillTrainingBoxBase
{
    [Constructible]
    public LockpickTrainingBox() : base(0x9A8)
    {
        Name = "Lockpick Training chest";
        Locked = true;
        LockLevel = 1;
    }

    public LockpickTrainingBox(Serial serial) : base(serial)
    {
    }

    public override void OnDoubleClick(Mobile from)
    {
        if (!ValidateTraining(from))
            return;

        var skill = from.Skills.Lockpicking;

        if (skill.Base >= TrainingConfig.MaxSkill)
        {
            from.SendMessage($"Your Lockpicking skill is already at the maximum level of {TrainingConfig.MaxSkill}.");
            return;
        }

        var skillValue = Math.Min(skill.Value, TrainingConfig.MaxSkillDifficulty);
        RequiredSkill = (int)skillValue;
        LockLevel = RequiredSkill;
        from.SendMessage($"Training Chest adjusted to {skillValue:F1} Lockpicking");
        Locked = true;
        UpdateTrainingTime(from);
    }

    public override void LockPick(Mobile from)
    {
        if (!ValidateTraining(from))
            return;

        var difficulty = RequiredSkill > 0 ? RequiredSkill : 50;

        from.CheckSkill(SkillName.Lockpicking, difficulty * 0.8, difficulty * 1.2);

        Locked = true;
    }

    public override void OnSingleClick(Mobile from)
    {
        LabelTo(from, $"Set to {RequiredSkill} Lockpicking");
        LabelTo(from, Name);
        from.SendMessage("Double click the box to set it to your Lockpicking Skill.");
    }

    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)]
[Flippable(0xE86, 0xE85)]
public sealed partial class RemoveTrapTrainingBox : SkillTrainingBoxBase
{
    [Constructible]
    public RemoveTrapTrainingBox() : base(0x9A8)
    {
        Name = "Remove Trap Training chest";
        Locked = false;
        LockLevel = 0;
        TrapOnLockpick = true;
        TrapLevel = 1;
        TrapType = TrapType.ExplosionTrap;
    }

    public RemoveTrapTrainingBox(Serial serial) : base(serial)
    {
    }

    public override void OnDoubleClick(Mobile from)
    {
        if (!ValidateTraining(from))
            return;

        var skill = from.Skills.RemoveTrap;

        if (skill.Base >= TrainingConfig.MaxSkill)
        {
            from.SendMessage($"Your Remove Trap skill is already at the maximum level of {TrainingConfig.MaxSkill}.");
            return;
        }

        var skillValue = Math.Min(skill.Value, TrainingConfig.MaxSkillDifficulty);
        RequiredSkill = (int)skillValue;
        TrapLevel = RequiredSkill;
        from.SendMessage($"Training Chest adjusted to {skillValue:F1} Remove Trap");
        Locked = false;
        TrapOnLockpick = true;
        TrapType = TrapType.ExplosionTrap;
        UpdateTrainingTime(from);
    }

    public override void LockPick(Mobile from)
    {
        if (!ValidateTraining(from))
            return;

        var difficulty = TrapLevel > 0 ? TrapLevel : 50;

        if (from.CheckSkill(SkillName.RemoveTrap, difficulty * 0.8, difficulty * 1.2))
        {
        }
        else
        {
            ExecuteTrap(from);
        }

        TrapType = TrapType.ExplosionTrap;
        TrapLevel = RequiredSkill > 0 ? RequiredSkill : 1;
        TrapOnLockpick = true;
    }

    public override void OnSingleClick(Mobile from)
    {
        LabelTo(from, $"Set to {RequiredSkill} Remove Trap");
        LabelTo(from, Name);
        from.SendMessage("Double click the box to set it to your Remove Trap Skill.");
    }

    private void Deserialize(IGenericReader reader, int version)
    {
    }
}