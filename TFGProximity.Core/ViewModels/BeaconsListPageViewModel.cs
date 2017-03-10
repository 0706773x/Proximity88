using System.Collections.ObjectModel;
using Prism.Mvvm;
using TFGProximity.Core.Managers;

namespace TFGProximity.Core.ViewModels
{
	public class BeaconsListPageViewModel : BindableBase
	{
		private IBeaconRanger BeaconRanger { get; }

		public ObservableCollection<BeaconViewModel> Beacons {
			get {
				return BeaconRanger.Beacons;
			}
		}

		public BeaconsListPageViewModel (IBeaconRanger beaconRanger)
		{
			BeaconRanger = beaconRanger;
		}
	}
}

