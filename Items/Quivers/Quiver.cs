using System;
using Server;

namespace Server.Items
{
	[FlipableAttribute( 0x2FB7, 0x3171 )]
	public class Quiver : BaseQuiver
	{

		public override string DefaultName
		{
			get { return "quiver"; }
		}
		
		[Constructable]
		public Quiver() : base()
		{
			WeightReduction = 30;
			Hue = 0x2E6; // Brown
		}

		public Quiver( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}
