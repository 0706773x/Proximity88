using System;
using CoreLocation;
using TFGProximity.Core.Enums;
using TFGProximity.Core.Models;

namespace TFGProximity.iOS.Helpers
{
	public class Beacon : IBeacon
	{
		private readonly CLBeacon _beacon;

		public Beacon (CLBeacon beacon)
		{
			_beacon = beacon;
		}

		public string UUID => _beacon.ProximityUuid.AsString ();

		public ushort Major => _beacon.Major?.UInt16Value ?? 0;

		public ushort Minor => _beacon.Minor?.UInt16Value ?? 0;

		/*public BeaconProximityEnum Proximity {
			get {
				switch (_beacon.Proximity) {
					case CLProximity.Unknown:
						return BeaconProximityEnum.Unknown;
					case CLProximity.Immediate:
						return BeaconProximityEnum.Immediate;
					case CLProximity.Near:
						return BeaconProximityEnum.Near;
					case CLProximity.Far:
						return BeaconProximityEnum.Far;
					default:
						return BeaconProximityEnum.Unknown;
				}
			}
		}*/

		public double Accuracy => _beacon.Accuracy;

		public int RSSI => (int)_beacon.Rssi;

		public override string ToString ()
		{
			return string.Format ("[Beacon: UUID={0}, Major={1}, Minor={2}, Accuracy={3}]", UUID, Major, Minor, Accuracy);
		}
	}
}
