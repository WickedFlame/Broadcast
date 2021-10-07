
using System.Collections.Generic;

namespace Broadcast
{
	/// <summary>
	/// Interface for server setup configuration
	/// </summary>
	public interface IServerSetup
	{
		/// <summary>
		/// The context for the configuration
		/// </summary>
		Dictionary<string, object> Context { get; }
	}
}
