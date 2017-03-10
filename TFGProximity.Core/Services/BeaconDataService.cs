using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TFGProximity.Core.Enums;
using TFGProximity.Core.Models;

namespace TFGProximity.Core.Services
{
	public class BeaconDataService : IBeaconDataService
	{
		private const string BASE_URL = "https://intervatebeacons.azurewebsites.net/API/DeviceController/GetDevices/72BD9101-0292-4E52-971C-F53F0C8618DF";	///1787/34981";

		protected IRestService RestService { get; }

		public BeaconDataService(IRestService restService)
		{
			RestService = restService;
		}

		public async Task<IList<TFGBeacon>> GetBeaconsAsync(int major, int minor)
		{
			var url = $"{BASE_URL}/{major}/{minor}";

			//Dictionary<string, object> parameters = null;

			//if (!string.IsNullOrEmpty (cityId)) {
			//	parameters = new Dictionary<string, object> {
			//		{ "dateFrom", DateTimeOffset.Now.AddMonths (-4).ToString ("O") },
			//		{ "cityId", cityId }
			//	};
			//}

			var resp = await RestService.GetAsync<List<TFGBeacon>> (url);

			return resp;

			/*var dummyBeacons = new List<TFGBeacon> {
				new TFGBeacon
				{
					UUID = "B9407F30-F5F8-466E-AFF9-25556B57FE6D",
					Major = 1787,
					Minor = 34981,
					BeaconRole = BeaconRoleEnum.Sentinel,
					EntryRSSI = 3,
					ExitRSSI = 4
				},
				new TFGBeacon
				{
					UUID = "B9407F30-F5F8-466E-AFF9-25556B57FE6D",
					Major = 2684,
					Minor = 29752,
					BeaconRole = BeaconRoleEnum.Sentinel,
					EntryRSSI = 3,
					ExitRSSI = 4
				},
				new TFGBeacon
				{
					UUID = "B9407F30-F5F8-466E-AFF9-25556B57FE6D",
					Major = 38703,
					Minor = 50,
					BeaconRole = BeaconRoleEnum.Entry,
					EntryRSSI = 3,
					ExitRSSI = 2
				},
				new TFGBeacon
				{
					UUID = "B9407F30-F5F8-466E-AFF9-25556B57FE6D",
					Major = 63255,
					Minor = 33936,
					BeaconRole = BeaconRoleEnum.Entry,
					EntryRSSI = 3,
					ExitRSSI = 2
				},
				new TFGBeacon
				{
					UUID = "B9407F30-F5F8-466E-AFF9-25556B57FE6D",
					Major = 29763,
					Minor = 44133,
					BeaconRole = BeaconRoleEnum.Proximity,
					EntryRSSI = 0.5,
					ExitRSSI = 1,
					ActionUrl = "http://www.foschini.co.za/"
				},
				new TFGBeacon
				{
					UUID = "B9407F30-F5F8-466E-AFF9-25556B57FE6D",
					Major = 61642,
					Minor = 48074,
					BeaconRole = BeaconRoleEnum.Proximity,
					EntryRSSI = 0.5,
					ExitRSSI = 1,
					ActionUrl = "http://www.markham.co.za/"
				}
			};

			return dummyBeacons;*/
		}
	}
}
