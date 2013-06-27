using System;
using Server.Misc;
using Server.Mobiles;

namespace Server.Items
{
	[FlipableAttribute( 0xE41, 0xE40 )]
	public class DefiledDonationChest : Container
	{
		public override int DefaultMaxWeight{ get{ return 0; } } // A value of 0 signals unlimited weight

		public override bool IsDecoContainer
		{
			get{ return false; }
		}

		[Constructable]
		public DefiledDonationChest() : base( 0xE41 )
		{
			Movable = false;
			Hue = 1175;
			Name = "a defiled resource chest";
		}

		public DefiledDonationChest( Serial serial ) : base( serial )
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

		public override bool OnDragDrop( Mobile from, Item dropped )
		{
			if ( !base.OnDragDrop( from, dropped ) )
				return false;

			DefiledRewards.TradeInItem( dropped, (PlayerMobile)from );

			return true;
		}

		public override bool OnDragDropInto( Mobile from, Item item, Point3D p )
		{
			if ( !base.OnDragDropInto( from, item, p ) )
				return false;

			DefiledRewards.TradeInItem( item, (PlayerMobile)from );

			return true;
		}
	}
}