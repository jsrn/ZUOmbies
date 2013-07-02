using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Items
{
	public class LesserReaperSeed : MonsterSeed
	{
		[Constructable]
		public LesserReaperSeed() : base( typeof( LesserReaper ) )
		{
			Name = "a lesser reaper seed";
		}

		public LesserReaperSeed( Serial serial ) : base( serial )
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