using System;
using Server.Targeting;
using Server.Network;
using Server.Misc;

namespace Server.Items
{
	[FlipableAttribute( 0xF52, 0xF51 )]
	public class ThrowingDagger : Item
	{
		public override string DefaultName
		{
			get { return "a throwing dagger"; }
		}

		[Constructable]
		public ThrowingDagger() : base( 0xF52 )
		{
			Weight = 1.0;
			Layer = Layer.OneHanded;
		}

		public ThrowingDagger( Serial serial ) : base( serial )
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

		public override void OnDoubleClick( Mobile from )
		{
			WeaponThrowing.ThrowWeapon( from, this );
		}
	}
}