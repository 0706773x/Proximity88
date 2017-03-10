using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using TFGProximity.Core.Helpers;

namespace TFGProximity.Core.ViewModels
{
	public class WebViewPageViewModel : BindableBase, INavigationAware
	{
		private INavigationService NavigationService { get; }
		private IEventAggregator EventAggregator { get; }

		private string _title;
		public string Title {
			get { 
				return _title; 
			}
			set { 
				SetProperty (ref _title, value); 
			}
		}

		private string _url;
		public string Url {
			get {
				return _url;
			}
			set {
				SetProperty (ref _url, value);
			}
		}

		public WebViewPageViewModel (INavigationService navigationService, IEventAggregator eventAggregator)
		{
			NavigationService = navigationService;
			EventAggregator = eventAggregator;

			NavigateCommand = new DelegateCommand (GoBack);
		}

		public void OnNavigatedFrom (NavigationParameters parameters)
		{
			System.Diagnostics.Debug.WriteLine ("WebViewPageViewModel OnNavigatedFrom");

			EventAggregator.GetEvent<BeaconActionShownEvent> ().Publish (true);
		}

		public void OnNavigatedTo (NavigationParameters parameters)
		{
			if (parameters.ContainsKey ("url")) {
				Url = (string)parameters["url"];
			}

			if (parameters.ContainsKey ("title"))
			{
				Title = (string)parameters["title"];
			}

			System.Diagnostics.Debug.WriteLine ($"WebViewPageViewModel OnNavigatedTo {Url} :: {Title}");
		}

		public DelegateCommand NavigateCommand { get; private set; }

		private async void GoBack ()
		{
			await NavigationService.GoBackAsync (null, true);
		}
	}
}
