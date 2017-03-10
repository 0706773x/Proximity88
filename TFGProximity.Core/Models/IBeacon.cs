using System;
using TFGProximity.Core.Enums;

namespace TFGProximity.Core.Models
{
	public interface IBeacon
	{
		string UUID { get; }
		ushort Minor { get; }
		ushort Major { get; }

		double Accuracy { get; }
		//BeaconProximityEnum Proximity { get; }
		int RSSI { get; }
	}
}
