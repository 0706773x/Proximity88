using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;
using TFGProximity.Core.Enums;
using TFGProximity.Core.Models;
using TFGProximity.Core.Services;

namespace TFGProximity.Core.Managers
{
	public class BeaconDataManager : IBeaconDataManager
	{
		private const string BeaconsCacheKey = "beacons";

		protected IBeaconDataService BeaconDataService { get; }

		public BeaconDataManager(IBeaconDataService beaconDataService)
		{
			BeaconDataService = beaconDataService;
		}

		public async Task<IList<TFGBeacon>> GetBeaconsAsync(int major, int minor, bool forceRefresh = false)
		{
			Debug.WriteLine ("BeaconDataManager GetBeaconsAsync");

			if (forceRefresh) {
				//await BlobCache.LocalMachine.InvalidateObject<List<TFGBeacon>>(BeaconsCacheKey);
				await BlobCache.LocalMachine.Invalidate (BeaconsCacheKey);
				await BlobCache.LocalMachine.Flush ();
			}

			var beacons = await BlobCache.LocalMachine.GetOrFetchObject(
				BeaconsCacheKey,
				async () =>
				{
					var res = await BeaconDataService.GetBeaconsAsync(major, minor);

					return res;
				}, DateTimeOffset.Now.AddDays(1));

#if DEBUG
			UpdateThreshholdDistances (ref beacons);
#endif

			return beacons;
		}

		private void UpdateThreshholdDistances (ref IList<TFGBeacon> beacons)
		{
			foreach (var beacon in beacons) {
				switch (beacon.BeaconRole) {
					case BeaconRoleEnum.Sentinel:
						beacon.DeviceDetail.EntryDistance = 2;
						beacon.DeviceDetail.ExitDistance = 2;
						break;

					case BeaconRoleEnum.Entry:
						beacon.DeviceDetail.EntryDistance = 2;
						beacon.DeviceDetail.ExitDistance = 3;
						break;

					case BeaconRoleEnum.Proximity:
						beacon.DeviceDetail.EntryDistance = 0.75;
						beacon.DeviceDetail.ExitDistance = 1;
						break;
				}
			}
		}
	}
}
