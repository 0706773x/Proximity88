using Serilog;
using Serilog.Events;

namespace TFGProximity.iOS.Plugins
{
	public class TFGLogger : Core.Interfaces.ILogger
	{
		public void Debug (string message)
		{
			System.Diagnostics.Debug.WriteLine (message);
		}

		public void Trace (string message)
		{
			if (Log.IsEnabled (LogEventLevel.Information)) {
				Log.Information (message);
			}

			System.Diagnostics.Debug.WriteLine (message);
		}
	}
}
