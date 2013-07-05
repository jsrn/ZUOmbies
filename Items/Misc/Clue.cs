using System;
using Server.Mobiles;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
	public class Clue : Item
	{
		private string m_Name;
		private double m_RequiredForensicEval;

		private string m_FailureMessage;
		private string m_SuccessMessage;
		private string m_GreatSuccessMessage;

		private List<PlayerMobile> m_Sleuths;

		[Constructable]
		public Clue() : base( 0x1422 )
		{
			Weight = 1.0;
			Name = "a clue";

			m_Sleuths = new List<PlayerMobile>();
		}

		public Clue( Serial serial ) : base( serial )
		{
		}

		public void AttemptToSolve( PlayerMobile from )
		{
			if ( m_Sleuths.Contains( from ) )
			{
				from.SendMessage( "You have already gleaned all you can from that." );
				return;
			}

			double minChance = m_RequiredForensicEval;
			double maxChance = m_RequiredForensicEval + 30;

			if ( from.CheckSkill( SkillName.Forensics, minChance, maxChance ) ) // Passed the basic
			{
				from.SendMessage( "You found a clue!" );
				
				if ( from.CheckSkill( SkillName.Forensics, minChance, maxChance ) ) // Try again for exceptional
				{
					from.SendMessage( "You found it really well!" );
				}

				m_Sleuths.Add( from );
			}
			else
			{
				from.SendMessage( "You can't seem to figure it out..." );
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}