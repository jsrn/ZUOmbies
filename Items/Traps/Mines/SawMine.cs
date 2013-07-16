using System;
using Server;

namespace Server.Items
{
	public class SawMine : BaseMine
	{
		public override int LabelNumber{ get{ return 1041047; } } // faction saw trap

		public override int AttackMessage{ get{ return 1010544; } } // The blade cuts deep into your skin!
		public override int DisarmMessage{ get{ return 1010540; } } // You carefully dismantle the saw mechanism and disable the trap.
		public override int EffectSound{ get{ return 0x218; } }
		public override int MessageHue{ get{ return 0x5A; } }

		public override void DoVisibleEffect()
		{
			Effects.SendLocationEffect( this.Location, this.Map, 0x11AD, 25, 10 );
		}

		public override void DoAttackEffect( Mobile m )
		{
			m.Damage( Utility.Dice( 6, 10, 40 ), m );
		}

		[Constructable]
		public SawMine() : this( null )
		{
		}

		public SawMine( Serial serial ) : base( serial )
		{
		}

		public SawMine( Mobile m ) : base( m, 0x11AC )
		{
			Name = "a saw trap";
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

	public class SawMineDeed : MineDeed
	{
		public override Type TrapType{ get{ return typeof( SawMine ); } }

		public SawMineDeed() : base( 0x1107 )
		{
			Name = "a saw trap";
		}
		
		public SawMineDeed( Serial serial ) : base( serial )
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