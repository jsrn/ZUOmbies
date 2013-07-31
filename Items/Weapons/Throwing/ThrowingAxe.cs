using System;
using Server.Targeting;
using Server.Network;
using Server.Misc;

namespace Server.Items
{
	[FlipableAttribute( 0xF43, 0xF44 )]
	public class ThrowingAxe : Item
	{
		public override string DefaultName
		{
			get { return "a throwing axe"; }
		}

		[Constructable]
		public ThrowingAxe() : base( 0xF43 )
		{
			Weight = 5.0;
			Layer = Layer.OneHanded;
		}

		public ThrowingAxe( Serial serial ) : base( serial )
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