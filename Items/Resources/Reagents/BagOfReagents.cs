using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class BagOfReagents : Bag
	{
		[Constructable]
		public BagOfReagents() : this( 50 )
		{
		}

		[Constructable]
		public BagOfReagents( int amount )
		{
			DropItem( new BlackPearl   ( Utility.Random( amount ) ) );
			DropItem( new Bloodmoss    ( Utility.Random( amount ) ) );
			DropItem( new Garlic       ( Utility.Random( amount ) ) );
			DropItem( new Ginseng      ( Utility.Random( amount ) ) );
			DropItem( new MandrakeRoot ( Utility.Random( amount ) ) );
			DropItem( new Nightshade   ( Utility.Random( amount ) ) );
			DropItem( new SulfurousAsh ( Utility.Random( amount ) ) );
			DropItem( new SpidersSilk  ( Utility.Random( amount ) ) );
		}

		public BagOfReagents( Serial serial ) : base( serial )
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
