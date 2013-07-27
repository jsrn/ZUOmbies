using System;
using Server;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
	public class BellOfTheLiving : Item
	{
		[Constructable]
		public BellOfTheLiving() : base( 0x91A )
		{
			Hue = 0x835;
			Movable = false;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( from.InRange( GetWorldLocation(), 2 ) )
				RingBell( from );
			else
				from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
		}

		public virtual void RingBell( Mobile from )
		{
			foreach ( Mobile m in from.GetMobilesInRange( 200 ) )
			{
				if ( m.Player )
				{
					if ( m == from )
						m.SendMessage( "You ring the bell." );
					else
						m.SendMessage( "You hear the ringing of alarm bells!" );
				}
			}
		}

		public BellOfTheLiving( Serial serial ) : base( serial )
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