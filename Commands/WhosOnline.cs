using System;
using Server;
using Server.Mobiles; 
using Server.Scripts.Commands;
using Server.Commands;

namespace Server.Scripts.Commands
{
	public class WhosOnline
	{
		public static void Initialize()
		{
			CommandSystem.Register( "WhosOnline", AccessLevel.Player, new CommandEventHandler( WhosOnline_OnCommand ) );
		}

		[Usage( "WhosOnline" )]
		[Description( "List the currently online players." )]
		private static void WhosOnline_OnCommand( CommandEventArgs e )
		{
			e.Mobile.SendMessage( "You take yourself down a peg or two." );
		}
	}
}