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
			Hue = 0x21;
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
			from.SendLocalizedMessage( 1061907 ); // Choose a book you wish to seal with the wax from the red leaf.
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
					item.LabelTo( from, 1061911 ); // You can only use red leaves to seal the ink into book pages!
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
						book.Writable = false;
						book.Name = book.Name + " [sealed by " + from.Name + "]";
						book.LabelTo( from, 1061910 ); // You seal the ink to the page using wax from the red leaf.
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