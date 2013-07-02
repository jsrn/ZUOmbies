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
				int points = 100;

				PlayerMobile target = (PlayerMobile)m;
				if( target.InjuryPoints >= 30 )
					points += 300;

				if ( target.Undead )
				{
					points = 0 - points;
					points /= 2;
				}
					
				GrantPoints( pm, points );
			}
			else if ( m is GreatHart )
				GrantPoints( pm, 3 );
			else if ( m is Hind )
				GrantPoints( pm, 5 );
			else if ( m is Chicken )
				GrantPoints( pm, 10 );
			else if ( m is Sheep || m is Goat )
				GrantPoints( pm, 20 );
			else if ( m is Cow || m is Bull )
				GrantPoints( pm, 30 );
			//thomasvane: Killing guard dogs grants 20, killing militia ( [add militiafighter ) grants 50
			else if ( m is BlackwellMilitia )
				GrantPoints( pm, 50 );
			else if( m is BaseCreature )
				GrantPoints( pm, 1 );
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
			if ( BaseCreature.Summon( bc, m, m.Location, -1, TimeSpan.FromDays( 1.0 ) ) )
				m.EvilPoints -= cost;
		}

		private static void GiveWeapon( string label, Type weaponType, int cost, PlayerMobile m )
		{
			BaseWeapon weapon = Activator.CreateInstance( weaponType ) as BaseWeapon;
			DefileWeapon( weapon );
			m.PlaceInBackpack( weapon );
			m.EvilPoints -= cost;
		}

		private static void GiveArmour( string label, Type armourType, int cost, PlayerMobile m )
		{
			BaseArmor armour = Activator.CreateInstance( armourType ) as BaseArmor;
			DefileArmour( armour );
			m.PlaceInBackpack( armour );
			m.EvilPoints -= cost;
		}

		private static void GiveItem( string label, Type itemType, int cost, PlayerMobile m )
		{
			Item item = Activator.CreateInstance( itemType ) as Item;
			m.PlaceInBackpack( item );
			m.EvilPoints -= cost;
		}

		private static void DefileArmour( BaseArmor armour )
		{
			armour.Quality = ArmorQuality.Exceptional;

			if ( armour.Resource == CraftResource.RegularLeather )
				armour.Resource = CraftResource.BarbedLeather;
			else if ( armour.Resource == CraftResource.Iron )
				armour.Resource = CraftResource.Valorite;

			armour.LootType = LootType.Cursed;
			armour.Hue = 1175;
		}

		private static void DefileWeapon( BaseWeapon weapon )
		{
			weapon.Quality = WeaponQuality.Exceptional;
			weapon.LootType = LootType.Cursed;

			if( weapon.Resource == CraftResource.Iron )
				weapon.Resource = CraftResource.Valorite;

			weapon.Hue = 0;
		}
	}
}

/*
thomasvane: I'd say we should figure out the value of the weapons first, based on req skill and resources needed.
thomasvane: Then we can assign to value of resources, and know we're not making them more valuable than the weapons.
*/