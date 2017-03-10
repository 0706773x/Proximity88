using System;
using Newtonsoft.Json;
using TFGProximity.Core.Enums;

namespace TFGProximity.Core.Models
{
	public class TFGBeacon
	{
		[JsonProperty ("ID")]
		public string ID { get; set; }

		[JsonProperty ("DeviceDetail")]
		public DeviceDetail DeviceDetail { get; set; }

		[JsonProperty ("DeviceProvider")]
		public DeviceProvider DeviceProvider { get; set; }

		[JsonProperty ("Major")]
		public int Major { get; set; }

		[JsonProperty ("Minor")]
		public int Minor { get; set; }

		/*[JsonProperty ("Name")]
		public object Name { get; set; }*/

		[JsonProperty ("Role")]
		public Role Role { get; set; }

		[JsonProperty ("Store")]
		public Store Store { get; set; }

		[JsonProperty ("UUID")]
		public string UUID { get; set; }

		[JsonProperty ("ExternalDeviceID")]
		public string ExternalDeviceID { get; set; }

		[JsonIgnore]
		public BeaconRoleEnum BeaconRole { 
			get {
				if (Role.Type.Equals ("Sentinel")) {
					return BeaconRoleEnum.Sentinel;
				}

				if (Role.Type.Equals ("Entry")) {
					return BeaconRoleEnum.Entry;
				}

				if (Role.Type.Equals ("Proximity")) {
					return BeaconRoleEnum.Proximity;
				}

				return BeaconRoleEnum.Unknown;
			} 
		}
	}

	public class DeviceDetail
	{
		[JsonProperty ("ID")]
		public int ID { get; set; }
		
		[JsonProperty ("ActiveURL")]
		public string ActiveURL { get; set; }

		[JsonProperty ("EntryDistance")]
		public double EntryDistance { get; set; }

		[JsonProperty ("ExitDistance")]
		public double ExitDistance { get; set; }

		[JsonProperty ("GroupID")]
		public int GroupID { get; set; }

		[JsonProperty ("RssiSmoothingFactor")]
		public int RssiSmoothingFactor { get; set; }
	}

	public class DeviceProvider
	{
		[JsonProperty ("ID")]
		public int ID { get; set; }

		[JsonProperty ("Name")]
		public string Name { get; set; }
	}

	public class Role
	{
		[JsonProperty ("ID")]
		public int ID { get; set; }

		[JsonProperty ("Description")]
		public string Description { get; set; }

		[JsonProperty ("Type")]
		public string Type { get; set; }
	}

	public class Store
	{
		[JsonProperty ("ID")]
		public string ID { get; set; }

		[JsonProperty ("Description")]
		public string Description { get; set; }
	}


	/*public class TFGBeacon
	{
		public string ID { get; set; }
		public string UUID { get; set; }
		public long Major { get; set; }
		public long Minor { get; set; }

		//public int TX { get; set; }
		public double EntryThreshholdDistance { get; set; }
		public double ExitThreshholdDistance { get; set; }

		public int SmoothingFactor { get; set; }

		public string ActionUrl { get; set; }

		public BeaconRoleEnum BeaconRole { get; set; }
	}*/
}
