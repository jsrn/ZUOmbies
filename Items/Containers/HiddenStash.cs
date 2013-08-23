using System;
using Server;
using Server.Network;

namespace Server.Items
{
	public abstract class HiddenStash : BaseContainer
	{
		private DateTime m_TimeOfPlacement;
		private Timer m_Concealing;

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime TimeOfPlacement
		{
			get{ return m_TimeOfPlacement; }
			set{ m_TimeOfPlacement = value; }
		}

		public virtual TimeSpan ConcealPeriod
		{
			get{ return TimeSpan.FromSeconds( 5.0 ); }
		}

		public virtual TimeSpan DecayPeriod
		{
			get { return TimeSpan.FromDays( 1.0 ); }
		}
 
		public virtual int IsValidLocation()
		{
			return IsValidLocation( GetWorldLocation(), Map );
		}

		public virtual int IsValidLocation( Point3D p, Map m )
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

			bool mined = false;

			foreach ( Item entity in eable )
			{
				if ( Math.Abs( location.Z - entity.Z ) <= 16 )
				{
					if ( entity is HiddenStash )
					{
						mined = true;
						break;
					}
				}
			}

			eable.Free();

			return mined;
		}

		public HiddenStash() : base( 0xE76 )
		{
			Visible = false;
			Movable = false;
			LiftOverride = true;
			m_TimeOfPlacement = DateTime.Now;
		}

		public HiddenStash( Serial serial ) : base( serial )
		{
		}

		public virtual bool CheckDecay()
		{
			TimeSpan decayPeriod = DecayPeriod;

			if ( decayPeriod == TimeSpan.MaxValue )
				return false;

			if ( (m_TimeOfPlacement + decayPeriod) < DateTime.Now )
			{
				Timer.DelayCall( TimeSpan.Zero, new TimerCallback( Delete ) );
				return true;
			}

			return false;
		}

		public virtual void BeginConceal()
		{
			if ( m_Concealing != null )
				m_Concealing.Stop();

			m_Concealing = Timer.DelayCall( ConcealPeriod, new TimerCallback( Conceal ) );
		}

		public virtual void Conceal()
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

			writer.Write( (DateTime) m_TimeOfPlacement );

			if ( Visible )
				BeginConceal();
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			m_TimeOfPlacement = reader.ReadDateTime();

			if ( Visible )
				BeginConceal();

			CheckDecay();
		}
	}
}