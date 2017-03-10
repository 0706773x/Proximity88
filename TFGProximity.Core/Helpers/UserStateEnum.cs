using System;
namespace TFGProximity.Core.Enums
{
	[Flags]
	public enum UserStateEnum
	{
		Unentered = 0,
		Detected = 1,
		Entered = 2,
		Proximity = 4
	}
}
