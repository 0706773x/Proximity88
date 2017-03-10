using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Unity;
using TFGProximity.Core.Enums;
using TFGProximity.Core.Helpers;
using TFGProximity.Core.Interfaces;
using TFGProximity.Core.Managers;
using Xamarin.Forms;

namespace TFGProximity.Core.ViewModels
{
	// Mint: sentinel
	// Aqua: entry
	// Purple: proximity

	public class MainPageViewModel : BindableBase
	{
		private bool _didEnter;
		private bool _showingBeaconAction;

		protected IBeaconRanger BeaconRanger { get; }
		protected INavigationService NavigationService { get; }
		protected IEventAggregator EventAggregator { get; }

		private IList<int> _proximityBeaconsShown;

		private ILogger _logger;
		private ILogger Logger {
			get {
				if (_logger == null) {
					_logger = (App.Current as PrismApplication).Container.Resolve<ILogger> ();
				}

				return _logger;
			}
		}

		private BeaconViewModel _beacon;
		public BeaconViewModel Beacon {
			get {
				return _beacon;
			}

			set {
				SetProperty (ref _beacon, value);
			}
		}

		private Tuple<UserStateEnum, UserStateEnum> _userStateFromTo;
		public Tuple<UserStateEnum, UserStateEnum> UserStateFromTo {
			get {
				return _userStateFromTo;
			}
			set {
				SetProperty (ref _userStateFromTo, value);
			}
		}

		private ICommand _showBeaconsCommand;
		public ICommand ShowBeaconsCommand { 
			get {
				return _showBeaconsCommand ?? (_showBeaconsCommand = new Command (async () => { 
					await NavigationService.NavigateAsync ("BeaconsListPage", null, false);
				}));
			} 
		}

		public MainPageViewModel(IBeaconRanger beaconRanger, INavigationService navigationService, IEventAggregator eventAggregator)
		{
			BeaconRanger = beaconRanger;
			NavigationService = navigationService;
			EventAggregator = eventAggregator;

			_proximityBeaconsShown = new List<int> ();

			EventAggregator.GetEvent<UserStateChangedEvent> ().Subscribe (HandleUserStateChanged);
			EventAggregator.GetEvent<BeaconActionShownEvent> ().Subscribe (HandleBeaconActionShown);
		}

		private void HandleBeaconActionShown (bool shown)
		{
			if (shown) {
				Logger.Trace ("MainPageVM HandleBeaconActionShown setting to false");
				_showingBeaconAction = false;
			}
		}

		/*public void OnNavigatedFrom (NavigationParameters parameters)
		{
		}*/

		/*public virtual void OnNavigatedTo (NavigationParameters parameters)
		{
			//if (parameters.ContainsKey("title"))
			//{
			//	Title = (string)parameters["title"] + " and Prism";
			//}

			_showingBeaconAction = false;
		}*/

		private async void HandleUserStateChanged (UserStateChange userStateChange)
		{
			UserStateFromTo = new Tuple<UserStateEnum, UserStateEnum> (userStateChange.UserStateFrom, 
	                                                               		userStateChange.UserStateTo);

			Beacon = userStateChange.Beacon;

			switch(userStateChange.UserStateFrom) {
				case UserStateEnum.Unentered:
					switch (userStateChange.UserStateTo) {
						case UserStateEnum.Detected:
							// fire Welcome message
							// retrieve Beacon Data ?
							Logger.Trace ("MainPageVM UNENTERED => DETECTED");
						break;
					}

					break;
				case UserStateEnum.Detected:
					switch (userStateChange.UserStateTo) {
						case UserStateEnum.Entered:
							if (!_didEnter) {
								Logger.Trace ("MainPageVM DETECTED => ENTERED");
								App.DidEnter = _didEnter = true;
								// fire Entered message
								await ShowWebView (Beacon);
							}

						break;

						case UserStateEnum.Unentered:
							if (_didEnter) {
								Logger.Trace ("MainPageVM DETECTED => UNENTERED");
								_didEnter = false;
								if (!App.HaveSentExitMessage) {
									App.HaveSentExitMessage = true;
									// fire Exit message
									await ShowWebView (Beacon);
								}
							}
						break;
					}

				break;

				case UserStateEnum.Entered:
					switch (userStateChange.UserStateTo) {
						case UserStateEnum.Detected:
							Logger.Trace ("MainPageVM ENTERED => DETECTED");
							// fire ??? message

						break;

						case UserStateEnum.Proximity:
							Logger.Trace ("MainPageVM ENTERED => PROXIMITY");
							// fire Proximity message
							await ShowWebView (Beacon);
								
						break;
					}

				break;

				case UserStateEnum.Proximity:
					switch (userStateChange.UserStateTo) {
						case UserStateEnum.Entered:
							// fire ??? message
							Logger.Trace ("MainPageVM PROXIMITY => ENTERED");

							var idx = _proximityBeaconsShown.IndexOf (userStateChange.Beacon.GetHashCode ());

							if (idx >= 0) {
								_proximityBeaconsShown.RemoveAt (idx);
							}

						break;
					}

				break;
			}
		}

		private async Task ShowWebView (BeaconViewModel beacon)
		{
			if (!string.IsNullOrWhiteSpace (beacon.ActionUrl) && !StillShowingProximity (beacon)) {
			//if (!string.IsNullOrWhiteSpace (beacon.ActionUrl) && !_showingBeaconAction) {
				_showingBeaconAction = true;

				_proximityBeaconsShown.Add (beacon.GetHashCode ());

				var param = new NavigationParameters ();
				param.Add ("url", beacon.ActionUrl);

				await NavigationService.NavigateAsync ("RootPage/WebViewPage", param, true);
			}
		}

		private bool StillShowingProximity (BeaconViewModel beacon)
		{
			if (beacon.Role != BeaconRoleEnum.Proximity) {
				return false;
			}

			return _proximityBeaconsShown.IndexOf (beacon.GetHashCode ()) >= 0;
		}
	}
}

