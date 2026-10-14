using System;
using System.Collections.Generic;
using ModernUO.Serialization;
using Server.Buffers;
using Server.Collections;
using Server.ContextMenus;
using Server.Engines.MLQuests;
using Server.Engines.MLQuests.Definitions;
using Server.Engines.MLQuests.Objectives;
using Server.Engines.Virtues;
using Server.Items;
using Server.Logging;
using Server.Misc;
using Server.Network;
using Server.Regions;
using EDI = Server.Mobiles.EscortDestinationInfo;

namespace Server.Mobiles;

[SerializationGenerator(3, false)]
public partial class BaseEscortable : BaseCreature
{
    private static readonly ILogger logger = LogFactory.GetLogger(typeof(BaseEscortable));

    public static readonly TimeSpan EscortDelay = TimeSpan.FromMinutes(5.0);

    public static readonly TimeSpan AbandonDelay =
        MLQuestSystem.Enabled ? TimeSpan.FromMinutes(1.0) : TimeSpan.FromMinutes(2.0);

    public static readonly TimeSpan DeleteTime =
        MLQuestSystem.Enabled ? TimeSpan.FromSeconds(100) : TimeSpan.FromSeconds(30);

    public static bool Initialized { get; set; }

    // Classic list
    // Used when: !MLQuestSystem.Enabled && !Core.ML
    public static readonly string[] TownNames =
    [
        "the town of Bastion", "the town of Myrish", "the town of Fairwinds",
        "the town of Stonehollow", "the town of Mireport", "the town of Cresthaven",
        "the town of Rivermouth", "the town of Teareek", "the town of Port Grim",
        "the town of Coldbarrow", "the town of Wharf's End", "the town of Landow"
    ];

    // ML list, pre-ML quest system
    // Used when: !MLQuestSystem.Enabled && Core.ML
    public static readonly string[] MlTownNames =
    [
        "the town of Bastion", "the town of Myrish", "the town of Fairwinds",
        "the town of Stonehollow", "the town of Mireport", "the town of Cresthaven",
        "the town of Rivermouth", "the town of Teareek", "the town of Port Grim",
        "the town of Coldbarrow", "the town of Wharf's End", "the town of Landow"
    ];

    // ML quest system general list
    // Used when: MLQuestSystem.Enabled && !Region.IsPartOf( "Haven Island" )
    private static readonly Dictionary<Type, (int[], int)> _mlQuestTypes = new()
    {
        { typeof(EscortToBastion), (Array.Empty<int>(), 0) }
    };

    // ML quest system New Haven list
    // Used when: MLQuestSystem.Enabled && Region.IsPartOf("Haven Island")
    // TODO: Find out if these specific quest messages are for special one-off quest characters
    // that teach you where things are in a city instead of the randomly spawned ones.
    private static readonly Dictionary<Type, (int[], int)> _mlQuestTypesNH = new()
    {
        { typeof(EscortToBastion), ([1042803, 1042804, 1042805, 1042831], 1042818) },
    };

    [SerializableField(0, setter: "private")]
    private string _destinationString;

    [SerializableField(1)]
    [DeserializeTimer(nameof(DeserializeDeleteTimer))]
    private Timer _deleteTimer;

    private void DeserializeDeleteTimer(TimeSpan delay) => Timer.DelayCall(delay, Delete);

    private void MigrateFrom(V2Content content)
    {
        _destinationString = content.DestinationString;
        _mlQuestType = content.MlQuestType;
        _mlQuestDestinationMessage = content.MlQuestDestinationMessage;
        _mlQuestPaymentMessage = content.MlQuestPaymentMessage;

        if (content.DeleteTimerDelay != TimeSpan.MinValue)
        {
            DeserializeDeleteTimer(content.DeleteTimerDelay);
        }
    }

    [SerializableField(2)]
    private Type _mlQuestType;

    [SerializableField(3)]
    private TextDefinition _mlQuestDestinationMessage;

    [SerializableField(4)]
    private TextDefinition _mlQuestPaymentMessage;

    private bool _deleteCorpse;

    private EDI _destination;

    private DateTime _lastSeenEscorter;

    // Not serialized
    private List<MLQuest> _mlQuest;

    [Constructible]
    public BaseEscortable() : base(AIType.AI_Melee, FightMode.Aggressor, 22)
    {
        InitBody();
        InitOutfit();

        Fame = 200;
        Karma = 4000;

        SetSpeed(0.2, 1.0);
    }

    public override bool StaticMLQuester => false; // Suppress automatic quest registration on creation/deserialization

    public override bool CanShout => !Controlled && !IsBeingDeleted;

    public bool IsBeingDeleted => _deleteTimer != null;

    public override bool Commandable => false; // Our master cannot boss us around!
    public override bool DeleteCorpseOnDeath => _deleteCorpse;

    [CommandProperty(AccessLevel.GameMaster)]
    public string Destination
    {
        get => _destination?.Name;
        set
        {
            DestinationString = value;
            _destination = EDI.Find(value);
        }
    }

    public static Dictionary<Mobile, BaseEscortable> EscortTable { get; } = new();

    protected override List<MLQuest> ConstructQuestList()
    {
        if (_mlQuestType == null)
        {
            var reg = Region;
            var types = reg.IsPartOf("Haven Island") ? _mlQuestTypesNH : _mlQuestTypes;

            // Get a rented buffer
            var list = STArrayPool<Type>.Shared.Rent(types.Keys.Count);

            // Copy to the new list
            types.Keys.CopyTo(list, 0);

            // Create a span that is the right size
            var listSpan = list.AsSpan(0, types.Keys.Count); // We need a span to manipulate

            // Shuffle
            listSpan.Shuffle();

            // Sequentially choose a quest until we have a valid one.
            for (var i = 0; i < listSpan.Length; ++i)
            {
                var questType = list[i];

                var quest = MLQuestSystem.FindQuest(questType);

                if (quest != null)
                {
                    var okay = true;

                    foreach (var obj in quest.Objectives)
                    {
                        if (obj is EscortObjective objective && objective.Destination.Contains(reg))
                        {
                            okay = false; // We're already there!
                            break;
                        }
                    }

                    if (okay)
                    {
                        var (destinationMessage, paymentMessage) = types[questType];
                        _mlQuestType = questType;
                        _mlQuestDestinationMessage = destinationMessage?.Length > 0 ? destinationMessage.RandomElement() : 0;
                        _mlQuestPaymentMessage = paymentMessage;
                        // Cached by BaseCreature in m_MLQuests
                        _mlQuest = [quest];
                        break;
                    }
                }
                else if (MLQuestSystem.Debug)
                {
                    logger.Warning("Escortable cannot be assigned quest type '{Type}' because the quest was not registered.", questType);
                }
            }

            if (_mlQuest != null && MLQuestSystem.Debug)
            {
                logger.Warning("No suitable quest found for '{Serial}'", Serial);
            }

            // Return the rented array
            STArrayPool<Type>.Shared.Return(list);
        }

        return _mlQuest;
    }

    public override void Shout(PlayerMobile pm)
    {
        /*
         * 1072301 - You there!  Care to hear how to earn some easy gold?
         * 1072302 - Adventurer!  I have an offer for you.
         * 1072303 - Wait!  I have an opportunity for you to make some gold!
         */
        MLQuestSystem.Tell(this, pm, Utility.Random(1072301, 3));
    }

    public virtual void InitBody()
    {
        SetStr(90, 100);
        SetDex(90, 100);
        SetInt(15, 25);

        Hue = Race.Human.RandomSkinHue();

        if (Female = Utility.RandomBool())
        {
            Body = 401;
            Name = NameList.RandomName("female");
        }
        else
        {
            Body = 400;
            Name = NameList.RandomName("male");
        }
    }

    public virtual void InitOutfit()
    {
        AddItem(new FancyShirt(Utility.RandomNeutralHue()));
        AddItem(new ShortPants(Utility.RandomNeutralHue()));
        AddItem(new Boots(Utility.RandomNeutralHue()));

        Utility.AssignRandomHair(this);

        PackGold(200, 250);
    }

    public virtual bool SayDestinationTo(Mobile m)
    {
        var dest = GetDestination();

        if (dest == null || !m.Alive)
        {
            return false;
        }

        var escorter = GetEscorter();

        if (escorter == null)
        {
            if (_mlQuestDestinationMessage != null)
            {
                if (_mlQuestDestinationMessage.Number > 0)
                {
                    Say(_mlQuestDestinationMessage.Number);
                    return true;
                }

                if (!string.IsNullOrEmpty(_mlQuestDestinationMessage.String))
                {
                    Say(_mlQuestDestinationMessage.String);
                    return true;
                }
            }

            // I'm looking to go somewhere:
            Say(1042807, AffixType.Append, dest.Name == "Ocllo" && m.Map == Map.Trammel ? "Haven" : dest.Name, "");
            return true;
        }

        if (escorter == m)
        {
            if (_mlQuestPaymentMessage != null)
            {
                if (_mlQuestPaymentMessage.Number > 0)
                {
                    Say(_mlQuestPaymentMessage.Number);
                    return true;
                }

                if (!string.IsNullOrEmpty(_mlQuestPaymentMessage.String))
                {
                    Say(_mlQuestPaymentMessage.String);
                    return true;
                }
            }

            // Lead on! Payment will be made when we arrive at ~1_DESTINATION~!
            Say(1042806, dest.Name == "Ocllo" && m.Map == Map.Trammel ? "Haven" : dest.Name);
            return true;
        }

        return false;
    }

    public virtual bool AcceptEscorter(Mobile m)
    {
        var dest = GetDestination();

        if (dest == null)
        {
            return false;
        }

        if (GetEscorter() != null || !m.Alive)
        {
            return false;
        }

        if (EscortTable.TryGetValue(m, out var escortable) && escortable?.Deleted == false &&
            escortable.GetEscorter() == m)
        {
            Say("I see you already have an escort.");
            return false;
        }

        if (m is PlayerMobile mobile && mobile.LastEscortTime + EscortDelay >= Core.Now)
        {
            var minutes =
                (int)Math.Ceiling((mobile.LastEscortTime + EscortDelay - Core.Now).TotalMinutes);

            if (minutes == 1)
            {
                Say($"You must rest {minutes} minute before we set out on this journey.");
            }
            else
            {
                Say($"You must rest {minutes} minutes before we set out on this journey.");
            }

            return false;
        }

        if (SetControlMaster(m))
        {
            _lastSeenEscorter = Core.Now;

            if (m is PlayerMobile playerMobile)
            {
                playerMobile.LastEscortTime = Core.Now;
            }

            Say(
                $"Lead on! Payment will be made when we arrive in {(dest.Name == "Ocllo" && m.Map == Map.Trammel ? "Haven" : dest.Name)}."
            );
            EscortTable[m] = this;
            StartFollow();
            return true;
        }

        return false;
    }

    public override bool HandlesOnSpeech(Mobile from) =>
        !MLQuestSystem.Enabled && (from.InRange(Location, 3) || base.HandlesOnSpeech(from));

    public override void OnSpeech(SpeechEventArgs e)
    {
        base.OnSpeech(e);

        if (GetDestination() == null || e.Handled || !e.Mobile.InRange(Location, 3))
        {
            return;
        }

        if (e.HasKeyword(0x1D)) // *destination*
        {
            e.Handled = SayDestinationTo(e.Mobile);
        }
        else if (e.HasKeyword(0x1E)) // *i will take thee*
        {
            e.Handled = AcceptEscorter(e.Mobile);
        }
    }

    public override void OnAfterDelete()
    {
        _deleteTimer?.Stop();

        _deleteTimer = null;

        base.OnAfterDelete();
    }

    public override void OnThink()
    {
        base.OnThink();
        CheckAtDestination();
    }

    protected override bool OnMove(Direction d)
    {
        if (!base.OnMove(d))
        {
            return false;
        }

        CheckAtDestination();

        return true;
    }

    // TODO: Pre-ML methods below, might be mergeable with the ML methods in EscortObjective

    public virtual void StartFollow()
    {
        StartFollow(GetEscorter());
    }

    public virtual void StartFollow(Mobile escorter)
    {
        if (escorter == null)
        {
            return;
        }

        ControlOrder = OrderType.Follow;
        ControlTarget = escorter;

        if (IsPrisoner && CantWalk)
        {
            CantWalk = false;
        }

        SetSpeed(0.1, 0.2, false);
    }

    public virtual void StopFollow()
    {
        ControlOrder = OrderType.None;
        ControlTarget = null;

        SetSpeed(0.2, 1.0);
    }

    public virtual Mobile GetEscorter()
    {
        if (!Controlled)
        {
            return null;
        }

        var master = ControlMaster;

        if (MLQuestSystem.Enabled || master == null)
        {
            return master;
        }

        if (master.Deleted || master.Map != Map || !master.InRange(Location, 30) || !master.Alive)
        {
            StopFollow();

            var lastSeenDelay = Core.Now - _lastSeenEscorter;

            if (lastSeenDelay >= AbandonDelay)
            {
                master.SendLocalizedMessage(1042473); // You have lost the person you were escorting.
                Say(1005653);                         // Hmmm. I seem to have lost my master.

                SetControlMaster(null);
                EscortTable.Remove(master);

                Timer.StartTimer(TimeSpan.FromSeconds(5.0), Delete);
                return null;
            }

            ControlOrder = OrderType.Stay;
            return master;
        }

        if (ControlOrder != OrderType.Follow)
        {
            StartFollow(master);
        }

        _lastSeenEscorter = Core.Now;
        return master;
    }

    public virtual void BeginDelete()
    {
        if (_deleteTimer != null)
        {
            _deleteTimer.Stop();
            _deleteTimer.Delay = DeleteTime;
            _deleteTimer.Start();
        }
        else
        {
            _deleteTimer = Timer.DelayCall(DeleteTime, Delete);
        }
    }

    public virtual bool CheckAtDestination()
    {
        if (MLQuestSystem.Enabled)
        {
            return false;
        }

        var dest = GetDestination();

        if (dest == null)
        {
            return false;
        }

        var escorter = GetEscorter();

        if (escorter == null)
        {
            return false;
        }

        if (dest.Contains(Location))
        {
            // We have arrived! I thank thee, ~1_PLAYER_NAME~! I have no further need of thy services. Here is thy pay.
            Say(1042809, escorter.Name);

            // not going anywhere
            _destination = null;
            DestinationString = null;

            var cont = escorter.Backpack ?? escorter.BankBox;

            var gold = new Gold(500, 1000);

            if (!cont.TryDropItem(escorter, gold, false))
            {
                gold.MoveToWorld(escorter.Location, escorter.Map);
            }

            StopFollow();
            SetControlMaster(null);
            EscortTable.Remove(escorter);
            BeginDelete();

            Titles.AwardFame(escorter, 10, true);

            var gainedPath = false;

            if (escorter is PlayerMobile pm)
            {
                var virtues = VirtueSystem.GetOrCreateVirtues(pm);
                if (virtues.CompassionGains > 0 && Core.Now > virtues.NextCompassionDay)
                {
                    virtues.NextCompassionDay = DateTime.MinValue;
                    virtues.CompassionGains = 0;
                }

                if (virtues.CompassionGains >= 5) // have already gained 5 times in one day, can gain no more
                {
                    // You must wait about a day before you can gain in compassion again.
                    pm.SendLocalizedMessage(1053004);
                }
                else if (VirtueSystem.Award(pm, VirtueName.Compassion, IsPrisoner ? 400 : 200, ref gainedPath))
                {
                    if (gainedPath)
                    {
                        pm.SendLocalizedMessage(1053005); // You have achieved a path in compassion!
                    }
                    else
                    {
                        pm.SendLocalizedMessage(1053002); // You have gained in compassion.
                    }

                    // in one day CompassionGains gets reset to 0
                    virtues.NextCompassionDay = Core.Now + TimeSpan.FromDays(1.0);

                    if (++virtues.CompassionGains >= 5)
                    {
                        // You must wait about a day before you can gain in compassion again.
                        pm.SendLocalizedMessage(1053004);
                    }
                }
                else
                {
                    // You have achieved the highest path of compassion and can no longer gain any further.
                    pm.SendLocalizedMessage(1053003);
                }
            }

            return true;
        }

        return false;
    }

    public override bool OnBeforeDeath()
    {
        _deleteCorpse = Controlled || IsBeingDeleted;

        return base.OnBeforeDeath();
    }

    private void Deserialize(IGenericReader reader, int version)
    {
        if (reader.ReadBool())
        {
            // NOTE: We cannot EDI.Find here, regions have not yet been loaded :-(
            _destinationString = reader.ReadString();
        }

        if (reader.ReadBool())
        {
            var deleteTime = reader.ReadDeltaTime();
            _deleteTimer = Timer.DelayCall(deleteTime - Core.Now, Delete);
            _deleteTimer.Start();
        }

        var quest = MLQuestSystem.ReadQuestRef(reader);

        if (MLQuestSystem.Enabled && quest != null && !StaticMLQuester)
        {
            _mlQuestType = quest.GetType();
            _mlQuest = [quest];

            // This isn't serialized before codegen
            if (_mlQuestTypesNH.TryGetValue(_mlQuestType, out var tuple))
            {
                var (destinationMessages, paymentMessage) = tuple;
                _mlQuestDestinationMessage = destinationMessages?.Length > 0 ? destinationMessages.RandomElement() : 0;
                _mlQuestPaymentMessage = paymentMessage;
            }
        }
    }

    [AfterDeserialization]
    private void AfterDeserialization()
    {
        if (MLQuestSystem.Enabled && !StaticMLQuester && _mlQuest != null && _mlQuestType != null)
        {
            var quest = MLQuestSystem.FindQuest(_mlQuestType);
            if (quest != null)
            {
                _mlQuest = [quest];
            }
        }
    }

    public override bool CanBeRenamedBy(Mobile from) => from.AccessLevel >= AccessLevel.GameMaster;

    public override void AddCustomContextEntries(Mobile from, ref PooledRefList<ContextMenuEntry> list)
    {
        if (from.Alive)
        {
            var escorter = GetEscorter();

            if (!MLQuestSystem.Enabled && GetDestination() != null)
            {
                if (escorter == null || escorter == from)
                {
                    list.Add(new AskDestinationEntry());
                }

                if (escorter == null)
                {
                    list.Add(new AcceptEscortEntry());
                }
            }

            if (escorter == from)
            {
                list.Add(new AbandonEscortEntry());
            }
        }

        base.AddCustomContextEntries(from, ref list);
    }

    public virtual string[] GetPossibleDestinations() => Core.ML ? MlTownNames : TownNames;

    public virtual string PickRandomDestination()
    {
        if (Map.Trammel.Regions.Count == 0 || Map == null || Map == Map.Internal || Location == Point3D.Zero)
        {
            return null; // Not yet fully initialized
        }

        var possible = GetPossibleDestinations();
        string picked = null;

        while (picked == null)
        {
            picked = possible.RandomElement();
            var test = EDI.Find(picked);

            if (test?.Contains(Location) != false)
            {
                picked = null;
            }
        }

        return picked;
    }

    public EDI GetDestination()
    {
        if (MLQuestSystem.Enabled || !Initialized)
        {
            return null;
        }

        if (_destinationString == null && _deleteTimer == null)
        {
            _destinationString = PickRandomDestination();
        }

        if (_destination != null && _destination.Name == _destinationString)
        {
            return _destination;
        }

        if (Map.Trammel.Regions.Count > 0)
        {
            _destination = EDI.Find(_destinationString);

            // The destination string used to exist, but doesn't anymore.
            if (_destination == null)
            {
                _destinationString = null;
                return null;
            }

            return _destination;
        }

        // Destination is invalid, so set it to null and return.
        return _destination = null;
    }
}

public class EscortDestinationInfo
{
    private static readonly ILogger logger = LogFactory.GetLogger(typeof(EscortDestinationInfo));

    private static Dictionary<string, EDI> _table;

    public EscortDestinationInfo(string name, Region region)
    {
        Name = name;
        Region = region;
    }

    public string Name { get; }

    public Region Region { get; }

    public bool Contains(Point3D p) => Region.Contains(p);

    public static void Initialize()
    {
        var list = Map.Trammel.Regions.Values;

        if (list.Count == 0)
        {
            return;
        }

        _table = new Dictionary<string, EDI>();

        foreach (var r in list)
        {
            if (r.Name != null)
            {
                _table[r.Name] = new EDI(r.Name, r);
            }
        }

        // Validate that we have at least one valid destination
        var validTown = false;
        var townNamesVariable = Core.ML ? nameof(BaseEscortable.MlTownNames) : nameof(BaseEscortable.TownNames);
        var towns = Core.ML ? BaseEscortable.MlTownNames : BaseEscortable.TownNames;
        for (var i = 0; i < towns.Length; ++i)
        {
            var town = towns[i];
            if (_table.ContainsKey(town))
            {
                validTown = true;
                break;
            }
        }

        BaseEscortable.Initialized = validTown;

        if (!validTown)
        {
            logger.Error("No valid escort destinations found. Please check {TownNames}.", townNamesVariable);
        }
    }

    public static EDI Find(string name)
    {
        if (name == null || _table == null)
        {
            return null;
        }

        _table.TryGetValue(name, out var info);
        return info;
    }
}

public class AskDestinationEntry : ContextMenuEntry
{
    public AskDestinationEntry() : base(6100, 3)
    {
    }

    public override void OnClick(Mobile from, IEntity target)
    {
        (target as BaseEscortable)?.SayDestinationTo(from);
    }
}

public class AcceptEscortEntry : ContextMenuEntry
{
    public AcceptEscortEntry() : base(6101, 3)
    {
    }

    public override void OnClick(Mobile from, IEntity target)
    {
        (target as BaseEscortable)?.AcceptEscorter(from);
    }
}

public class AbandonEscortEntry : ContextMenuEntry
{
    public AbandonEscortEntry() : base(6102, 3)
    {
    }

    public override void OnClick(Mobile from, IEntity target)
    {
        (target as BaseEscortable)?.Delete(); // OSI just seems to delete instantly
    }
}
