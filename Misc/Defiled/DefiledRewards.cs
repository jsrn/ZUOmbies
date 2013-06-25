using System;
using Server;
using Server.Mobiles;
using Server.Items;
using System.Collections.Generic; // For Dictionary
using Server.Gumps;

namespace Server.Misc
{
	public class DefiledRewards
	{

		public static void GiveReward( DefiledRewardEntry entry, PlayerMobile m )
		{
			string reward = entry.Label;
			Type type = entry.Type;

			if( typeof( BaseWeapon ).IsAssignableFrom( type ) )
			{	
				m.SendMessage( "Giving base weapon" );
				GiveWeapon( reward, entry.Type, entry.Cost, m );
			}
			else if( typeof( BaseArmor ).IsAssignableFrom( type ) )
			{
				m.SendMessage( "Giving base armour" );
				GiveArmour( reward, entry.Type, entry.Cost, m );
			}
			else if( typeof( BaseCreature ).IsAssignableFrom( type ) )
			{
				m.SendMessage( "Giving base familiar" );
				GiveFamiliar( reward, entry.Type, entry.Cost, m );
			}
			else if( typeof( Item ).IsAssignableFrom( type ) )
			{
				// 
			}
		}

		private static void GiveFamiliar( string label, Type familiarType, int cost, PlayerMobile m )
		{
			m.SendMessage( "Giving familiar: " + label );
		}

		private static void GiveWeapon( string label, Type weaponType, int cost, PlayerMobile m )
		{
			BaseWeapon weapon = Activator.CreateInstance( weaponType ) as BaseWeapon;
			weapon.Name = "defiled " + label;
			m.PlaceInBackpack( weapon );
		}

		private static void GiveArmour( string label, Type armourType, int cost, PlayerMobile m )
		{
			BaseArmor armour = Activator.CreateInstance( armourType ) as BaseArmor;
			armour.Name = "defiled " + label;
			armour.Hue = 1175;
			m.PlaceInBackpack( armour );
		}
	}
}