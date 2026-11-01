/*
 * Base class for Oz'Thoth's Static Tool category menu gumps.
 * Modernized for .NET 10 / ModernUO
 */

using System;
using Server.Commands;
using Server.Network;

namespace Server.Gumps.OzThoth;

/// <summary>
/// Represents a category button entry with display name and command.
/// </summary>
/// <param name="Name">The display name shown on the button.</param>
/// <param name="Command">The command to execute (without prefix).</param>
public readonly record struct CategoryEntry(string Name, string Command);

/// <summary>
/// Base class for category menu gumps that display buttons to open sub-gumps.
/// </summary>
public abstract class CategoryGumpBase : Gump
{
    /// <summary>
    /// Gets the title displayed at the top of the gump.
    /// </summary>
    protected abstract string Title { get; }

    /// <summary>
    /// Gets the category entries to display.
    /// </summary>
    protected abstract ReadOnlySpan<CategoryEntry> Categories { get; }

    /// <summary>
    /// Creates a new instance of this gump type.
    /// </summary>
    protected abstract CategoryGumpBase CreateInstance();

    protected CategoryGumpBase() : base(0, 0)
    {
        Draggable = true;
        Closable = true;
        Resizable = false;
        Disposable = false;

        var categories = Categories;

        AddPage(0);
        AddBackground(268, 135, 345, 479, 3500);
        AddHtml(330, 160, 211, 31, $"<basefont size=5><center>{Title}", false, false);

        int leftX = 290;
        int rightX = 450;
        int startY = 200;
        int rowHeight = 40;

        for (int i = 0; i < categories.Length; i++)
        {
            int x = i % 2 == 0 ? leftX : rightX;
            int y = startY + (i / 2 * rowHeight);
            int buttonId = i + 1;

            AddButton(x, y, 1154, 1155, buttonId, GumpButtonType.Reply, 0);
            AddHtml(x + 30, y, 113, 25, $"<basefont size=5><center>{categories[i].Name}", false, false);
        }
    }

    public override void OnResponse(NetState sender, in RelayInfo info)
    {
        var from = sender.Mobile;
        int buttonId = info.ButtonID;

        if (buttonId <= 0)
        {
            return;
        }

        var categories = Categories;
        int index = buttonId - 1;

        if (index >= 0 && index < categories.Length)
        {
            CommandSystem.Handle(from, $"{CommandSystem.Prefix}{categories[index].Command}");
            from.SendGump(CreateInstance());
        }
    }
}
