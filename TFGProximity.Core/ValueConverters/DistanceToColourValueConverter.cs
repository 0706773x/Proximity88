using System;
using System.Globalization;
using Xamarin.Forms;

namespace TFGProximity.Core.ValueConverters
{
	public class DistanceToColourValueConverter : IValueConverter
	{
		#region IValueConverter implementation

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var distance = (double)value;

			if (distance < 0.0d)
			{
				return Color.Gray;
			}
			else if (distance < 1.0d)
			{
				return Color.Green;
			}
			else if (distance < 3.0d)
			{
				return Color.Yellow;
			}
			else {
				return Color.Red;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
