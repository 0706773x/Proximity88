using System;
using CoreLocation;
using Foundation;
using TFGProximity.Core.Helpers;
using TFGProximity.Core.Services;

namespace TFGProximity.iOS.Plugins
{
	public class KontaktBeaconService : IBeaconService
	{
		private CLLocationManager _locationManager;
		private CLBeaconRegion _region;

		public KontaktBeaconService()
		{
			_locationManager = new CLLocationManager();

			_locationManager.RequestAlwaysAuthorization();
		}

		private void OnBeaconRanged(CLBeacon beacon)
		{
			var handle = RangedBeacon;

			if (handle != null)
			{
				handle(this, new BeaconEventArgs((int)beacon.Major, (int)beacon.Minor, beacon.Accuracy));
			}
		}

		private void OnMonitored()
		{
			var handle = Monitored;

			if (handle != null)
			{
				handle(this, EventArgs.Empty);
			}
		}

		private void OnRegionEntered()
		{
			var handle = RegionEntered;

			if (handle != null)
			{
				handle(this, EventArgs.Empty);
			}
		}

		private void OnRegionLeft()
		{
			var handle = RegionExited;

			if (handle != null)
			{
				handle(this, EventArgs.Empty);
			}
		}

		#region IBeaconHelper implementation

		public event EventHandler<BeaconEventArgs> RangedBeacon;
		public event EventHandler<EventArgs> Monitored;
		public event EventHandler<EventArgs> RegionEntered;
		public event EventHandler<EventArgs> RegionExited;

		public void Init (string uuid, string regionName)
		{
			_region = new CLBeaconRegion(new NSUuid(uuid), regionName);
			//_region = new CLBeaconRegion(new NSUuid("f7826da6-4fa2-4e98-8024-bc5b71e0893e"), "TFGProximity");
		}

		public void StartRangingBeacons()
		{
			// TODO :: check that region has been initialised

			_locationManager.StartRangingBeacons(_region);

			_locationManager.DidRangeBeacons += (sender, e) =>
			{
				foreach (var beacon in e.Beacons)
				{
					OnBeaconRanged(beacon);
				}
			};
		}

		public void StopRangingBeacons()
		{
			// TODO :: check that region has been initialised

			_locationManager.StopRangingBeacons(_region);
		}

		public void StartMonitoringBeacons()
		{
			// TODO :: check that region has been initialised

			_locationManager.StartMonitoring(_region);

			/*_locationManager.DidStartMonitoringForRegion += (sender, e) => {
				OnMonitored();
			};*/

			_locationManager.RegionEntered += (sender, e) =>
			{
				OnRegionEntered();
			};

			_locationManager.RegionLeft += (sender, e) =>
			{
				OnRegionLeft();
			};
		}

		public void StopMonitoringBeacons()
		{
			// TODO :: check that region has been initialised

			_locationManager.StopMonitoring(_region);
		}

		/*public bool CanRangeBeacons
		{
			get
			{
				return CLLocationManager.Status == CLAuthorizationStatus.Authorized || CLLocationManager.Status == CLAuthorizationStatus.NotDetermined;
			}
		}*/

		#endregion
	}
}
