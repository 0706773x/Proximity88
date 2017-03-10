using Xamarin.Forms;

namespace TFGProximity.Core.Views
{
	public partial class WebViewPage : ContentPage
	{
		public WebViewPage ()
		{
			InitializeComponent ();

			var doneButton = new ToolbarItem { Text = "Done" };

			doneButton.SetBinding (ToolbarItem.CommandProperty, new Binding ("NavigateCommand"));

			ToolbarItems.Add (doneButton);
		}
	}
}

