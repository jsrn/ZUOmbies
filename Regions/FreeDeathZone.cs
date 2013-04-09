using Server;
using Server.Mobiles;

namespace Server.Regions
{
	public class FreeDeathZone : BaseRegion
	{

		public FreeDeathZone( XmlElement xml, Map map, Region parent ) : base( xml, map, parent )
		{
		}
		
		public override void OnEnter( Mobile m )
		{
			if( typeof( m ) == typeof( PlayerMobile ) ){
				m.SetFreeDeaths(true);
			}
		}

		public override void OnExit( Mobile m )
		{
			if( typeof( m ) == typeof( PlayerMobile ) ){
				m.SetFreeDeaths(false);
			}
		}

	}
}
