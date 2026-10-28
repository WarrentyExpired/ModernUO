using System;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using System.Globalization;
using Server;

namespace Server.Engines.Craft;

public class CraftQueueGump : DynamicGump
{
    private const int LabelColor = 0x7FFF;
    private const int GreyLabelColor = 0x3DEF;

    private readonly Mobile _from;
    private readonly CraftSystem _system;
    private readonly BaseTool _tool;

    public override bool Singleton => true;

    public CraftQueueGump(Mobile from, CraftSystem system, BaseTool tool) : base(575, 40)
    {
        _from = from;
        _system = system;
        _tool = tool;
    }

    private static bool GetResourceDetails(Type resourceType, out string name, out int hue)
    {
        name = null;
        hue = 0;

        if (resourceType == null)
            return false;

        CraftResource resource = CraftResources.GetFromType(resourceType);
        if (resource == CraftResource.None ||
            resource == CraftResource.Iron ||
            resource == CraftResource.RegularWood ||
            resource == CraftResource.RegularLeather)
        {
            return false;
        }

        CraftResourceInfo info = CraftResources.GetInfo(resource);
        if (info == null)
            return false;

        name = info.Name;
        hue = info.Hue;
        return true;
    }

    protected override void BuildLayout(ref DynamicGumpBuilder builder)
    {
        var context = _system.GetContext(_from);
        if (context == null) return;

        builder.AddPage(0);

        builder.AddBackground(0, 0, 300, 437, 5054);
        builder.AddImageTiled(10, 10, 280, 22, 2624);
        builder.AddAlphaRegion(10, 10, 280, 417);

        builder.AddHtml(10, 12, 280, 20, "<CENTER><BASEFONT COLOR=#FFCC00>Crafting Queue</BASEFONT></CENTER>");

        // Bottom Action Controls
        builder.AddButton(15, 402, 4017, 4019, 0);
        builder.AddHtml(50, 405, 60, 18, "<BASEFONT COLOR=#FFFFFF>Close</BASEFONT>");

        // Start/Stop button
        if (context.IsQueueRunning)
        {
            builder.AddButton(110, 402, 4005, 4007, 1);
            builder.AddHtml(145, 405, 60, 18, "<BASEFONT COLOR=#FFFFFF>Stop</BASEFONT>");
        }
        else
        {
            builder.AddButton(110, 402, 4005, 4007, 1);
            builder.AddHtml(145, 405, 60, 18, "<BASEFONT COLOR=#FFFFFF>Start</BASEFONT>");
        }

        // Clear queue button
        builder.AddButton(205, 402, 4014, 4016, 2);
        builder.AddHtml(240, 405, 60, 18, "<BASEFONT COLOR=#FFFFFF>Clear</BASEFONT>");

        var queue = context.Queue;

        if (queue.Count == 0)
        {
            builder.AddPage(1);
            builder.AddHtml(10, 50, 280, 20, "<CENTER><BASEFONT COLOR=#808080>Queue is empty.</BASEFONT></CENTER>");
            return;
        }

        const int itemsPerPage = 7;
        int pageCount = (queue.Count + itemsPerPage - 1) / itemsPerPage;

        for (int page = 1; page <= pageCount; page++)
        {
            builder.AddPage(page);

            int startIndex = (page - 1) * itemsPerPage;
            int endIndex = Math.Min(startIndex + itemsPerPage, queue.Count);

            int y = 40;
            for (int i = startIndex; i < endIndex; i++)
            {
                var order = queue[i];

                int textColor = order.IsFinished ? GreyLabelColor : LabelColor;
                string statColor = order.IsFinished ? "#808080" : "#FFFFFF";

                int xPos = 15;

                if (GetResourceDetails(order.ResourceType, out string resName, out int resHue))
                {
                    int resourceLabelHue = order.IsFinished ? 0x384 : resHue;

                    string formattedResName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(resName);

                    builder.AddLabel(xPos, y, 1152, formattedResName);
                    xPos += (formattedResName.Length * 7) + 5;
                }

                // Line 1: Localized Cliloc Name or Custom String Name
                if (order.Item.NameNumber > 0)
                {
                    builder.AddHtmlLocalized(xPos, y, 160, 20, order.Item.NameNumber, textColor);
                }
                else
                {
                    builder.AddLabel(xPos, y, order.IsFinished ? 0x384 : 0x480, order.Item.NameString ?? "Unknown Item");
                }
                // Line 2: Stats (Crafted / Exc / Fail)
                builder.AddHtml(
                    25,
                    y + 18,
                    260,
                    20,
                    $"<BASEFONT COLOR={statColor}>({order.AmountCompleted}/{order.AmountRequested}) | Exc: {order.AmountExceptional} | Fail: {order.AmountFailed}</BASEFONT>"
                );

                y += 46;
            }

            // Page Navigation Controls
            if (page > 1)
            {
                builder.AddButton(15, 375, 4014, 4016, 0, GumpButtonType.Page, page - 1);
            }
            if (page < pageCount)
            {
                builder.AddButton(250, 375, 4005, 4007, 0, GumpButtonType.Page, page + 1);
            }
        }
    }

    public override void OnResponse(NetState sender, in RelayInfo info)
    {
        var context = _system.GetContext(_from);
        if (context == null) return;

        switch (info.ButtonID)
        {
            case 0: // Close
                break;
                case 1: // Start / Stop Toggle
                if (context.IsQueueRunning)
                {
                    // Player clicked STOP: set running flag to false
                    context.IsQueueRunning = false;
                    _from.SendMessage("Crafting queue stopped.");
                    _from.CloseGump<CraftQueueGump>();
                    _from.SendGump(new CraftQueueGump(_from, _system, _tool));
                }
                else
                {
                    // Player clicked START
                    var active = context.GetActiveOrder();
                    if (active != null)
                    {
                        var currentTool = _tool;
                        if (currentTool == null || currentTool.Deleted || currentTool.UsesRemaining <= 0)
                        {
                            currentTool = CraftItem.FindTool(_from, _system);
                        }

                        if (currentTool != null && !currentTool.Deleted && currentTool.UsesRemaining > 0)
                        {
                            context.IsQueueRunning = true; // Flag queue as actively running
                            _from.SendGump(new CraftQueueGump(_from, _system, currentTool));
                            active.Item.Craft(_from, _system, active.ResourceType, currentTool);
                        }
                        else
                        {
                            _from.SendMessage("You do not have a valid tool to start crafting.");
                            _from.CloseGump<CraftQueueGump>();
                            _from.CloseGump<CraftGump>();
                            _from.CloseGump<CraftGumpItem>();
                        }
                    }
                    else
                    {
                        _from.SendMessage("There are no pending items in your queue.");
                        _from.SendGump(new CraftQueueGump(_from, _system, _tool));
                    }
                }
                break;

            case 2: // Clear
                context.IsQueueRunning = false;
                context.Queue.Clear();
                _from.SendGump(new CraftQueueGump(_from, _system, _tool));
                break;
        }
    }
}
