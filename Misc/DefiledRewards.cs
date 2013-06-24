using System;
using Server;
using Server.Mobiles;
using Server.Items;
using System.Collections.Generic; // For Dictionary

namespace Server.Misc
{
	public class DefiledRewards
	{

		public static void GiveReward( string category, string reward, int cost, PlayerMobile m )
		{
			m.SendMessage( "Picking " + reward + " from " + category);

			switch( category )
			{
				case "Familiars":
					GiveFamiliar( reward, cost, m );
					break;
				case "Weapons":
					GiveWeapon( reward, cost, m );
					break;
				case "Armour":
					GiveArmour( reward, cost, m );
					break;
			}
		}

		private static void GiveFamiliar( string familiar, int cost, PlayerMobile m )
		{
			m.SendMessage( "Giving familiar: " + familiar );
		}

		private static void GiveWeapon( string weapon, int cost, PlayerMobile m )
		{
			m.SendMessage( "Giving weapon: " + weapon );
			BaseWeapon rewardWeapon = null;
			string name = "";

			switch( weapon )
			{
				case "Longsword":
					rewardWeapon = new Longsword();
					name = "longsword";
					break;
				case "Mace":
					rewardWeapon = new Mace();
					name = "mace";
					break;
			}
			rewardWeapon.Name = "defiled " + name;
			rewardWeapon.Hue = 1175;
			m.PlaceInBackpack( rewardWeapon );
		}

		private static void GiveArmour( string armour, int cost, PlayerMobile m )
		{
			m.SendMessage( "Giving armour: " + armour );
		}
	}
}