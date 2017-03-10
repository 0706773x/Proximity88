using System;
namespace TFGProximity.Core.Enums
{
	[Flags]
	public enum BeaconProximityEnum
	{
		Unknown = 0,
		Immediate = 1,
		Near = 2,
		Far = 4
	}
}
