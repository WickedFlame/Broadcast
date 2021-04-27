using System;
using Broadcast.Storage;

namespace Broadcast.Configuration
{
	/// <summary>
	/// The options used by the Server
	/// </summary>
	public class Options
	{
		/// <summary>
		/// Gets the default Options if none others are passed to the Servers
		/// </summary>
		public static Options Default { get; private set; } = new Options();

		/// <summary>
		/// Gets or sets the designated name of the <see cref="IBroadcaster"/> server.
		/// Each <see cref="IBroadcaster"/> gets an individual Id that is generated with each creation.
		/// </summary>
		public string ServerName { get; set; } = Environment.MachineName;

		/// <summary>
		/// Gets or set the milliseconds that the Hearbeat is propagated to the <see cref="IStorage"/>
		/// </summary>
		public int HeartbeatInterval { get; set; } = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;

		/// <summary>
		/// Setup a new instance of the default Options
		/// </summary>
		/// <param name="setup"></param>
		public static void Setup(Action<Options> setup)
		{
			var options = new Options();
			setup(options);
			Default = options;
		}
	}
}
