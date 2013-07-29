using System;
using Server;

namespace Server.Items
{
	public class SpikeMine : BaseMine
	{
		public override int LabelNumber{ get{ return 1044601; } } // faction spike trap

		public override int AttackMessage{ get{ return 1010545; } } // Large spikes in the ground spring up piercing your skin!
		public override int DisarmMessage{ get{ return 1010541; } } // You carefully dismantle the trigger on the spikes and disable the trap.
		public override int EffectSound{ get{ return 0x22E; } }
		public override int MessageHue{ get{ return 0x5A; } }

		public override void DoVisibleEffect()
		{
			Effects.SendLocationEffect( this.Location, this.Map, 0x11A4, 12, 6 );
		}

		public override void DoAttackEffect( Mobile m )
		{
			m.Damage( Utility.RandomMinMax( 40, 60 ), m );
		}

		[Constructable]
		public SpikeMine() : this( null )
		{
		}

		public SpikeMine( Mobile m ) : base( m, 0x11A0 )
		{
			Name = "a spike trap";
		}

		public SpikeMine( Serial serial ) : base( serial )
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
		}
	}

	public class SpikeMineDeed : MineDeed
	{
		public override Type TrapType{ get{ return typeof( SpikeMine ); } }

		public SpikeMineDeed() : base( 0x11A5 )
		{
			Name = "a spike trap";
		}
		
		public SpikeMineDeed( Serial serial ) : base( serial )
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
		}
	}
}