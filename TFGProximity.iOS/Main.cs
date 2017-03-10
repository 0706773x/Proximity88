using Mindscape.Raygun4Net;
using UIKit;

namespace TFGProximity.iOS
{
	public class Application
	{
		// This is the main entry point of the application.
		static void Main(string[] args)
		{
			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.

			//RaygunClient.Initialize ("ZIXarqf/CIWWUj4tqAdY5Q==").AttachPulse ();
			RaygunClient.Attach ("ZIXarqf/CIWWUj4tqAdY5Q==");

			UIApplication.Main(args, null, "AppDelegate");
		}
	}
}
