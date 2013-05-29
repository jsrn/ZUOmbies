using System;
using System.Collections;
using Server.Engines.Plants;
using Server.Engines.Quests;
using Server.Engines.Quests.Hag;
using Server.Engines.Quests.Matriarch;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
	public class WaterBucket : BaseBeverage
	{
		public override int BaseLabelNumber { get { return 1042965; } } // a WaterBucket of Ale
		public override int MaxQuantity { get { return 10; } }
		public override bool Fillable { get { return true; } }

		public override int ComputeItemID()
		{
			if( IsEmpty )
			{
				return 0x14e0;
			}
				
			return 0x2004;
		}

		[Constructable]
		public WaterBucket( BeverageType type ) : base( type )
		{
			Weight = 1.0;
			Name = "a bucket";
			Quantity = 0;
		}

		public WaterBucket( Serial serial ) : base( serial )
		{
		}

		[Constructable]
		public WaterBucket( ) : base ( BeverageType.Water )
		{
			Weight = 1.0;
			Name = "a bucket";
			Quantity = 0;
		}


		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void Fill_OnTarget( Mobile from, object targ )
		{
			base.Fill_OnTarget( from, targ );

			if( !IsEmpty )
			{
				switch( this.Content )
				{
					case BeverageType.Ale: Name = "a bucket of ale"; break;
					case BeverageType.Wine: Name = "a bucket of wine"; break;
					case BeverageType.Milk: Name = "a bucket of milk"; break;
					case BeverageType.Cider: Name = "a bucket of cider"; break;
					case BeverageType.Water: Name = "a bucket of water"; break;
					case BeverageType.Liquor: Name = "a bucket of liquor"; break;
				}
			}
		}

		public override void Pour_OnTarget( Mobile from, object targ )
		{
			base.Pour_OnTarget( from, targ );

			if( IsEmpty )
			{
				Name = "a bucket";
			}
		}
	}
}