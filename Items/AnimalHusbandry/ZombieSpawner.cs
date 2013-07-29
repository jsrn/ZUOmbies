using System;
using Server;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Items
{
	public class ZombieSpawner : Spawner
	{
		private List<string> m_Animals = new List<string>();
		private int m_LightLevel;

		[CommandProperty( AccessLevel.GameMaster )]
		public int LightLevel
		{
			get { return GetLightLevel(); }
		}
		
		[Constructable]
		public ZombieSpawner( string type ) : base( null )
		{
			List<string> animals = new List<string>();
			animals.Add( type );
			SpawnNames = animals;
			MinDelay = TimeSpan.FromSeconds( 5.0 );
			MaxDelay = TimeSpan.FromSeconds( 5.0 );
			Count = 7;
			HomeRange = 10;
			Name = "zombie spawner";
		}

		[Constructable]
		public ZombieSpawner() : this( "zombie" )
		{
		}

		public ZombieSpawner( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	
		public override void OnTick()
		{
			m_LightLevel = GetLightLevel();

			int fullCount = Count;

			if( m_LightLevel >= 20 )
			{
				int newCount = Math.Abs( 19 - m_LightLevel );
				Count = newCount;
			}
			else
			{
				Count = 0; // Don't spawn with a light level less than 20
			}

			base.OnTick();

			if ( Count < CurrentSpawnCount )
				ClearExcessZombies();

			Count = fullCount;
		}

		private void ClearExcessZombies()
		{
			bool deleted = false;

			for( int i = 0; i < Spawned.Count && !deleted; i++)
			{
				Mobile m = Spawned[i] as Mobile;

				if( m is null )
					return;

				bool seen = false;

				foreach ( Mobile seer in m.GetMobilesInRange( 16 ) )
				{
					if ( seer.Player )
						seen = true;
				}

				if ( !seen )
				{
					Spawned[i].Delete();
					deleted = true;
				}
			}
		}

		private int GetLightLevel()
		{
			if ( LightCycle.LevelOverride > int.MinValue )
				return LightCycle.LevelOverride;

			int dayLevel = LightCycle.DayLevel;
			int nightLevel = LightCycle.NightLevel;

			int hours, minutes;

			Server.Items.Clock.GetTime( this.Map, this.X, this.Y, out hours, out minutes );

			if ( hours < 4 )
				return nightLevel;

			if ( hours < 6 )
				return nightLevel + (((((hours - 4) * 60) + minutes) * (dayLevel - nightLevel)) / 120);

			if ( hours < 22 )
				return dayLevel;

			if ( hours < 24 )
				return dayLevel + (((((hours - 22) * 60) + minutes) * (nightLevel - dayLevel)) / 120);

			return -1; // should never be
		}
	}
}