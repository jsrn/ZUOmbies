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
		
		public static void ResetDeathCount( Mobile m )
		{
			m.Target = new ResetDeathPointsTarget();
		}
		
		public static void Permakill( Mobile m )
		{
			m.Target = new PermakillTarget();
		}
		
		private class PermakillTarget : Target
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
	
		private class ResetDeathPointsTarget : Target
		{
			public ResetDeathPointsTarget() :  base ( 8, false, TargetFlags.None )
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
