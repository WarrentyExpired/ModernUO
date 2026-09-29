using System;
using ModernUO.Serialization;
using Server;
using Server.Commands;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
    [SerializationGenerator(0, false)]
    public partial class SecretDoorLink : Item
    {
        [SerializableField(0)]
        private Serial _triggerSerial;

        [SerializableField(1)]
        private Serial _secretSerial;

        [SerializableField(2)]
        private Point3D _triggerSpot;

        [SerializableField(3)]
        private Map _triggerMap;

        [SerializableField(4)]
        private Point3D _secretHomeLoc;

        [SerializableField(5)]
        private Map _secretHomeMap;

        [SerializableField(6)]
        private bool _secretIsHidden;

        private Timer _timer;

        [Constructible]
        public SecretDoorLink() : base(0x1F14)
        {
            Name = "Secret Door Link (controller)";
            Movable = true;
            StartTimer();
        }

        // ✅ This is the key fix for "doesn't work after restart"
        [AfterDeserialization(false)]
        private void AfterDeserialization()
        {
            StartTimer();
            Timer.DelayCall(TimeSpan.Zero, Tick);
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();
            _timer?.Stop();
            _timer = null;
        }

        private void StartTimer()
        {
            _timer?.Stop();
            _timer = Timer.DelayCall(TimeSpan.FromMilliseconds(400), TimeSpan.FromMilliseconds(400), Tick);
        }

        private Item TriggerItem => _triggerSerial.IsValid ? World.FindItem(_triggerSerial) : null;
        private Item SecretItem => _secretSerial.IsValid ? World.FindItem(_secretSerial) : null;

        private void Tick()
        {
            if (!_triggerSerial.IsValid || !_secretSerial.IsValid)
                return;

            var trigger = TriggerItem;
            var secret = SecretItem;

            if (trigger == null || trigger.Deleted || secret == null || secret.Deleted)
                return;

            if (_triggerMap == null || _secretHomeMap == null)
                return;

            bool onSpot =
                trigger.Map == _triggerMap &&
                trigger.Location == _triggerSpot;

            // REVERSED behavior:
            // Trigger ON spot => secret disappears
            // Trigger OFF spot => secret appears
            if (onSpot)
                HideSecret(secret);
            else
                ShowSecret(secret);
        }

        private void ShowSecret(Item secret)
        {
            if (!_secretIsHidden)
                return;

            secret.MoveToWorld(_secretHomeLoc, _secretHomeMap);
            secret.Visible = true;

            _secretIsHidden = false;
        }

        private void HideSecret(Item secret)
        {
            if (_secretIsHidden)
                return;

            secret.Visible = false;
            secret.MoveToWorld(Point3D.Zero, Map.Internal);

            _secretIsHidden = true;
        }

        // ---------- GM Commands ----------
        public static void Initialize()
        {
            CommandSystem.Register("SecretDoorLink", AccessLevel.GameMaster, SecretDoorLinkCommand);
            CommandSystem.Register("SecretDoorStatus", AccessLevel.GameMaster, SecretDoorStatusCommand);
        }

        [Usage("SecretDoorLink")]
        [Description("Links a trigger item to a secret item that disappears when the trigger is placed on a chosen tile, and reappears when moved off.")]
        private static void SecretDoorLinkCommand(CommandEventArgs e)
        {
            e.Mobile.SendMessage("Target the TRIGGER item (the one you will move).");
            e.Mobile.Target = new PickTriggerTarget();
        }

        [Usage("SecretDoorStatus")]
        [Description("Target a SecretDoorLink controller to print its configuration.")]
        private static void SecretDoorStatusCommand(CommandEventArgs e)
        {
            e.Mobile.SendMessage("Target the SecretDoorLink controller.");
            e.Mobile.Target = new StatusTarget();
        }

        private class PickTriggerTarget : Target
        {
            public PickTriggerTarget() : base(12, false, TargetFlags.None) { }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is not Item trigger || trigger.Deleted)
                {
                    from.SendMessage("That is not a valid item.");
                    return;
                }

                from.SendMessage("Now target the SECRET item (the door/item that will appear/disappear).");
                from.Target = new PickSecretTarget(trigger);
            }
        }

        private class PickSecretTarget : Target
        {
            private readonly Item _trigger;

            public PickSecretTarget(Item trigger) : base(12, false, TargetFlags.None)
            {
                _trigger = trigger;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is not Item secret || secret.Deleted)
                {
                    from.SendMessage("That is not a valid item.");
                    return;
                }

                from.SendMessage("Now target the LOCATION (ground tile) where the trigger must be placed.");
                from.Target = new PickLocationTarget(_trigger, secret);
            }
        }

        private class PickLocationTarget : Target
        {
            private readonly Item _trigger;
            private readonly Item _secret;

            public PickLocationTarget(Item trigger, Item secret) : base(18, true, TargetFlags.None)
            {
                _trigger = trigger;
                _secret = secret;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is not IPoint3D p)
                {
                    from.SendMessage("Invalid location.");
                    return;
                }

                var map = from.Map;

                var controller = new SecretDoorLink
                {
                    _triggerSerial = _trigger.Serial,
                    _secretSerial = _secret.Serial,
                    _triggerSpot = new Point3D(p),
                    _triggerMap = map,
                    _secretHomeLoc = _secret.Location,
                    _secretHomeMap = _secret.Map,
                    _secretIsHidden = false
                };

                controller.MoveToWorld(from.Location, from.Map);

                // Force initial evaluation
                controller.Tick();

                from.SendMessage("Linked! Trigger ON spot => secret disappears. Trigger OFF spot => secret appears.");
            }
        }

        private class StatusTarget : Target
        {
            public StatusTarget() : base(12, false, TargetFlags.None) { }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is not SecretDoorLink link || link.Deleted)
                {
                    from.SendMessage("That is not a SecretDoorLink controller.");
                    return;
                }

                from.SendMessage($"Trigger: {link._triggerSerial}  Secret: {link._secretSerial}");
                from.SendMessage($"Spot: {link._triggerSpot} on {link._triggerMap}");
                from.SendMessage($"Secret home: {link._secretHomeLoc} on {link._secretHomeMap}");
                from.SendMessage($"Hidden now: {link._secretIsHidden}");
            }
        }
    }
}
