using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CoreLocation;
using Foundation;
using Microsoft.Practices.Unity;
using Prism.Unity;
using TFGProximity.Core.Helpers;
using TFGProximity.Core.Models;
using TFGProximity.Core.Services;

namespace TFGProximity.Droid.Plugins
{
    public class NativeBeaconService : IBeaconService
    {
        private bool _isRanging;

        //private readonly IList<BeaconRegion> _rangingRegions;

        private CLLocationManager _locationManager;

        public NativeBeaconService()
        {
            _locationManager = new CLLocationManager();

            //_rangingRegions = new List<BeaconRegion> ();

            //UpdateRangingList ();

            WhenRanged = Observable
                .FromEventPattern<IEnumerable<IBeacon>>(
                    x => Ranged += x,
                    x => Ranged -= x
                )
                .Select(x => x.EventArgs);

            _locationManager.RegionEntered += (sender, args) => {
                /*var logger = (Core.App.Current as PrismApplication).Container.Resolve<Core.Interfaces.ILogger> ();
				logger.Trace ("NativeBeaconService RegionEntered");*/

                var region = FromNative(args.Region);

                //var region = RangingRegions?.FirstOrDefault ();

                //logger.Trace ($"NativeBeaconService RegionEntered {region.Identifier}");

                if (region != null)
                {
                    OnRegionStatusChanged(region, true);
                }
            };

            _locationManager.RegionLeft += (sender, args) => {
                /*var logger = (Core.App.Current as PrismApplication).Container.Resolve<Core.Interfaces.ILogger> ();
				logger.Trace ("NativeBeaconService RegionLeft");*/

                var region = FromNative(args.Region);

                //var region = RangingRegions?.FirstOrDefault ();

                if (region != null)
                {
                    OnRegionStatusChanged(region, false);
                }
            };

            _locationManager.DidRangeBeacons += (sender, args) => {
                if (args.Beacons != null && args.Beacons.Any())
                {
                    var beacons = args.Beacons.Select(x => new Helpers.Beacon(x));
                    OnRanged(beacons);
                }
            };
        }

        //public IReadOnlyList<BeaconRegion> RangingRegions { get; private set; }

        public IObservable<IEnumerable<IBeacon>> WhenRanged { get; }

        public event EventHandler<IEnumerable<IBeacon>> Ranged;
        public event EventHandler<BeaconEventArgs> RegionEntered;
        public event EventHandler<BeaconEventArgs> RegionExited;

        public async Task<bool> InitAsync()
        {
            var tcs = new TaskCompletionSource<bool>();

            var funcPnt = new EventHandler<CLAuthorizationChangedEventArgs>((sender, args) => {
                if (args.Status == CLAuthorizationStatus.NotDetermined)
                {
                    return; // not done yet
                }

                var success = args.Status == CLAuthorizationStatus.AuthorizedAlways;

                tcs.TrySetResult(success);
            });

            _locationManager.AuthorizationChanged += funcPnt;

            _locationManager.RequestAlwaysAuthorization();

            var status = await tcs.Task;

            _locationManager.AuthorizationChanged -= funcPnt;

            //_inited = true;

            return status;
        }

        public void StartMonitoring(BeaconRegion region)
        {
            Debug.WriteLine($"NativeBeaconService StartMonitoring {region.Identifier}");

            var native = ToNative(region);

            _locationManager.StartMonitoring(native);
        }

        public void StartRanging(BeaconRegion region)
        {
            if (!_isRanging)
            {
                _isRanging = true;
                Debug.WriteLine($"NativeBeaconService StartRanging {region.Identifier}");

                //_rangingRegions.Add (region);
                //UpdateRangingList ();

                var native = ToNative(region);
                _locationManager.StartRangingBeacons(native);
            }
        }

        /*public void StopAllRanging ()
		{
			if (_isRanging) {
				_isRanging = false;

				Debug.WriteLine ("NativeBeaconService StopAllRanging");

				var list = _rangingRegions.ToList ();

				foreach (var region in list) {
					var native = ToNative (region);
					_locationManager.StopRangingBeacons (native);
				}

				_rangingRegions.Clear ();
				UpdateRangingList ();
			}
		}*/

        public void StopMonitoring(BeaconRegion region)
        {
            Debug.WriteLine($"NativeBeaconService StopMonitoring {region.Identifier}");

            var native = ToNative(region);

            _locationManager.StopMonitoring(native);
        }

        public void StopRanging(BeaconRegion region)
        {
            if (_isRanging)
            {
                _isRanging = false;
                Debug.WriteLine($"NativeBeaconService StopRanging {region.Identifier}");

                var native = ToNative(region);
                _locationManager.StopRangingBeacons(native);
            }
        }

        private void OnRegionStatusChanged(BeaconRegion region, bool entering)
        {
            if (entering)
            {
                RegionEntered?.Invoke(this, new BeaconEventArgs(region));
            }
            else
            {
                RegionExited?.Invoke(this, new BeaconEventArgs(region));
            }
        }

        private void OnRanged(IEnumerable<IBeacon> beacons)
        {
            /*var beacon = beacons.FirstOrDefault ();

			if (beacon != null) {
				Debug.WriteLine ($"NativeBeaconService OnRanged {beacon.UUID} {beacon.Major}_{beacon.Minor} :: {beacon.Accuracy}");

				Ranged?.Invoke (this, beacon);
			}*/

            /*foreach (var beacon in beacons) {
				Debug.WriteLine ($"NativeBeaconService OnRanged {beacon.UUID} {beacon.Major}_{beacon.Minor} :: {beacon.Accuracy}");
			}*/

            Ranged?.Invoke(this, beacons);
        }

        /*private void UpdateRangingList ()
		{
			//RangingRegions = new ReadOnlyCollection<BeaconRegion> (_rangingRegions);
		}*/

        private static BeaconRegion FromNative(CLRegion native)
        {
            return new BeaconRegion(string.Empty, native.Identifier);
        }

        private static BeaconRegion FromNative(CLBeaconRegion native)
        {
            Debug.WriteLine($"NativeBeaconService FromNative Radius {native.Radius}");

            return new BeaconRegion(
                native.ProximityUuid.AsString(),
                native.Identifier,
                native.Major?.UInt16Value,
                native.Minor?.UInt16Value
            );
        }

        private static CLBeaconRegion ToNative(BeaconRegion region)
        {
            var uuid = new NSUuid(region.UUID);
            CLBeaconRegion native = null;

            if (region.Major > 0 && region.Minor > 0)
            {
                native = new CLBeaconRegion(uuid, region.Major.Value, region.Minor.Value, region.Identifier);
            }
            else if (region.Major > 0)
            {
                native = new CLBeaconRegion(uuid, region.Major.Value, region.Identifier);
            }
            else
            {
                native = new CLBeaconRegion(uuid, region.Identifier);
            }

            native.NotifyEntryStateOnDisplay = true;
            native.NotifyOnEntry = true;
            native.NotifyOnExit = true;

            return native;
        }
    }
}
