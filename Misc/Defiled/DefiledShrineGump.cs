using System;
using Server;
using Server.Network;
using Server.Targets;
using Server.Spells;
using Server.Spells.Seventh;
using Server.Mobiles;
using Server.Misc;
using Server.Items;

namespace Server.Gumps
{
	public class DefiledRewardEntry
	{
		private int m_Art, m_Cost;
		private string m_Label;
		private Type m_Type;

		public DefiledRewardEntry( int Art, string Label, int Cost, Type Type )
		{
			m_Art = Art;
			m_Label = Label;
			m_Cost = Cost;
			m_Type = Type;
		}

		public int ArtID { get { return m_Art; } }
		public string Label { get { return m_Label; } }
		public int Cost { get { return m_Cost; } }
		public Type Type { get { return m_Type; } }
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
				// skeletons and skeleton archers should be 50, at one pet slot
				new DefiledRewardEntry( 8423, "skeleton", 50, typeof( Skeleton ) ),
				new DefiledRewardEntry( 8423, "bone archer", 50, typeof( SkeletonArcher ) ),
				// Ghouls are 100 at two pet slots.
				new DefiledRewardEntry( 8423, "ghoul", 100, typeof( Ghoul ) ),
				new DefiledRewardEntry( 8423, "wraith", 50, typeof( Wraith ) ),
				// Bone knights are 300 at 3 pet slots
				new DefiledRewardEntry( 8423, "bone knight", 300, typeof( BoneKnight ) ),
				// specters are 500 at 4 pet slots
				new DefiledRewardEntry( 8423, "spectre", 500, typeof( Spectre ) ),
				new DefiledRewardEntry( 8423, "shade", 50, typeof( Shade ) ),
				new DefiledRewardEntry( 8405, "dire wolf", 50, typeof( DireWolf ) ) ),

			new DefiledRewardCategory( "Weapons",
				new DefiledRewardEntry( 3937, "longsword", 50, typeof( DefiledLongsword ) ),
				new DefiledRewardEntry( 3932, "mace", 50, typeof( DefiledMace ) ),
				new DefiledRewardEntry( 5042, "bow", 50, typeof( DefiledBow ) ) ),

			new DefiledRewardCategory( "Armour",
				new DefiledRewardEntry( 5139, "gorget", 50, typeof( PlateGorget ) ),
				new DefiledRewardEntry( 5143, "plate arms", 50, typeof( PlateArms ) ),
				new DefiledRewardEntry( 5141, "plate tunic", 100, typeof( PlateChest ) ),
				new DefiledRewardEntry( 5146, "plate legs", 100, typeof( PlateLegs ) ),
				new DefiledRewardEntry( 5055, "chain tunic", 50, typeof( ChainChest ) ),
				new DefiledRewardEntry( 5054, "chain legs", 50, typeof( ChainLegs ) ) ),

			new DefiledRewardCategory( "Spells",
				new DefiledRewardEntry( 8787, "spellbook", 50, typeof( NecromancerSpellbook ) ) ),

			new DefiledRewardCategory( "Items",
				new DefiledRewardEntry( 8787, "corpser seed", 50, typeof( CorpserSeed ) ),
				// Reaper seeds are 750 and take no slots, but don't obay the "owner" beyond their basic code to not attack undead.
				new DefiledRewardEntry( 8787, "reaper seed", 750, typeof( ReaperSeed ) ) )
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
			y += 5;
			AddHtml( 5, y, 150, 25, "Balance: " + ((PlayerMobile)from).EvilPoints, true, false );
			y += 30;
			int points = 30 - ((PlayerMobile)from).InjuryPoints;
			AddHtml( 5, y, 150, 25, "Favour: " + points + "/30", true, false );
			y += 25;
			AddHtml( 5, y, 150, 25, "Worship", true, false );
			AddButton( 155, y, 4005, 4007, 2, GumpButtonType.Reply, 3 );

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
			PlayerMobile pm = (PlayerMobile)m_From;
			if ( info.ButtonID == 1 && info.Switches.Length > 0 ) // Okay
			{
				int cnum = info.Switches[0];
				int cat = cnum%256;
				int ent = cnum>>8;

				if ( cat >= 0 && cat < Categories.Length )
				{
					if ( ent >= 0 && ent < Categories[cat].Entries.Length )
					{
						DefiledRewardEntry entry = Categories[cat].Entries[ent];

						if( pm.EvilPoints >= entry.Cost )
						{
							DefiledRewards.GiveReward( entry, pm );
						}
						else
						{
							pm.SendMessage( "You must earn more of the guardian's favour." );
						}
					}
				}
			}
			else if ( info.ButtonID == 2 ) // Worshio
			{
				if ( pm.InjuryPoints == 0 )
				{
					pm.SendMessage( "You worship the Guardian." );
				}
				else if( pm.EvilPoints < 10 )
				{
					pm.SendMessage( "You worship the Guardian, but he cares little for you." );
				}
				else
				{
					pm.SendMessage( "You beseech The Guardian for the power to continue." );
					while( pm.EvilPoints >= 10 && pm.InjuryPoints > 0 )
					{
						pm.EvilPoints -= 10;
						pm.InjuryPoints -= 1;
					}
				}
			} 
		}
	}
}