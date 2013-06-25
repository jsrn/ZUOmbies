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
		public static void GrantPoints( PlayerMobile pm, Mobile m )
		{
			if( m is PlayerMobile )
			{
				GrantPoints( pm, 100 );

				PlayerMobile target = (PlayerMobile)m;
				if( target.InjuryPoints >= 30 )
					GrantPoints( pm, 300 );
			}
			else if( m is BaseCreature )
			{
				GrantPoints( pm, 25 );
			}
		}

		public static void GrantPoints( PlayerMobile pm, int amount )
		{
			pm.EvilPoints += amount;
		}

		public static void GiveReward( DefiledRewardEntry entry, PlayerMobile m )
		{
			string reward = entry.Label;
			Type type = entry.Type;

			if( typeof( BaseWeapon ).IsAssignableFrom( type ) )
			{	
				GiveWeapon( reward, entry.Type, entry.Cost, m );
			}
			else if( typeof( BaseArmor ).IsAssignableFrom( type ) )
			{
				GiveArmour( reward, entry.Type, entry.Cost, m );
			}
			else if( typeof( BaseCreature ).IsAssignableFrom( type ) )
			{
				GiveFamiliar( reward, entry.Type, entry.Cost, m );
			}
			else if( typeof( Item ).IsAssignableFrom( type ) )
			{
				GiveItem( reward, entry.Type, entry.Cost, m );
			}
		}

		// Hues are 2406 and 1175, depending on item

		private static void GiveFamiliar( string label, Type familiarType, int cost, PlayerMobile m )
		{
			m.SendMessage( "Giving familiar: " + label );
			BaseCreature bc = Activator.CreateInstance( familiarType ) as BaseCreature;
			bc.Name = "defiled " + label;
			BaseCreature.Summon( bc, m, m.Location, -1, TimeSpan.FromDays( 1.0 ) );
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

		private static void GiveItem( string label, Type itemType, int cost, PlayerMobile m )
		{
			Item item = Activator.CreateInstance( itemType ) as Item;
			m.PlaceInBackpack( item );
		}
	}
}