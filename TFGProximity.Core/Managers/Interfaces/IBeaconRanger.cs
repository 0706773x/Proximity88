using System;
using System.Collections.ObjectModel;
using TFGProximity.Core.ViewModels;

namespace TFGProximity.Core.Managers
{
	public interface IBeaconRanger
	{
		ObservableCollection<BeaconViewModel> Beacons { get; }
	}
}
