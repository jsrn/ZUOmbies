using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a reapers corpse" )]
	public class LesserReaper : Reaper
	{
		[Constructable]
		public LesserReaper() : base()
		{
			Name = "a lesser reaper";

			SetStr( 56, 160 );
			SetDex( 56, 70 );
			SetInt( 91, 170 );

			SetHits( 40, 90 );

			SetDamage( 7, 9 );

			SetSkill( SkillName.EvalInt, 80.1, 90.0 );
			SetSkill( SkillName.Magery, 80.1, 90.0 );
			SetSkill( SkillName.MagicResist, 9.1, 115.0 );
			SetSkill( SkillName.Tactics, 35.1, 50.0 );
			SetSkill( SkillName.Wrestling, 40.1, 50.0 );
		}

		public LesserReaper( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}