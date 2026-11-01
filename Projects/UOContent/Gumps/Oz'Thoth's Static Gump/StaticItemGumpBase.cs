/*
 * Base class for Oz'Thoth's Static Tool item selection gumps.
 * Modernized for .NET 10 / ModernUO
 */

using System;
using Server.Commands;
using Server.Network;

namespace Server.Gumps.OzThoth;

/// <summary>
/// Base class for paginated item selection gumps.
/// Subclasses define the item IDs and the command to execute on selection.
/// </summary>
public abstract class StaticItemGumpBase : Gump
{
    private const int ItemsPerPage = 12;
    private const int PageButtonOffset = 10000;
    private const int ItemButtonOffset = 1000000;

    private readonly int _page;

    /// <summary>
    /// Gets the item IDs to display in this gump.
    /// </summary>
    protected abstract ReadOnlySpan<int> ItemIds { get; }

    /// <summary>
    /// Gets the command format string. Use {0} for command prefix and {1} for item ID.
    /// Example: "{0}Tile Static {1}" or "{0}M Add Static {1}"
    /// </summary>
    protected abstract string CommandFormat { get; }

    /// <summary>
    /// Creates a new instance of this gump type with the specified page.
    /// </summary>
    protected abstract StaticItemGumpBase CreateInstance(int page);

    protected StaticItemGumpBase(int page = 0) : base(0, 0)
    {
        var itemIds = ItemIds;
        int pageCount = (itemIds.Length + ItemsPerPage - 1) / ItemsPerPage;

        _page = Math.Clamp(page, 0, Math.Max(0, pageCount - 1));

        Closable = true;
        Disposable = true;
        Draggable = true;
        Resizable = false;

        AddPage(0);
        AddBackground(0, 0, 720, 180, 3500);

        // Page number display
        AddHtmlLocalized(15, 15, 60, 20, 1042971, _page.ToString(), 0x1, false, false); // #

        // Next button
        AddHtmlLocalized(20, 38, 60, 20, 1043353, 0x1, false, false); // Next
        int nextPage = _page + 1 < pageCount ? _page + 1 : 0;
        AddButton(15, 55, 0xFA5, 0xFA7, PageButtonOffset + nextPage, GumpButtonType.Reply, 0);

        // Back button
        AddHtmlLocalized(20, 93, 60, 20, 1011393, 0x1, false, false); // Back
        int prevPage = _page > 0 ? _page - 1 : 0;
        AddButton(15, 110, 0xFAE, 0xFB0, PageButtonOffset + prevPage, GumpButtonType.Reply, 0);

        // Items
        for (int i = 0; i < ItemsPerPage; i++)
        {
            int index = (_page * ItemsPerPage) + i;
            if (index >= itemIds.Length)
            {
                break;
            }

            int itemId = itemIds[index];
            int offset = (i + 1) * 50;

            if (itemId > 0)
            {
                AddButton(45 + offset, 20, 0x2624, 0x2625, ItemButtonOffset + index, GumpButtonType.Reply, itemId);
                AddItem(50 + offset, 40, itemId);
            }
            else
            {
                AddImage(25 + offset, 20, 0x2625, 900);
            }
        }
    }

    public override void OnResponse(NetState sender, in RelayInfo info)
    {
        var from = sender.Mobile;
        int button = info.ButtonID;

        if (button <= 0)
        {
            return;
        }

        int page = _page;
        var itemIds = ItemIds;

        if (button >= ItemButtonOffset)
        {
            int index = button - ItemButtonOffset;
            if (index >= 0 && index < itemIds.Length)
            {
                string command = string.Format(CommandFormat, CommandSystem.Prefix, itemIds[index]);
                CommandSystem.Handle(from, command);
            }
        }
        else if (button >= PageButtonOffset)
        {
            page = button - PageButtonOffset;
        }

        from.SendGump(CreateInstance(page));
    }
}
