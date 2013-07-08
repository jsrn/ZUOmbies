using System;
using Server.Network;
using Server;
using Server.Mobiles;
using Server.Items;

namespace Server.Misc
{
	public class CheckInjuryPoints
	{
		public static void CheckInjuries( PlayerMobile injured, Mobile killer )
		{
			if ( killer is BaseCreature ) // Killer is monster/creature
			{
				BaseCreature bc = (BaseCreature)killer;
				HandleKillByBaseCreature( injured, bc );
			}
			else if ( killer is PlayerMobile && !injured.IsInFreeDeathZone ) // Killer is player, and not in zone
			{
				PlayerMobile pm = (PlayerMobile)killer;
				HandleKillByPlayer( injured, pm );
			}	
		}

		private static void HandleKillByBaseCreature( PlayerMobile injured, BaseCreature killer )
		{
			int injuryPointsGained = 0;
			if( killer.GetMaster() != null )
			{
				if( !injured.IsInFreeDeathZone )
					injuryPointsGained = 5;
			}
			else
			{
				injuryPointsGained = (int) (( killer.Fame / 24000.0 ) * 30);

				if ( injuryPointsGained < 5 )
					injuryPointsGained = 5;
			}

			injured.InjuryPoints += injuryPointsGained;
			injured.ResetDeathTime();
		}

		private static void HandleKillByPlayer( PlayerMobile injured, PlayerMobile killer )
		{
			int injuryPointsGained = 0;
			if ( !(killer.Weapon is Fists) )
			{
				injuryPointsGained = 5;
				injured.ResetDeathTime();
			}

			if( killer.Undead )
				DefiledRewards.GrantPoints( killer, injured );

			injured.InjuryPoints += injuryPointsGained;
		}
	}
}