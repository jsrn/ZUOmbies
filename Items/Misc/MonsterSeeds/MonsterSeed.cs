using System;
using Server;
using Server.Mobiles;
using Server.Targeting;
using Server.Network;

namespace Server.Items
{
	public class MonsterSeed : Item
	{
		private Type m_Creature;

		public Type CreatureType { get { return m_Creature; } }

		[Constructable]
		public MonsterSeed() : this( typeof( Zombie ) )
		{
		}

		public MonsterSeed( Type creatureType ) : base( 0xDCD )
		{
			Name = "a monster seed";
			Weight = 1.0;
			Hue = 0x42;
			m_Creature = creatureType;
		}

		public MonsterSeed( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042664 ); // You must have the object in your backpack to use it.
				return;
			}

			if ( !from.CanBeginAction( typeof( MonsterSeed ) ) )
			{
				from.SendMessage( "You must wait before planting another seed." );
				return;
			}

			from.Target = new InternalTarget( this, m_Creature );
			from.SendMessage( "Choose a spot to plant the seed." );
		}

		private class InternalTarget : Target
		{
			private int[] tiles = new int[]
			{
				// Dirt
				0x71, 0x7C, 0x82, 0xA7, 0xDC, 0xE3, 0xE8, 0xEB, 0x141, 0x144, 0x14C, 0x14F,
				0x169, 0x174, 0x1DC, 0x1E7, 0x1EC, 0x1EF, 0x272, 0x275, 0x27E, 0x281, 0x2D0,
				0x2D7, 0x2E5, 0x2FF, 0x303, 0x31F, 0x32C, 0x32F, 0x33D, 0x340, 0x345, 0x34C,
				0x355, 0x358, 0x367, 0x36E, 0x377, 0x37A, 0x38D, 0x390, 0x395, 0x39C, 0x3A5,
				0x3A8, 0x3F6, 0x405, 0x547, 0x54E, 0x553, 0x556, 0x597, 0x59E, 0x623, 0x63A,
				0x6F3, 0x6FA, 0x777, 0x791, 0x79A, 0x7A9, 0x7AE, 0x7B1,
				// Furrow
				0x9, 0x15, 0x150, 0x15C,
				// Swamp
				0x9C4, 0x9EB, 0x3D65, 0x3D65, 0x3DC0, 0x3DD9, 0x3DDB, 0x3DDC, 0x3DDE, 0x3EF0,
				0x3FF6, 0x3FF6, 0x3FFC, 0x3FFE,
				// Snow
				0x10C, 0x10F, 0x114, 0x117, 0x119, 0x11D, 0x179, 0x18A, 0x385, 0x38C, 0x391,
				0x394, 0x39D, 0x3A4, 0x3A9, 0x3AC, 0x5BF, 0x5D6, 0x5DF, 0x5E2, 0x745, 0x748,
				0x751, 0x758, 0x75D, 0x760, 0x76D, 0x773
			};

			private MonsterSeed m_Seed;
			private Type m_Creature;

			public InternalTarget( MonsterSeed seed, Type creature ) : base( 3, true, TargetFlags.None )
			{
				m_Seed = seed;
				m_Creature = creature;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_Seed.Deleted )
					return;

				if ( !m_Seed.IsChildOf( from.Backpack ) )
				{
					from.SendLocalizedMessage( 1042664 ); // You must have the object in your backpack to use it.
					return;
				}

				if ( !from.CanBeginAction( typeof( MonsterSeed ) ) )
				{
					from.SendMessage( "You must wait before planting another seed." );
					return;
				}

				LandTarget land = targeted as LandTarget;

				if ( land == null )
				{
					from.SendMessage( "You cannot plant that there." );
				}
				else
				{
					//MonsterSeedEffect effect = MonsterSeedEffect.Create( from, land, m_Seed.CreatureType );

					if ( false ) // Target is in tiles
					{
						from.SendMessage( "That would be futile." );
					}
					else
					{
						m_Seed.Consume();

						from.SendMessage( "You push the seed into the ground." );
						from.Emote( "*Pushes a seed into the ground.*" );

						object[] arg = new object[] { from, land, m_Creature };


						Timer.DelayCall( TimeSpan.FromSeconds( 2.0 ), new TimerStateCallback( SpawnCreature ), arg);
					}
				}
			}

			protected override void OnTargetOutOfRange( Mobile from, object targeted )
			{
				from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 502825 ); // That location is too far away
			}

			public void SpawnCreature( object arg )
			{
				object[] args = (object[])arg;
				Mobile from = (Mobile)args[0];
				LandTarget target = (LandTarget)args[1];
				Type creatureType = (Type)args[2];

				BaseCreature creature = Activator.CreateInstance( creatureType ) as BaseCreature;

				for ( int i = 0; i < 5; i++ ) // Try 5 times
				{
					int x = target.X;
					int y = target.Y;
					int z = from.Map.GetAverageZ( x, y );

					if ( from.Map.CanSpawnMobile( x, y, target.Z ) )
					{
						creature.MoveToWorld( new Point3D( x, y, target.Z ), from.Map );
					}
					//else if ( from.Map.CanSpawnMobile( x, y, z ) )
					//{
					//	creature.MoveToWorld( new Point3D( x, y, z ), from.Map );
					//}
				}
			}
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