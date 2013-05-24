using System;
using Server.Network;
using Server;
using Server.Mobiles;

namespace Server.Misc
{
	public class InjuryDecayTimer : Timer
	{
		public static void Initialize()
		{
			new InjuryDecayTimer().Start();
		}

		public InjuryDecayTimer() : base( TimeSpan.FromMinutes( 30.0 ), TimeSpan.FromMinutes( 30.0 ) )
		{
			Priority = TimerPriority.OneMinute;
		}

		protected override void OnTick()
		{
			InjuryDecay();			
		}

		public static void InjuryDecay()
		{
			foreach ( NetState state in NetState.Instances )
			{
				if (state.Mobile != null)
				{
					PlayerMobile m = (PlayerMobile)state.Mobile;

					if( m.InjuryPoints >= 1 )
						m.InjuryPoints -= 1;
				}
			}
		}
	}
}