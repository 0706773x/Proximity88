using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using TFGProximity.Core.Helpers;
using Xamarin.Forms;

namespace TFGProximity.Core.Views
{
	public partial class DemoPage : ContentPage
	{
		private bool _doAnimation = true;

		private CancellationTokenSource _tokenSource;

		public DemoPage ()
		{
			InitializeComponent ();
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();

			_doAnimation = true;

			_tokenSource = new CancellationTokenSource ();
			var token = _tokenSource.Token;

			var tfgPurple = Color.FromRgb (90, 36, 90);
			var black = Color.FromRgb (255, 255, 255);

			var t = Task.Factory.StartNew (async () => {
				while (_doAnimation) {
					try {
						if (!_tokenSource.IsCancellationRequested) {
							await this.ColorTo (tfgPurple, black, c => BackgroundColor = c, 2000);
						}

						if (!_tokenSource.IsCancellationRequested) {
							await this.ColorTo (black, tfgPurple, c => BackgroundColor = c, 2000);
						}
					} catch (Exception ex) {
						Debug.WriteLine ($"Exception thrown in Colour Animation :: {ex.Message}");
					}
				}
			}, token);
		}

		protected override void OnDisappearing ()
		{
			_tokenSource.Cancel ();
			_doAnimation = false;

			base.OnDisappearing ();
		}
	}
}

