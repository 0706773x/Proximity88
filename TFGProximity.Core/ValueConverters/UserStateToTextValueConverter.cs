using System;
using System.Globalization;
using TFGProximity.Core.Enums;
using Xamarin.Forms;

namespace TFGProximity.Core.ValueConverters
{
	public class UserStateToTextValueConverter : IValueConverter
	{
		#region IValueConverter implementation

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var userStateFromTo = (Tuple<UserStateEnum, UserStateEnum>)value;

			if (userStateFromTo == null) {
				return "Scanning ...";	// string.Empty;
			}

			return $"{userStateFromTo.Item1} => {userStateFromTo.Item2}";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
