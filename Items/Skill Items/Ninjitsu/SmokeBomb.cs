using System;
using Server;

namespace Server.Items
{
	public class SmokeBomb : Item
	{
		[Constructable]
		public SmokeBomb() : base( 0x2808 )
		{
			Weight = 1.0;
		}

		public SmokeBomb( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				// The item must be in your backpack to use it.
				from.SendLocalizedMessage( 1060640 );
			}
			else if ( from.NextSkillTime > DateTime.Now )
			{
				// You must wait a few seconds before you can use that item.
				from.SendLocalizedMessage( 1070772 );
			}
			else if ( from.Stam < 20 )
			{
				from.SendMessage( "You don't have enough stamina to do that." );
			}
			else
			{
				SkillHandlers.Hiding.CombatOverride = true;

				if ( from.UseSkill( SkillName.Hiding ) )
				{
					from.Stam -= 20;

					// Effects nicked from the GM hide/unhide command
					Effects.SendLocationEffect( new Point3D( from.X + 1, from.Y, from.Z + 4 ), from.Map, 0x3728, 13 );
					Effects.SendLocationEffect( new Point3D( from.X + 1, from.Y, from.Z ), from.Map, 0x3728, 13 );
					Effects.SendLocationEffect( new Point3D( from.X + 1, from.Y, from.Z - 4 ), from.Map, 0x3728, 13 );
					Effects.SendLocationEffect( new Point3D( from.X, from.Y + 1, from.Z + 4 ), from.Map, 0x3728, 13 );
					Effects.SendLocationEffect( new Point3D( from.X, from.Y + 1, from.Z ), from.Map, 0x3728, 13 );
					Effects.SendLocationEffect( new Point3D( from.X, from.Y + 1, from.Z - 4 ), from.Map, 0x3728, 13 );

					Effects.SendLocationEffect( new Point3D( from.X + 1, from.Y + 1, from.Z + 11 ), from.Map, 0x3728, 13 );
					Effects.SendLocationEffect( new Point3D( from.X + 1, from.Y + 1, from.Z + 7 ), from.Map, 0x3728, 13 );
					Effects.SendLocationEffect( new Point3D( from.X + 1, from.Y + 1, from.Z + 3 ), from.Map, 0x3728, 13 );
					Effects.SendLocationEffect( new Point3D( from.X + 1, from.Y + 1, from.Z - 1 ), from.Map, 0x3728, 13 );

					from.PlaySound( 0x22F );

					Consume();
				}

				SkillHandlers.Hiding.CombatOverride = false;
			}
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