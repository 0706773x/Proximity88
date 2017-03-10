using System.ComponentModel;
using TFGProximity.Core.Helpers;
using TFGProximity.Core.ViewModels;
using Xamarin.Forms;

namespace TFGProximity.Core.Views
{
	public partial class MainPage : ContentPage
	{
		public MainPage ()
		{
			InitializeComponent ();

			var viewModel = BindingContext as MainPageViewModel;

			if (viewModel != null) {
				viewModel.PropertyChanged += ViewModel_PropertyChanged;
			}
		}

		/*protected async override void OnAppearing ()
		{
			base.OnAppearing ();

			await this.ColorTo (Color.FromRgb (0, 0, 0), Color.FromRgb (255, 255, 255), c => BackgroundColor = c, 5000);
			this.BackgroundColor = Color.Default;
		}*/

		private async void ViewModel_PropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "UserStateFromTo") {
				var viewModel = sender as MainPageViewModel;

				var fromColour = UserStateToColorMapper.Map (viewModel.UserStateFromTo.Item1);

				var toColour = UserStateToColorMapper.Map (viewModel.UserStateFromTo.Item2);

				await this.ColorTo (fromColour, toColour, c => BackgroundColor = c, 2000);
			}
		}
	}
}

