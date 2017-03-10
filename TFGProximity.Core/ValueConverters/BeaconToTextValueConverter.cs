using System;
using System.Globalization;
using TFGProximity.Core.ViewModels;
using Xamarin.Forms;

namespace TFGProximity.Core.ValueConverters
{
	public class BeaconToTextValueConverter : IValueConverter
	{
		#region IValueConverter implementation

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var beacon = value as BeaconViewModel;

			if (beacon == null) {
				return string.Empty;
			}

			return $"{beacon.Role} :: {beacon.Distance} :: {beacon.EntryThreshholdDistance} :: {beacon.ExitThreshholdDistance}";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
