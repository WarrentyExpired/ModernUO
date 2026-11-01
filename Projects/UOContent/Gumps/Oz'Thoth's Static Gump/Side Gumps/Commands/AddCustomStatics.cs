/*
 * Oz'Thoth's Static Tool - Customs Category Gump
 * Modernized for .NET 10 / ModernUO
 * Note: This gump is a placeholder for custom static entries to be added.
 */

using System;
using Server.Commands;

namespace Server.Gumps.OzThoth;

/// <summary>
/// Category menu for selecting custom static sub-gumps.
/// </summary>
public sealed class AddCustomGump : CategoryGumpBase
{
    // No categories defined by default - add custom entries as needed
    private static readonly CategoryEntry[] CustomCategories = [];

    protected override string Title => "Customs";
    protected override ReadOnlySpan<CategoryEntry> Categories => CustomCategories;
    protected override CategoryGumpBase CreateInstance() => new AddCustomGump();

    public static void Initialize()
    {
        CommandSystem.Register("AddCustomsStatic", AccessLevel.GameMaster, AddCustomsStatic_OnCommand);
    }

    [Usage("AddCustomsStatic")]
    [Description("Displays a menu from which you can interactively add custom statics.")]
    private static void AddCustomsStatic_OnCommand(CommandEventArgs e)
    {
        e.Mobile.CloseGump<AddCustomGump>();
        e.Mobile.SendGump(new AddCustomGump());
    }

    public AddCustomGump() : base()
    {
    }
}
