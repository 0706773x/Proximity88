using System.Collections.Generic;
using Prism.Events;
using TFGProximity.Core.Models;

namespace TFGProximity.Core.Helpers
{
	public class BeaconsRangedEvent : PubSubEvent<IEnumerable<IBeacon>>
	{
	}
}
