using Server.Mobiles;
using Server.Items;
using Server.Targeting;
using System;

namespace Server.Misc
{
	public class WeaponThrowing
	{
		public static void ThrowWeapon( Mobile from, Item thrown )
		{
			if ( from.Items.Contains( thrown ) )
			{
				ThrowingTarget t = new ThrowingTarget( thrown );
				from.Target = t;
			}
			else
			{
				from.SendMessage( "You must be holding that weapon to use it." );
			}
		}

		private class ThrowingTarget : Target
		{
			private Item m_Weapon;

			public ThrowingTarget( Item weapon ) : base( 10, false, TargetFlags.Harmful )
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
						from.RevealingAction();

						Direction to = from.GetDirectionTo( m );

						from.Direction = to;

						from.Animate( from.Mounted ? 26 : 9, 7, 1, true, false, 0 );

						if ( Utility.RandomDouble() >= (Math.Sqrt( m.Dex / 100.0 ) * 0.8) )
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