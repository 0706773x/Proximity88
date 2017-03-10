using System;
using System.Globalization;
using TFGProximity.Core.Enums;
using Xamarin.Forms;

namespace TFGProximity.Core.ValueConverters
{
	public class BeaconRoleToColourValueConverter : IValueConverter
	{
		#region IValueConverter implementation

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var beaconRole = (BeaconRoleEnum)value;

			switch(beaconRole) {
				case BeaconRoleEnum.Sentinel:
					return Color.Lime;
				case BeaconRoleEnum.Entry:
					return Color.Aqua;
				case BeaconRoleEnum.Proximity:
					return Color.Purple;
				default:
					return Color.Lime;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
