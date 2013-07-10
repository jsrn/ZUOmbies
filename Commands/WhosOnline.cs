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
			int count = 0;
			foreach( NetState ns in NetState.Instances )
			{
				if( ns.Mobile != null )
				{
					string name = ns.Mobile.Name;
					if( ns.Mobile.AccessLevel > AccessLevel.Player )
					{
						name = "* " + name;
					}
					e.Mobile.SendMessage( name );
					count++;
				}	
			}
			e.Mobile.SendMessage( "Total: " + count );
		}
	}
}