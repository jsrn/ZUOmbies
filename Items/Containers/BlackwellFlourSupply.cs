using System;
using System.Collections;
using System.Collections.Generic;
using Server.Multis;

namespace Server.Items
{
	public class BlackwellFlourSupply : Container
	{
		private Timer m_Timer;

		[Constructable]
		public BlackwellFlourSupply() : base( 0xE3D )
		{
			Movable = false;
			LiftOverride = true;
			Name = "Blackwell flour supply";

			m_Timer = new RefillTimer( this );
			m_Timer.Start();
		}

		public BlackwellFlourSupply( Serial serial ) : base( serial )
		{
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

			m_Timer = new RefillTimer( this );
			m_Timer.Start();
		}

		public void Refill()
		{
			int currentcount = this.Items.Count;
			int toAdd = Utility.RandomMinMax( 10, 100 );

			while( currentcount < toAdd )
			{
				SackFlour newSack = Activator.CreateInstance( typeof( SackFlour ) ) as SackFlour;
				DropItem( newSack );
				currentcount++;
			}
		}

		private class RefillTimer : Timer
		{
			private BlackwellFlourSupply m_Supply;

			public RefillTimer( BlackwellFlourSupply supply ) : base( TimeSpan.Zero, TimeSpan.FromDays( 7.0 ) )
			{
				m_Supply = supply;
				Priority = TimerPriority.OneMinute;
			}

			protected override void OnTick()
			{
				m_Supply.Refill();
			}
		}
	}
}