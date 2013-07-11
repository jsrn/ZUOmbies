using System;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Items
{
	public class AnimalSpawner : Spawner
	{
		private int m_AnimalsRemaining;
		private List<string> m_Animals = new List<string>();

		[CommandProperty( AccessLevel.GameMaster )]
		public int AnimalsRemaining
		{
			get { return m_AnimalsRemaining; }
			set { m_AnimalsRemaining = value; }
		}
		
		[Constructable]
		public AnimalSpawner() : base( null )
		{
			m_AnimalsRemaining = 30;

			List<string> animals = new List<string>();
			animals.Add( "sheep" );
			SpawnNames = animals;
		}

		public AnimalSpawner( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
			writer.Write( m_AnimalsRemaining );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			m_AnimalsRemaining = reader.ReadInt();
		}
	
		public override void Spawn( int index )
		{
			m_AnimalsRemaining -= 1;

			base.Spawn( index );

			if( m_AnimalsRemaining == 0 )
				Delete();
		}
	}
}