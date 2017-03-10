using System.Collections.Generic;
using System.Threading.Tasks;
using TFGProximity.Core.Models;

namespace TFGProximity.Core.Managers
{
	public interface IBeaconDataManager
	{
		Task<IList<TFGBeacon>> GetBeaconsAsync(int major, int minor, bool forceRefresh = false);
	}
}
