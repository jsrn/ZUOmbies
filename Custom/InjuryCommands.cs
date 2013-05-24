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
	public class InjuryCommands
	{
		public static void Initialize()
		{
			CommandSystem.Register( "HealInjuries", AccessLevel.GameMaster, new CommandEventHandler( HealInjuries_OnCommand ) );
			CommandSystem.Register( "Permakill", AccessLevel.GameMaster, new CommandEventHandler( Permakill_OnCommand ) );
		}

		[Usage( "HealInjuries" )]
		[Description( "Sets the targetted player's death count to 0." )]
		private static void HealInjuries_OnCommand( CommandEventArgs e )
		{
			e.Mobile.Target = new HealInjuriesTarget();
		}
		
		[Usage( "Permakill" )]
		[Description( "Permakill the targetted mobile." )]
		private static void Permakill_OnCommand( CommandEventArgs e )
		{
			e.Mobile.Target = new PermakillTarget();
		}
	}

	class HealInjuriesTarget : Target
	{
		public HealInjuriesTarget() :  base ( 8, false, TargetFlags.None )
		{
			AllowNonlocal = true;
		}

		protected override void OnTarget( Mobile from, object target )
		{
			if ( target is PlayerMobile)
			{
				PlayerMobile pl = (PlayerMobile)target;
				pl.resetDeathPoints();
				pl.SendMessage( "You have been given a second lease of life!" );
				PlayerMobile you = (PlayerMobile)from;
				from.SendMessage( "They have been given a second lease of life." );
			} else
			{
				from.SendMessage( "That does not need your assistance!" );
			}
		}
	}

	class PermakillTarget : Target
	{
		public PermakillTarget() :  base ( 8, false, TargetFlags.None )
		{
			AllowNonlocal = true;
		}

		protected override void OnTarget( Mobile from, object target )
		{
			if ( target is PlayerMobile)
			{
				PlayerMobile pl = (PlayerMobile)target;
				pl.PermaKill();
				pl.SendMessage( "The light fades..." );
				PlayerMobile you = (PlayerMobile)from;
				from.SendMessage( "They will not be doing that any more." );
			}
			else
			{
				from.SendMessage( "You can not permakill that!" );
			}
		}	
	}
}