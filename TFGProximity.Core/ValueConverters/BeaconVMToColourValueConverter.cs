using System;
using System.Globalization;
using TFGProximity.Core.Enums;
using TFGProximity.Core.ViewModels;
using Xamarin.Forms;

namespace TFGProximity.Core.ValueConverters
{
	public class BeaconVMToColourValueConverter : IValueConverter
	{
		#region IValueConverter implementation

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var beaconVM = value as BeaconViewModel;

			if (beaconVM == null) {
				return Color.White;
			}

			switch (beaconVM.Role) {
				case BeaconRoleEnum.Sentinel:
					return Color.Lime;
				case BeaconRoleEnum.Entry:
					return Color.Aqua;
				case BeaconRoleEnum.Proximity:
					return Color.Pink;
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
