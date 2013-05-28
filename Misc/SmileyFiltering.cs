using System;
using Server;
using Server.Network;

namespace Server.Misc
{
	public class SmileyFiltering
	{
		private static bool Enabled = true;

		public static void Initialize()
		{
			if ( Enabled )
			{
				EventSink.Speech += new SpeechEventHandler( EventSink_Speech );
			}
		}

		private static void EventSink_Speech( SpeechEventArgs e )
		{
			e.Speech = ReplaceSmileyFaces( e.Speech );
		}

		private static string ReplaceSmileyFaces( string text )
		{
			string[] m_Disallowed = new string[]
			{
				":)", ":(", ":P", ":p",
				":D", "XD", "xD", ":O",
				":o", ":s", ":S", ":|",
				";)", ":\\", ":/", "\\o/",
				":C", ":c"
			};

			string[] m_Replacements = new string[]
			{
				"*smiles*", "*frowns*", "*sticks out tongue*", "*sticks out tongue*",
				"*grins*", "*grins*", "*grins*", "*looks shocked*",
				"*looks shocked*", "*looks concerned*", "*looks concerned*", "*blank stare*",
				"*winks*", "*looks unimpressed*", "*looks unimpressed*", "*waves arms*",
				"*looks sad*", "*looks sad*"
			};

			for ( int i = 0; i < m_Disallowed.Length; ++i )
			{
				text = text.Replace( m_Disallowed[i], m_Replacements[i] );
			}
			return text;
		}
	}
}