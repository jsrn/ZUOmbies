using System;
using Server;

namespace Server.Items
{
	public class GasMine : BaseMine
	{
		public override int AttackMessage{ get{ return 1010542; } } // A noxious green cloud of poison gas envelops you!
		public override int DisarmMessage{ get{ return 502376; } } // The poison leaks harmlessly away due to your deft touch.
		public override int EffectSound{ get{ return 0x230; } }
		public override int MessageHue{ get{ return 0x44; } }

		public override void DoVisibleEffect()
		{
			Effects.SendLocationEffect( this.Location, this.Map, 0x3709, 28, 10, 0x1D3, 5 );
		}

		public override void DoAttackEffect( Mobile m )
		{
			m.ApplyPoison( m, Poison.Lethal );
		}

		[Constructable]
		public GasMine() : this( null )
		{
		}

		public GasMine( Mobile m ) : base( m, 0x113C )
		{
			Name = "a gas mine";
		}

		public GasMine( Serial serial ) : base( serial )
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

	public class GasMineDeed : MineDeed
	{
		public override Type TrapType{ get{ return typeof( GasMine ); } }

		public GasMineDeed() : base( 0x11AB )
		{
			Name = "a gas mine";
		}
		
		public GasMineDeed( Serial serial ) : base( serial )
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