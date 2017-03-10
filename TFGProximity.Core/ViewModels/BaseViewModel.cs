using System;
using Acr.UserDialogs;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Unity;
using TFGProximity.Core.Helpers;
using TFGProximity.Core.Interfaces;

namespace TFGProximity.Core.ViewModels
{
	public abstract class BaseViewModel : BindableBase, INavigationAware
	{
		protected INavigationService NavigationService { get; }
		protected IEventAggregator EventAggregator { get; }
		protected IUserDialogs UserDialogs { get; }

		private ILogger _logger;
		protected ILogger Logger {
			get {
				if (_logger == null) {
					_logger = (App.Current as PrismApplication).Container.Resolve<ILogger> ();
				}

				return _logger;
			}
		}

		protected BaseViewModel (INavigationService navigationService, IEventAggregator eventAggregator, IUserDialogs userDialogs)
		{
			NavigationService = navigationService;
			EventAggregator = eventAggregator;
			UserDialogs = userDialogs;
		}

		public virtual void OnNavigatedFrom (NavigationParameters parameters)
		{
			EventAggregator.GetEvent<IsBusyEvent> ().Unsubscribe (HandleIsBusyEvent);
		}

		public virtual void OnNavigatedTo (NavigationParameters parameters)
		{
			EventAggregator.GetEvent<IsBusyEvent> ().Subscribe (HandleIsBusyEvent);
		}

		protected virtual void HandleIsBusyEvent (bool isBusy)
		{
			if (isBusy) {
				UserDialogs.ShowLoading ();
			} else {
				UserDialogs.HideLoading ();
			}
		}
	}
}
