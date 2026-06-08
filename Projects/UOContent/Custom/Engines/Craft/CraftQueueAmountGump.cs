using System;
using Server.Gumps;
using Server.Items;
using Server.Network;

namespace Server.Engines.Craft;

public class CraftQueueAmountGump : DynamicGump
{
    private readonly Mobile _from;
    private readonly CraftSystem _craftSystem;
    private readonly BaseTool _tool;
    private readonly int _index;

    public override bool Singleton => true;

    public CraftQueueAmountGump(Mobile from, CraftSystem craftSystem, BaseTool tool, int index) : base(150, 150)
    {
        _from = from;
        _craftSystem = craftSystem;
        _tool = tool;
        _index = index;
    }

    protected override void BuildLayout(ref DynamicGumpBuilder builder)
    {
        builder.AddPage();

        // Compact slate background layout box
        builder.AddBackground(0, 0, 220, 130, 5054);
        builder.AddAlphaRegion(10, 10, 200, 110);

        builder.AddLabel(25, 15, 0x480, "Enter Target Quantity:");

        // Text Input Field Border and Box
        builder.AddImageTiled(25, 42, 170, 22, 2624);
        builder.AddTextEntry(30, 42, 160, 20, 0x480, 1, "1");

        // OK Action button
        builder.AddButton(25, 80, 4005, 4007, 1);
        builder.AddLabel(60, 82, 0x480, "OK");

        // Cancel Action button
        builder.AddButton(125, 80, 4017, 4019, 0);
        builder.AddLabel(160, 82, 0x480, "Cancel");
    }

    public override void OnResponse(NetState sender, in RelayInfo info)
    {
        var queue = CraftQueueManager.GetQueue(_from);

        if (info.ButtonID == 1 && _index < queue.Count)
        {
            // FIXED: In ModernUO, this returns a raw string directly!
            var textEntry = info.GetTextEntry(1);

            // Validate that the player typed a real number greater than zero
            if (int.TryParse(textEntry, out int amount) && amount > 0)
            {
                queue[_index].TargetAmount = amount;
            }
            else
            {
                _from.SendMessage(0x22, "Invalid quantity entered. Amount must be a positive number.");
            }
        }

        // Always bounce the player back to the master workshop queue layout screen smoothly
        _from.SendGump(new CraftQueueGump(_from, _craftSystem, _tool));
    }
}
