using System;
using Server;
using Server.Network;

namespace Server.Items
{
	public class HiddenStash : BaseContainer
	{
		private Timer m_Concealing;

		public TimeSpan ConcealPeriod
		{
			get{ return TimeSpan.FromSeconds( 5.0 ); }
		}

		public int IsValidLocation()
		{
			return IsValidLocation( GetWorldLocation(), Map );
		}

		public int IsValidLocation( Point3D p, Map m )
		{
			if ( m == null )
				return 502956; // You cannot place a trap on that.

			if ( TileAlreadyContainsStash( p, m ) )
				return 502956; // You cannot place a trap on that.

			return 0;
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
}