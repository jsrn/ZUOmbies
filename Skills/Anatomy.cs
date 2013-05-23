using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.SkillHandlers
{
	public class Anatomy
	{
		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.Anatomy].Callback = new SkillUseCallback( OnUse );
		}

		public static TimeSpan OnUse( Mobile m )
		{
			m.Target = new Anatomy.InternalTarget();

			m.SendLocalizedMessage( 500321 ); // Whom shall I examine?

			return TimeSpan.FromSeconds( 1.0 );
		}

		private class InternalTarget : Target
		{
			public InternalTarget() :  base ( 8, false, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( targeted is TownCrier )
				{
					((TownCrier)targeted).PrivateOverheadMessage( MessageType.Regular, 0x3B2, 500322, from.NetState ); // This person looks fine to me, though he may have some news...
				}
				else if ( targeted is BaseVendor && ((BaseVendor)targeted).IsInvulnerable )
				{
					((BaseVendor)targeted).PrivateOverheadMessage( MessageType.Regular, 0x3B2, 500326, from.NetState ); // That can not be inspected.
				}
				else if ( targeted is Mobile )
				{
					Mobile targ = (Mobile)targeted;

					if( targ is PlayerMobile )
					{
						PlayerMobile targPM = (PlayerMobile)targ;
						int targetInjuryPoints = targPM.getDeathPoints();

						string message = "";

						if( from == targeted ){
							message += "You ";
						} else {
							message += "They ";
						}

						if( targetInjuryPoints == 0 )
						{
							message += "look fine. [0/30]";
						}
						else if( targetInjuryPoints < 10 )
						{
							message += "look a bit bruised up. [" + targetInjuryPoints + "/30]";
						}
						else if( targetInjuryPoints < 20 )
						{
							message += "look quite badly beaten. [" + targetInjuryPoints + "/30]";
						}
						else if( targetInjuryPoints < 30 )
						{
							message += "are badly injured! [" + targetInjuryPoints + "/30]";
						}

						targ.PrivateOverheadMessage( MessageType.Regular, 0x3B2, true, message, from.NetState );

						if( from == targeted ){
							int hungerAmount = ((PlayerMobile)targ).Hunger;
							string hungerMessage = "";

							if( hungerAmount == 1)
								hungerMessage += "You are stuffed! [" + hungerMessage + "/20]";
							else if (hungerAmount < 5)
								hungerMessage += "You are peckish. [" + hungerMessage + "/20]";
							else if (hungerAmount < 10)
								hungerMessage += "You are a little hungry. [" + hungerMessage + "/20]";
							else if (hungerAmount < 15)
								hungerMessage += "You are quite hungry. [" + hungerMessage + "/20]";
							else
								hungerMessage += "You are very hungry!. [" + hungerMessage + "/20]";

							targ.PrivateOverheadMessage( MessageType.Regular, 0x3B2, true, hungerMessage, from.NetState );
						}
					}
					else
					{
						int marginOfError = Math.Max( 0, 25 - (int)(from.Skills[SkillName.Anatomy].Value / 4) );

						int str = targ.Str + Utility.RandomMinMax( -marginOfError, +marginOfError );
						int dex = targ.Dex + Utility.RandomMinMax( -marginOfError, +marginOfError );
						int stm = ((targ.Stam * 100) / Math.Max( targ.StamMax, 1 )) + Utility.RandomMinMax( -marginOfError, +marginOfError );

						int strMod = str / 10;
						int dexMod = dex / 10;
						int stmMod = stm / 10;

						if ( strMod < 0 ) strMod = 0;
						else if ( strMod > 10 ) strMod = 10;

						if ( dexMod < 0 ) dexMod = 0;
						else if ( dexMod > 10 ) dexMod = 10;

						if ( stmMod > 10 ) stmMod = 10;
						else if ( stmMod < 0 ) stmMod = 0;

						if ( from.CheckTargetSkill( SkillName.Anatomy, targ, 0, 100 ) )
						{
							targ.PrivateOverheadMessage( MessageType.Regular, 0x3B2, 1038045 + (strMod * 11) + dexMod, from.NetState ); // That looks [strong] and [dexterous].

							if ( from.Skills[SkillName.Anatomy].Base >= 65.0 )
								targ.PrivateOverheadMessage( MessageType.Regular, 0x3B2, 1038303 + stmMod, from.NetState ); // That being is at [10,20,...] percent endurance.
						}
						else
						{
							targ.PrivateOverheadMessage( MessageType.Regular, 0x3B2, 1042666, from.NetState ); // You can not quite get a sense of their physical characteristics.
						}
					}
				}
				else if ( targeted is Item )
				{
					((Item)targeted).SendLocalizedMessageTo( from, 500323, "" ); // Only living things have anatomies!
				}
			}
		}
	}
}