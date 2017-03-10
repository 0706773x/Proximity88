using System;
namespace TFGProximity.Core.Interfaces
{
	public interface ILogger
	{
		void Debug (string message);
		void Trace (string message);
	}
}
