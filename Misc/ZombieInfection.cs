using System;
using Server;
using Server.Mobiles;

namespace Server.Misc
{
	public class ZombieInfection
	{

		public static bool WasBitten( PlayerMobile pm, Mobile zombie )
		{
			double infectionChance;

			if( zombie is Zombie )
				infectionChance = 0.017;
			else if ( zombie is ZombieBrute )
				infectionChance = 0.02;
			else
				infectionChance = 0.0;

			infectionChance -= GetArmourBiteProtection( pm );

			return Utility.RandomDouble() < infectionChance;
		}

		private static double GetArmourBiteProtection( PlayerMobile pm )
		{

			return -100.0;
		}
	}
}