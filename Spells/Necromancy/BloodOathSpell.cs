using System;
using System.Collections;
using Server.Network;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Spells.Necromancy
{
	public class BloodOathSpell : NecromancerSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Blood Oath", "In Jux Mani Xen",
				-1,
				9031
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1.5 ); } }

		public override double RequiredSkill{ get{ return 20.0; } }
		public override int RequiredMana{ get{ return 13; } }
		public override int RequiredEvil{ get{ return 13; } }

		public BloodOathSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public void Target( Mobile m )
		{
			if ( Caster == m )
			{
				Caster.SendMessage( "You cannot enter a blood oath with yourself." );
			}
			else if ( !(m is PlayerMobile || m is BaseCreature) )
			{
				Caster.SendMessage( "You cannot enter a blood oath with that." );
			}
			else if ( CheckHSequence( m ) )
			{
				// Heals a target at a cost of 1/3 the healed HP to yourself
				SpellHelper.Turn( Caster, m );

				if ( m.Spell != null )
					m.Spell.OnCasterHurt();
				
				Caster.PlaySound( 0x175 );

				Caster.FixedParticles( 0x375A, 1, 17, 9919, 33, 7, EffectLayer.Waist );
				Caster.FixedParticles( 0x3728, 1, 13, 9502, 33, 7, (EffectLayer)255 );

				m.FixedParticles( 0x375A, 1, 17, 9919, 33, 7, EffectLayer.Waist );
				m.FixedParticles( 0x3728, 1, 13, 9502, 33, 7, (EffectLayer)255 );

				int toHeal = (int)(Caster.Skills[SkillName.SpiritSpeak].Value * 0.4);
				toHeal += Utility.Random( 1, 10 );

				int toDamage = toHeal / 3;

				if ( Caster.Hits - toDamage < 10 )
					Caster.Hits = 10;
				else
					Caster.Damage( toDamage );

				SpellHelper.Heal( toHeal, m, Caster );
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private BloodOathSpell m_Owner;

			public InternalTarget( BloodOathSpell owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is Mobile )
					m_Owner.Target( (Mobile) o );
				else
					from.SendLocalizedMessage( 1060508 ); // You can't curse that.
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}