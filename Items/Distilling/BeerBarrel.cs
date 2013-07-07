using System;

namespace Server.Items
{
	public class BeerBarrel : Container
	{
		public override bool IsDecoContainer
		{
			get{ return false; }
		}

		[Constructable]
		public BeerBarrel() : base( 0xE77 )
		{
			Name = "a beer barrel";
			Movable = false;
		}

		public BeerBarrel( Serial serial ) : base( serial )
		{
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{
			if ( !base.OnDragDrop( from, dropped ) )
				return false;

			MutateFermentable( from, dropped );

			return true;
		}

		public override bool OnDragDropInto( Mobile from, Item item, Point3D p )
		{
			if ( !base.OnDragDropInto( from, item, p ) )
				return false;

			MutateFermentable( from, item );

			return true;
		}

		private void MutateFermentable( Mobile from, Item item )
		{
			if ( item is SackFlour )
			{
				item.Delete();
				for ( int i = 0; i < 5; i++ )
				{
					Item beverage = new BeverageBottle( BeverageType.Ale );
					AddItem( beverage );
				}
				from.SendMessage( "You add the materials to the barrel, and retrieve some of the older stock." );
			}
			else
			{
				from.SendMessage( "That cannot be used to make anything." );
			}
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