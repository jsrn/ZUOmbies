using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
	public class Bola : Item
	{
		[Constructable]
		public Bola() : this( 1 )
		{
		}

		[Constructable]
		public Bola( int amount ) : base( 0x26AC )
		{
			Weight = 4.0;
			Stackable = true;
			Amount = amount;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1040019 ); // The bola must be in your pack to use it.
			}
			else if ( !from.CanBeginAction( typeof( Bola ) ) )
			{
				from.SendLocalizedMessage( 1049624 ); // You have to wait a few moments before you can use another bola!
			}
			else if ( from.Target is BolaTarget )
			{
				from.SendLocalizedMessage( 1049631 ); // This bola is already being used.
			}
			else if ( !BasePotion.HasFreeHand( from ) )
			{
				from.SendMessage( "You need a free hand to throw this." );
			}
			else if ( from.Mounted )
			{
				from.SendLocalizedMessage( 1040016 ); // You cannot use this while riding a mount
			}
			else
			{
				EtherealMount.StopMounting( from );

				from.Target = new BolaTarget( this );
				from.LocalOverheadMessage( MessageType.Emote, 0x3B2, 1049632 ); // * You begin to swing the bola...*
				from.NonlocalOverheadMessage( MessageType.Emote, 0x3B2, 1049633, from.Name ); // ~1_NAME~ begins to menacingly swing a bola...
			}
		}

		private static void ReleaseBolaLock( object state )
		{
			((Mobile)state).EndAction( typeof( Bola ) );
		}

		private static void FinishThrow( object state )
		{
			object[] states = (object[])state;

			Mobile from = (Mobile)states[0];
			Mobile to = (Mobile)states[1];

			new Bola().MoveToWorld( to.Location, to.Map );

			to.Damage( 1, from );

			if ( to.Mounted )
			{
				if ( to is ChaosDragoon || to is ChaosDragoonElite )
					from.SendLocalizedMessage( 1042047 ); // You fail to knock the rider from its mount.

				IMount mt = to.Mount;
				if ( mt != null && !( to is ChaosDragoon || to is ChaosDragoonElite ) )
					mt.Rider = null;

				to.SendLocalizedMessage( 1040023 ); // You have been knocked off of your mount!

				BaseMount.SetMountPrevention( to, BlockMountType.Dazed, TimeSpan.FromSeconds( 3.0 ) );
			}
			else
			{
				if ( from.Dex < Utility.RandomMinMax( 1, 100 ) )
				{
					if ( to.Dex < Utility.RandomMinMax( 1, 100 ) )
					{
						to.Freeze( TimeSpan.FromSeconds( 2.0 ) );
						from.SendMessage( "You throw the bola, and it entangles your foe!" );
						to.SendMessage( "You are entangled in a bola!" );
					}
					else
					{
						from.SendMessage( "You throw the bola, but they dodge out of the way." );
					}
				}
				else
				{
					from.SendMessage( "You throw the bola, but miss." );
				}
			}

			Timer.DelayCall( TimeSpan.FromSeconds( 20.0 ), new TimerStateCallback( ReleaseBolaLock ), from );
		}

		private class BolaTarget : Target
		{
			private Bola m_Bola;

			public BolaTarget( Bola bola ) : base( 8, false, TargetFlags.Harmful )
			{
				m_Bola = bola;
			}

			protected override void OnTarget( Mobile from, object obj )
			{
				if ( m_Bola.Deleted )
					return;

				if ( obj is Mobile )
				{
					Mobile to = (Mobile)obj;

					if ( !m_Bola.IsChildOf( from.Backpack ) )
					{
						from.SendLocalizedMessage( 1040019 ); // The bola must be in your pack to use it.
					}
					else if ( from.FindItemOnLayer( Layer.OneHanded ) == null || from.FindItemOnLayer( Layer.TwoHanded ) == null )
					{
						from.SendMessage( "You need a free hand to throw this." );
					}
					else if ( from.Mounted )
					{
						from.SendLocalizedMessage( 1040016 ); // You cannot use this while riding a mount
					}
					else if ( !from.CanBeHarmful( to ) )
					{
					}
					else if ( from.BeginAction( typeof( Bola ) ) )
					{
						EtherealMount.StopMounting( from );

						Item one = from.FindItemOnLayer( Layer.OneHanded );
						Item two = from.FindItemOnLayer( Layer.TwoHanded );

						if ( one != null )
							from.AddToBackpack( one );

						if ( two != null )
							from.AddToBackpack( two );

						from.DoHarmful( to );

						BaseMount.SetMountPrevention( from, BlockMountType.BolaRecovery, TimeSpan.FromSeconds( 3.0 ) );

						m_Bola.Consume();

						from.Direction = from.GetDirectionTo( to );
						from.Animate( 11, 5, 1, true, false, 0 );
						from.MovingEffect( to, 0x26AC, 10, 0, false, false );

						Timer.DelayCall( TimeSpan.FromSeconds( 0.5 ), new TimerStateCallback( FinishThrow ), new object[]{ from, to } );
					}
					else
					{
						from.SendLocalizedMessage( 1049624 ); // You have to wait a few moments before you can use another bola!
					}
				}
				else
				{
					from.SendLocalizedMessage( 1049629 ); // You cannot throw a bola at that.
				}
			}
		}

		public Bola( Serial serial ) : base( serial )
		{
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