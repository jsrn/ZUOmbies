using System;
using Server;
using Server.Network;

namespace Server.Misc
{
	public enum ProfanityAction
	{
		None,			// no action taken
		Disallow,		// speech is not displayed
		Criminal,		// makes the player criminal, not killable by guards
		CriminalAction,	// makes the player criminal, can be killed by guards
		Disconnect,		// player is kicked
		Other			// some other implementation
	}

	public class ProfanityProtection
	{
		private static bool Enabled = false;
		private static ProfanityAction Action = ProfanityAction.Other; // change here what to do when profanity is detected

		public static void Initialize()
		{
			if ( Enabled )
				EventSink.Speech += new SpeechEventHandler( EventSink_Speech );
		}

		private static bool OnProfanityDetected( Mobile from, string speech )
		{
			switch ( Action )
			{
				case ProfanityAction.None: return true;
				case ProfanityAction.Disallow: return false;
				case ProfanityAction.Criminal: from.Criminal = true; return true;
				case ProfanityAction.CriminalAction: from.CriminalAction( false ); return true;
				case ProfanityAction.Disconnect:
				{
					NetState ns = from.NetState;

					if ( ns != null )
						ns.Dispose();

					return false;
				}
				default:
				case ProfanityAction.Other: // TODO: Provide custom implementation if this is chosen
				{
					// This is where we need to replace the offending part of the text with a more RP friendly
					// alternative

					return true;
				}
			}
		}

		private static void EventSink_Speech( SpeechEventArgs e )
		{
			Mobile from = e.Mobile;

			if ( from.AccessLevel > AccessLevel.Player )
				return;

			if ( !NameVerification.Validate( e.Speech, 0, int.MaxValue, true, true, false, int.MaxValue, m_Exceptions, m_Disallowed, m_StartDisallowed ) )
				e.Blocked = !OnProfanityDetected( from, e.Speech );
		}

		public static char[]	Exceptions{	get{ return m_Exceptions; } }
		public static string[]	StartDisallowed{ get{ return m_StartDisallowed; } }
		public static string[]	Disallowed{ get{ return m_Disallowed; } }

		private static char[] m_Exceptions = new char[]
			{
				' ', '-', '.', '\'', '"', ',', '_', '+', '=', '~', '`', '!', '^', '*', '\\', '/', ';', ':', '<', '>', '[', ']', '{', '}', '?', '|', '(', ')', '%', '$', '&', '#', '@'
			};

		private static string[] m_StartDisallowed = new string[]{};

		private static string[] m_Disallowed = new string[]
			{
				":)",
				"=)",
				":(",
				"=(",
				":P",
				":D",
				":p",
				"XD",
				"xD"
			};

		private static string[] m_Replacements = new string[]
			{
				"*smiles*",
				"*smiles*",
				"*frowns*",
				"*frowns*",
				"*sticks out tongue*",
				"*grins*",
				"*sticks out tongue*",
				"*grins*",
				"*grins*"
			};
	}
}