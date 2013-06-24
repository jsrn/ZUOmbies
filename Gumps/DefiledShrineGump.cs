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
		// Familiars
		public static readonly DefiledRewardEntry Skeleton = new DefiledRewardEntry( 8423, "skeleton [50]" );
		public static readonly DefiledRewardEntry HordeMinion = new DefiledRewardEntry( 8405, "undead hound [50]" );
		// Weapons
		public static readonly DefiledRewardEntry CursedSword = new DefiledRewardEntry( 8426, "cursed sword [50]" );
		// Armour
		public static readonly DefiledRewardEntry CursedGorget = new DefiledRewardEntry( 8426, "cursed gorget [50]" );
		public static readonly DefiledRewardEntry CursedPlateArms =	new DefiledRewardEntry( 8426, "cursed plate arms [50]" );
		public static readonly DefiledRewardEntry CursedPlateTunic = new DefiledRewardEntry( 8426, "cursed plate tunic [100]" );
		public static readonly DefiledRewardEntry CursedPlateLegs =	new DefiledRewardEntry( 8426, "cursed plate legs [100]" );

		private int m_Art, m_X, m_Y;
		private string m_Label;

		private DefiledRewardEntry( int Art, string Label )
		{
			m_Art = Art;
			m_Label = Label;
		}

		public int ArtID { get { return m_Art; } }
		public string Label { get { return m_Label; } }
	}


	public class DefiledShrineGump : Gump
	{
		private class DefiledRewardCategory
		{
			private string m_Label;
			private DefiledRewardEntry[] m_Entries;

			public DefiledRewardCategory( string label, params DefiledRewardEntry[] entries )
			{
				m_Label = label;
				m_Entries = entries;
			}

			public DefiledRewardEntry[] Entries{ get { return m_Entries; } }
			public string Label { get { return m_Label; } }
		}

		private static DefiledRewardCategory[] Categories = new DefiledRewardCategory[]
			{
				new DefiledRewardCategory( "Familiars", // Animals
					DefiledRewardEntry.Skeleton,
					DefiledRewardEntry.HordeMinion ),

				new DefiledRewardCategory( "Weapons", // Monsters
					DefiledRewardEntry.CursedSword )
			};


		private Mobile m_From;
		private Item m_Scroll;

		public DefiledShrineGump( Mobile from ) : base( 50, 50 )
		{
			m_From = from;

			int x,y;
			AddPage( 0 );
			AddBackground( 0, 0, 585, 393, 5054 );
			AddBackground( 195, 36, 387, 275, 3000 );
			AddHtml( 0, 0, 510, 18, "<center>Defiled Shrine</center>", false, false );
			AddHtmlLocalized( 60, 355, 150, 18, 1011036, false, false ); // OKAY
			AddButton( 25, 355, 4005, 4007, 1, GumpButtonType.Reply, 1 );
			AddHtmlLocalized( 320, 355, 150, 18, 1011012, false, false ); // CANCEL
			AddButton( 285, 355, 4005, 4007, 0, GumpButtonType.Reply, 2 );

			y = 35;
			for ( int i=0;i<Categories.Length;i++ )
			{
				DefiledRewardCategory cat = (DefiledRewardCategory)Categories[i];
				AddHtml( 5, y, 150, 25, cat.Label, true, false );
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

					AddHtml( x, y, 100, 18, entry.Label, false, false );
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
						m_From.SendMessage( "Picked cat " + cat + ", entry " + ent );
					}
				}
			}
		}
	}
}