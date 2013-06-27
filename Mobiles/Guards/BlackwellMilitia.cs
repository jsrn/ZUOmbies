using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
	public class BlackwellMilitia : BaseCreature
	{
		[Constructable]
		public BlackwellMilitia() : this( 200 )
		{
		}

		[Constructable]
		public BlackwellMilitia( int budget ) : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			if ( budget < 200 )
				Delete();

			Title = ", Blackwell Militia";

			SpeechHue = Utility.RandomDyedHue();

			Hue = Utility.RandomSkinHue();

			Female = false;
			Body = 0x190;
			Name = NameList.RandomName( "male" );

			Utility.AssignRandomHair( this );
			Utility.AssignRandomFacialHair( this, HairHue );

			BuySkills( budget );
		}

		private void BuySkills( int balance )
		{
			int swords = 50;
			int anatomy = 50;
			int tactics = 50;
			int parry = 50;
			int archery = 50;

			// 100 on skills
			for( int i = 0; i < 10; i++ )
			{
				switch( Utility.Random( 5 ) )
				{
					case 0:
						swords += 10;
						break;
					case 1:
						anatomy += 10;
						break;
					case 2:
						tactics += 10;
						break;
					case 3:
						parry += 10;
						break;
					case 4:
						archery += 10;
						break;
				}

				balance -= 10;
			}

			SetSkill( SkillName.Swords, 	Math.Min( swords, 100 	) );
			SetSkill( SkillName.Anatomy, 	Math.Min( anatomy, 100 	) );
			SetSkill( SkillName.Tactics, 	Math.Min( tactics, 100 	) );
			SetSkill( SkillName.Parry, 		Math.Min( parry, 100 	) );
			SetSkill( SkillName.Archery, 	Math.Min( archery, 100 	) );

			// 50 on stats
			int str = 50;
			int dex = 50;
			int intel = 10;

			for( int i = 0; i < 5; i++ )
			{
				switch( Utility.Random( 3 ) )
				{
					case 0:
						str += 10;
						break;
					case 1:
						dex += 10;
						break;
					case 2:
						intel += 10;
						break;
				}

				balance -= 10;
			}

			InitStats( str, dex, intel );

			BuyEquipment( balance );
		}

		private void BuyEquipment( int balance )
		{
			// At least 50 on gear
			double swords = Skills[ SkillName.Swords ].Value;
			double archery = Skills[ SkillName.Archery ].Value;

			if ( swords >= archery )
			{
				AddItem( new Broadsword() );
				
				if( Utility.RandomBool() )
				{
					AddItem( new WoodenShield() );
					balance -= 10;
				}	
			}
			else
			{
				AddItem( new Bow() );
				PackItem( new Arrow( Utility.RandomMinMax( 20, 40 ) ) );
			}
			balance -= 20;

			// At least 20
			AddItem( new LeatherLegs() );
			balance -= 20;

			// 10
			if ( balance >= 15 )
			{
				AddItem( new LeatherChest() );
				balance -= 15;
			}
				
			if ( balance >= 15 )
			{
				AddItem( new LeatherCap() );
				balance -= 15;
			}
				
			if ( balance >= 15 )
			{
				AddItem( new LeatherGloves() );
				balance -= 15;
			}
				
			if ( balance >= 15 )
			{
				AddItem( new LeatherArms() );
				balance -= 15;
			}
				
			if ( balance >= 15 )
			{
				AddItem( new LeatherGorget() );
				balance -= 15;
			}
				
			// Free base clothing
			AddItem( new Boots() );
			AddItem( new Shirt( 2406 ) );

			if( balance != 0 )
			{
				PackItem( new Gold( balance ) );
			}
		}

		public override bool ClickTitle { get { return false; } }

		public override bool IsEnemy( Mobile m )
		{
			if ( m.Player && ((PlayerMobile)m).Undead )
				return true;

			if ( m.Player || m is BaseVendor )
				return false;

			if ( m is BaseCreature )
			{
				BaseCreature bc = (BaseCreature)m;

				Mobile master = bc.GetMaster();
				if( master != null )
					return IsEnemy( master );
			}

			return m.Karma < 0;
		}

		public BlackwellMilitia( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}