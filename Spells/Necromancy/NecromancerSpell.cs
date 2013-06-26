using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Spells.Necromancy
{
	public abstract class NecromancerSpell : Spell
	{
		public abstract double RequiredSkill{ get; }
		public abstract int RequiredMana{ get; }
		public abstract int RequiredEvil{ get; }

		public override SkillName CastSkill{ get{ return SkillName.Necromancy; } }
		public override SkillName DamageSkill{ get{ return SkillName.SpiritSpeak; } }

		//public override int CastDelayBase{ get{ return base.CastDelayBase; } } // Reference, 3

		public override bool ClearHandsOnCast{ get{ return false; } }

		public override double CastDelayFastScalar{ get{ return (Core.SE? base.CastDelayFastScalar : 0); } } // Necromancer spells are not affected by fast cast items, though they are by fast cast recovery

		public NecromancerSpell( Mobile caster, Item scroll, SpellInfo info ) : base( caster, scroll, info )
		{
		}

		public override int ComputeKarmaAward()
		{
			//TODO: Verify this formula being that Necro spells don't HAVE a circle.

			//return -(70 + (10 * (int)Circle));

			return -(40 + (int)(10 * (CastDelayBase.TotalSeconds / CastDelaySecondsPerTick)));
		}

		public override void GetCastSkills( out double min, out double max )
		{
			min = RequiredSkill;
			max = Scroll != null ? min : RequiredSkill + 40.0;
		}

		public override int GetMana()
		{
			return RequiredMana;
		}

		public override bool CheckCast()
		{
			int mana = ScaleMana( RequiredMana );

			if ( !base.CheckCast() )
				return false;

			if ( Caster is PlayerMobile && ((PlayerMobile)Caster).EvilPoints < RequiredEvil )
			{
				Caster.SendMessage( "You lack the required favour of the Guardian to cast that spell." );
				return false;
			}
			else if ( Caster.Mana < mana )
			{
				Caster.SendLocalizedMessage( 1060174, mana.ToString() ); // You must have at least ~1_MANA_REQUIREMENT~ Mana to use this ability.
				return false;
			}

			return true;
		}

		public override bool CheckFizzle()
		{
			int requiredEvil = this.RequiredEvil;

			if ( AosAttributes.GetValue( Caster, AosAttribute.LowerRegCost ) > Utility.Random( 100 ) )
				requiredEvil = 0;

			int mana = ScaleMana( RequiredMana );

			if ( Caster is PlayerMobile && ((PlayerMobile)Caster).EvilPoints < RequiredEvil )
			{
				Caster.SendMessage( "You lack the required favour of the Guardian to cast that spell." );
				return false;
			}
			else if ( Caster.Mana < mana )
			{
				Caster.SendLocalizedMessage( 1060174, mana.ToString() ); // You must have at least ~1_MANA_REQUIREMENT~ Mana to use this ability.
				return false;
			}

			if( Caster is PlayerMobile )
				((PlayerMobile)Caster).EvilPoints -= requiredEvil;

			if ( !base.CheckFizzle() )
				return false;

			Caster.Mana -= mana;

			return true;
		}
	}
}