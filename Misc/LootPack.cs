using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server
{
	public class LootPack
	{
		public static int GetLuckChance( Mobile killer, Mobile victim )
		{
			if ( !Core.AOS )
				return 0;

			int luck = killer.Luck;

			PlayerMobile pmKiller = killer as PlayerMobile;
			if( pmKiller != null && pmKiller.SentHonorContext != null && pmKiller.SentHonorContext.Target == victim )
				luck += pmKiller.SentHonorContext.PerfectionLuckBonus;

			if ( luck < 0 )
				return 0;

			if ( !Core.SE && luck > 1200 )
				luck = 1200;

			return (int)(Math.Pow( luck, 1 / 1.8 ) * 100);
		}

		public static int GetLuckChanceForKiller( Mobile dead )
		{
			List<DamageStore> list = BaseCreature.GetLootingRights( dead.DamageEntries, dead.HitsMax );

			DamageStore highest = null;

			for ( int i = 0; i < list.Count; ++i )
			{
				DamageStore ds = list[i];

				if ( ds.m_HasRight && (highest == null || ds.m_Damage > highest.m_Damage) )
					highest = ds;
			}

			if ( highest == null )
				return 0;

			return GetLuckChance( highest.m_Mobile, dead );
		}

		public static bool CheckLuck( int chance )
		{
			return ( chance > Utility.Random( 10000 ) );
		}

		private LootPackEntry[] m_Entries;

		public LootPack( LootPackEntry[] entries )
		{
			m_Entries = entries;
		}

		public void Generate( Mobile from, Container cont, bool spawning, int luckChance )
		{
			if ( cont == null )
				return;

			bool checkLuck = Core.AOS;

			for ( int i = 0; i < m_Entries.Length; ++i )
			{
				LootPackEntry entry = m_Entries[i];

				bool shouldAdd = ( entry.Chance > Utility.Random( 10000 ) );

				if ( !shouldAdd && checkLuck )
				{
					checkLuck = false;

					if ( LootPack.CheckLuck( luckChance ) )
						shouldAdd = ( entry.Chance > Utility.Random( 10000 ) );
				}

				if ( !shouldAdd )
					continue;

				Item item = entry.Construct( from, luckChance, spawning );

				if ( item != null )
				{
					if ( !item.Stackable || !cont.TryDropItem( from, item, false ) )
						cont.DropItem( item );
				}
			}
		}

		public static readonly LootPackItem[] Gold = new LootPackItem[]
			{
				new LootPackItem( typeof( Gold ), 1 )
			};

		public static readonly LootPackItem[] Instruments = new LootPackItem[]
			{
				new LootPackItem( typeof( BaseInstrument ), 1 )
			};


		public static readonly LootPackItem[] LowScrollItems = new LootPackItem[]
			{
				new LootPackItem( typeof( ClumsyScroll ), 1 )
			};

		public static readonly LootPackItem[] MedScrollItems = new LootPackItem[]
			{
				new LootPackItem( typeof( ArchCureScroll ), 1 )
			};

		public static readonly LootPackItem[] HighScrollItems = new LootPackItem[]
			{
				new LootPackItem( typeof( SummonAirElementalScroll ), 1 )
			};

		public static readonly LootPackItem[] GemItems = new LootPackItem[]
			{
				new LootPackItem( typeof( Amber ), 1 )
			};

		public static readonly LootPackItem[] PotionItems = new LootPackItem[]
			{
				new LootPackItem( typeof( AgilityPotion ), 1 ),
				new LootPackItem( typeof( StrengthPotion ), 1 ),
				new LootPackItem( typeof( RefreshPotion ), 1 ),
				new LootPackItem( typeof( LesserCurePotion ), 1 ),
				new LootPackItem( typeof( LesserHealPotion ), 1 ),
				new LootPackItem( typeof( LesserPoisonPotion ), 1 )
			};

		#region Old Magic Items
		public static readonly LootPackItem[] OldMagicItems = new LootPackItem[]
			{
				new LootPackItem( typeof( BaseJewel ), 1 ),
				new LootPackItem( typeof( BaseArmor ), 4 ),
				new LootPackItem( typeof( BaseWeapon ), 3 ),
				new LootPackItem( typeof( BaseRanged ), 1 ),
				new LootPackItem( typeof( BaseShield ), 1 )
			};
		#endregion

		#region AOS Magic Items

		public static readonly LootPackItem[] AosMagicItemsAverageType1 = new LootPackItem[] {};

		public static readonly LootPackItem[] AosMagicItemsAverageType2 = new LootPackItem[] {};

		public static readonly LootPackItem[] AosMagicItemsRichType1 = new LootPackItem[] {};

		public static readonly LootPackItem[] AosMagicItemsRichType2 = new LootPackItem[] {};

		public static readonly LootPackItem[] AosMagicItemsFilthyRichType1 = new LootPackItem[] {};

		public static readonly LootPackItem[] AosMagicItemsFilthyRichType2 = new LootPackItem[] {};

		public static readonly LootPackItem[] AosMagicItemsUltraRich = new LootPackItem[] {};
		#endregion

		#region Pre-AOS definitions
		public static readonly LootPack OldPoor = new LootPack( new LootPackEntry[]
			{
				new LootPackEntry(  true, Gold,			1.00, "1d25" ),
				new LootPackEntry( false, Instruments,	  0.02, 1 )
			} );

		public static readonly LootPack OldMeager = new LootPack( new LootPackEntry[]
			{
				new LootPackEntry(  true, Gold,			2.00, "5d10+25" ),
				new LootPackEntry( false, Instruments,	  0.10, 1 )
			} );

		public static readonly LootPack OldAverage = new LootPack( new LootPackEntry[]
			{
				new LootPackEntry(  true, Gold,			3.00, "10d10+50" ),
				new LootPackEntry( false, Instruments,	  0.40, 1 )
			} );

		public static readonly LootPack OldRich = new LootPack( new LootPackEntry[]
			{
				new LootPackEntry(  true, Gold,			4.00, "10d10+250" ),
				new LootPackEntry( false, Instruments,	  1.00, 1 )
			} );

		public static readonly LootPack OldFilthyRich = new LootPack( new LootPackEntry[]
			{
				new LootPackEntry(  true, Gold,			5.00, "2d125+400" ),
				new LootPackEntry( false, Instruments,	  2.00, 1 )
			} );

		public static readonly LootPack OldUltraRich = new LootPack( new LootPackEntry[]
			{
				new LootPackEntry(  true, Gold,			6.00, "5d100+500" ),
				new LootPackEntry( false, Instruments,	  2.00, 1 )
			} );

		public static readonly LootPack OldSuperBoss = new LootPack( new LootPackEntry[]
			{
				new LootPackEntry(  true, Gold,			7.00, "5d100+500" ),
				new LootPackEntry( false, Instruments,	  2.00, 1 )
			} );
		#endregion

		#region Generic accessors
		public static LootPack Poor{ get{ return OldPoor; } }
		public static LootPack Meager{ get{ return OldMeager; } }
		public static LootPack Average{ get{ return OldAverage; } }
		public static LootPack Rich{ get{ return OldRich; } }
		public static LootPack FilthyRich{ get{ return OldFilthyRich; } }
		public static LootPack UltraRich{ get{ return OldUltraRich; } }
		public static LootPack SuperBoss{ get{ return OldSuperBoss; } }
		#endregion

		public static readonly LootPack LowScrolls = new LootPack( new LootPackEntry[]
			{
				new LootPackEntry( false, LowScrollItems,	100.00, 1 )
			} );

		public static readonly LootPack MedScrolls = new LootPack( new LootPackEntry[]
			{
				new LootPackEntry( false, MedScrollItems,	100.00, 1 )
			} );

		public static readonly LootPack HighScrolls = new LootPack( new LootPackEntry[]
			{
				new LootPackEntry( false, HighScrollItems,	100.00, 1 )
			} );

		public static readonly LootPack Gems = new LootPack( new LootPackEntry[]
			{
				new LootPackEntry( false, GemItems,			100.00, 1 )
			} );

		public static readonly LootPack Potions = new LootPack( new LootPackEntry[]
			{
				new LootPackEntry( false, PotionItems,		100.00, 1 )
			} );
	}

	public class LootPackEntry
	{
		private int m_Chance;
		private LootPackDice m_Quantity;

		private int m_MaxProps, m_MinIntensity, m_MaxIntensity;

		private bool m_AtSpawnTime;

		private LootPackItem[] m_Items;

		public int Chance
		{
			get{ return m_Chance; }
			set{ m_Chance = value; }
		}

		public LootPackDice Quantity
		{
			get{ return m_Quantity; }
			set{ m_Quantity = value; }
		}

		public int MaxProps
		{
			get{ return m_MaxProps; }
			set{ m_MaxProps = value; }
		}

		public int MinIntensity
		{
			get{ return m_MinIntensity; }
			set{ m_MinIntensity = value; }
		}

		public int MaxIntensity
		{
			get{ return m_MaxIntensity; }
			set{ m_MaxIntensity = value; }
		}

		public LootPackItem[] Items
		{
			get{ return m_Items; }
			set{ m_Items = value; }
		}

		private static bool IsInTokuno( Mobile m )
		{
			if ( m.Region.IsPartOf( "Fan Dancer's Dojo" ) )
				return true;

			if ( m.Region.IsPartOf( "Yomotsu Mines" ) )
				return true;

			return ( m.Map == Map.Tokuno );
		}

		public Item Construct( Mobile from, int luckChance, bool spawning )
		{
			if ( m_AtSpawnTime != spawning )
				return null;

			int totalChance = 0;

			for ( int i = 0; i < m_Items.Length; ++i )
				totalChance += m_Items[i].Chance;

			int rnd = Utility.Random( totalChance );

			for ( int i = 0; i < m_Items.Length; ++i )
			{
				LootPackItem item = m_Items[i];

				if ( rnd < item.Chance )
					return Mutate( from, luckChance, item.Construct( IsInTokuno( from ) ) );

				rnd -= item.Chance;
			}

			return null;
		}

		private int GetRandomOldBonus()
		{
			int rnd = Utility.RandomMinMax( m_MinIntensity, m_MaxIntensity );

			if ( 50 > rnd )
				return 1;
			else
				rnd -= 50;

			if ( 25 > rnd )
				return 2;
			else
				rnd -= 25;

			if ( 14 > rnd )
				return 3;
			else
				rnd -= 14;

			if ( 8 > rnd )
				return 4;

			return 5;
		}

		public Item Mutate( Mobile from, int luckChance, Item item )
		{
			if ( item != null )
			{
				if ( item is BaseWeapon && 1 > Utility.Random( 100 ) )
				{
					item.Delete();
					item = new FireHorn();
					return item;
				}

				if ( item.Stackable )
					item.Amount = m_Quantity.Roll();
			}

			return item;
		}

		public LootPackEntry( bool atSpawnTime, LootPackItem[] items, double chance, string quantity ) : this( atSpawnTime, items, chance, new LootPackDice( quantity ), 0, 0, 0 )
		{
		}

		public LootPackEntry( bool atSpawnTime, LootPackItem[] items, double chance, int quantity ) : this( atSpawnTime, items, chance, new LootPackDice( 0, 0, quantity ), 0, 0, 0 )
		{
		}

		public LootPackEntry( bool atSpawnTime, LootPackItem[] items, double chance, string quantity, int maxProps, int minIntensity, int maxIntensity ) : this( atSpawnTime, items, chance, new LootPackDice( quantity ), maxProps, minIntensity, maxIntensity )
		{
		}

		public LootPackEntry( bool atSpawnTime, LootPackItem[] items, double chance, int quantity, int maxProps, int minIntensity, int maxIntensity ) : this( atSpawnTime, items, chance, new LootPackDice( 0, 0, quantity ), maxProps, minIntensity, maxIntensity )
		{
		}

		public LootPackEntry( bool atSpawnTime, LootPackItem[] items, double chance, LootPackDice quantity, int maxProps, int minIntensity, int maxIntensity )
		{
			m_AtSpawnTime = atSpawnTime;
			m_Items = items;
			m_Chance = (int)(100 * chance);
			m_Quantity = quantity;
			m_MaxProps = maxProps;
			m_MinIntensity = minIntensity;
			m_MaxIntensity = maxIntensity;
		}

		public int GetBonusProperties()
		{
			int p0=0, p1=0, p2=0, p3=0, p4=0, p5=0;

			switch ( m_MaxProps )
			{
				case 1: p0= 3; p1= 1; break;
				case 2: p0= 6; p1= 3; p2= 1; break;
				case 3: p0=10; p1= 6; p2= 3; p3= 1; break;
				case 4: p0=16; p1=12; p2= 6; p3= 5; p4=1; break;
				case 5: p0=30; p1=25; p2=20; p3=15; p4=9; p5=1; break;
			}

			int pc = p0+p1+p2+p3+p4+p5;

			int rnd = Utility.Random( pc );

			if ( rnd < p5 )
				return 5;
			else
				rnd -= p5;

			if ( rnd < p4 )
				return 4;
			else
				rnd -= p4;

			if ( rnd < p3 )
				return 3;
			else
				rnd -= p3;

			if ( rnd < p2 )
				return 2;
			else
				rnd -= p2;

			if ( rnd < p1 )
				return 1;

			return 0;
		}
	}

	public class LootPackItem
	{
		private Type m_Type;
		private int m_Chance;

		public Type Type
		{
			get{ return m_Type; }
			set{ m_Type = value; }
		}

		public int Chance
		{
			get{ return m_Chance; }
			set{ m_Chance = value; }
		}

		private static Type[]   m_BlankTypes = new Type[]{ typeof( BlankScroll ) };
		private static Type[][] m_NecroTypes = new Type[][]
			{
				new Type[] // low
				{
					typeof( AnimateDeadScroll ),		typeof( BloodOathScroll ),		typeof( CorpseSkinScroll ),	typeof( CurseWeaponScroll ),
					typeof( EvilOmenScroll ),			typeof( HorrificBeastScroll ),	typeof( MindRotScroll ),	typeof( PainSpikeScroll ),
					typeof( SummonFamiliarScroll ),		typeof( WraithFormScroll )
				},
				new Type[] // med
				{
					typeof( LichFormScroll ),			typeof( PoisonStrikeScroll ),	typeof( StrangleScroll ),	typeof( WitherScroll )
				},

				((Core.SE) ?
				new Type[] // high
				{
					typeof( VengefulSpiritScroll ),		typeof( VampiricEmbraceScroll ), typeof( ExorcismScroll )
				} : 
				new Type[] // high
				{
					typeof( VengefulSpiritScroll ),		typeof( VampiricEmbraceScroll )
				})
			};

		public static Item RandomScroll( int index, int minCircle, int maxCircle )
		{
			--minCircle;
			--maxCircle;

			int scrollCount = ((maxCircle - minCircle) + 1) * 8;

			if ( index == 0 )
				scrollCount += m_BlankTypes.Length;

			if ( Core.AOS )
				scrollCount += m_NecroTypes[index].Length;

			int rnd = Utility.Random( scrollCount );

			if ( index == 0 && rnd < m_BlankTypes.Length )
				return Loot.Construct( m_BlankTypes );
			else if ( index == 0 )
				rnd -= m_BlankTypes.Length;

			if ( Core.AOS && rnd < m_NecroTypes.Length )
				return Loot.Construct( m_NecroTypes[index] );
			else if ( Core.AOS )
				rnd -= m_NecroTypes[index].Length;

			return Loot.RandomScroll( minCircle * 8, (maxCircle * 8) + 7, SpellbookType.Regular );
		}

		public Item Construct( bool inTokuno )
		{
			try
			{
				Item item;

				if ( m_Type == typeof( BaseRanged ) )
					item = Loot.RandomRangedWeapon( inTokuno );
				else if ( m_Type == typeof( BaseWeapon ) )
					item = Loot.RandomWeapon( inTokuno );
				else if ( m_Type == typeof( BaseArmor ) )
					item = Loot.RandomArmorOrHat( inTokuno );
				else if ( m_Type == typeof( BaseShield ) )
					item = Loot.RandomShield();
				else if ( m_Type == typeof( BaseJewel ) )
					item = Core.AOS ? Loot.RandomJewelry() : Loot.RandomArmorOrShieldOrWeapon();
				else if ( m_Type == typeof( BaseInstrument ) )
					item = Loot.RandomInstrument();
				else if ( m_Type == typeof( Amber ) ) // gem
					item = Loot.RandomGem();
				else if ( m_Type == typeof( ClumsyScroll ) ) // low scroll
					item = RandomScroll( 0, 1, 3 );
				else if ( m_Type == typeof( ArchCureScroll ) ) // med scroll
					item = RandomScroll( 1, 4, 7 );
				else if ( m_Type == typeof( SummonAirElementalScroll ) ) // high scroll
					item = RandomScroll( 2, 8, 8 );
				else
					item = Activator.CreateInstance( m_Type ) as Item;

				return item;
			}
			catch
			{
			}

			return null;
		}

		public LootPackItem( Type type, int chance )
		{
			m_Type = type;
			m_Chance = chance;
		}
	}

	public class LootPackDice
	{
		private int m_Count, m_Sides, m_Bonus;

		public int Count
		{
			get{ return m_Count; }
			set{ m_Count = value; }
		}

		public int Sides
		{
			get{ return m_Sides; }
			set{ m_Sides = value; }
		}

		public int Bonus
		{
			get{ return m_Bonus; }
			set{ m_Bonus = value; }
		}

		public int Roll()
		{
			int v = m_Bonus;

			for ( int i = 0; i < m_Count; ++i )
				v += Utility.Random( 1, m_Sides );

			return v;
		}

		public LootPackDice( string str )
		{
			int start = 0;
			int index = str.IndexOf( 'd', start );

			if ( index < start )
				return;

			m_Count = Utility.ToInt32( str.Substring( start, index-start ) );

			bool negative;

			start = index + 1;
			index = str.IndexOf( '+', start );

			if ( negative = (index < start) )
				index = str.IndexOf( '-', start );

			if ( index < start )
				index = str.Length;

			m_Sides = Utility.ToInt32( str.Substring( start, index-start ) );

			if ( index == str.Length )
				return;

			start = index + 1;
			index = str.Length;

			m_Bonus = Utility.ToInt32( str.Substring( start, index-start ) );

			if ( negative )
				m_Bonus *= -1;
		}

		public LootPackDice( int count, int sides, int bonus )
		{
			m_Count = count;
			m_Sides = sides;
			m_Bonus = bonus;
		}
	}
}