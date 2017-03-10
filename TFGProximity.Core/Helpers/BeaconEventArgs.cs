using System;
using TFGProximity.Core.Models;

namespace TFGProximity.Core.Helpers
{
	public class BeaconEventArgs : EventArgs
	{
		public BeaconRegion BeaconRegion { get; private set; }

		public BeaconEventArgs(BeaconRegion beaconRegion)
		{
			BeaconRegion = beaconRegion;
		}
	}

	public class BeaconRangedEventArgs : BeaconEventArgs
	{
		public double Distance { get; private set; }

		public BeaconRangedEventArgs (BeaconRegion beaconRegion, double distance) : base (beaconRegion)
		{
			Distance = distance;
		}
	}
}
