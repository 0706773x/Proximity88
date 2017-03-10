using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TFGProximity.Core.Services
{
	public class JsonRestService : IRestService
	{
		private HttpClient HttpClient { get; set; }

		public JsonRestService ()
		{
			HttpClient = new HttpClient {
				MaxResponseContentBufferSize = 512000,
				Timeout = TimeSpan.FromSeconds (30)
			};
		}

		#region GET

		public async Task<T> GetAsync<T> (string url, Dictionary<string, object> parameters = null, Dictionary<string, string> headerValues = null, bool beLoggedInAware = false) where T : new()
		{
			if (parameters != null && parameters.Count > 0) {
				url = AddUrlParams (url, parameters);
			}

			if (headerValues != null) {
				SetHeaderValues (HttpClient, headerValues);
			}

			try {
				var response = await HttpClient.GetAsync (url, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait (false);

				var responseString = await response.Content.ReadAsStringAsync ().ConfigureAwait (false);

				var result = await Task<T>.Factory.StartNew (() => JsonConvert.DeserializeObject<T> (responseString, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore })).ConfigureAwait (false);

				return result;
			} catch (TaskCanceledException tcex) {
				// if cancellation wasn't explicitly requested, it was probably a Timeout
				if (!tcex.CancellationToken.IsCancellationRequested) {
					throw new TimeoutException ("The connection timed out; please check your internet connection and try again.", tcex);
				}

				throw;
			}
			catch (Exception ex) {
				var i = 0;

				throw;
			}
		}

		#endregion

		#region Private Helper Methods

		private static void SetHeaderValues (HttpClient httpClient, Dictionary<string, string> headerValues)
		{
			httpClient.DefaultRequestHeaders.Clear ();

			foreach (KeyValuePair<string, string> keyValuePair in headerValues) {
				httpClient.DefaultRequestHeaders.Add (keyValuePair.Key, keyValuePair.Value);
			}
		}

		private static string AddUrlParams (string baseUrl, Dictionary<string, object> parameters)
		{
			var stringBuilder = new StringBuilder (baseUrl);
			var hasFirstParam = baseUrl.Contains ("?");

			foreach (var parameter in parameters) {
				var format = hasFirstParam ? "&{0}={1}" : "?{0}={1}";
				stringBuilder.AppendFormat (
					format,
					Uri.EscapeDataString (parameter.Key),
					parameter.Value == null ? string.Empty : Uri.EscapeDataString (parameter.Value.ToString ()));

				hasFirstParam = true;
			}

			return stringBuilder.ToString ();
		}

		#endregion
	}
}
