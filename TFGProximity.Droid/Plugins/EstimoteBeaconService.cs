using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EstimoteSdk;
using Microsoft.Practices.Unity;
using Prism.Unity;
using TFGProximity.Core.Helpers;
using TFGProximity.Core.Interfaces;
using TFGProximity.Core.Models;
using TFGProximity.Core.Services;
using Xamarin.Forms;

namespace TFGProximity.Droid.Plugins
{
	public class EstimoteBeaconService : Java.Lang.Object, IBeaconService, BeaconManager.IServiceReadyCallback
	{
		private BeaconManager _beaconManager;

		private BeaconRegion _beaconRegion;

		private bool _isConnected;

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
			_beaconManager = new BeaconManager (Forms.Context);
		}

		public IObservable<IEnumerable<IBeacon>> WhenRanged { get; }

		public event EventHandler<IEnumerable<IBeacon>> Ranged;
		public event EventHandler<BeaconEventArgs> RegionEntered;
		public event EventHandler<BeaconEventArgs> RegionExited;

		public async Task<bool> InitAsync ()
		{
			await Task.Yield ();

			return true;

			/*var tcs = new TaskCompletionSource<bool> ();

			_beaconManager.Connect (this);

			var status = await tcs.Task;

			return status;*/
		}

		public void StartMonitoring (BeaconRegion region)
		{
			/*_beaconRegion = region;

			_beaconManager.EnteredRegion -= BeaconManager_EnteredRegion;
			_beaconManager.EnteredRegion += BeaconManager_EnteredRegion;

			_beaconManager.ExitedRegion -= BeaconManager_ExitedRegion;
			_beaconManager.ExitedRegion += BeaconManager_ExitedRegion;*/
		}

		public void StartRanging (BeaconRegion region)
		{
			_beaconRegion = region;

			_beaconManager.Ranging -= BeaconManager_Ranging;
			_beaconManager.Ranging += BeaconManager_Ranging;

			if (!_isConnected) {
				_beaconManager.Connect (this);
			}
		}

		public void StopMonitoring (BeaconRegion region)
		{
			/*var nativeRegion = ToNative (region);

			if (nativeRegion != null) {
				_beaconManager.StopMonitoring (nativeRegion);
				_beaconManager.Disconnect ();

				_isConnected = false;

				_beaconManager.EnteredRegion -= BeaconManager_EnteredRegion;
				_beaconManager.ExitedRegion -= BeaconManager_ExitedRegion;

				_beaconRegion = null;
			}*/
		}

		public void StopRanging (BeaconRegion region)
		{
			var nativeRegion = ToNative (region);

			if (nativeRegion != null) {
				_beaconManager.StopRanging (nativeRegion);
				_beaconManager.Disconnect ();

				_isConnected = false;

				_beaconManager.Ranging -= BeaconManager_Ranging;

				_beaconRegion = null;
			}
		}

		public void OnServiceReady ()
		{
			_isConnected = true;

			var nativeRegion = ToNative (_beaconRegion);

			if (nativeRegion != null) {
				// This method is called when BeaconManager is up and running.
				//_beaconManager.StartMonitoring (nativeRegion);
				_beaconManager.StartRanging (nativeRegion);
			}
		}

		private void OnRanged (IEnumerable<IBeacon> beacons)
		{
			Ranged?.Invoke (this, beacons);
		}

		private void BeaconManager_EnteredRegion (object sender, BeaconManager.EnteredRegionEventArgs e)
		{
			var region = FromNative (e.Region);

			if (region != null) {
				OnRegionStatusChanged (region, true);
			}
		}

		private void BeaconManager_ExitedRegion (object sender, BeaconManager.ExitedRegionEventArgs e)
		{
			var region = FromNative (e.P0);

			if (region != null) {
				OnRegionStatusChanged (region, false);
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

		private void BeaconManager_Ranging (object sender, BeaconManager.RangingEventArgs e)
		{
			var beacons = e.Beacons.Select (x => new Helpers.Beacon (x));

			if (beacons.Any ()) {
				OnRanged (beacons);
			}
		}

		private static BeaconRegion FromNative (Region nativeRegion)
		{
			var major = nativeRegion.Major == null ? (ushort)0 : (ushort)nativeRegion.Major;
			var minor = nativeRegion.Minor == null ? (ushort)0 : (ushort)nativeRegion.Minor;

			return new BeaconRegion (nativeRegion.ProximityUUID.ToString (),
									 nativeRegion.Identifier,
									 major, minor);
		}

		private static Region ToNative (BeaconRegion region)
		{
			Region nativeRegion = null;

			if (region.Major > 0 && region.Minor > 0) {
				nativeRegion = new Region (region.Identifier, region.UUID, (int)region.Major, (int)region.Minor);
			} else if (region.Major > 0) {
				nativeRegion = new Region (region.Identifier, region.UUID, (int)region.Major);
			} else {
				nativeRegion = new Region (region.Identifier, region.UUID);
			}

			return nativeRegion;
		}
	}
}
