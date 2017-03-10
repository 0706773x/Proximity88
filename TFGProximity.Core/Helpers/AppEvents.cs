using Prism.Events;

namespace TFGProximity.Core.Helpers
{
	public class AppOnResumeEvent : PubSubEvent<string>
	{
	}

	public class AppOnSleepEvent : PubSubEvent<string>
	{
	}
}
