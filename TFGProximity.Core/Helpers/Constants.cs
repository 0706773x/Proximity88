using System;
using TFGProximity.Core.Models;

namespace TFGProximity.Core.Helpers
{
	public static class Constants
	{
		//public static string UUID => "6ebe7ef8-cd57-11e4-afdc-1681e6b88ec1";
		//public static string UUID => "f7826da6-4fa2-4e98-8024-bc5b71e0893e";
		public static string UUID => "B9407F30-F5F8-466E-AFF9-25556B57FE6D";
		public static string RegionName => "TFGProximity";

		public static BeaconRegion BeaconRegion => new BeaconRegion (Constants.UUID, Constants.RegionName);
	}
}
