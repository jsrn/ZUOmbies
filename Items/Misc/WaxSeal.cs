using System;
using Server;
using Server.Targeting;

namespace Server.Items
{
	public class WaxSeal : Item
	{
		[Constructable]
		public WaxSeal() : base( 0xF7C )
		{
			Weight = 1.0;
			Name = "a wax seal";
		}

		public WaxSeal( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042664 ); // You must have the object in your backpack to use it.
				return;
			}

			from.Target = new InternalTarget( this );
			from.SendMessage( "Choose the parchment on which you wish to apply the wax seal." );
		}

		private class InternalTarget : Target
		{
			private WaxSeal m_WaxSeal;

			public InternalTarget( WaxSeal waxSeal ) : base( 3, false, TargetFlags.None )
			{
				m_WaxSeal = waxSeal;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_WaxSeal.Deleted )
					return;

				if ( !m_WaxSeal.IsChildOf( from.Backpack ) )
				{
					from.SendLocalizedMessage( 1042664 ); // You must have the object in your backpack to use it.
					return;
				}

				Item item = targeted as Item;

				if ( item == null || !item.IsChildOf( from.Backpack ) )
				{
					from.SendLocalizedMessage( 1042664 ); // You must have the object in your backpack to use it.
				}
				else if ( !(item is BaseBook) )
				{
					from.SendMessage( "You may only apply the seal to parchments!" );
				}
				else
				{
					BaseBook book = (BaseBook)item;

					if ( !book.Writable )
					{
						book.LabelTo( from, 1061909 ); // The ink in this book has already been sealed.
					}
					else
					{
						book.Author = from.Name;
						book.Writable = false;
						from.SendMessage( "You stamp the parchment with your seal." );
					}
				}
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