using Acr.UserDialogs;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Microsoft.Practices.Unity;
using Prism.Unity;
using TFGProximity.Core;
using TFGProximity.Core.Services;
using TFGProximity.Droid.Plugins;
using Mindscape.Raygun4Net;

namespace TFGProximity.Droid
{
	[Activity(Label = "TFG Proximity", Icon = "@drawable/ic_launcher", MainLauncher = true, 
	          ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			global::Xamarin.Forms.Forms.Init(this, bundle);

            RaygunClient.Initialize("ZIXarqf/CIWWUj4tqAdY5Q==").AttachCrashReporting().AttachPulse(this);

            UserDialogs.Init (() => this);

			LoadApplication(new App(new AndroidInitializer()));
		}
	}

	public class AndroidInitializer : IPlatformInitializer
	{
		public void RegisterTypes(IUnityContainer container)
		{
			container.RegisterInstance<IBeaconService> (new EstimoteBeaconService ());
			container.RegisterInstance<Core.Interfaces.ILogger> (new TFGLogger ());
		}
	}
}
