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
		private int m_RequiredInt;

		private string m_FailureMessage;
		private string m_SuccessMessage;
		private string m_GreatSuccessMessage;

		private List<PlayerMobile> m_Sleuths;

		[CommandProperty( AccessLevel.GameMaster )]
		public double RequiredSkill
		{
			get{ return m_RequiredForensicEval; }
			set{ m_RequiredForensicEval = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int RequiredInt
		{
			get{ return m_RequiredInt; }
			set{ m_RequiredInt = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string FailureMessage
		{
			get{ return m_FailureMessage; }
			set{ m_FailureMessage = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string SuccessMessage
		{
			get{ return m_SuccessMessage; }
			set{ m_SuccessMessage = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string GreatSuccessMessage
		{
			get{ return m_GreatSuccessMessage; }
			set{ m_GreatSuccessMessage = value; }
		}

		[Constructable]
		public Clue() : base( 0x1422 )
		{
			Weight = 1.0;
			Name = "a clue";

			m_RequiredForensicEval = 0;

			m_FailureMessage = "You can't seem to figure it out...";
			m_SuccessMessage = "Something fishy is going on here...";
			m_GreatSuccessMessage = "... and you know exactly what!";

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
				from.SendMessage( m_SuccessMessage );
				
				if ( from.CheckSkill( SkillName.Forensics, minChance, maxChance ) ) // Try again for exceptional
				{
					from.SendMessage( m_GreatSuccessMessage );
				}

				m_Sleuths.Add( from );
			}
			else
			{
				from.SendMessage( m_FailureMessage );
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