using System;
using Server;
using Server.Mobiles; 
using Server.Scripts.Commands;
using Server.Commands;

namespace Server.Scripts.Commands
{
	public class DemoteSelf
	{
		public static void Initialize()
		{
			CommandSystem.Register( "DemoteSelf", AccessLevel.GameMaster, new CommandEventHandler( DemoteSelf_OnCommand ) );
		}

		[Usage( "DemoteSelf" )]
		[Description( "Drop your access level to player." )]
		private static void DemoteSelf_OnCommand( CommandEventArgs e )
		{
			e.Mobile.SendMessage( "You take yourself down a peg or two." );
			e.Mobile.AccessLevel = AccessLevel.Player;
		}
	}
}