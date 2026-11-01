/*
 * Oz'Thoth's Static Tool v.4
 *
 * Original credits:
 * - Vorspire: Page persistence functionality
 * - Espcevan: Original AddWall and AddStair commands
 * - Pyro: Text entries and command button improvements
 * - TheArt/Oz'Thoth: Original tool creation
 *
 * Modernized for .NET 10 / ModernUO
 */

using System;
using Server.Commands;
using Server.Network;

namespace Server.Gumps.OzThoth;

/// <summary>
/// Represents a command button with display name and command to execute.
/// </summary>
/// <param name="Name">The display name shown on the button.</param>
/// <param name="Command">The command to execute (without prefix).</param>
public readonly record struct CommandPair(string Name, string Command);

/// <summary>
/// Main Static Tool gump for GM building operations.
/// </summary>
public sealed class OzStaticToolGump : Gump
{
    private const int SeparatorLineHue = 1461;
    private const string FontStyle = "<basefont size=5 color=#140B00>";
    private const string SignLabel = "<center>Oz'Thoth's Static Tool v.4";

    private static readonly CommandPair[] CommonCommands =
    [
        new("Item ID", "Get ItemID"),
        new("Props", "Props"),
        new("Wipe", "Wipe"),
        new("Delete", "Delete")
    ];

    private static readonly CommandPair[] StaticItemCommands =
    [
        new("Arches", "AddArch"),
        new("Doors", "AddDoor"),
        new("Fences", "AddFence"),
        new("Floors", "AddFloor"),
        new("Food", "AddCookingStatic"),
        new("Furniture", "AddFurniture"),
        new("Gear", "AddGearStatic"),
        new("Ground", "AddGroundSt"),
        new("Light", "AddLightsStatic"),
        new("Plants", "AddPlants"),
        new("Rocks", "AddRock"),
        new("Roofs", "AddRoof"),
        new("Sign Post", "AddSignPStatic"),
        new("Signs", "AddSignStatic"),
        new("Stairs", "AddStair"),
        new("Walls", "AddWall"),
        new("Misc 1", "AddMiscStatic1"),
        new("Misc 2", "AddMiscStatic2"),
        new("Custom", "AddCustomsStatic"),
        new("Anim", "AddAnimationStatic")
    ];

    private int _heightSingle;
    private int _heightArea;
    private int _directionSingle;
    private int _directionArea;

    public static void Initialize()
    {
        CommandSystem.Register("StaticsTool", AccessLevel.GameMaster, OzThothsStatic_OnCommand);
        CommandSystem.Register("STool", AccessLevel.GameMaster, OzThothsStatic_OnCommand);
        CommandSystem.Register("ST", AccessLevel.GameMaster, OzThothsStatic_OnCommand);
    }

    [Usage("StaticTool")]
    [Description("Opens Oz'Thoth's in-game static building tool.")]
    private static void OzThothsStatic_OnCommand(CommandEventArgs e)
    {
        var from = e.Mobile;
        from.CloseGump<OzStaticToolGump>();
        from.SendGump(new OzStaticToolGump());
    }

    public OzStaticToolGump(int heightSingle = 1, int heightArea = 1, int directionSingle = 1, int directionArea = 1)
        : base(0, 0)
    {
        _heightSingle = heightSingle;
        _heightArea = heightArea;
        _directionSingle = directionSingle;
        _directionArea = directionArea;

        Closable = false;
        Disposable = false;
        Draggable = true;
        Resizable = false;

        AddPage(0);
        BuildTitleBar();
        BuildCenterPanel();
        BuildRightPanel();
        BuildLeftPanel();
    }

    private void BuildTitleBar()
    {
        AddImageTiled(49, 46, 570, 376, 2524, 170, 112, 796);

        // Sign with signature
        AddImage(469, 66, 100, 1938);
        AddImage(474, 66, 100, 1938);
        AddHtml(492, 87, 99, 59, $"<basefont size=5 color=#332b00>{SignLabel}", false, false);
        AddHtml(493, 88, 99, 59, $"<basefont size=5 color=#b29600>{SignLabel}", false, false);
        AddHtml(494, 89, 99, 59, $"<basefont size=5 color=#e5c100>{SignLabel}", false, false);

        // Minimize and close buttons
        AddButton(578, 47, 2710, 2711, 1, GumpButtonType.Reply, 0);
        AddButton(598, 47, 2708, 2709, 0, GumpButtonType.Reply, 0);
    }

    private void BuildCenterPanel()
    {
        AddBackground(282, 65, 198, 355, 9350);

        // Height section
        AddHtml(345, 82, 62, 26, $"{FontStyle}Height", false, false);
        AddImageTiled(311, 99, 131, 16, 58, 16, 16, SeparatorLineHue);

        // Height Single
        AddHtml(306, 109, 57, 26, $"{FontStyle}<center>Single", false, false);
        AddButton(326, 137, 5600, 5604, 2, GumpButtonType.Reply, 0); // Up
        AddImageTiled(318, 154, 32, 20, 9304);
        AddButton(326, 177, 5602, 5606, 3, GumpButtonType.Reply, 0); // Down
        AddTextEntry(315, 153, 36, 20, _directionArea == 0 ? 1461 : 0, 0, $"{_heightSingle}");

        // Height Area
        AddHtml(396, 110, 44, 26, $"{FontStyle}<center>Area", false, false);
        AddButton(411, 138, 5600, 5604, 4, GumpButtonType.Reply, 0); // Up
        AddImageTiled(402, 154, 32, 20, 9304);
        AddButton(411, 178, 5602, 5606, 5, GumpButtonType.Reply, 0); // Down
        AddTextEntry(400, 154, 36, 20, _directionArea == 0 ? 1461 : 0, 1, $"{_heightArea}");

        // Direction section
        AddImageTiled(311, 199, 131, 16, 58, 16, 16, SeparatorLineHue);
        AddHtml(289, 212, 174, 26, $"{FontStyle}<center>Direction", false, false);
        AddImageTiled(311, 229, 131, 16, 58, 16, 16, SeparatorLineHue);

        // Direction Single
        AddHtml(306, 239, 57, 26, $"{FontStyle}<center>Single", false, false);
        AddButton(326, 270, 5600, 5604, 9, GumpButtonType.Reply, 0);  // North
        AddButton(295, 294, 5603, 5607, 7, GumpButtonType.Reply, 0);  // West
        AddImageTiled(318, 292, 32, 20, 9304);
        AddButton(354, 294, 5601, 5605, 8, GumpButtonType.Reply, 0);  // East
        AddButton(326, 319, 5602, 5606, 6, GumpButtonType.Reply, 0);  // South
        AddTextEntry(316, 292, 36, 20, _directionArea == 0 ? 1461 : 0, 2, $"{_directionSingle}");

        // Direction Area
        AddHtml(396, 239, 44, 26, $"{FontStyle}<center>Area", false, false);
        AddButton(411, 270, 5600, 5604, 13, GumpButtonType.Reply, 0); // North
        AddButton(382, 294, 5603, 5607, 11, GumpButtonType.Reply, 0); // West
        AddImageTiled(402, 292, 32, 20, 9304);
        AddButton(437, 294, 5601, 5605, 12, GumpButtonType.Reply, 0); // East
        AddButton(411, 319, 5602, 5606, 10, GumpButtonType.Reply, 0); // South
        AddTextEntry(400, 292, 36, 20, _directionArea == 0 ? 1461 : 0, 3, $"{_directionArea}", 5);

        // Common commands
        const int start = 14;
        for (int i = 0; i < CommonCommands.Length; i++)
        {
            int columnSpacing = i % 2 == 1 ? 90 : 0;
            int rowSpacing = i / 2 == 1 ? 30 : 0;
            int imageOffset = i / 2 == 1 ? -2 : 0;

            AddButton(300 + columnSpacing, 353 + rowSpacing, 22153 + imageOffset, 22155 + (imageOffset == -2 ? -3 : 0), start + i, GumpButtonType.Reply, 0);
            AddHtml(318 + columnSpacing, 351 + rowSpacing, 62, 26, $"{FontStyle}{CommonCommands[i].Name}", false, false);
        }
    }

    private void BuildRightPanel()
    {
        AddBackground(469, 161, 150, 260, 9300);

        AddHtml(471, 173, 145, 26, $"{FontStyle}<center>Helpful Tips", false, false);
        AddImageTiled(474, 196, 140, 16, 58, 16, 16, SeparatorLineHue);
        AddHtml(471, 212, 145, 192,
            $"{FontStyle}" +
            "[Add Static ItemID<br/>" +
            "[Tile Static ItemID<br/>" +
            "[TileZ # Static ItemID for Height</br>" +
            "To Randomize statics add number after ItemID<br/>" +
            "To do multiple Deletes:<br/>" +
            "[M Delete",
            false, false);
    }

    private void BuildLeftPanel()
    {
        AddBackground(48, 65, 243, 357, 3500);

        const int startButtonId = 14 + 4; // After common commands
        int x = 65;
        int startY = 84;
        int y = startY;

        for (int i = 0; i < StaticItemCommands.Length; i++)
        {
            if (i > 0 && i % 11 == 0)
            {
                x += 110;
                y = startY;
            }

            AddButton(x, y, 5601, 5605, startButtonId + i, GumpButtonType.Reply, 0);
            AddHtml(x + 21, y - 2, 84, 26, $"{FontStyle}{StaticItemCommands[i].Name}", false, false);
            y += 30;
        }
    }

    private void AddImageTiled(int x, int y, int width, int height, int gumpID, int gumpWidth, int gumpHeight, int hue)
    {
        for (int xi = 0; xi < width; xi += gumpWidth)
        {
            for (int yi = 0; yi < height; yi += gumpHeight)
            {
                AddImage(Math.Min(x + xi, x + width - gumpWidth), Math.Min(y + yi, y + height - gumpHeight), gumpID, hue);
            }
        }
    }

    public override void OnResponse(NetState sender, in RelayInfo info)
    {
        var from = sender.Mobile;
        int buttonId = info.ButtonID;

        if (buttonId == 0 || from.AccessLevel < AccessLevel.GameMaster)
        {
            from.CloseGump<OzStaticToolGump>();
            return;
        }

        // Parse text entries
        _heightSingle = ParseTextEntry(info, 0, 1);
        _heightArea = ParseTextEntry(info, 1, 1);
        _directionSingle = ParseTextEntry(info, 2, 1);
        _directionArea = ParseTextEntry(info, 3, 1);

        string prefix = CommandSystem.Prefix;

        switch (buttonId)
        {
            case 1: // Minimize
                from.SendGump(new MinimizedOzGump());
                return;

            // Height adjustment
            case 2: CommandSystem.Handle(from, $"{prefix}Inc z {_heightSingle}"); break;
            case 3: CommandSystem.Handle(from, $"{prefix}Inc z -{_heightSingle}"); break;
            case 4: CommandSystem.Handle(from, $"{prefix}Area Inc z {_heightArea}"); break;
            case 5: CommandSystem.Handle(from, $"{prefix}Area Inc z -{_heightArea}"); break;

            // Direction adjustment
            case 6: CommandSystem.Handle(from, $"{prefix}Inc y {_directionSingle}"); break;
            case 7: CommandSystem.Handle(from, $"{prefix}Inc x -{_directionSingle}"); break;
            case 8: CommandSystem.Handle(from, $"{prefix}Inc x {_directionSingle}"); break;
            case 9: CommandSystem.Handle(from, $"{prefix}Inc y -{_directionSingle}"); break;
            case 10: CommandSystem.Handle(from, $"{prefix}Area Inc y {_directionArea}"); break;
            case 11: CommandSystem.Handle(from, $"{prefix}Area Inc x -{_directionArea}"); break;
            case 12: CommandSystem.Handle(from, $"{prefix}Area Inc x {_directionArea}"); break;
            case 13: CommandSystem.Handle(from, $"{prefix}Area Inc y -{_directionArea}"); break;

            default:
                HandleCommandButton(from, buttonId, prefix);
                break;
        }

        from.SendGump(new OzStaticToolGump(_heightSingle, _heightArea, _directionSingle, _directionArea));
    }

    private static void HandleCommandButton(Mobile from, int buttonId, string prefix)
    {
        const int commonStart = 14;
        const int staticStart = commonStart + 4;

        if (buttonId >= commonStart && buttonId < staticStart)
        {
            int index = buttonId - commonStart;
            CommandSystem.Handle(from, $"{prefix}{CommonCommands[index].Command}");
        }
        else if (buttonId >= staticStart && buttonId < staticStart + StaticItemCommands.Length)
        {
            int index = buttonId - staticStart;
            CommandSystem.Handle(from, $"{prefix}{StaticItemCommands[index].Command}");
        }
    }

    private static int ParseTextEntry(RelayInfo info, int id, int defaultValue)
    {
        string text = info.GetTextEntry(id);
        return int.TryParse(text, out int result) ? result : defaultValue;
    }
}

/// <summary>
/// Minimized version of the Static Tool gump.
/// </summary>
public sealed class MinimizedOzGump : Gump
{
    public MinimizedOzGump() : base(0, 0)
    {
        Closable = false;
        Disposable = false;
        Draggable = true;
        Resizable = false;

        AddPage(0);
        AddBackground(16, 11, 110, 26, 9350);
        AddButton(106, 17, 2118, 2117, 1, GumpButtonType.Reply, 0);
        AddHtml(19, 16, 83, 16, "<basefont size=5 color=#140B00>Static Tool", false, false);
    }

    public override void OnResponse(NetState sender, in RelayInfo info)
    {
        if (info.ButtonID == 1)
        {
            var from = sender.Mobile;
            from.SendGump(new OzStaticToolGump());
            from.CloseGump<MinimizedOzGump>();
        }
    }
}
