using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TFGProximity.Core.Models;

namespace TFGProximity.Core.Services
{
	public interface IBeaconDataService
	{
		Task<IList<TFGBeacon>> GetBeaconsAsync(int major, int minor);
	}
}
