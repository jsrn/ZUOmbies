using Server.Mobiles;

namespace Server.Misc
{
	public class Bullying
	{
		public static bool CanShove( PlayerMobile bully, Mobile shoved )
		{
			if ( !(shoved is BaseCreature) )
				return false;

			if ( shoved is Bird
			|| shoved is TropicalBird
			|| shoved is Cat
			|| shoved is Rabbit
			|| shoved is Squirrel
			|| shoved is Rat
			|| shoved is Snake
			 )
			{
				return true;
			}

			return false;
		}
	}
}