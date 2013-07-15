using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Regions;

namespace Server.Factions
{
	public enum AllowedPlacing
	{
		Everywhere,
		AnyFactionTown,
		ControlledFactionTown,
		FactionStronghold
	}

	public abstract class BaseFactionTrap : BaseTrap
	{
		private Faction m_Faction;
		private Mobile m_Placer;
		private DateTime m_TimeOfPlacement;

		private Timer m_Concealing;

		[CommandProperty( AccessLevel.GameMaster )]
		public Faction Faction
		{
			get{ return m_Faction; }
			set{ m_Faction = value; }
		}

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

		public virtual int SilverFromDisarm{ get{ return 100; } }

		public virtual int MessageHue{ get{ return 0; } }

		public virtual int AttackMessage{ get{ return 0; } }
		public virtual int DisarmMessage{ get{ return 0; } }

		public virtual AllowedPlacing AllowedPlacing{ get{ return AllowedPlacing.Everywhere; } }

		public virtual TimeSpan ConcealPeriod
		{
			get{ return TimeSpan.FromMinutes( 1.0 ); }
		}

		public virtual TimeSpan DecayPeriod
		{
			get { return TimeSpan.FromDays( 1.0 ); }
		}

		public override void OnTrigger( Mobile from )
		{
			if ( !IsEnemy( from ) )
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

			if ( !CheckDecay() && CheckRange( m.Location, oldLocation, 6 ) )
			{
				if ( ( m.Skills[SkillName.DetectHidden].Value - 80.0) / 20.0 > Utility.RandomDouble() )
				{
					Visible = true;
					trap.BeginConceal();
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

		public BaseFactionTrap( Faction f, Mobile m, int itemID ) : base( itemID )
		{
			Visible = false;

			m_Faction = f;
			m_TimeOfPlacement = DateTime.Now;
			m_Placer = m;
		}

		public BaseFactionTrap( Serial serial ) : base( serial )
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

			Faction.WriteReference( writer, m_Faction );
			writer.Write( (Mobile) m_Placer );
			writer.Write( (DateTime) m_TimeOfPlacement );

			if ( Visible )
				BeginConceal();
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			m_Faction = Faction.ReadReference( reader );
			m_Placer = reader.ReadMobile();
			m_TimeOfPlacement = reader.ReadDateTime();

			if ( Visible )
				BeginConceal();

			CheckDecay();
		}

		public override void OnDelete()
		{
			if ( m_Faction != null && m_Faction.Traps.Contains( this ) )
				m_Faction.Traps.Remove( this );

			base.OnDelete();
		}

		public virtual bool IsEnemy( Mobile mob )
		{
			return mob.AccessLevel == AccessLevel.Player;
		}
	}
}