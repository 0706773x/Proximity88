using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TFGProximity.Core.Services
{
	public interface IRestService
	{
		#region GET

		Task<T> GetAsync<T> (string url, Dictionary<string, object> parameters = null, Dictionary<string, string> headerValues = null, bool beLoggedInAware = false) where T : new();

		#endregion
	}
}
