using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public abstract class CorpserSeed : MonsterSeed
	{
		public CorpserSeed( int itemID ) : base( itemID )
		{
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