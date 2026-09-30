using System;
using Server;
using Server.Items;
using Server.Commands;
using Server.Mobiles;

namespace Server.Commands
{
	public class BandSelf
	{
		public static void Initialize()
		{
			CommandSystem.Register("Bandself", AccessLevel.Player, new CommandEventHandler(Bandself_OnCommand));
		}

		[Usage("Bandself")]
		[Description("Uses a bandage from the player's backpack on themselves.")]
		public static void Bandself_OnCommand(CommandEventArgs e)
		{
			Mobile from = e.Mobile;

			if (BandageContext.GetContext(from) != null)
			{
				from.SendMessage( 33, "You are already applying a bandage." );
				return;
			}
			Bandage bandage = from.Backpack.FindItemByType<Bandage>();

			if (bandage != null)
			{
				if (BandageContext.BeginHeal(from, from) != null)
				{
					bandage.Consume();
				}
			}
			else
			{
				from.SendMessage("You do not have any bandages in your backpack.");
			}
		}
	}
}
