using System;
using System.Globalization;
using TFGProximity.Core.Enums;
using TFGProximity.Core.Helpers;
using Xamarin.Forms;

namespace TFGProximity.Core.ValueConverters
{
	public class UserStateToColourValueConverter : IValueConverter
	{
		#region IValueConverter implementation

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var userState = (UserStateEnum)value;

			return UserStateToColorMapper.Map (userState);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
