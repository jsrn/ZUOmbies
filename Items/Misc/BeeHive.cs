using System;
using Server;
using Server.Targeting;

namespace Server.Items
{
	public class BeeHive : Item
	{
		private bool m_IsFull = true;

		[Constructable]
		public BeeHive() : base( 0x91A )
		{
			Weight = 1.0;
			Name = "a bee hive";
			Movable = false;
		}

		public BeeHive( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if( m_IsFull )
			{
				if( Utility.RandomDouble() < 0.2 )
				{
					from.SendMessage( "You gather some beeswax and put it in your pack." );
					from.Backpack.DropItem( new Beeswax() );
				}

				from.SendMessage( "You gather some honey from the bee hive and put it in a jar." );
				from.Backpack.DropItem( new JarHoney() );
				m_IsFull = false;
				Timer.DelayCall( TimeSpan.FromSeconds( 30.0 ), new TimerCallback( ResetHoneyContents ) );
			}
			else
			{
				from.SendMessage( "There is no honey to gather." );
			}
		}

		private void ResetHoneyContents()
		{
			m_IsFull = true;
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