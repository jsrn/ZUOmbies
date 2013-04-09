using Server;
using Server.Mobiles;
using System;
using System.Xml;

namespace Server.Regions
{
	public class FreeDeathZone : BaseRegion
	{

		public FreeDeathZone( XmlElement xml, Map map, Region parent ) : base( xml, map, parent )
		{
		}
		
		public override void OnEnter( Mobile m )
		{
			if( m is PlayerMobile ){
				PlayerMobile p = (PlayerMobile)m;
				p.SetFreeDeaths(true);
			}
		}

		public override void OnExit( Mobile m )
		{
			if( m is PlayerMobile ){
				PlayerMobile p = (PlayerMobile)m;
				p.SetFreeDeaths(false);
			}
		}

	}
}
