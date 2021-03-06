﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace TFGProximity.Core.Helpers
{
	// from http://www.codeproject.com/Tips/694370/How-to-Listen-to-Property-Chang
	public class ItemsChangeObservableCollection<T> : ObservableCollection<T> where T : INotifyPropertyChanged
	{
		public ItemsChangeObservableCollection () : base ()
		{
		}

		public ItemsChangeObservableCollection (List<T> list) : base (list)
		{
		}

		protected override void OnCollectionChanged (NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add) {
				RegisterPropertyChanged (e.NewItems);
			} else if (e.Action == NotifyCollectionChangedAction.Remove) {
				UnRegisterPropertyChanged (e.OldItems);
			} else if (e.Action == NotifyCollectionChangedAction.Replace) {
				UnRegisterPropertyChanged (e.OldItems);
				RegisterPropertyChanged (e.NewItems);
			}

			base.OnCollectionChanged (e);
		}

		protected override void ClearItems ()
		{
			UnRegisterPropertyChanged (this);

			base.ClearItems ();
		}

		private void RegisterPropertyChanged (IList items)
		{
			foreach (INotifyPropertyChanged item in items) {
				if (item != null) {
					item.PropertyChanged += Item_PropertyChanged;
				}
			}
		}

		private void UnRegisterPropertyChanged (IList items)
		{
			foreach (INotifyPropertyChanged item in items) {
				if (item != null) {
					item.PropertyChanged -= Item_PropertyChanged;
				}
			}
		}

		private void Item_PropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			base.OnCollectionChanged (new NotifyCollectionChangedEventArgs (NotifyCollectionChangedAction.Reset));
		}
	}
}
