using System;

namespace Server.Items
{
	public class CiderBarrel : Container
	{
		public override bool IsDecoContainer
		{
			get{ return false; }
		}

		[Constructable]
		public CiderBarrel() : base( 0xE77 )
		{
			Name = "a cider barrel";
			Movable = false;
		}

		public CiderBarrel( Serial serial ) : base( serial )
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
			if ( item is Apple )
			{
				item.Delete();
				for ( int i = 0; i < ((Apple)item).Amount; i++ )
				{
					Item beverage = new Jug( BeverageType.Cider ); // 0 == ale
					DropItem( beverage );
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