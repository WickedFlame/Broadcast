using System.Collections.Generic;

namespace Broadcast.Setup
{
	/// <summary>
	/// Implementation for <see cref="IServerSetup"/>
	/// </summary>
	public class ServerSetup : IServerSetup
	{
		/// <summary>
		/// The context for the configuration
		/// </summary>
		public Dictionary<string, object> Context { get; } = new Dictionary<string, object>();
	}
}
