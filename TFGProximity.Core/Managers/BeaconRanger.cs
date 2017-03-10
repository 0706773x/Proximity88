using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Microsoft.Practices.Unity;
using MoreLinq;
using Prism.Events;
using Prism.Unity;
using TFGProximity.Core.Enums;
using TFGProximity.Core.Helpers;
using TFGProximity.Core.Interfaces;
using TFGProximity.Core.Models;
using TFGProximity.Core.ViewModels;

namespace TFGProximity.Core.Managers
{
	public class BeaconRanger : IBeaconRanger
	{
		private bool _inited;
		private bool _initing;
		//private bool _showingBeaconAction;

		private IBeaconDataManager BeaconDataManager { get; }
		private IEventAggregator EventAggregator { get; }

		private ILogger _logger;
		private ILogger Logger {
			get {
				if (_logger == null) {
					_logger = (App.Current as PrismApplication).Container.Resolve<ILogger> ();
				}

				return _logger;
			}
		}

		private IUserDialogs _userDialogs;
		private IUserDialogs UserDialogs {
			get {
				if (_userDialogs == null) {
					_userDialogs = (App.Current as PrismApplication).Container.Resolve<IUserDialogs> ();
				}

				return _userDialogs;
			}
		}

		private IList<TFGBeacon> TFGBeacons { get; set; }

		private ObservableCollection<BeaconViewModel> _beacons;
		public ObservableCollection<BeaconViewModel> Beacons {
			get {
				_beacons.Sort ((a, b) => { return a.Role.CompareTo (b.Role); });

				return _beacons;
			}
			set {
				if (_beacons != value) {
					_beacons = value;
				}
			}
		}

		//private BeaconViewModel _nearestBeacon;

		private IList<BeaconViewModel> ReadOnlyBeacons => Beacons.ToList ();

		private UserStateEnum _prevUserState;

		private UserStateEnum _userState;
		protected UserStateEnum UserState {
			get {
				return _userState;
			}

			set {
				_prevUserState = UserState;

				if (_userState != value) {
					_userState = value;

					//EventAggregator.GetEvent<UserStateChangedEvent> ().Publish (new UserStateChange (_prevUserState, UserState, _nearestBeacon));
				}
			}
		}

		public BeaconRanger (IBeaconDataManager beaconDataManager, IEventAggregator eventAggregator)
		{
			BeaconDataManager = beaconDataManager;
			EventAggregator = eventAggregator;

			Beacons = new ObservableCollection<BeaconViewModel> ();

			UserState = UserStateEnum.Unentered;

			Beacons.CollectionChanged += Beacons_CollectionChanged;

			EventAggregator.GetEvent<BeaconsRangedEvent> ().Subscribe (HandleBeaconsRanged);
		}

		#region Private Methods

		private void Beacons_CollectionChanged (object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add) {
				if (e.NewItems != null) {
					foreach (var item in e.NewItems) {
						(item as INotifyPropertyChanged).PropertyChanged += Item_PropertyChanged;
					}
				}
				if (e.OldItems != null) {
					foreach (var item in e.OldItems) {
						(item as INotifyPropertyChanged).PropertyChanged -= Item_PropertyChanged;
					}
				}
			}
		}

		private async void HandleBeaconsRanged (IEnumerable<IBeacon> beacons)
		{
			//Logger.Debug ("HandleBeaconsRanged");

			if (_initing) {
				return;
			}

			if (!_inited) {
				await HandleFirstBeaconAsync (beacons.FirstOrDefault ());
			}

			foreach (var beacon in beacons) {
				//Debug.WriteLine ($"MainPageViewModel HandleBeaconsRanged {beacon.ToString ()}");

				if (beacon.Accuracy > 0) {
					// is this a beacon that belongs to this store?
					var matched = TFGBeacons?.SingleOrDefault (x => x.Major == beacon.Major && x.Minor == beacon.Minor);

					if (matched != null) {
						var existingItem = ReadOnlyBeacons.FirstOrDefault (x => x.Major == beacon.Major && x.Minor == beacon.Minor);

						//Debug.WriteLine ($"MainPageViewModel HandleBeaconsRanged {existingItem}");

						if (existingItem == null) {
							var beaconVM = new BeaconViewModel {
								Major = beacon.Major,
								Minor = beacon.Minor,
								Distance = beacon.Accuracy
							};

							beaconVM.Role = matched.BeaconRole;
							beaconVM.EntryThreshholdDistance = matched.DeviceDetail.EntryDistance;
							beaconVM.ExitThreshholdDistance = matched.DeviceDetail.ExitDistance;
							beaconVM.ActionUrl = matched.DeviceDetail.ActiveURL;

							lock ((Beacons as ICollection).SyncRoot) {
								Logger.Debug ($"HandleBeaconsRanged Beacon added :: {beacon.ToString ()}");
								Beacons.Add (beaconVM);
							}
						} else {
							existingItem.Distance = beacon.Accuracy;
						}
					}
				}
			}
		}

		private void Item_PropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			var beacon = sender as BeaconViewModel;

			if (beacon == null) {
				return;
			}

			Logger.Trace ($"Beacon {beacon.Role} {beacon.Distance} {beacon.EntryThreshholdDistance} {beacon.ExitThreshholdDistance}");

			//_nearestBeacon = GetNearestBeacon (beacon.Role);
			//Logger.Trace ($"Nearest Beacon {_nearestBeacon.Role} {_nearestBeacon.Distance} {_nearestBeacon.EntryThreshholdDistance} {_nearestBeacon.ExitThreshholdDistance}");

			Logger.Trace ($"Current User State {UserState}");

			switch (UserState) {
				case UserStateEnum.Unentered:
					switch (beacon.Role) {
						case BeaconRoleEnum.Sentinel:
							if (beacon.Distance <= beacon.EntryThreshholdDistance) {
								Logger.Trace ("UNENTERED => DETECTED");
								UserState = UserStateEnum.Detected;
								EventAggregator.GetEvent<UserStateChangedEvent> ().Publish (new UserStateChange (UserStateEnum.Unentered, UserState, beacon));
							}
							break;
					}
					break;

				case UserStateEnum.Detected:
					switch (beacon.Role) {
						case BeaconRoleEnum.Sentinel:
							if (beacon.Distance > beacon.ExitThreshholdDistance) {
								var nearestEntryBeacon = GetNearestBeacon (BeaconRoleEnum.Entry);
								Logger.Trace ($"Nearest Entry Beacon {nearestEntryBeacon.Role} {nearestEntryBeacon.Distance} {nearestEntryBeacon.EntryThreshholdDistance} {nearestEntryBeacon.ExitThreshholdDistance}");

								//if (nearestEntryBeacon.Distance > beacon.Distance) {
								//	Logger.Trace ("entry beacon further than sentinel");
								if (nearestEntryBeacon.Distance > nearestEntryBeacon.EntryThreshholdDistance) {
									Logger.Trace ("DETECTED => UNENTERED");
									UserState = UserStateEnum.Unentered;
									EventAggregator.GetEvent<UserStateChangedEvent> ().Publish (new UserStateChange (UserStateEnum.Detected, UserState, beacon));
								}
							}
							break;

						case BeaconRoleEnum.Entry:
							if (beacon.Distance <= beacon.EntryThreshholdDistance) {
								Logger.Trace ("DETECTED => ENTERED");
								UserState = UserStateEnum.Entered;
								EventAggregator.GetEvent<UserStateChangedEvent> ().Publish (new UserStateChange (UserStateEnum.Detected, UserState, beacon));
							}
							break;

						case BeaconRoleEnum.Proximity:
							if (beacon.Distance <= beacon.EntryThreshholdDistance) {
								Logger.Trace ("DETECTED => PROXIMITY");
								UserState = UserStateEnum.Proximity;
								EventAggregator.GetEvent<UserStateChangedEvent> ().Publish (new UserStateChange (UserStateEnum.Detected, UserState, beacon));
							}
							break;
					}
					break;

				case UserStateEnum.Entered:
					switch (beacon.Role) {
						case BeaconRoleEnum.Sentinel:
							if (beacon.Distance <= beacon.EntryThreshholdDistance) {
								Logger.Trace ("ENTERED => DETECTED");
								UserState = UserStateEnum.Detected;
								EventAggregator.GetEvent<UserStateChangedEvent> ().Publish (new UserStateChange (UserStateEnum.Entered, UserState, beacon));
							}
							break;
						/*case BeaconRoleEnum.Entry:
							if (_nearestBeacon.Distance > _nearestBeacon.ExitThreshholdDistance) {
								Logger.Trace ("ENTERED => DETECTED");
								UserState = UserStateEnum.Detected;
							}
							break;*/

						case BeaconRoleEnum.Proximity:
							if (beacon.Distance <= beacon.EntryThreshholdDistance) {
								Logger.Trace ("ENTERED => PROXIMITY");
								UserState = UserStateEnum.Proximity;
								EventAggregator.GetEvent<UserStateChangedEvent> ().Publish (new UserStateChange (UserStateEnum.Entered, UserState, beacon));
							}
							break;
					}
					break;

				case UserStateEnum.Proximity:
					switch (beacon.Role) {
						case BeaconRoleEnum.Proximity:
							if (beacon.Distance > beacon.ExitThreshholdDistance) {
								Logger.Trace ("PROXIMITY => ENTERED");
								UserState = UserStateEnum.Entered;
								EventAggregator.GetEvent<UserStateChangedEvent> ().Publish (new UserStateChange (UserStateEnum.Entered, UserState, beacon));
							}
							break;
					}
					break;
			}
		}

		private async Task HandleFirstBeaconAsync (IBeacon beacon)
		{
			if (beacon == null) {
				return;
			}

			_initing = true;

			try {
				//EventAggregator.GetEvent<IsBusyEvent> ().Publish (true);
				UserDialogs.ShowLoading ();
				// TODO :: what if this takes really long?
				TFGBeacons = await BeaconDataManager.GetBeaconsAsync (beacon.Major, beacon.Minor);
				//EventAggregator.GetEvent<IsBusyEvent> ().Publish (false);
				UserDialogs.HideLoading ();
			} catch (Exception ex) {
				UserDialogs.HideLoading ();

				UserDialogs.ShowError ($"Couldn't get TFG Beacons {ex.Message}");
			}

			var matched = TFGBeacons?.SingleOrDefault (x => x.Major == beacon.Major && x.Minor == beacon.Minor);

			if (matched != null) {
				var beaconVM = new BeaconViewModel {
					Major = beacon.Major,
					Minor = beacon.Minor,
					Distance = beacon.Accuracy
				};

				beaconVM.Role = matched.BeaconRole;
				beaconVM.EntryThreshholdDistance = matched.DeviceDetail.EntryDistance;
				beaconVM.ExitThreshholdDistance = matched.DeviceDetail.ExitDistance;
				beaconVM.ActionUrl = matched.DeviceDetail.ActiveURL;

				lock ((Beacons as ICollection).SyncRoot) {
					Logger.Debug ($"HandleFirstBeaconAsync Beacon added :: {beaconVM.ToString ()}");
					Beacons.Add (beaconVM);
				}

				_inited = true;
			}

			_initing = false;
		}

		private BeaconViewModel GetNearestBeacon (BeaconRoleEnum role)
		{
			var beacon = ReadOnlyBeacons.Where (x => x.Role == role).MinBy (x => x.Distance);

			return beacon;
		}

		#endregion
	}
}
