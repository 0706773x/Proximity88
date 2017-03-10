using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CoreLocation;
using Estimote;
using Foundation;
using Microsoft.Practices.Unity;
using Prism.Unity;
using TFGProximity.Core.Helpers;
using TFGProximity.Core.Interfaces;
using TFGProximity.Core.Models;
using TFGProximity.Core.Services;

namespace TFGProximity.iOS.Plugins
{
	public class EstimoteBeaconService : IBeaconService
	{
		private BeaconManager _beaconManager;

		private ILogger _logger;
		private ILogger Logger {
			get {
				if (_logger == null) {
					_logger = (Core.App.Current as PrismApplication).Container.Resolve<ILogger> ();
				}

				return _logger;
			}
		}

		public EstimoteBeaconService ()
		{
			_beaconManager = new BeaconManager {
				ReturnAllRangedBeaconsAtOnce = true
			};

			WhenRanged = Observable
				.FromEventPattern<IEnumerable<IBeacon>> (
					x => Ranged += x,
					x => Ranged -= x
				)
				.Select (x => x.EventArgs);

			_beaconManager.EnteredRegion += (sender, args) => {
				var region = FromNative (args.Region);

				if (region != null) {
					OnRegionStatusChanged (region, true);
				}
			};

			_beaconManager.ExitedRegion += (sender, args) => {
				var region = FromNative (args.Region);

				if (region != null) {
					OnRegionStatusChanged (region, false);
				}
			};

			_beaconManager.RangedBeacons += (sender, args) => {
				var beacons = args.Beacons.Select (x => new Helpers.Beacon (x));

				if (beacons.Any ()) {
					OnRanged (beacons);
				}
			};
		}

		public IObservable<IEnumerable<IBeacon>> WhenRanged { get; }

		public event EventHandler<IEnumerable<IBeacon>> Ranged;
		public event EventHandler<BeaconEventArgs> RegionEntered;
		public event EventHandler<BeaconEventArgs> RegionExited;

		public async Task<bool> InitAsync ()
		{
			var tcs = new TaskCompletionSource<bool> ();

			var funcPnt = new EventHandler<AuthorizationStatusChangedEventArgs> ((sender, args) => {
				if (args.Status == CLAuthorizationStatus.NotDetermined) {
					return; // not done yet
				}

				var success = args.Status == CLAuthorizationStatus.AuthorizedAlways;

				tcs.TrySetResult (success);
			});

			_beaconManager.AuthorizationStatusChanged += funcPnt;

			_beaconManager.RequestAlwaysAuthorization ();

			var status = await tcs.Task;

			_beaconManager.AuthorizationStatusChanged -= funcPnt;

			return status;
		}

		public void StartMonitoring (BeaconRegion region)
		{
			Logger.Trace ($"EstimoteBeaconService StartMonitoring {region.Identifier}");

			var nativeRegion = ToNative (region);

			if (nativeRegion != null) {
				_beaconManager.StartMonitoringForRegion (nativeRegion);
			}
		}

		public void StopMonitoring (BeaconRegion region)
		{
			Logger.Trace ($"EstimoteBeaconService StopMonitoring {region.Identifier}");

			var nativeRegion = ToNative (region);

			if (nativeRegion != null) {
				_beaconManager.StopMonitoringForRegion (nativeRegion);
			}
		}

		public void StartRanging (BeaconRegion region)
		{
			Logger.Trace ($"EstimoteBeaconService StartRanging {region.Identifier}");

			var nativeRegion = ToNative (region);

			if (nativeRegion != null) {
				_beaconManager.StartRangingBeaconsInRegion (nativeRegion);
			}
		}

		public void StopRanging (BeaconRegion region)
		{
			Logger.Trace ($"EstimoteBeaconService StopRanging {region.Identifier}");

			var nativeRegion = ToNative (region);

			if (nativeRegion != null) {
				_beaconManager.StopRangingBeaconsInRegion (nativeRegion);
			}
		}

		private void OnRegionStatusChanged (BeaconRegion region, bool entering)
		{
			Logger.Trace ($"EstimoteBeaconService OnRegionStatusChanged {region.Identifier} {entering}");

			if (entering) {
				RegionEntered?.Invoke (this, new BeaconEventArgs (region));
			} else {
				RegionExited?.Invoke (this, new BeaconEventArgs (region));
			}
		}

		private void OnRanged (IEnumerable<IBeacon> beacons)
		{
			/*var beacon = beacons.FirstOrDefault ();

			if (beacon != null) {
				Ranged?.Invoke (this, beacon);
			}*/

			Ranged?.Invoke (this, beacons);
		}

		private static BeaconRegion FromNative (CLBeaconRegion nativeRegion)
		{
			return new BeaconRegion (
				nativeRegion.ProximityUuid.AsString (),
				nativeRegion.Identifier,
				nativeRegion.Major?.UInt16Value,
				nativeRegion.Minor?.UInt16Value
			);
		}

		private static CLBeaconRegion ToNative (BeaconRegion region)
		{
			var uuid = new NSUuid (region.UUID);
			CLBeaconRegion nativeRegion = null;

			if (region.Major > 0 && region.Minor > 0) {
				nativeRegion = new CLBeaconRegion (uuid, region.Major.Value, region.Minor.Value, region.Identifier);
			} else if (region.Major > 0) {
				nativeRegion = new CLBeaconRegion (uuid, region.Major.Value, region.Identifier);
			} else {
				nativeRegion = new CLBeaconRegion (uuid, region.Identifier);
			}

			nativeRegion.NotifyEntryStateOnDisplay = true;
			nativeRegion.NotifyOnEntry = true;
			nativeRegion.NotifyOnExit = true;

			return nativeRegion;
		}
	}
}
