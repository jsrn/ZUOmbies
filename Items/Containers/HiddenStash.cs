using System;
using Server.Mobiles;
using Server.Targeting;
using Server;
using Server.Engines.Craft;

namespace Server.Items
{
	public class HiddenStash : BaseContainer
	{
		private Timer m_Concealing;

		public TimeSpan ConcealPeriod
		{
			get{ return TimeSpan.FromSeconds( 15.0 ); }
		}

		public bool IsValidLocation()
		{
			return IsValidLocation( GetWorldLocation(), Map );
		}

		public bool IsValidLocation( Point3D p, Map m )
		{
			if ( m == null )
				return false;

			if ( TileAlreadyContainsStash( p, m ) )
				return false;

			return true;
		}

		private bool TileAlreadyContainsStash( Point3D location, Map map )
		{
			IPooledEnumerable eable = map.GetItemsInRange( location, 0 );

			bool hasStash = false;

			foreach ( Item entity in eable )
			{
				if ( Math.Abs( location.Z - entity.Z ) <= 16 )
				{
					if ( entity is HiddenStash )
					{
						hasStash = true;
						break;
					}
				}
			}

			eable.Free();

			return hasStash;
		}

		[Constructable]
		public HiddenStash() : base( 0xE76 )
		{
			Visible = false;
			Movable = false;
			LiftOverride = true;
			Name = "a hidden stash";
		}

		public HiddenStash( Serial serial ) : base( serial )
		{
		}

		public void BeginConceal()
		{
			if ( m_Concealing != null )
				m_Concealing.Stop();

			m_Concealing = Timer.DelayCall( ConcealPeriod, new TimerCallback( Conceal ) );
		}

		public void Conceal()
		{
			if ( m_Concealing != null )
				m_Concealing.Stop();

			m_Concealing = null;

			if ( !Deleted )
				Visible = false;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			if ( Visible )
				BeginConceal();
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( Visible )
				BeginConceal();
		}
	}

	public class StashDeed : Item, ICraftable
	{
		[Constructable]
		public StashDeed() : base( 0x14F0 )
		{
			Name = "a hidden stash";
			Weight = 1.0;
		}

		public StashDeed( Serial serial ) : base( serial )
		{
		}

		public HiddenStash Construct( Mobile from )
		{
			try{ return Activator.CreateInstance( typeof ( HiddenStash ) ) as HiddenStash; }
			catch{ return null; }
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( IsChildOf( from.Backpack ) )
			{
				from.Target = new InternalTarget( this );
				from.SendMessage( "Where would you like to hide this stash?" );
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
			private StashDeed m_Deed;

			public InternalTarget( StashDeed deed ) : base( -1, true, TargetFlags.None )
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
					HiddenStash stash = m_Deed.Construct( from );

					if ( stash == null )
						return;

					if ( !stash.IsValidLocation( new Point3D( p.X, p.Y, p.Z ), from.Map ) )
					{
						from.SendMessage( "You cannot hide a stash there." );
						stash.Delete();
						return;
					}

					from.SendMessage( "You carefully conceal the stash." );
					stash.MoveToWorld( new Point3D( p.X, p.Y, p.Z ), from.Map );
					m_Deed.Delete();
				}
				else
				{
					from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
				}
			}
		}
	}
}