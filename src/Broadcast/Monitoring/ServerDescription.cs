using System;

namespace Broadcast.Monitoring
{
	/// <summary>
	/// Description of a registered server
	/// </summary>
	public class ServerDescription
	{
		/// <summary>
		/// Gets or sets the Id of the Server
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the Name of the Server
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the last heartbeat of the Server
		/// </summary>
		public DateTime Heartbeat { get; set; }
	}
}
