using System;
using Server;
using Server.Mobiles;
using Server.Items;
using System.Collections.Generic; // For Dictionary

namespace Server.Misc
{
	public class ZombieInfection
	{

		public static void CheckBites( PlayerMobile pm, Mobile zombie )
		{
			double infectionChance;

			if( zombie is Zombie )
				infectionChance = 0.02;
			else if ( zombie is ZombieBrute )
				infectionChance = 0.03;
			else
				infectionChance = 0.0;

			double chance = Utility.RandomDouble();

			bool wouldHaveBeenBitten = chance < infectionChance;

			infectionChance -= GetArmourBiteProtection( pm );

			bool bitten = chance < infectionChance;

			if ( bitten )
			{
				pm.SendMessage( "The zombie strikes you with a grievious blow!" );
				BroadcastBite( pm );
			}
			else if ( wouldHaveBeenBitten )
				pm.SendMessage( "The zombie strikes you with a grievious blow, but your armour protects you." );
		}

		private static void BroadcastBite( PlayerMobile pm )
		{
			foreach ( Mobile m in pm.GetMobilesInRange( 8 ) )
			{
				if ( m is PlayerMobile && pm != m )
				{
					m.SendMessage( pm.Name + " gets savagely mauled by a zombie." );
				}
			}
		}

		private static double GetArmourBiteProtection( PlayerMobile pm )
		{
			double biteProtection = 0;
			// Protection based on a fixed rate of 2% for regular zombies
			// So full armour would block all of the chance of zombie bites

			// If you're wearing a shield
			double shieldFactor = 0.3;
			if ( pm.ShieldArmor as BaseShield != null )
				biteProtection += shieldFactor;

			// If you're wearing boots
			double feetFactor = 0.1;
			Item shoes = pm.FindItemOnLayer( Layer.Shoes );
			if( shoes != null )
			{
				if( shoes is Shoes )
				{
					biteProtection += feetFactor * 0.66;
				}
				else if ( shoes is Boots || shoes is ThighBoots )
				{
					biteProtection += feetFactor;
				}
			}
			
			Dictionary<BaseArmor, double> armourRatings = new Dictionary<BaseArmor, double>();
			if ( pm.NeckArmor as BaseArmor != null )
				armourRatings.Add( pm.NeckArmor as BaseArmor, 0.3 );
			if ( pm.HeadArmor as BaseArmor != null )
				armourRatings.Add( pm.HeadArmor as BaseArmor, 0.3 );
			if ( pm.ChestArmor as BaseArmor != null )
				armourRatings.Add( pm.ChestArmor as BaseArmor, 0.2 );
			if ( pm.LegsArmor as BaseArmor != null )
				armourRatings.Add( pm.LegsArmor as BaseArmor, 0.2 );
			if ( pm.HandArmor as BaseArmor != null )
				armourRatings.Add( pm.HandArmor as BaseArmor, 0.3 );
			if ( pm.ArmsArmor as BaseArmor != null )
				armourRatings.Add( pm.ArmsArmor as BaseArmor, 0.3 );

			foreach(KeyValuePair<BaseArmor, double> entry in armourRatings)
			{
				if( entry.Key != null )
					biteProtection += GetResourceScale( entry.Key.Resource ) * entry.Value;
			}

			return biteProtection;
		}

		private static double GetResourceScale( CraftResource res )
		{
			switch ( res )
			{
				case CraftResource.Iron:			return 1;
				case CraftResource.DullCopper:		return 1;
				case CraftResource.ShadowIron:		return 1;
				case CraftResource.Copper:			return 1;
				case CraftResource.Bronze:			return 1;
				case CraftResource.Gold:			return 1;
				case CraftResource.Agapite:			return 1;
				case CraftResource.Verite:			return 1;
				case CraftResource.Valorite:		return 1;
				case CraftResource.RegularLeather:	return ( 0.66 );
				case CraftResource.SpinedLeather:	return ( 0.66 );
				case CraftResource.HornedLeather:	return ( 0.66 );
				case CraftResource.BarbedLeather:	return ( 0.66 );
				default: return 0;
			}
		}
	}
}