using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	[FlipableAttribute( 0xF62, 0xF63 )]
	public class Spear : BaseSpear
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ArmorIgnore; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ParalyzingBlow; } }

		public override int AosStrengthReq{ get{ return 50; } }
		public override int AosMinDamage{ get{ return 13; } }
		public override int AosMaxDamage{ get{ return 15; } }
		public override int AosSpeed{ get{ return 42; } }
		public override float MlSpeed{ get{ return 2.75f; } }

		public override int OldStrengthReq{ get{ return 30; } }
		public override int OldMinDamage{ get{ return 2; } }
		public override int OldMaxDamage{ get{ return 36; } }
		public override int OldSpeed{ get{ return 46; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 80; } }

		public override int DefMaxRange{ get{ return 2; } }

		[Constructable]
		public Spear() : base( 0xF62 )
		{
			Weight = 7.0;
		}

		public Spear( Serial serial ) : base( serial )
		{
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

		public override void OnDoubleClick( Mobile from )
		{
			if ( from.Items.Contains( this ) )
			{
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
			private Spear m_Spear;

			public InternalTarget( Spear spear ) : base( 10, false, TargetFlags.Harmful )
			{
				m_Spear = spear;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_Spear.Deleted )
				{
					return;
				}
				else if ( !from.Items.Contains( m_Spear ) )
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

						if ( Utility.RandomDouble() >= (Math.Sqrt( m.Dex / 100.0 ) * 0.8) )
						{
							from.MovingEffect( m, 0x1BFE, 7, 1, false, false, 0x481, 0 );

							AOS.Damage( m, from, Utility.Random( 5, from.Str / 10 ), 100, 0, 0, 0, 0 );

							m_Spear.MoveToWorld( m.Location, m.Map );
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

							m_Spear.MoveToWorld( new Point3D( x, y, m.Z ), m.Map );

							from.MovingEffect( m_Spear, 0x1BFE, 7, 1, false, false, 0x481, 0 );

							from.SendMessage( "You miss." );
						}
					}
				}
			}
		}
	}
}