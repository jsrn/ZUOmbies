using System;

namespace Server.Items
{
	public class Clue : Item
	{
		private double m_RequiredForensicEval;
		private string m_SuccessMessage;
		private string m_FailureMessage;
		private string m_Name;

		[Constructable]
		public Clue() : base( 0x1422 )
		{
			Weight = 1.0;
		}

		public Clue( Serial serial ) : base( serial )
		{
		}

		public void AttemptToSolve( PlayerMobile from )
		{
			from.CheckSkill( SkillName.Forensics, target, m_RequiredForensicEval - 15, m_RequiredForensicEval + 15 );
			from.SendMessage( "jeepers!" );
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