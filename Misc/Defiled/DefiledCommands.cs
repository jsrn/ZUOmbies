using System;
using Server;
using Server.Mobiles; 
using Server.Scripts.Commands;
using Server.Commands;
using Server.Targeting;

namespace Server.Scripts.Commands
{
	public class DefiledCommands
	{
		public static void Initialize()
		{
			CommandSystem.Register( "MakeZombie", AccessLevel.GameMaster, new CommandEventHandler( MakeZombie_OnCommand ) );
			CommandSystem.Register( "MakeLich", AccessLevel.GameMaster, new CommandEventHandler( MakeLich_OnCommand ) );
			CommandSystem.Register( "ResetForm", AccessLevel.GameMaster, new CommandEventHandler( ResetForm_OnCommand ) );
		}

		[Usage( "MakeZombie" )]
		[Description( "Turn the targetted player into a zombie." )]
		private static void MakeZombie_OnCommand( CommandEventArgs e )
		{
			e.Mobile.Target = new ChangeFormTarget( 3 );
		}

		[Usage( "MakeLich" )]
		[Description( "Turn the targetted player into a lich." )]
		private static void MakeLich_OnCommand( CommandEventArgs e )
		{
			e.Mobile.Target = new ChangeFormTarget( 0x18 );
		}

		[Usage( "ResetForm" )]
		[Description( "Change them back." )]
		private static void ResetForm_OnCommand( CommandEventArgs e )
		{
			e.Mobile.Target = new ChangeFormTarget( 0 );
		}

		class ChangeFormTarget : Target
		{
			private int m_NewForm;

			public ChangeFormTarget( int NewForm ) :  base ( 8, false, TargetFlags.None )
			{
				AllowNonlocal = true;
				m_NewForm = NewForm;
			}

			protected override void OnTarget( Mobile from, object target )
			{
				if ( target is PlayerMobile )
				{
					PlayerMobile pl = (PlayerMobile)target;
					pl.BodyMod = m_NewForm;
					pl.SendMessage( "You take a different form." );
				}
				else
				{
					from.SendMessage( "You can not change that." );
				}
			}	
		}
	}
}