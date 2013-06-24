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
			{
				from.SendMessage( "You cannot do that in your current state." );
				return;
			}
				
			if ( !from.InRange( this.GetWorldLocation(), 2 ) )
			{
				from.SendMessage( "You must be closer to the shrine to pray." );
				return;
			}
				
			from.SendMessage( "You pray at the defiled shrine." );
			from.CloseGump( typeof( DefiledShrineGump ) );
			from.SendGump( new DefiledShrineGump( from ) );
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