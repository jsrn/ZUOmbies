using System;
using System.Collections.Generic;
using Server;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
	[FlipableAttribute( 0xE80, 0x9A8 )]
	public class StrongBox : BaseContainer, IChopable
	{
		private Mobile m_Owner;
		private BaseHouse m_House;

		public override double DefaultWeight{ get{ return 100; } }
		public override int LabelNumber { get { return 1023712; } }

		[Constructable]
		public StrongBox() : this( null, null )
		{
		}

		public StrongBox( Mobile owner, BaseHouse house ) : base( 0xE80 )
		{
			m_Owner = owner;
			MaxItems = 10;
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Owner
		{
			get
			{
				return m_Owner;
			}
			set
			{
				m_Owner = value;
				InvalidateProperties();
			}
		}

		public override int DefaultMaxWeight{ get{ return 0; } }

		public StrongBox( Serial serial ) : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( m_Owner );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_Owner = reader.ReadMobile();
					break;
				}
			}
		}

		public override bool Decays
		{
			get
			{
				if ( m_Owner != null && !m_Owner.Deleted )
					return false;
				else
					return true;
			}
		}

		public override void AddNameProperty( ObjectPropertyList list )
		{
			if ( m_Owner != null )
				list.Add( 1042887, m_Owner.Name ); // a strong box owned by ~1_OWNER_NAME~
			else
				base.AddNameProperty( list );
		}

		public override void OnSingleClick( Mobile from )
		{
			if ( m_Owner != null )
			{
				LabelTo( from, "{0}'s strongbox", m_Owner.Name );

				if ( CheckContentDisplay( from ) )
					LabelTo( from, "({0} items, {1} stones)", TotalItems, TotalWeight );
			}
			else
			{
				base.OnSingleClick( from );
			}
		}

		public override bool IsAccessibleTo( Mobile m )
		{
			if ( m_Owner == null || m_Owner.Deleted || m.AccessLevel >= AccessLevel.GameMaster )
				return true;

			return m == m_Owner && base.IsAccessibleTo( m );
		}

		private void Chop( Mobile from )
		{
			Effects.PlaySound( Location, Map, 0x3B3 );
			from.SendLocalizedMessage( 500461 ); // You destroy the item.
			Destroy();
		}

		public void OnChop( Mobile from )
		{
			if ( from == m_Owner || m_House.IsOwner( from ) )
				Chop( from );
		}

		public Container ConvertToStandardContainer()
		{
			Container metalBox = new MetalBox();
			List<Item> subItems = new List<Item>( Items );

			foreach ( Item subItem in subItems )
			{
				metalBox.AddItem( subItem );
			}

			this.Delete();

			return metalBox;
		}
	}
}
