using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Engines.Harvest
{
	public class TreasureHunting : HarvestSystem
	{
		private static TreasureHunting m_System;

		public static TreasureHunting System
		{
			get
			{
				if ( m_System == null )
					m_System = new TreasureHunting();

				return m_System;
			}
		}

		private HarvestDefinition m_treasuresAndTrinkets;

		public HarvestDefinition TreasuresAndTrinkets
		{
			get{ return m_treasuresAndTrinkets; }
		}

		private TreasureHunting()
		{
			HarvestResource[] res;
			HarvestVein[] veins;

			#region TreasureHunting for treasures and trinkets
			HarvestDefinition treasuresAndTrinkets = m_treasuresAndTrinkets = new HarvestDefinition();

			// Resource banks are every 8x8 tiles
			treasuresAndTrinkets.BankWidth = 8;
			treasuresAndTrinkets.BankHeight = 8;

			// Every bank holds from 10 to 34 ore
			treasuresAndTrinkets.MinTotal = 0;
			treasuresAndTrinkets.MaxTotal = 2;

			// A resource bank will respawn its content every 1 hour
			treasuresAndTrinkets.MinRespawn = TimeSpan.FromHours( 1.0 );
			treasuresAndTrinkets.MaxRespawn = TimeSpan.FromHours( 1.0 );

			// Skill checking is done on the Mining skill
			treasuresAndTrinkets.Skill = SkillName.Mining;

			// Set the list of harvestable tiles
			treasuresAndTrinkets.Tiles = m_grassAndDirtTiles;

			// Players must be within 2 tiles to harvest
			treasuresAndTrinkets.MaxRange = 2;

			// One ore per harvest action
			treasuresAndTrinkets.ConsumedPerHarvest = 1;
			treasuresAndTrinkets.ConsumedPerFeluccaHarvest = 1;

			// The digging effect
			treasuresAndTrinkets.EffectActions = new int[]{ 11 };
			treasuresAndTrinkets.EffectSounds = new int[]{ 0x125, 0x126 };
			treasuresAndTrinkets.EffectCounts = new int[]{ 1 };
			treasuresAndTrinkets.EffectDelay = TimeSpan.FromSeconds( 1.6 );
			treasuresAndTrinkets.EffectSoundDelay = TimeSpan.FromSeconds( 0.9 );

			treasuresAndTrinkets.NoResourcesMessage = 503040; // There is no metal here to mine.
			treasuresAndTrinkets.DoubleHarvestMessage = 503042; // Someone has gotten to the metal before you.
			treasuresAndTrinkets.TimedOutOfRangeMessage = 503041; // You have moved too far away to continue mining.
			treasuresAndTrinkets.OutOfRangeMessage = 500446; // That is too far away.
			treasuresAndTrinkets.FailMessage = 503043; // You loosen some rocks but fail to find any useable ore.
			treasuresAndTrinkets.PackFullMessage = 1010481; // Your backpack is full, so the ore you mined is lost.
			treasuresAndTrinkets.ToolBrokeMessage = 1044038; // You have worn out your tool!

			res = new HarvestResource[]
				{
					new HarvestResource( 00.0, 00.0, 100.0, "You found a coin!", typeof( Gold ),			typeof( Gold ) )
				};

			veins = new HarvestVein[]
				{
					new HarvestVein( 100.6, 0.0, res[0], null   ) // Gold
				};

			treasuresAndTrinkets.Resources = res;
			treasuresAndTrinkets.Veins = veins;

			treasuresAndTrinkets.BonusResources = new BonusHarvestResource[] { };

			treasuresAndTrinkets.RandomizeVeins = true;

			Definitions.Add( treasuresAndTrinkets );
			#endregion
		}

		public override Type GetResourceType( Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, HarvestResource resource )
		{
			if ( def == m_treasuresAndTrinkets )
				return resource.Types[0];

			return base.GetResourceType( from, tool, def, map, loc, resource );
		}

		public override bool CheckHarvest( Mobile from, Item tool )
		{
			if ( !base.CheckHarvest( from, tool ) )
				return false;

			if ( from.Mounted )
			{
				from.SendLocalizedMessage( 501864 ); // You can't mine while riding.
				return false;
			}
			else if ( from.IsBodyMod && !from.Body.IsHuman )
			{
				from.SendLocalizedMessage( 501865 ); // You can't mine while polymorphed.
				return false;
			}

			return true;
		}

		public override void SendSuccessTo( Mobile from, Item item, HarvestResource resource )
		{
			if ( item is BaseGranite )
				from.SendLocalizedMessage( 1044606 ); // You carefully extract some workable stone from the ore vein!
			else
				base.SendSuccessTo( from, item, resource );
		}

		public override bool CheckHarvest( Mobile from, Item tool, HarvestDefinition def, object toHarvest )
		{
			if ( !base.CheckHarvest( from, tool, def, toHarvest ) )
				return false;

			if ( from.Mounted )
			{
				from.SendLocalizedMessage( 501864 ); // You can't mine while riding.
				return false;
			}
			else if ( from.IsBodyMod && !from.Body.IsHuman )
			{
				from.SendLocalizedMessage( 501865 ); // You can't mine while polymorphed.
				return false;
			}

			return true;
		}

		public override HarvestVein MutateVein( Mobile from, Item tool, HarvestDefinition def, HarvestBank bank, object toHarvest, HarvestVein vein )
		{
			return base.MutateVein( from, tool, def, bank, toHarvest, vein );
		}

		private static int[] m_Offsets = new int[]
			{
				-1, -1,
				-1,  0,
				-1,  1,
				 0, -1,
				 0,  1,
				 1, -1,
				 1,  0,
				 1,  1
			};

		public override void OnHarvestFinished( Mobile from, Item tool, HarvestDefinition def, HarvestVein vein, HarvestBank bank, HarvestResource resource, object harvested )
		{
		}

		public override bool BeginHarvesting( Mobile from, Item tool )
		{
			if ( !base.BeginHarvesting( from, tool ) )
				return false;

			from.SendLocalizedMessage( 503033 ); // Where do you wish to dig?
			return true;
		}

		public override void OnHarvestStarted( Mobile from, Item tool, HarvestDefinition def, object toHarvest )
		{
			base.OnHarvestStarted( from, tool, def, toHarvest );

			from.RevealingAction();
		}

		public override void OnBadHarvestTarget( Mobile from, Item tool, object toHarvest )
		{
			if ( toHarvest is LandTarget )
				from.SendLocalizedMessage( 501862 ); // You can't mine there.
			else
				from.SendLocalizedMessage( 501863 ); // You can't mine that.
		}

		#region Tile lists
		private static int[] m_grassAndDirtTiles = new int[]
			{
				// Grass and dirt
				3, 4, 5, 6, 9, 10, 11, 12, 13, 14, 15, 16, 17
				18, 19, 20, 21,

				113, 114, 115, 116, 117, 118, 119, 120, 121,

				// Sand
				22, 23, 24, 25, 26, 27, 28, 29, 30, 31,
				32, 33, 34, 35, 36, 37, 38, 39, 40, 41,
				42, 43, 44, 45, 46, 47, 48, 49, 50, 51,
				52, 53, 54, 55, 56, 57, 58, 59, 60, 61,
				62, 68, 69, 70, 71, 72, 73, 74, 75,

				286, 287, 288, 289, 290, 291, 292, 293, 294, 295,
				296, 297, 298, 299, 300, 301, 402, 424, 425, 426,
				427, 441, 442, 443, 444, 445, 446, 447, 448, 449,
				450, 451, 452, 453, 454, 455, 456, 457, 458, 459,
				460, 461, 462, 463, 464, 465, 642, 643, 644, 645,
				650, 651, 652, 653, 654, 655, 656, 657, 821, 822,
				823, 824, 825, 826, 827, 828, 833, 834, 835, 836,
				845, 846, 847, 848, 849, 850, 851, 852, 857, 858,
				859, 860, 951, 952, 953, 954, 955, 956, 957, 958,
				967, 968, 969, 970,

				1447, 1448, 1449, 1450, 1451, 1452, 1453, 1454, 1455,
				1456, 1457, 1458, 1611, 1612, 1613, 1614, 1615, 1616,
				1617, 1618, 1623, 1624, 1625, 1626, 1635, 1636, 1637,
				1638, 1639, 1640, 1641, 1642, 1647, 1648, 1649, 1650
			};
		#endregion
	}
}