using System;
using Server;
using Server.Mobiles; 
using Server.Gumps; 
using Server.Targeting;
using Server.Scripts.Commands;
using Server.Items;
using Server.Commands;

namespace Server.Scripts.Commands
{
	public class DeathCountCommand
	{
		public static void Initialize()
		{
			CommandSystem.Register( "DeathCount", AccessLevel.Player, new CommandEventHandler( DeathCount_OnCommand ) );
			CommandSystem.Register( "ResetDeathCount", AccessLevel.GameMaster, new CommandEventHandler( ResetDeathCount_OnCommand ) );
			CommandSystem.Register( "Permakill", AccessLevel.GameMaster, new CommandEventHandler( Permakill_OnCommand ) );

		}

		[Usage( "DeathCount" )]
		[Description( "Tells the player how many death points they have." )]
		private static void DeathCount_OnCommand( CommandEventArgs e )
		{
			DeathCountSystem.StartPopPouch( e.Mobile );
		}
		
		[Usage( "ResetDeathCount" )]
		[Description( "Sets the targetted player's death count to 0." )]
		private static void ResetDeathCount_OnCommand( CommandEventArgs e )
		{
			DeathCountSystem.ResetDeathCount( e.Mobile );
		}
		
		[Usage( "Permakill" )]
		[Description( "Permakill the targetted mobile." )]
		private static void Permakill_OnCommand( CommandEventArgs e )
		{
			DeathCountSystem.Permakill( e.Mobile );
		}
	}
}
