using System;
namespace TFGProximity.Core.Models
{
	public class BeaconRegion
	{
		public string UUID { get; }
		public string Identifier { get; }
		public ushort? Major { get; }
		public ushort? Minor { get; }


		public BeaconRegion (string uuid, string identifier, ushort? major = null, ushort? minor = null)
		{
			UUID = uuid;
			Identifier = identifier;
			Major = major;
			Minor = minor;
		}

		public override string ToString ()
		{
			return $"[UUID: {UUID} - Identifier: {Identifier} - Major: {Major ?? 0} - Minor: {Minor ?? 0}]";
		}
	}
}
