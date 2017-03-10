using Prism.Mvvm;
using TFGProximity.Core.Enums;

namespace TFGProximity.Core.ViewModels
{
	public class BeaconViewModel : BindableBase
	{
		private double _distance;
		public double Distance {
			get {
				return _distance;
			}
			set {
				SetProperty (ref _distance, value);
			}
		}

		private int _minor;
		public int Minor {
			get {
				return _minor;
			}
			set {
				SetProperty (ref _minor, value);
			}
		}

		private int _major;
		public int Major {
			get {
				return _major;
			}
			set {
				SetProperty (ref _major, value);
			}
		}

		/*private int _rssi;
		public int RSSI {
			get {
				return _rssi;
			}
			set {
				SetProperty (ref _rssi, value);
			}
		}*/

		private BeaconRoleEnum _role;
		public BeaconRoleEnum Role {
			get {
				return _role;
			}
			set {
				SetProperty (ref _role, value);
			}
		}

		private double _entryThreshholdDistance;
		public double EntryThreshholdDistance {
			get {
				return _entryThreshholdDistance;
			}

			set {
				SetProperty (ref _entryThreshholdDistance, value);
			}
		}

		private double _exitThreshholdDistance;
		public double ExitThreshholdDistance {
			get {
				return _exitThreshholdDistance;
			}

			set {
				SetProperty (ref _exitThreshholdDistance, value);
			}
		}

		private string _actionUrl;
		public string ActionUrl {
			get {
				return _actionUrl;
			}
			set {
				SetProperty (ref _actionUrl, value);
			}
		}

		public override string ToString ()
		{
			return string.Format ($"[BeaconViewModel: Distance={Distance}, Minor={Minor}, Major={Major}, Role={Role}, EntryRSSI={EntryThreshholdDistance}, ExitRSSI={ExitThreshholdDistance}, ActionUrl={ActionUrl}]");
		}

		public override int GetHashCode ()
		{
			unchecked // Overflow is fine, just wrap
			{
				int hash = 17;
				hash = hash * 23 + Minor.GetHashCode ();
				hash = hash * 23 + Major.GetHashCode ();
				return hash;
			}
		}
	}
}
