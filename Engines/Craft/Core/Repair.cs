using System;
using Server;
using Server.Mobiles;
using Server.Targeting;
using Server.Items;

namespace Server.Engines.Craft
{
	public class Repair
	{
		public Repair()
		{
		}

		public static void Do( Mobile from, CraftSystem craftSystem, BaseTool tool )
		{
			from.Target = new InternalTarget( craftSystem, tool );
			from.SendLocalizedMessage( 1044276 ); // Target an item to repair.
		}

		private class InternalTarget : Target
		{
			private CraftSystem m_CraftSystem;
			private BaseTool m_Tool;

			public InternalTarget( CraftSystem craftSystem, BaseTool tool ) :  base ( 2, false, TargetFlags.None )
			{
				m_CraftSystem = craftSystem;
				m_Tool = tool;
			}

			private static void EndGolemRepair( object state )
			{
				((Mobile)state).EndAction( typeof( Golem ) );
			}

			private int GetWeakenChance( Mobile mob, SkillName skill, int curHits, int maxHits )
			{
				// 40% - (1% per hp lost) - (1% per 10 craft skill)
				return (40 + (maxHits - curHits)) - (int)((mob.Skills[skill].Value) / 10);
			}

			private bool CheckWeaken( Mobile mob, SkillName skill, int curHits, int maxHits )
			{
				return ( GetWeakenChance( mob, skill, curHits, maxHits ) > Utility.Random( 100 ) );
			}

			private int GetRepairDifficulty( int curHits, int maxHits )
			{
				return (((maxHits - curHits) * 1250) / Math.Max( maxHits, 1 )) - 250;
			}

			private bool CheckRepairDifficulty( Mobile mob, SkillName skill, int curHits, int maxHits )
			{
				double difficulty = GetRepairDifficulty( curHits, maxHits ) * 0.1;

				return mob.CheckSkill( skill, difficulty - 25.0, difficulty + 25.0 );
			}

			private bool IsSpecialClothing( BaseClothing clothing )
			{
				// Armor repairable but not craftable

				if( m_CraftSystem is DefTailoring )
				{
					return (clothing is BearMask)
						|| (clothing is DeerMask);
				}

				return false;
			}

			private bool IsSpecialWeapon( BaseWeapon weapon )
			{
				// Weapons repairable but not craftable

				if ( m_CraftSystem is DefTinkering )
				{
					return ( weapon is Cleaver )
						|| ( weapon is Hatchet )
						|| ( weapon is Pickaxe )
						|| ( weapon is ButcherKnife )
						|| ( weapon is SkinningKnife );
				}
				else if ( m_CraftSystem is DefCarpentry )
				{
					return ( weapon is Club )
						|| ( weapon is BlackStaff )
						|| ( weapon is MagicWand );
				}
				else if ( m_CraftSystem is DefBlacksmithy )
				{
					return ( weapon is Pitchfork );
				}

				return false;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				int number;

				if ( m_CraftSystem.CanCraft( from, m_Tool, targeted.GetType() ) == 1044267 )
				{
					number = 1044282; // You must be near a forge and and anvil to repair items. * Yes, there are two and's *
				}
				else if ( m_CraftSystem is DefTinkering && targeted is Golem )
				{
					Golem g = (Golem)targeted;
					int damage = g.HitsMax - g.Hits;

					if ( g.IsDeadBondedPet )
					{
						number = 500426; // You can't repair that.
					}
					else if ( damage <= 0 )
					{
						number = 500423; // That is already in full repair.
					}
					else
					{
						double skillValue = from.Skills[SkillName.Tinkering].Value;

						if ( skillValue < 60.0 )
						{
							number = 1044153; // You don't have the required skills to attempt this item.
						}
						else if ( !from.CanBeginAction( typeof( Golem ) ) )
						{
							number = 501789; // You must wait before trying again.
						}
						else
						{
							if ( damage > (int)(skillValue * 0.3) )
								damage = (int)(skillValue * 0.3);

							damage += 30;

							if ( !from.CheckSkill( SkillName.Tinkering, 0.0, 100.0 ) )
								damage /= 2;

							Container pack = from.Backpack;

							if ( pack != null )
							{
								int v = pack.ConsumeUpTo( typeof( IronIngot ), (damage+4)/5 );

								if ( v > 0 )
								{
									g.Hits += v*5;

									number = 1044279; // You repair the item.

									from.BeginAction( typeof( Golem ) );
									Timer.DelayCall( TimeSpan.FromSeconds( 12.0 ), new TimerStateCallback( EndGolemRepair ), from );
								}
								else
								{
									number = 1044037; // You do not have sufficient metal to make that.
								}
							}
							else
							{
								number = 1044037; // You do not have sufficient metal to make that.
							}
						}
					}
				}
				else if ( targeted is BaseWeapon )
				{
					BaseWeapon weapon = (BaseWeapon)targeted;
					SkillName skill = m_CraftSystem.MainSkill;
					int toWeaken = 0;

					if ( Core.AOS )
					{
						toWeaken = 1;
					}
					else if ( skill != SkillName.Tailoring )
					{
						double skillLevel = from.Skills[skill].Base;

						if ( skillLevel >= 90.0 )
							toWeaken = 1;
						else if ( skillLevel >= 70.0 )
							toWeaken = 2;
						else
							toWeaken = 3;
					}

					if ( m_CraftSystem.CraftItems.SearchForSubclass( weapon.GetType() ) == null && !IsSpecialWeapon( weapon ) )
					{
						number = 1044277; // That item cannot be repaired.
					}
					else if ( !weapon.IsChildOf( from.Backpack ) )
					{
						number = 1044275; // The item must be in your backpack to repair it.
					}
					else if ( weapon.MaxHitPoints <= 0 || weapon.HitPoints == weapon.MaxHitPoints )
					{
						number = 1044281; // That item is in full repair
					}
					else if ( weapon.MaxHitPoints <= toWeaken )
					{
						number = 1044278; // That item has been repaired many times, and will break if repairs are attempted again.
					}
					else
					{
						if ( CheckWeaken( from, skill, weapon.HitPoints, weapon.MaxHitPoints ) )
						{
							weapon.MaxHitPoints -= toWeaken;
							weapon.HitPoints = Math.Max( 0, weapon.HitPoints - toWeaken );
						}

						if ( CheckRepairDifficulty( from, skill, weapon.HitPoints, weapon.MaxHitPoints ) )
						{
							number = 1044279; // You repair the item.
							m_CraftSystem.PlayCraftEffect( from );
							weapon.HitPoints = weapon.MaxHitPoints;
						}
						else
						{
							number = 1044280; // You fail to repair the item.
							m_CraftSystem.PlayCraftEffect( from );
						}
					}
				}
				else if ( targeted is BaseArmor )
				{
					BaseArmor armor = (BaseArmor)targeted;
					SkillName skill = m_CraftSystem.MainSkill;
					int toWeaken = 0;

					if ( Core.AOS )
					{
						toWeaken = 1;
					}
					else if ( skill != SkillName.Tailoring )
					{
						double skillLevel = from.Skills[skill].Base;

						if ( skillLevel >= 90.0 )
							toWeaken = 1;
						else if ( skillLevel >= 70.0 )
							toWeaken = 2;
						else
							toWeaken = 3;
					}

					if ( m_CraftSystem.CraftItems.SearchForSubclass( armor.GetType() ) == null )
					{
						number = 1044277; // That item cannot be repaired.
					}
					else if ( !armor.IsChildOf( from.Backpack ) )
					{
						number = 1044275; // The item must be in your backpack to repair it.
					}
					else if ( armor.MaxHitPoints <= 0 || armor.HitPoints == armor.MaxHitPoints )
					{
						number = 1044281; // That item is in full repair
					}
					else if ( armor.MaxHitPoints <= toWeaken )
					{
						number = 1044278; // That item has been repaired many times, and will break if repairs are attempted again.
					}
					else
					{
						if ( CheckWeaken( from, skill, armor.HitPoints, armor.MaxHitPoints ) )
						{
							armor.MaxHitPoints -= toWeaken;
							armor.HitPoints = Math.Max( 0, armor.HitPoints - toWeaken );
						}

						if ( CheckRepairDifficulty( from, skill, armor.HitPoints, armor.MaxHitPoints ) )
						{
							number = 1044279; // You repair the item.
							m_CraftSystem.PlayCraftEffect( from );
							armor.HitPoints = armor.MaxHitPoints;
						}
						else
						{
							number = 1044280; // You fail to repair the item.
							m_CraftSystem.PlayCraftEffect( from );
						}
					}
				}
				else if ( targeted is BaseClothing )
				{
					BaseClothing clothing = (BaseClothing)targeted;
					SkillName skill = m_CraftSystem.MainSkill;
					int toWeaken = 0;

					if ( Core.AOS )
					{
						toWeaken = 1;
					}
					else if ( skill != SkillName.Tailoring )
					{
						double skillLevel = from.Skills[skill].Base;

						if ( skillLevel >= 90.0 )
							toWeaken = 1;
						else if ( skillLevel >= 70.0 )
							toWeaken = 2;
						else
							toWeaken = 3;
					}

 					if (m_CraftSystem.CraftItems.SearchForSubclass(clothing.GetType()) == null && !IsSpecialClothing(clothing) && !((targeted is TribalMask) || (targeted is HornedTribalMask)) )
 					{
						number = 1044277; // That item cannot be repaired.
					}
					else if ( !clothing.IsChildOf( from.Backpack ) )
					{
						number = 1044275; // The item must be in your backpack to repair it.
					}
					else if ( clothing.MaxHitPoints <= 0 || clothing.HitPoints == clothing.MaxHitPoints )
					{
						number = 1044281; // That item is in full repair
					}
					else if ( clothing.MaxHitPoints <= toWeaken )
					{
						number = 1044278; // That item has been repaired many times, and will break if repairs are attempted again.
					}
					else
					{
						if ( CheckWeaken( from, skill, clothing.HitPoints, clothing.MaxHitPoints ) )
						{
							clothing.MaxHitPoints -= toWeaken;
							clothing.HitPoints = Math.Max( 0, clothing.HitPoints - toWeaken );
						}

						if ( CheckRepairDifficulty( from, skill, clothing.HitPoints, clothing.MaxHitPoints ) )
						{
							number = 1044279; // You repair the item.
							m_CraftSystem.PlayCraftEffect( from );
							clothing.HitPoints = clothing.MaxHitPoints;
						}
						else
						{
							number = 1044280; // You fail to repair the item.
							m_CraftSystem.PlayCraftEffect( from );
						}
					}
				}
				else if ( targeted is Item )
				{
					number = 1044277; // That item cannot be repaired.
				}
				else
				{
					number = 500426; // You can't repair that.
				}

				CraftContext context = m_CraftSystem.GetContext( from );
				from.SendGump( new CraftGump( from, m_CraftSystem, m_Tool, number ) );
			}
		}
	}
}