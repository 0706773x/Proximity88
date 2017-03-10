using System;
using System.Collections.Generic;
using System.Diagnostics;
using Acr.Notifications;
using Acr.UserDialogs;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Unity;
using TFGProximity.Core.Helpers;
using TFGProximity.Core.Interfaces;
using TFGProximity.Core.Managers;
using TFGProximity.Core.Models;
using TFGProximity.Core.Services;
using TFGProximity.Core.Views;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace TFGProximity.Core
{
	public partial class App : PrismApplication
	{
		private bool _locationInited;
		private bool _haveNotifiedRegionEntry;

		public static bool DidEnter;
		public static bool HaveSentExitMessage;

		public App (IPlatformInitializer initializer = null) : base (initializer)
		{
		}

		protected override void OnInitialized ()
		{
			InitializeComponent ();

			Akavache.BlobCache.ApplicationName = "TFGProximity";
			Akavache.BlobCache.EnsureInitialized ();

			var beaconService = Container.Resolve<IBeaconService> ();

			beaconService.InitAsync ().ContinueWith (x => {
				if (x.Result) {
					Debug.WriteLine ("App BeaconService Inited");

					_locationInited = true;

					beaconService.RegionEntered += BeaconService_RegionEntered;
					beaconService.RegionExited += BeaconService_RegionExited;

					beaconService.Ranged += BeaconService_Ranged;

					Debug.WriteLine ("App beaconService.StartRanging");
					beaconService.StartRanging (Constants.BeaconRegion);

					//beaconService.StartMonitoring (new BeaconRegion (Constants.UUID, Constants.RegionName));
				}
			});

			NavigationService.NavigateAsync ("RootPage/DemoPage");
			//NavigationService.NavigateAsync ("RootPage/MainPage");
		}

		protected override void RegisterTypes ()
		{
			Container.RegisterTypeForNavigation<RootPage> ();
			Container.RegisterTypeForNavigation<MainPage> ();
			Container.RegisterTypeForNavigation<DemoPage> ();
			Container.RegisterTypeForNavigation<BeaconsListPage> ();
			Container.RegisterTypeForNavigation<WebViewPage> ();
			Container.RegisterTypeForNavigation<WelcomePage> ();

			Container.RegisterInstance<INotifications> (Notifications.Instance);
			Container.RegisterInstance<IUserDialogs> (UserDialogs.Instance);

			var jsonService = new JsonRestService ();
			Container.RegisterInstance<IRestService> (jsonService);

			var beaconDataService = new BeaconDataService (jsonService);
			Container.RegisterInstance<IBeaconDataService> (beaconDataService);

			var beaconDataManager = new BeaconDataManager (beaconDataService);
			Container.RegisterInstance<IBeaconDataManager> (beaconDataManager);

			var eventAggregator = Container.Resolve<IEventAggregator> ();
			Container.RegisterInstance<IBeaconRanger> (new BeaconRanger (beaconDataManager,
																		eventAggregator));
		}

		protected override void OnStart ()
		{
			base.OnStart ();

			/*var beaconDataManager = Container.Resolve<IBeaconDataManager> ();
			await beaconDataManager.GetBeaconsAsync (true);*/

			var notifications = Container.Resolve<INotifications> ();
			notifications.Badge = 0;
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps

			var logger = Container.Resolve<ILogger> ();
			logger.Trace ("App OnSleep");

			if (_locationInited) {
				var beaconService = Container.Resolve<IBeaconService> ();

				//beaconService.StopAllRanging ();
				beaconService.StopRanging (Constants.BeaconRegion);
				beaconService.StartMonitoring (Constants.BeaconRegion);
			}
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes

			var logger = Container.Resolve<ILogger> ();
			logger.Trace ("App OnResume");

			if (_locationInited) {
				var beaconService = Container.Resolve<IBeaconService> ();

				// TODO :: replace with StopAllMonitoring()?
				beaconService.StopMonitoring (Constants.BeaconRegion);
				beaconService.StartRanging (Constants.BeaconRegion);
			}
		}

		private void BeaconService_RegionEntered (object sender, BeaconEventArgs e)
		{
			Debug.WriteLine ($"You have entered {e.BeaconRegion.Identifier} :: {_haveNotifiedRegionEntry}");

			if (!_haveNotifiedRegionEntry) {
				_haveNotifiedRegionEntry = true;

				var notifications = Container.Resolve<INotifications> ();
				notifications.Send ("TFG Store of the Future", $"Please open your TFG App [{DateTime.Now.ToString ("u")}]");

				// TODO :: start background ranging?
				/*var beaconService = Container.Resolve<IBeaconService> ();
				beaconService.StartRanging (Constants.BeaconRegion);*/
			}
		}

		private void BeaconService_RegionExited (object sender, BeaconEventArgs e)
		{
			Debug.WriteLine ($"You have exited {e.BeaconRegion.Identifier}");

			if (DidEnter/* && !HaveSentExitMessage*/) {
				var notifications = Container.Resolve<INotifications> ();
				notifications.Send ("TFG Store of the Future", $"Thank you for visiting, would you like tell us about your experience? [{DateTime.Now.ToString ("u")}]");
			}

			DidEnter = false;
			HaveSentExitMessage = false;
			_haveNotifiedRegionEntry = false;
		}

		private void BeaconService_Ranged (object sender, IEnumerable<IBeacon> e)
		{
			/*#if DEBUG
						var beacon = e.FirstOrDefault ();
						if (beacon != null) {
							var logger = Container.Resolve<ILogger> ();
							logger.Trace ($"App BeaconService_Ranged {beacon}");
						}
			#endif*/

			var eventAggregator = Container.Resolve<IEventAggregator> ();
			eventAggregator.GetEvent<BeaconsRangedEvent> ().Publish (e);
		}
	}
}

