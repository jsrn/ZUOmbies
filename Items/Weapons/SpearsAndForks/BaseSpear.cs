using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public abstract class BaseSpear : BaseMeleeWeapon
	{
		public override int DefHitSound{ get{ return 0x23C; } }
		public override int DefMissSound{ get{ return 0x238; } }

		public override SkillName DefSkill{ get{ return SkillName.Fencing; } }
		public override WeaponType DefType{ get{ return WeaponType.Piercing; } }
		public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.Pierce2H; } }

		public BaseSpear( int itemID ) : base( itemID )
		{
		}

		public BaseSpear( Serial serial ) : base( serial )
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

		public override void OnHit( Mobile attacker, Mobile defender, double damageBonus )
		{
			base.OnHit( attacker, defender, damageBonus );

			if ( !Core.AOS && Layer == Layer.TwoHanded && (attacker.Skills[SkillName.Anatomy].Value / 400.0) >= Utility.RandomDouble() )
			{
				defender.SendMessage( "You receive a paralyzing blow!" ); // Is this not localized?
				defender.Freeze( TimeSpan.FromSeconds( 2.0 ) );

				attacker.SendMessage( "You deliver a paralyzing blow!" ); // Is this not localized?
				attacker.PlaySound( 0x11C );
			}

			if ( !Core.AOS && Poison != null && PoisonCharges > 0 )
			{
				--PoisonCharges;

				if ( Utility.RandomDouble() >= 0.5 ) // 50% chance to poison
					defender.ApplyPoison( attacker, Poison );
			}
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