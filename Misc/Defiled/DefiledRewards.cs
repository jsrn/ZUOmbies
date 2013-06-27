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

		public static void TradeInItem( Item item, PlayerMobile from )
		{
			int points = 0;

			if( item is BaseWeapon || item is BaseArmor )
				points = 1;
			//I was thinking 3pts per dead body part, that you can pick up from a chopped up humanoid
			else if ( item is BonePile || item is LeftArm || item is LeftLeg || item is RibCage || item is RightArm || item is RightLeg || item is Torso )
				points = 3;
			//Hoagie: 5 for the head
			else if ( item is Head )
				points = 5;

			string message = "You contribute to the war effort";

			if( points == 0 )
				message += ", but the Guardian is not impressed.";
			else
				message += ". [ " + points + " ]";

			from.SendMessage( message );

			GrantPoints( from, points );
			item.Delete();
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

/*

thomasvane: I'd say we should figure out the value of the weapons first, based on req skill and resources needed.
thomasvane: Then we can assign to value of resources, and know we're not making them more valuable than the weapons.
thomasvane: Since deer are the primary hunted game for the town, you get 3 pts for killing great harts, 5 for hinds since they're more vital to breeding
thomasvane: Killing livestock grants 10 for chickens, 20 for sheep and goats, and 30 for cows and bulls if we add them later
thomasvane: Killing guard dogs grants 20, killing militia ( [add militiafighter ) grants 50
thomasvane: Downing a player is 100 and perma killing one is 400

*/