using TFGProximity.Core.Enums;
using Xamarin.Forms;

namespace TFGProximity.Core.Helpers
{
	public static class UserStateToColorMapper
	{
		public static Color Map (UserStateEnum userState)
		{
			switch (userState) {
				case UserStateEnum.Unentered:
					return Color.White;
				case UserStateEnum.Detected:
					return Color.Yellow;
				case UserStateEnum.Entered:
					return Color.Green;
				case UserStateEnum.Proximity:
					return Color.Purple;
				default:
					return Color.White;
			}
		}
	}
}
