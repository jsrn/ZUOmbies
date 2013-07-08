using System;
using Server;
using Server.Mobiles; 
using Server.Scripts.Commands;
using Server.Commands;
using Server.Network;

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
			e.Mobile.SendMessage( "The following players are online:" );
			foreach( NetState ns in NetState.Instances )
			{
				if( ns.Mobile != null )
					e.Mobile.SendMessage( ns.Mobile.Name );
			}
		}
	}
}