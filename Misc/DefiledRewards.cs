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

			if ( entry is DefiledWeaponRewardEntry )
			{
				DefiledWeaponRewardEntry weaponEntry = (DefiledWeaponRewardEntry)entry;
				GiveWeapon( reward, weaponEntry.Weapon, weaponEntry.Cost, m );
			}
			else if ( entry is DefiledArmourRewardEntry )
			{
				DefiledArmourRewardEntry armourEntry = (DefiledArmourRewardEntry)entry;
				GiveArmour( reward, armourEntry.Armour, armourEntry.Cost, m );
			}
			else if ( entry is DefiledFamiliarRewardEntry )
			{
				DefiledFamiliarRewardEntry familiarEntry = (DefiledFamiliarRewardEntry)entry;
				m.SendMessage( "Picking " + reward + " from familiars");
			}
		}

		private static void GiveFamiliar( string familiar, int cost, PlayerMobile m )
		{
			m.SendMessage( "Giving familiar: " + familiar );
		}

		private static void GiveWeapon( string label, BaseWeapon weapon, int cost, PlayerMobile m )
		{
			weapon.Name = "defiled " + label;
			m.PlaceInBackpack( weapon );
		}

		private static void GiveArmour( string label, BaseArmor armour, int cost, PlayerMobile m )
		{
			armour.Name = "defiled " + label;
			armour.Hue = 1175;
			m.PlaceInBackpack( armour );
		}
	}
}