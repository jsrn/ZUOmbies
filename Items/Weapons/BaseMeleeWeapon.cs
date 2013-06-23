using System;
using Server;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public abstract class BaseMeleeWeapon : BaseWeapon
	{
		public BaseMeleeWeapon( int itemID ) : base( itemID )
		{
		}

		public BaseMeleeWeapon( Serial serial ) : base( serial )
		{
		}

		public override int AbsorbDamage( Mobile attacker, Mobile defender, int damage )
		{
			damage = base.AbsorbDamage( attacker, defender, damage );

			if ( Core.AOS )
				return damage;
			
			int absorb = defender.MeleeDamageAbsorb;

			if ( absorb > 0 )
			{
				if ( absorb > damage )
				{
					int react = damage / 5;

					if ( react <= 0 )
						react = 1;

					defender.MeleeDamageAbsorb -= damage;
					damage = 0;

					attacker.Damage( react, defender );

					attacker.PlaySound( 0x1F1 );
					attacker.FixedEffect( 0x374A, 10, 16 );
				}
				else
				{
					defender.MeleeDamageAbsorb = 0;
					defender.SendLocalizedMessage( 1005556 ); // Your reactive armor spell has been nullified.
					DefensiveSpell.Nullify( defender );
				}
			}

			return damage;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( from.Items.Contains( this ) )
			{
				from.SendMessage( "Who do you wish to throw this at?" );
				InternalTarget t = new InternalTarget( this );
				from.Target = t;
			}
			else
			{
				from.SendMessage( "You must be holding that weapon to use it." );
			}
		}

		private class InternalTarget : Target
		{
			private BaseMeleeWeapon m_Weapon;

			public InternalTarget( BaseMeleeWeapon weapon ) : base( 10, false, TargetFlags.Harmful )
			{
				m_Weapon = weapon;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_Weapon.Deleted )
				{
					return;
				}
				else if ( !from.Items.Contains( m_Weapon ) )
				{
					from.SendMessage( "You must be holding that weapon to use it." );
				}
				else if ( targeted is Mobile )
				{
					Mobile m = (Mobile)targeted;

					if ( m != from && from.HarmfulCheck( m ) )
					{
						Direction to = from.GetDirectionTo( m );

						from.Direction = to;

						from.Animate( from.Mounted ? 26 : 9, 7, 1, true, false, 0 );

						int theirRating = m.Dex;

						if( theirRating > 100 )
							theirRating = 100;

						int yourRating = ( from.Dex + from.Str ) / 2;
						if ( yourRating > 100 )
							yourRating = 100;


						bool threw = Utility.RandomMinMax( 1, 100 ) < yourRating;

						bool hit = threw && Utility.RandomMinMax( 1, 100 ) > theirRating;

						if ( hit )
						{
							from.MovingEffect( m, 0x1BFE, 7, 1, false, false, 0x481, 0 );

							m_Weapon.MoveToWorld( m.Location, m.Map );

							AOS.Damage( m, from, Utility.Random( 5, from.Str / 10 ), 100, 0, 0, 0, 0 );
						}
						else
						{
							int x = 0, y = 0;

							switch ( to & Direction.Mask )
							{
								case Direction.North: --y; break;
								case Direction.South: ++y; break;
								case Direction.West: --x; break;
								case Direction.East: ++x; break;
								case Direction.Up: --x; --y; break;
								case Direction.Down: ++x; ++y; break;
								case Direction.Left: --x; ++y; break;
								case Direction.Right: ++x; --y; break;
							}

							x += Utility.Random( -1, 3 );
							y += Utility.Random( -1, 3 );

							x += m.X;
							y += m.Y;

							m_Weapon.MoveToWorld( new Point3D( x, y, m.Z ), m.Map );

							from.MovingEffect( m_Weapon, 0x1BFE, 7, 1, false, false, 0x481, 0 );

							from.SendMessage( "You miss." );
						}
					}
				}
			}
		}
	}
}
