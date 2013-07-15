using System;
using Server;

namespace Server.Items
{
	public class TrapRemovalKit : Item
	{
		private int m_Charges;

		[CommandProperty( AccessLevel.GameMaster )]
		public int Charges
		{
			get{ return m_Charges; }
			set{ m_Charges = value; }
		}

		[Constructable]
		public TrapRemovalKit() : base( 7867 )
		{
			m_Charges = 25;
			Name = "trap removal kit";
		}

		public void ConsumeCharge( Mobile consumer )
		{
			--m_Charges;

			if ( m_Charges <= 0 )
			{
				Delete();

				if ( consumer != null )
					consumer.SendLocalizedMessage( 1042531 ); // You have used all of the parts in your trap removal kit.
			}
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, Name );
			LabelTo( from, "Charges: " + m_Charges );
		}

		public TrapRemovalKit( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

			writer.WriteEncodedInt( (int) m_Charges );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{
					m_Charges = reader.ReadEncodedInt();
					break;
				}
				case 0:
				{
					m_Charges = 25;
					break;
				}
			}
		}
	}
}