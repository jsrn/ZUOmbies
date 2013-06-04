using System;
using Server.Items;

namespace Server.Engines.Craft
{
	public class DefTailoring : CraftSystem
	{
		public override SkillName MainSkill
		{
			get	{ return SkillName.Tailoring; }
		}

		public override int GumpTitleNumber
		{
			get { return 1044005; } // <CENTER>TAILORING MENU</CENTER>
		}

		private static CraftSystem m_CraftSystem;

		public static CraftSystem CraftSystem
		{
			get
			{
				if ( m_CraftSystem == null )
					m_CraftSystem = new DefTailoring();

				return m_CraftSystem;
			}
		}

		public override CraftECA ECA{ get{ return CraftECA.ChanceMinusSixtyToFourtyFive; } }

		public override double GetChanceAtMin( CraftItem item )
		{
			return 0.5; // 50%
		}

		private DefTailoring() : base( 1, 1, 1.25 )// base( 1, 1, 4.5 )
		{
		}

		public override int CanCraft( Mobile from, BaseTool tool, Type itemType )
		{
			if( tool == null || tool.Deleted || tool.UsesRemaining < 0 )
				return 1044038; // You have worn out your tool!
			else if ( !BaseTool.CheckAccessible( tool, from ) )
				return 1044263; // The tool must be on your person to use.

			return 0;
		}

		public static bool IsNonColorable(Type type)
		{
			for (int i = 0; i < m_TailorNonColorables.Length; ++i)
			{
				if (m_TailorNonColorables[i] == type)
				{
					return true;
				}
			}

			return false;
		}

		private static Type[] m_TailorNonColorables = new Type[]
			{
				typeof( OrcHelm )
			};

		private static Type[] m_TailorColorables = new Type[]
			{
				typeof( GozaMatEastDeed ), typeof( GozaMatSouthDeed ),
				typeof( SquareGozaMatEastDeed ), typeof( SquareGozaMatSouthDeed ),
				typeof( BrocadeGozaMatEastDeed ), typeof( BrocadeGozaMatSouthDeed ),
				typeof( BrocadeSquareGozaMatEastDeed ), typeof( BrocadeSquareGozaMatSouthDeed )
			};

		public override bool RetainsColorFrom( CraftItem item, Type type )
		{
			if ( type != typeof( Cloth ) && type != typeof( UncutCloth ) )
				return false;

			type = item.ItemType;

			bool contains = false;

			for ( int i = 0; !contains && i < m_TailorColorables.Length; ++i )
				contains = ( m_TailorColorables[i] == type );

			return contains;
		}

		public override void PlayCraftEffect( Mobile from )
		{
			from.PlaySound( 0x248 );
		}

		public override int PlayEndingEffect( Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, bool makersMark, CraftItem item )
		{
			if ( toolBroken )
				from.SendLocalizedMessage( 1044038 ); // You have worn out your tool

			if ( failed )
			{
				if ( lostMaterial )
					return 1044043; // You failed to create the item, and some of your materials are lost.
				else
					return 1044157; // You failed to create the item, but no materials were lost.
			}
			else
			{
				if ( quality == 0 )
					return 502785; // You were barely able to make this item.  It's quality is below average.
				else if ( makersMark && quality == 2 )
					return 1044156; // You create an exceptional quality item and affix your maker's mark.
				else if ( quality == 2 )
					return 1044155; // You create an exceptional quality item.
				else				
					return 1044154; // You create the item.
			}
		}

		public override void InitCraftList()
		{
			#region Hats
			AddCraft( typeof( SkullCap ), 1011375, 1025444, 0.0, 25.0, typeof( Cloth ), 1044286, 2, 1044287 );
			AddCraft( typeof( Bandana ), 1011375, 1025440, 0.0, 25.0, typeof( Cloth ), 1044286, 2, 1044287 );
			AddCraft( typeof( FloppyHat ), 1011375, 1025907, 6.2, 31.2, typeof( Cloth ), 1044286, 11, 1044287 );
			AddCraft( typeof( Cap ), 1011375, 1025909, 6.2, 31.2, typeof( Cloth ), 1044286, 11, 1044287 );
			AddCraft( typeof( WideBrimHat ), 1011375, 1025908, 6.2, 31.2, typeof( Cloth ), 1044286, 12, 1044287 );
			AddCraft( typeof( StrawHat ), 1011375, 1025911, 6.2, 31.2, typeof( Cloth ), 1044286, 10, 1044287 );
			AddCraft( typeof( TallStrawHat ), 1011375, 1025910, 6.7, 31.7, typeof( Cloth ), 1044286, 13, 1044287 );
			AddCraft( typeof( WizardsHat ), 1011375, 1025912, 7.2, 32.2, typeof( Cloth ), 1044286, 15, 1044287 );
			AddCraft( typeof( Bonnet ), 1011375, 1025913, 6.2, 31.2, typeof( Cloth ), 1044286, 11, 1044287 );
			AddCraft( typeof( FeatheredHat ), 1011375, 1025914, 6.2, 31.2, typeof( Cloth ), 1044286, 12, 1044287 );
			AddCraft( typeof( TricorneHat ), 1011375, 1025915, 6.2, 31.2, typeof( Cloth ), 1044286, 12, 1044287 );
			AddCraft( typeof( JesterHat ), 1011375, 1025916, 7.2, 32.2, typeof( Cloth ), 1044286, 15, 1044287 );
			AddCraft( typeof( FlowerGarland ), 1011375, 1028965, 10.0, 35.0, typeof( Cloth ), 1044286, 5, 1044287 );
			AddCraft( typeof( ClothHood ), 1011375, "cloth hood", 80.0, 105.0, typeof( Cloth ), 1044286, 13, 1044287 );
			#endregion

			#region Shirts
			AddCraft( typeof( Doublet ), 1015269, 1028059, 0, 25.0, typeof( Cloth ), 1044286, 8, 1044287 );
			AddCraft( typeof( Shirt ), 1015269, 1025399, 20.7, 45.7, typeof( Cloth ), 1044286, 8, 1044287 );
			AddCraft( typeof( FancyShirt ), 1015269, 1027933, 24.8, 49.8, typeof( Cloth ), 1044286, 8, 1044287 );
			AddCraft( typeof( Tunic ), 1015269, 1028097, 00.0, 25.0, typeof( Cloth ), 1044286, 12, 1044287 );
			AddCraft( typeof( Surcoat ), 1015269, 1028189, 8.2, 33.2, typeof( Cloth ), 1044286, 14, 1044287 );
			AddCraft( typeof( PlainDress ), 1015269, 1027937, 12.4, 37.4, typeof( Cloth ), 1044286, 10, 1044287 );
			AddCraft( typeof( FancyDress ), 1015269, 1027935, 33.1, 58.1, typeof( Cloth ), 1044286, 12, 1044287 );
			AddCraft( typeof( Cloak ), 1015269, 1025397, 41.4, 66.4, typeof( Cloth ), 1044286, 14, 1044287 );
			AddCraft( typeof( Robe ), 1015269, 1027939, 53.9, 78.9, typeof( Cloth ), 1044286, 16, 1044287 );
			AddCraft( typeof( JesterSuit ), 1015269, 1028095, 8.2, 33.2, typeof( Cloth ), 1044286, 24, 1044287 );
			AddCraft( typeof( FurCape ), 1015269, 1028969, 35.0, 60.0, typeof( Cloth ), 1044286, 13, 1044287 );
			AddCraft( typeof( GildedDress ), 1015269, 1028973, 37.5, 62.5, typeof( Cloth ), 1044286, 16, 1044287 );
			AddCraft( typeof( FormalShirt ), 1015269, 1028975, 26.0, 51.0, typeof( Cloth ), 1044286, 16, 1044287 );
			AddCraft( typeof( Waistcoat ), 1015269, "waistcoat", 30.0, 55.0, typeof( Cloth ), 1044286, 12, 1044287 );
			#endregion

			#region Pants
			AddCraft( typeof( ShortPants ), 1015279, 1025422, 24.8, 49.8, typeof( Cloth ), 1044286, 6, 1044287 );
			AddCraft( typeof( LongPants ), 1015279, 1025433, 24.8, 49.8, typeof( Cloth ), 1044286, 8, 1044287 );
			AddCraft( typeof( Kilt ), 1015279, 1025431, 20.7, 45.7, typeof( Cloth ), 1044286, 8, 1044287 );
			AddCraft( typeof( Skirt ), 1015279, 1025398, 29.0, 54.0, typeof( Cloth ), 1044286, 10, 1044287 );
			AddCraft( typeof( FurSarong ), 1015279, 1028971, 35.0, 60.0, typeof( Cloth ), 1044286, 12, 1044287 );
			#endregion

			#region Misc
			AddCraft( typeof( BodySash ), 1015283, 1025441, 4.1, 29.1, typeof( Cloth ), 1044286, 4, 1044287 );
			AddCraft( typeof( HalfApron ), 1015283, 1025435, 20.7, 45.7, typeof( Cloth ), 1044286, 6, 1044287 );
			AddCraft( typeof( FullApron ), 1015283, 1025437, 29.0, 54.0, typeof( Cloth ), 1044286, 10, 1044287 );
			AddCraft( typeof( Quiver ), 1015283, "quiver", 65.0, 115.0, typeof( Leather ), 1044462, 28, 1044463 );
			AddCraft( typeof( OilCloth ), 1015283, 1041498, 74.6, 99.6, typeof( Cloth ), 1044286, 1, 1044287 );
			AddCraft( typeof( Pouch ), 1015283, "pouch", 0.0, 30.0, typeof( Leather ), 1044462, 5, 1044463 );
			AddCraft( typeof( Bag ), 1015283, "bag", 30.0, 60.0, typeof( Leather ), 1044462, 15, 1044463 );
			AddCraft( typeof( Backpack ), 1015283, "backpack", 60.0, 90.0, typeof( Leather ), 1044462, 20, 1044463 );
			AddCraft( typeof( Bedroll ), 1015283, "bedroll", 40.0, 80.0, typeof( Cloth ), 1044286, 20, 1044287 );
			AddCraft( typeof( GozaMatEastDeed ), 1015283, 1030404, 55.0, 80.0, typeof( Cloth ), 1044286, 25, 1044287 );
			AddCraft( typeof( GozaMatSouthDeed ), 1015283, 1030405, 55.0, 80.0, typeof( Cloth ), 1044286, 25, 1044287 );
			AddCraft( typeof( SquareGozaMatEastDeed ), 1015283, 1030407, 55.0, 80.0, typeof( Cloth ), 1044286, 25, 1044287 );
			AddCraft( typeof( SquareGozaMatSouthDeed ), 1015283, 1030406, 55.0, 80.0, typeof( Cloth ), 1044286, 25, 1044287 );
			AddCraft( typeof( BrocadeGozaMatEastDeed ), 1015283, 1030408, 55.0, 80.0, typeof( Cloth ), 1044286, 25, 1044287 );
			AddCraft( typeof( BrocadeGozaMatSouthDeed ), 1015283, 1030409, 55.0, 80.0, typeof( Cloth ), 1044286, 25, 1044287 );
			AddCraft( typeof( BrocadeSquareGozaMatEastDeed ), 1015283, 1030411, 55.0, 80.0, typeof( Cloth ), 1044286, 25, 1044287 );
			AddCraft( typeof( BrocadeSquareGozaMatSouthDeed ), 1015283, 1030410, 55.0, 80.0, typeof( Cloth ), 1044286, 25, 1044287 );
			#endregion

			#region Footwear
			AddCraft( typeof( FurBoots ), 1015288, 1028967, 50.0, 75.0, typeof( Cloth ), 1044286, 12, 1044287 );
			AddCraft( typeof( Sandals ), 1015288, 1025901, 12.4, 37.4, typeof( Leather ), 1044462, 4, 1044463 );
			AddCraft( typeof( Shoes ), 1015288, 1025904, 16.5, 41.5, typeof( Leather ), 1044462, 6, 1044463 );
			AddCraft( typeof( Boots ), 1015288, 1025899, 33.1, 58.1, typeof( Leather ), 1044462, 8, 1044463 );
			AddCraft( typeof( ThighBoots ), 1015288, 1025906, 41.4, 66.4, typeof( Leather ), 1044462, 10, 1044463 );
			#endregion

			#region Leather Armor
			AddCraft( typeof( LeatherGorget ), 1015293, 1025063, 53.9, 78.9, typeof( Leather ), 1044462, 4, 1044463 );
			AddCraft( typeof( LeatherCap ), 1015293, 1027609, 6.2, 31.2, typeof( Leather ), 1044462, 2, 1044463 );
			AddCraft( typeof( LeatherGloves ), 1015293, 1025062, 51.8, 76.8, typeof( Leather ), 1044462, 3, 1044463 );
			AddCraft( typeof( LeatherArms ), 1015293, 1025061, 53.9, 78.9, typeof( Leather ), 1044462, 4, 1044463 );
			AddCraft( typeof( LeatherLegs ), 1015293, 1025067, 66.3, 91.3, typeof( Leather ), 1044462, 10, 1044463 );
			AddCraft( typeof( LeatherChest ), 1015293, 1025068, 70.5, 95.5, typeof( Leather ), 1044462, 12, 1044463 );
			AddCraft( typeof( LeatherMempo ), 1015293, 1030181, 80.0, 105.0, typeof( Leather ), 1044462, 8, 1044463 );
			#endregion

			#region Studded Armor
			AddCraft( typeof( StuddedGorget ), 1015300, 1025078, 78.8, 103.8, typeof( Leather ), 1044462, 6, 1044463 );
			AddCraft( typeof( StuddedGloves ), 1015300, 1025077, 82.9, 107.9, typeof( Leather ), 1044462, 8, 1044463 );
			AddCraft( typeof( StuddedArms ), 1015300, 1025076, 87.1, 112.1, typeof( Leather ), 1044462, 10, 1044463 );
			AddCraft( typeof( StuddedLegs ), 1015300, 1025082, 91.2, 116.2, typeof( Leather ), 1044462, 12, 1044463 );
			AddCraft( typeof( StuddedChest ), 1015300, 1025083, 94.0, 119.0, typeof( Leather ), 1044462, 14, 1044463 );
			AddCraft( typeof( StuddedMempo ), 1015300, 1030216, 80.0, 105.0, typeof( Leather ), 1044462, 8, 1044463 );
			#endregion

			#region Female Armor
			AddCraft( typeof( LeatherShorts ), 1015306, 1027168, 62.2, 87.2, typeof( Leather ), 1044462, 8, 1044463 );
			AddCraft( typeof( LeatherSkirt ), 1015306, 1027176, 58.0, 83.0, typeof( Leather ), 1044462, 6, 1044463 );
			AddCraft( typeof( LeatherBustierArms ), 1015306, 1027178, 58.0, 83.0, typeof( Leather ), 1044462, 6, 1044463 );
			AddCraft( typeof( StuddedBustierArms ), 1015306, 1027180, 82.9, 107.9, typeof( Leather ), 1044462, 8, 1044463 );
			AddCraft( typeof( FemaleLeatherChest ), 1015306, 1027174, 62.2, 87.2, typeof( Leather ), 1044462, 8, 1044463 );
			AddCraft( typeof( FemaleStuddedChest ), 1015306, 1027170, 87.1, 112.1, typeof( Leather ), 1044462, 10, 1044463 );
			#endregion

			// Set the overridable material
			SetSubRes( typeof( Leather ), 1049150 );

			// Add every material you want the player to be able to choose from
			// This will override the overridable material
			AddSubRes( typeof( Leather ),		1049150, 00.0, 1044462, 1049311 );
			AddSubRes( typeof( SpinedLeather ),	1049151, 65.0, 1044462, 1049311 );
			AddSubRes( typeof( HornedLeather ),	1049152, 80.0, 1044462, 1049311 );
			AddSubRes( typeof( BarbedLeather ),	1049153, 99.0, 1044462, 1049311 );

			MarkOption = true;
			Repair = false;
			CanEnhance = Core.AOS;
		}
	}
}