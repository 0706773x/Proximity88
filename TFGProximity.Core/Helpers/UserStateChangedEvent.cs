using System;
using Prism.Events;
using TFGProximity.Core.Enums;
using TFGProximity.Core.ViewModels;

namespace TFGProximity.Core.Helpers
{
	public class UserStateChange
	{
		public UserStateEnum UserStateFrom { get; set; }
		public UserStateEnum UserStateTo { get; set; }
		public BeaconViewModel Beacon { get; set; }

		public UserStateChange (UserStateEnum userStateFrom, UserStateEnum userStateTo, BeaconViewModel beaconVM)
		{
			UserStateFrom = userStateFrom;
			UserStateTo = userStateTo;
			Beacon = beaconVM;
		}
	}

	public class UserStateChangedEvent : PubSubEvent<UserStateChange>
	{
	}
}
