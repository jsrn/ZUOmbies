using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public abstract class ReaperSeed : MonsterSeed
	{
		public ReaperSeed( int itemID ) : base( itemID )
		{
		}

		public ReaperSeed( Serial serial ) : base( serial )
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