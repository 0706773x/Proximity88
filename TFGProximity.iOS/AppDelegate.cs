using System;
using Foundation;
using Microsoft.Practices.Unity;
using Prism.Unity;
using Serilog;
using Serilog.Sinks.RollingFileAlternate;
using TFGProximity.Core;
using TFGProximity.Core.Services;
using TFGProximity.iOS.Plugins;
using UIKit;

namespace TFGProximity.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init ();

			Log.Logger = new LoggerConfiguration ()
				.WriteTo.RollingFileAlternate (Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments))
				//.WriteTo.NSLog ()
				.CreateLogger ();

			var initializer = new iOSInitializer ();

			LoadApplication (new App (initializer));

			return base.FinishedLaunching (app, options);
		}

		public override void OnActivated (UIApplication uiApplication)
		{
			base.OnActivated (uiApplication);
		}

		public override void DidEnterBackground (UIApplication uiApplication)
		{
			base.DidEnterBackground (uiApplication);

			Log.CloseAndFlush ();
		}
	}

	public class iOSInitializer : IPlatformInitializer
	{
		public void RegisterTypes(IUnityContainer container)
		{
			container.RegisterInstance<IBeaconService>(new EstimoteBeaconService());
			container.RegisterInstance<Core.Interfaces.ILogger> (new TFGLogger ());
		}
	}
}
