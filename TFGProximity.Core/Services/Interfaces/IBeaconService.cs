using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TFGProximity.Core.Helpers;
using TFGProximity.Core.Models;

namespace TFGProximity.Core.Services
{
	public interface IBeaconService
	{
		Task<bool> InitAsync ();

		IObservable<IEnumerable<IBeacon>> WhenRanged { get; }

		event EventHandler<IEnumerable<IBeacon>> Ranged;
		event EventHandler<BeaconEventArgs> RegionEntered;
		event EventHandler<BeaconEventArgs> RegionExited;

		//IReadOnlyList<BeaconRegion> RangingRegions { get; }
		void StartRanging (BeaconRegion region);
		void StopRanging (BeaconRegion region);
		//void StopAllRanging ();

		void StartMonitoring (BeaconRegion region);
		void StopMonitoring (BeaconRegion region);
	}
}
