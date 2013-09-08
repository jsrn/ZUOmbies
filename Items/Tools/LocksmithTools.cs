using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
	public enum LocksmithCommand
	{
		None,
		UnlockDoor,
		Relock,
		ForgeKey
	}

	public class LocksmithTools : Item
	{
		private int m_Charges;

		[CommandProperty( AccessLevel.GameMaster )]
		public int Charges
		{
			get{ return m_Charges; }
			set{ m_Charges = value; }
		}

		private LocksmithCommand m_Command;

		[CommandProperty( AccessLevel.GameMaster )]
		public LocksmithCommand Command{ get{ return m_Command; } set{ m_Command = value; InvalidateProperties(); } }

		[Constructable]
		public LocksmithTools() : base( 0x1EBA )
		{
			Weight = 1.0;
			Name = "locksmith tools";
			m_Charges = 10;
		}

		public override int LabelNumber{ get{ return 1041280; } } // an interior decorator

		public LocksmithTools( Serial serial ) : base( serial )
		{
		}

		public void Consume()
		{
			m_Charges -= 1;

			if ( m_Charges == 0 )
				Delete();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.WriteEncodedInt( (int) m_Charges );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			m_Charges = reader.ReadEncodedInt();
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !CheckUse( this, from ) )
				return;
			
			if ( from.FindGump( typeof( LocksmithTools.InternalGump ) ) == null )
				from.SendGump( new InternalGump( this ) );

			if ( m_Command != LocksmithCommand.None )
				from.Target = new InternalTarget( this );
		}

		public static bool CheckUse( LocksmithTools tool, Mobile from )
		{
			if ( !BasePotion.HasFreeHand( from ) )
			{
				from.SendMessage( "You need a free hand to use these tools." );
				return false;
			}
			return true;
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, Name );
			LabelTo( from, "Charges: " + m_Charges );
		}

		private class InternalGump : Gump
		{
			private LocksmithTools m_Decorator;

			public InternalGump( LocksmithTools decorator ) : base( 150, 50 )
			{
				m_Decorator = decorator;

				AddBackground( 0, 0, 200, 200, 2600 );

				AddButton( 50, 45, ( decorator.Command == LocksmithCommand.UnlockDoor ? 2154 : 2152 ), 2154, 1, GumpButtonType.Reply, 0 );
				AddHtml( 90, 50, 70, 40, "Unlock Door", false, false );

				AddButton( 50, 95, ( decorator.Command == LocksmithCommand.Relock ? 2154 : 2152 ), 2154, 2, GumpButtonType.Reply, 0 );
				AddHtml( 90, 100, 70, 40, "Relock", false, false );

				AddButton( 50, 145, ( decorator.Command == LocksmithCommand.ForgeKey ? 2154 : 2152 ), 2154, 3, GumpButtonType.Reply, 0 );
				AddHtml( 90, 150, 70, 40, "Forge Key", false, false );
			}

			public override void OnResponse( NetState sender, RelayInfo info )
			{
				LocksmithCommand command = LocksmithCommand.None;

				switch ( info.ButtonID )
				{
					case 1: command = LocksmithCommand.UnlockDoor; break;
					case 2: command = LocksmithCommand.Relock; break;
					case 3: command = LocksmithCommand.ForgeKey; break;
				}

				if ( command != LocksmithCommand.None )
				{
					m_Decorator.Command = command;
					sender.Mobile.SendGump( new InternalGump( m_Decorator ) );
					sender.Mobile.Target = new InternalTarget( m_Decorator );
				}
				else
					Target.Cancel( sender.Mobile );
			}
		}

		private class InternalTarget : Target
		{
			private LocksmithTools m_Decorator;

			public InternalTarget( LocksmithTools decorator ) : base( -1, false, TargetFlags.None )
			{
				CheckLOS = false;

				m_Decorator = decorator;
			}

			protected override void OnTargetNotAccessible( Mobile from, object targeted )
			{
				OnTarget( from, targeted );
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( targeted is Item && LocksmithTools.CheckUse( m_Decorator, from ) )
				{
					Item item = (Item)targeted;
					
					switch ( m_Decorator.Command )
					{
						case LocksmithCommand.Relock:	Relock( item, from );	break;
						case LocksmithCommand.ForgeKey:	ForgeKey( item, from );	break;
						case LocksmithCommand.UnlockDoor:	UnlockDoor( item, from );	break;
					}
				}
				
				from.Target = new InternalTarget( m_Decorator );
			}
			
			protected override void OnTargetCancel( Mobile from, TargetCancelType cancelType )
			{
				if ( cancelType == TargetCancelType.Canceled )
					from.CloseGump( typeof( LocksmithTools.InternalGump ) );
			}

			private void UnlockDoor( Item item, Mobile from )
			{
				if ( !(item is BaseDoor) )
				{
					from.SendMessage( "That is not a door." );
					Target.Cancel( from );
					return;
				}

				BaseDoor door = (BaseDoor)item;

				if( !door.Locked )
				{
					from.SendMessage( "That door is not locked." );
					Target.Cancel( from );
					return;
				}

				double requiredLockpicking = 100.0;
				double requiredTinkering = 20.0;

				double hayDoorSkill = 00.0;
				double lightWoodDoorSkill = 10.0;
				double unbracedDarkwoodDoorSkill = 20.0;
				double bracedDarkwoodDoorSkill = 40.0;
				double metalDoorSkill = 80.0;

				int id = door.ClosedID;

				if ( id == 1685 || id == 1687 || id == 1693 || id == 1695 )
					requiredTinkering = hayDoorSkill;
				else if ( id == 1749 || id == 1751 || id == 1757 || id == 1759 )
					requiredTinkering = lightWoodDoorSkill;
				else if ( id == 1701 || id == 1703 || id == 1709 || id == 1711 )
					requiredTinkering = unbracedDarkwoodDoorSkill;
				else if ( id == 1765 || id == 1767 || id == 1773 || id == 1775 )
					requiredTinkering = bracedDarkwoodDoorSkill;
				else if ( id == 1653 || id == 1655 || id == 1661 || id == 1663 )
					requiredTinkering = metalDoorSkill;

				if ( from.Skills[SkillName.Lockpicking].Value < requiredLockpicking )
				{
					from.SendMessage( "You try to pick the door's lock, but you have no idea what you're doing." );
					Target.Cancel( from );
					return;
				}

				if ( from.Skills[SkillName.Tinkering].Value < requiredTinkering )
				{
					from.SendMessage( "You do not have enough tinkering ability to pick this type of lock." );
					Target.Cancel( from );
					return;
				}

				if ( Utility.RandomMinMax( 1, 10 ) == 1 )
				{
					from.SendMessage( "You successfully pick the lock." );
					door.Locked = false;
					m_Decorator.Consume();
				}
				else
				{
					from.SendMessage( "You try to pick the lock, but accidentally break the pick." );
					from.PlaySound( 0x3A4 );
					m_Decorator.Consume();
				}

				Target.Cancel( from );
			}

			private void Relock( Item item, Mobile from )
			{
				from.SendMessage( "Not yet implemented." );
			}

			private void ForgeKey( Item item, Mobile from )
			{
				from.SendMessage( "Not yet implemented." );
			}
		}
	}
}