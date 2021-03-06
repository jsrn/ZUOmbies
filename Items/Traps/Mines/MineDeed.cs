using System;
using Server.Mobiles;
using Server.Targeting;
using Server;
using Server.Engines.Craft;

namespace Server.Items
{
	public abstract class MineDeed : Item, ICraftable
	{
		public abstract Type TrapType{ get; }

		public MineDeed( int itemID ) : base( itemID )
		{
			Weight = 1.0;
		}

		public MineDeed( bool createdFromDeed ) : this( 0x14F0 )
		{
		}

		public MineDeed( Serial serial ) : base( serial )
		{
		}

		public virtual BaseMine Construct( Mobile from )
		{
			try{ return Activator.CreateInstance( TrapType, new object[]{ from } ) as BaseMine; }
			catch{ return null; }
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( IsChildOf( from.Backpack ) )
			{
				from.Target = new InternalTarget( this );
				from.SendMessage( "Where would you like to place this trap?" );
			}
			else
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
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

		#region ICraftable Members
		public int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue )
		{
			ItemID = 0x14F0;
			return 1;
		}
		#endregion

		private class InternalTarget : Target
		{
			private MineDeed m_Deed;

			public InternalTarget( MineDeed deed ) : base( -1, true, TargetFlags.None )
			{
				m_Deed = deed;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				IPoint3D p = targeted as IPoint3D;
				Map map = from.Map;

				if ( p == null || map == null || m_Deed.Deleted )
					return;

				if ( m_Deed.IsChildOf( from.Backpack ) )
				{
					BaseMine trap = m_Deed.Construct( from );

					if ( trap == null )
						return;

					int message = trap.IsValidLocation( new Point3D( p.X, p.Y, p.Z ), from.Map );

					if ( message > 0 )
					{
						from.SendLocalizedMessage( message, "", 0x23 );
						trap.Delete();
					}
					else
					{
						from.SendLocalizedMessage( 1010360 ); // You arm the trap and carefully hide it from view
						trap.MoveToWorld( new Point3D( p.X, p.Y, p.Z ), from.Map );
						m_Deed.Delete();
					}
				}
				else
				{
					from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
				}
			}
		}
	}
}