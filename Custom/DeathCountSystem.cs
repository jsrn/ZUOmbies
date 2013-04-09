using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using System.Collections;
using System.Collections.Generic; 

namespace Server.Commands
{
	public class DeathCountSystem
	{
		public static readonly bool PopPouchEnabled = true;
		
		public static void StartPopPouch( Mobile m )
		{
			if (PopPouchEnabled == true)
			{
				PlayerMobile pl = (PlayerMobile)m;
				m.SendMessage( "You have accumulated " + pl.getDeathPoints() + " death points." );
				return; 
			}
			else
			{
			return;
			}
		}
		
		public static void ResetDeathCount( Mobile m )
		{
			if (PopPouchEnabled == true)
			{
				m.Target = new InternalTarget();
			}
			else
			{
			return;
			}
		}
	
		private class InternalTarget : Target
		{
			public InternalTarget() :  base ( 8, false, TargetFlags.None )
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
	}
}