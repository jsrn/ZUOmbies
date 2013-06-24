using System;
using Server;
using Server.Network;
using Server.Targets;
using Server.Spells;
using Server.Spells.Seventh;
using Server.Mobiles;
using Server.Misc;

namespace Server.Gumps
{
	public class DefiledRewardEntry
	{
		// Familiars
		public static readonly DefiledRewardEntry Skeleton = new DefiledRewardEntry( 8423, "Skeleton", 50 );
		public static readonly DefiledRewardEntry UndeadHound = new DefiledRewardEntry( 8405, "Hellhound", 50 );
		// Weapons
		public static readonly DefiledRewardEntry Longsword = new DefiledRewardEntry( 3937, "Longsword", 50 );
		public static readonly DefiledRewardEntry Mace = new DefiledRewardEntry( 3932, "Mace", 50 );
		// Armour
		public static readonly DefiledRewardEntry Gorget = new DefiledRewardEntry( 5139, "Gorget", 50 );
		public static readonly DefiledRewardEntry PlateArms =	new DefiledRewardEntry( 5143, "Plate arms", 50 );
		public static readonly DefiledRewardEntry PlateTunic = new DefiledRewardEntry( 5141, "Plate tunic", 100 );
		public static readonly DefiledRewardEntry PlateLegs =	new DefiledRewardEntry( 5146, "Plate legs", 100 );

		public static readonly DefiledRewardEntry ChainTunic = new DefiledRewardEntry( 5055, "Chain tunic", 50 );
		public static readonly DefiledRewardEntry ChainLeggings = new DefiledRewardEntry( 5054, "Chain legs", 50 );

		private int m_Art, m_X, m_Y, m_Cost;
		private string m_Label;

		private DefiledRewardEntry( int Art, string Label, int Cost )
		{
			m_Art = Art;
			m_Label = Label;
			m_Cost = Cost;
		}

		public int ArtID { get { return m_Art; } }
		public string Label { get { return m_Label; } }
		public int Cost { get { return m_Cost; } }
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
				new DefiledRewardCategory( "Familiars",
					DefiledRewardEntry.Skeleton,
					DefiledRewardEntry.UndeadHound ),

				new DefiledRewardCategory( "Weapons",
					DefiledRewardEntry.Longsword,
					DefiledRewardEntry.Mace ),

				new DefiledRewardCategory( "Armour",
					DefiledRewardEntry.Gorget,
					DefiledRewardEntry.PlateArms,
					DefiledRewardEntry.PlateTunic,
					DefiledRewardEntry.PlateLegs,
					DefiledRewardEntry.ChainTunic,
					DefiledRewardEntry.ChainLeggings )
			};


		private Mobile m_From;

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

					AddHtml( x, y, 110, 18, entry.Label + " [" + entry.Cost + "]", false, false );
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
						DefiledRewardEntry entry = Categories[cat].Entries[ent];
						string label = entry.Label;
						string category = Categories[cat].Label;
						int cost = entry.Cost;
						DefiledRewards.GiveReward( category, label, cost, (PlayerMobile)m_From );
					}
				}
			}
		}
	}
}

/*

thomasvane: But yeah, back to the list. Cheapo option, skells/skell archers, then ghouls, then corpser seeds, then bone knights, then shades, then reaper seeds.
Hoagie: Awesome stuff
thomasvane: Ghouls are like wraiths/specters/shades, but not see through and have no magery.

thomasvane: Maybe buying the scolls in the first place costs points.
thomasvane: Hell, maybe even the initial book costs points.
thomasvane: They have to prove they're worthy of His gifts.
2406 and 1175


*/