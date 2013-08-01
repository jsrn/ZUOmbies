using System;
using Server;
using Server.Network;

namespace Server.Items
{
	public abstract class BaseMine : BaseTrap
	{
		private Mobile m_Placer;
		private DateTime m_TimeOfPlacement;
		private Timer m_Concealing;

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Placer
		{
			get{ return m_Placer; }
			set{ m_Placer = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime TimeOfPlacement
		{
			get{ return m_TimeOfPlacement; }
			set{ m_TimeOfPlacement = value; }
		}

		public virtual int EffectSound{ get{ return 0; } }

		public virtual int MessageHue{ get{ return 0; } }

		public virtual int AttackMessage{ get{ return 0; } }
		public virtual int DisarmMessage{ get{ return 0; } }

		public virtual TimeSpan ConcealPeriod
		{
			get{ return TimeSpan.FromSeconds( 30.0 ); }
		}

		public virtual TimeSpan DecayPeriod
		{
			get { return TimeSpan.FromDays( 1.0 ); }
		}

		public override void OnTrigger( Mobile from )
		{
			if ( !IsEnemy( from ) )
				return;

			if ( Visible )
				return;

			Conceal();

			DoVisibleEffect();
			Effects.PlaySound( this.Location, this.Map, this.EffectSound );
			DoAttackEffect( from );

			Delete();
		}

		public abstract void DoVisibleEffect();
		public abstract void DoAttackEffect( Mobile m );
        
		public virtual int IsValidLocation()
		{
			return IsValidLocation( GetWorldLocation(), Map );
		}

		public virtual int IsValidLocation( Point3D p, Map m )
		{
			if( m == null )
				return 502956; // You cannot place a trap on that.

			return 0;
		}

		public override void OnMovement( Mobile m, Point3D oldLocation )
		{
			base.OnMovement( m, oldLocation );

			int range = (int)(m.Skills[SkillName.DetectHidden].Value / 20.0);

			if ( range < 1 )
				range = 1;

			if ( !CheckDecay() && CheckRange( m.Location, oldLocation, range ) )
			{
				if ( m.Skills[SkillName.DetectHidden].Value / 2.0 > Utility.RandomMinMax( 1, 100 ) )
				{
					Visible = true;
					BeginConceal();
					PrivateOverheadLocalizedMessage( m, 1010154, MessageHue, "", "" ); // [Faction Trap]
				}
			}
		}

		public void PrivateOverheadLocalizedMessage( Mobile to, int number, int hue, string name, string args )
		{
			if ( to == null )
				return;

			NetState ns = to.NetState;

			if ( ns != null )
				ns.Send( new MessageLocalized( Serial, ItemID, MessageType.Regular, hue, 3, number, name, args ) );
		}

		public BaseMine( Mobile m, int itemID ) : base( itemID )
		{
			Visible = false;
			m_TimeOfPlacement = DateTime.Now;
			m_Placer = m;
		}

		public BaseMine( Serial serial ) : base( serial )
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

			writer.Write( (Mobile) m_Placer );
			writer.Write( (DateTime) m_TimeOfPlacement );

			if ( Visible )
				BeginConceal();
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			m_Placer = reader.ReadMobile();
			m_TimeOfPlacement = reader.ReadDateTime();

			if ( Visible )
				BeginConceal();

			CheckDecay();
		}

		public virtual bool IsEnemy( Mobile mob )
		{
			return mob.AccessLevel == AccessLevel.Player;
		}
	}
}