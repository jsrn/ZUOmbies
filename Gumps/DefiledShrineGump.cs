using System;
using Server;
using Server.Network;
using Server.Targets;
using Server.Spells;
using Server.Spells.Seventh;

namespace Server.Gumps
{
	public class DefiledRewardEntry
	{
		public static readonly DefiledRewardEntry Chicken =		new DefiledRewardEntry( 8401, 0xD0, 1015236, 15, 10 );
		public static readonly DefiledRewardEntry Dog =			new DefiledRewardEntry( 8405, 0xD9, 1015237, 17, 10 );
		public static readonly DefiledRewardEntry Wolf =		new DefiledRewardEntry( 8426, 0xE1, 1015238, 18, 10 );
		public static readonly DefiledRewardEntry Panther =		new DefiledRewardEntry( 8473, 0xD6, 1015239, 20, 14 );
		public static readonly DefiledRewardEntry Gorilla =		new DefiledRewardEntry( 8437, 0x1D, 1015240, 23, 10 );
		public static readonly DefiledRewardEntry BlackBear =	new DefiledRewardEntry( 8399, 0xD3, 1015241, 22, 10 );
		public static readonly DefiledRewardEntry GrizzlyBear =	new DefiledRewardEntry( 8411, 0xD4, 1015242, 22, 12 );
		public static readonly DefiledRewardEntry PolarBear =	new DefiledRewardEntry( 8417, 0xD5, 1015243, 26, 10 );
		public static readonly DefiledRewardEntry HumanMale =	new DefiledRewardEntry( 8397, 0x190, 1015244, 29, 8 );
		public static readonly DefiledRewardEntry HumanFemale =	new DefiledRewardEntry( 8398, 0x191, 1015254, 29, 10 );
		public static readonly DefiledRewardEntry Slime =		new DefiledRewardEntry( 8424, 0x33, 1015246, 5, 10 );
		public static readonly DefiledRewardEntry Orc =			new DefiledRewardEntry( 8416, 0x11, 1015247, 29, 10 );
		public static readonly DefiledRewardEntry LizardMan =	new DefiledRewardEntry( 8414, 0x21, 1015248, 26, 10 );
		public static readonly DefiledRewardEntry Gargoyle =	new DefiledRewardEntry( 8409, 0x04, 1015249, 22, 10 );
		public static readonly DefiledRewardEntry Ogre =		new DefiledRewardEntry( 8415, 0x01, 1015250, 24, 9 );
		public static readonly DefiledRewardEntry Troll =		new DefiledRewardEntry( 8425, 0x36, 1015251, 25, 9 );
		public static readonly DefiledRewardEntry Ettin =		new DefiledRewardEntry( 8408, 0x02, 1015252, 25, 8 );
		public static readonly DefiledRewardEntry Daemon =		new DefiledRewardEntry( 8403, 0x09, 1015253, 25, 8 );


		private int m_Art, m_Body, m_Num, m_X, m_Y;

		private DefiledRewardEntry( int Art, int Body, int LocNum, int X, int Y )
		{
			m_Art = Art;
			m_Body = Body;
			m_Num = LocNum;
			m_X = X;
			m_Y = Y;
		}

		public int ArtID { get { return m_Art; } }
		public int BodyID { get { return m_Body; } }
		public int LocNumber{ get { return m_Num; } }
		public int X{ get{ return m_X; } }
		public int Y{ get{ return m_Y; } }
	}


	public class DefiledShrineGump : Gump
	{
		private class DefiledRewardCategory
		{
			private int m_Num;
			private DefiledRewardEntry[] m_Entries;

			public DefiledRewardCategory( int num, params DefiledRewardEntry[] entries )
			{
				m_Num = num;
				m_Entries = entries;
			}

			public DefiledRewardEntry[] Entries{ get { return m_Entries; } }
			public int LocNumber{ get { return m_Num; } }
		}

		private static DefiledRewardCategory[] Categories = new DefiledRewardCategory[]
			{
				new DefiledRewardCategory( 1015235, // Animals
					DefiledRewardEntry.Chicken,
					DefiledRewardEntry.Dog,
					DefiledRewardEntry.Wolf,
					DefiledRewardEntry.Panther,
					DefiledRewardEntry.Gorilla,
					DefiledRewardEntry.BlackBear,
					DefiledRewardEntry.GrizzlyBear,
					DefiledRewardEntry.PolarBear,
					DefiledRewardEntry.HumanMale ),

				new DefiledRewardCategory( 1015245, // Monsters
					DefiledRewardEntry.Slime,
					DefiledRewardEntry.Orc,
					DefiledRewardEntry.LizardMan,
					DefiledRewardEntry.Gargoyle,
					DefiledRewardEntry.Ogre,
					DefiledRewardEntry.Troll,
					DefiledRewardEntry.Ettin,
					DefiledRewardEntry.Daemon,
					DefiledRewardEntry.HumanFemale )
			};


		private Mobile m_Caster;
		private Item m_Scroll;

		public DefiledShrineGump( Mobile caster, Item scroll ) : base( 50, 50 )
		{
			m_Caster = caster;
			m_Scroll = scroll;

			int x,y;
			AddPage( 0 );
			AddBackground( 0, 0, 585, 393, 5054 );
			AddBackground( 195, 36, 387, 275, 3000 );
			AddHtmlLocalized( 0, 0, 510, 18, 1015234, false, false ); // <center>Polymorph Selection Menu</center>
			AddHtmlLocalized( 60, 355, 150, 18, 1011036, false, false ); // OKAY
			AddButton( 25, 355, 4005, 4007, 1, GumpButtonType.Reply, 1 );
			AddHtmlLocalized( 320, 355, 150, 18, 1011012, false, false ); // CANCEL
			AddButton( 285, 355, 4005, 4007, 0, GumpButtonType.Reply, 2 );

			y = 35;
			for ( int i=0;i<Categories.Length;i++ )
			{
				DefiledRewardCategory cat = (DefiledRewardCategory)Categories[i];
				AddHtmlLocalized( 5, y, 150, 25, cat.LocNumber, true, false );
				AddButton( 155, y, 4005, 4007, 0, GumpButtonType.Page, i+1 );
				y += 25;
			}

			for ( int i=0;i<Categories.Length;i++ )
			{
				DefiledRewardCategory cat = (DefiledRewardCategory)Categories[i];
				AddPage( i+1 );

				for ( int c=0;c<cat.Entries.Length;c++ )
				{
					DefiledRewardEntry entry = (DefiledRewardEntry)cat.Entries[c];
					x = 198 + (c%3)*129;
					y = 38 + (c/3)*67;

					AddHtmlLocalized( x, y, 100, 18, entry.LocNumber, false, false );
					AddItem( x+20, y+25, entry.ArtID );
					AddRadio( x, y+20, 210, 211, false, (c<<8) + i );
				}
			}
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			if ( info.ButtonID == 1 && info.Switches.Length > 0 )
			{
				int cnum = info.Switches[0];
				int cat = cnum%256;
				int ent = cnum>>8;

				if ( cat >= 0 && cat < Categories.Length )
				{
					if ( ent >= 0 && ent < Categories[cat].Entries.Length )
					{
						Spell spell = new PolymorphSpell( m_Caster, m_Scroll, Categories[cat].Entries[ent].BodyID );
						spell.Cast();
					}
				}
			}
		}
	}

	public class NewDefiledShrineGump : Gump
	{
		private static readonly DefiledRewardEntry[] m_Entries = new DefiledRewardEntry[]
			{
				DefiledRewardEntry.Chicken,
				DefiledRewardEntry.Dog,
				DefiledRewardEntry.Wolf,
				DefiledRewardEntry.Panther,
				DefiledRewardEntry.Gorilla,
				DefiledRewardEntry.BlackBear,
				DefiledRewardEntry.GrizzlyBear,
				DefiledRewardEntry.PolarBear,
				DefiledRewardEntry.HumanMale,
				DefiledRewardEntry.HumanFemale,
				DefiledRewardEntry.Slime,
				DefiledRewardEntry.Orc,
				DefiledRewardEntry.LizardMan,
				DefiledRewardEntry.Gargoyle,
				DefiledRewardEntry.Ogre,
				DefiledRewardEntry.Troll,
				DefiledRewardEntry.Ettin,
				DefiledRewardEntry.Daemon
			};

		private Mobile m_Caster;
		private Item m_Scroll;

		public NewDefiledShrineGump( Mobile caster, Item scroll ) : base( 0, 0 )
		{
			m_Caster = caster;
			m_Scroll = scroll;

			AddPage( 0 );

			AddBackground( 0, 0, 520, 404, 0x13BE );
			AddImageTiled( 10, 10, 500, 20, 0xA40 );
			AddImageTiled( 10, 40, 500, 324, 0xA40 );
			AddImageTiled( 10, 374, 500, 20, 0xA40 );
			AddAlphaRegion( 10, 10, 500, 384 );

			AddHtmlLocalized( 14, 12, 500, 20, 1015234, 0x7FFF, false, false ); // <center>Polymorph Selection Menu</center>

			AddButton( 10, 374, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 45, 376, 450, 20, 1060051, 0x7FFF, false, false ); // CANCEL

			for ( int i = 0; i < m_Entries.Length; i++ )
			{
				DefiledRewardEntry entry = m_Entries[i];

				int page = i / 10 + 1;
				int pos = i % 10;

				if ( pos == 0 )
				{
					if ( page > 1 )
					{
						AddButton( 400, 374, 0xFA5, 0xFA7, 0, GumpButtonType.Page, page );
						AddHtmlLocalized( 440, 376, 60, 20, 1043353, 0x7FFF, false, false ); // Next
					}

					AddPage( page );

					if ( page > 1 )
					{
						AddButton( 300, 374, 0xFAE, 0xFB0, 0, GumpButtonType.Page, 1 );
						AddHtmlLocalized( 340, 376, 60, 20, 1011393, 0x7FFF, false, false ); // Back
					}
				}

				int x = ( pos % 2 == 0 ) ? 14 : 264;
				int y = ( pos / 2 ) * 64 + 44;

				AddImageTiledButton( x, y, 0x918, 0x919, i + 1, GumpButtonType.Reply, 0, entry.ArtID, 0x0, entry.X, entry.Y );
				AddHtmlLocalized( x + 84, y, 250, 60, entry.LocNumber, 0x7FFF, false, false );
			}
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			int idx = info.ButtonID - 1;

			if ( idx < 0 || idx >= m_Entries.Length )
				return;

			Spell spell = new PolymorphSpell( m_Caster, m_Scroll, m_Entries[idx].BodyID );
			spell.Cast();
		}
	}
}