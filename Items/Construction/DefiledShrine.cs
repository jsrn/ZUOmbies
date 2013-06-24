using System;
using Server;
using Server.Targeting;
using Server.Gumps;

namespace Server.Items
{
	[Flipable( 0xEDB, 0xEDC )]
	public class DefiledShrine : Item
	{
		[Constructable]
		public DefiledShrine() : base( 0xEDB )
		{
			Weight = 1.0;
			Name = "a defiled shrine";
			Movable = false;
			Hue = 1762;
		}

		public DefiledShrine( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !from.Alive )
				return;

			if ( !from.InRange( this.GetWorldLocation(), 2 ) )
			{
				from.SendMessage( "You must be closer to the shrine to pray." );
				return;
			}
			else
			{
				from.SendMessage( "You pray at the defiled shrine." );
				from.CloseGump( typeof( DefiledShrineGump ) );
				from.SendGump( new DefiledShrineGump( from ) );
			}			
		}

		public override void OnDoubleClickDead( Mobile m )
		{
			if ( m.Alive )
				return;

			if ( !m.InRange( this.GetWorldLocation(), 2 ) )
				m.SendLocalizedMessage( 500446 ); // That is too far away.
			else if( m.Map != null && m.Map.CanFit( m.Location, 16, false, false ) )
			{
				m.CloseGump( typeof( ResurrectGump ) );
				m.SendGump( new ResurrectGump( m, ResurrectMessage.VirtueShrine ) );
			}
			else
				m.SendLocalizedMessage( 502391 ); // Thou can not be resurrected there!
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