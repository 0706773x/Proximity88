using System;
using TFGProximity.Core.Enums;
using TFGProximity.Core.Models;

namespace TFGProximity.Droid.Helpers
{
	public class Beacon : IBeacon
	{
		private readonly EstimoteSdk.Beacon _beacon;

		public Beacon (EstimoteSdk.Beacon beacon)
		{
			_beacon = beacon;
		}

		public double Accuracy {
			get {
				var distance = CalculateDistance (_beacon.MeasuredPower, _beacon.Rssi);

				return distance;
			}
		}

		public ushort Major => (ushort) _beacon.Major;

		public ushort Minor => (ushort)_beacon.Minor;

		/*public BeaconProximityEnum Proximity {
			get {
				throw new NotImplementedException ();
			}
		}*/

		public int RSSI => _beacon.Rssi;

		public string UUID => _beacon.ProximityUUID.ToString ();

		internal static double CalculateDistance (int txPower, double rssi)
		{
			if (rssi == 0) {
				return -1.0; // if we cannot determine accuracy, return -1.
			}

			double ratio = rssi * 1.0 / txPower;
			if (ratio < 1.0) {
				return Math.Pow (ratio, 10);
			} else {
				double accuracy = (0.89976) * Math.Pow (ratio, 7.7095) + 0.111;
				return accuracy;
			}
		}
	}
}
