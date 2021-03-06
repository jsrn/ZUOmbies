using System;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Targeting;
using Server.Regions;

namespace Server.SkillHandlers
{
	public class DetectHidden
	{
		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.DetectHidden].Callback = new SkillUseCallback( OnUse );
		}

		public static TimeSpan OnUse( Mobile src )
		{
			src.SendLocalizedMessage( 500819 );//Where will you search?
			src.Target = new InternalTarget();

			return TimeSpan.FromSeconds( 6.0 );
		}

		private class InternalTarget : Target
		{
			public InternalTarget() : base( 12, true, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile src, object targ )
			{
				bool foundAnyone = false;

				Point3D p;
				if ( targ is Mobile )
					p = ((Mobile)targ).Location;
				else if ( targ is Item )
					p = ((Item)targ).Location;
				else if ( targ is IPoint3D )
					p = new Point3D( (IPoint3D)targ );
				else 
					p = src.Location;

				double srcSkill = src.Skills[SkillName.DetectHidden].Value;
				int range = (int)(srcSkill / 10.0);

				// Checking for hidden stashes is free
				IPooledEnumerable itemsInRange = src.Map.GetItemsInRange( p, 1 );

				foreach ( Item item in itemsInRange )
				{
					if ( item is HiddenStash )
					{
						HiddenStash stash = (HiddenStash) item;

						src.SendMessage( "You inspect the area closely, and find a hidden stash." );

						stash.Visible = true;
						stash.BeginConceal();

						foundAnyone = true;
					}
				}

				if ( !src.CheckSkill( SkillName.DetectHidden, 0.0, 100.0 ) )
					range /= 2;

				if ( range > 0 )
				{
					IPooledEnumerable inRange = src.Map.GetMobilesInRange( p, range );

					foreach ( Mobile trg in inRange )
					{
						if ( trg.Hidden && src != trg )
						{
							double ss = srcSkill + Utility.Random( 21 ) - 10;
							double ts = trg.Skills[SkillName.Hiding].Value + Utility.Random( 21 ) - 10;

							if ( src.AccessLevel >= trg.AccessLevel && ss >= ts )
							{
								if ( trg is ShadowKnight && (trg.X != p.X || trg.Y != p.Y) )
									continue;

								trg.RevealingAction();
								trg.SendLocalizedMessage( 500814 ); // You have been revealed!
								foundAnyone = true;
							}
						}
					}

					inRange.Free();

					itemsInRange = src.Map.GetItemsInRange( p, range );

					foreach ( Item item in itemsInRange )
					{
						if ( item is BaseMine )
						{
							BaseMine trap = (BaseMine) item;

							if ( src.CheckTargetSkill( SkillName.DetectHidden, trap, 00.0, 100.0 ) )
							{
								src.SendMessage( "You inspect the area closely, and find a trap." );

								trap.Visible = true;
								trap.BeginConceal();

								foundAnyone = true;
							}
						}
					}

					itemsInRange.Free();
				}

				if ( !foundAnyone )
				{
					src.SendLocalizedMessage( 500817 ); // You can see nothing hidden there.
				}
			}
		}
	}
}
