using System;
using System.Collections.Generic;
using Server.Network;
using Server;
using Server.Mobiles;

namespace Server.Misc
{
	public class InjuryDecayTimer : Timer
	{
		private bool DecayWhileOffline = false;

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
			if ( DecayWhileOffline )
			{
				InjuryDecayAllPlayers();
			}
			else
			{
				InjuryDecayOnlinePlayers();
			}
		}

		private static void InjuryDecayOnlinePlayers()
		{
			foreach ( NetState state in NetState.Instances )
			{
				if ( state.Mobile != null )
					DecrementInjuryPoints( (PlayerMobile)state.Mobile );
			}
		}

		private static void InjuryDecayAllPlayers()
		{
			List<Mobile> mobs = new List<Mobile>( World.Mobiles.Values );
			
			foreach ( Mobile m in mobs )
			{
				if ( m.Player )
					DecrementInjuryPoints( (PlayerMobile)m );
			}
		}

		private static void DecrementInjuryPoints( PlayerMobile player )
		{
			if ( !player.Alive )
				return;

			if ( player.InjuryPoints == 0 )
				return;

			if ( player.Undead )
				return;

			player.InjuryPoints -= 1;
		}
	}
}