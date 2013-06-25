using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Items
{
	public class CorpserSeed : MonsterSeed
	{
		[Constructable]
		public CorpserSeed() : base( typeof( Corpser ) )
		{
			Name = "a corpser seed";
		}

		public CorpserSeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
		}
	}
}