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
		/// Gets or sets the designated name of the <see cref="IBroadcaster"/> server.
		/// Each <see cref="IBroadcaster"/> gets an individual Id that is generated with each creation.
		/// </summary>
		public string ServerName { get; set; } = Environment.MachineName;

		/// <summary>
		/// Gets or set the milliseconds that the Hearbeat is propagated to the <see cref="IStorage"/>
		/// </summary>
		public int HeartbeatInterval { get; set; } = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;

        /// <summary>
        /// Gets or set the milliseconds that the storage cleanup task is run. Defaults to each minute (60000 ms)
        /// </summary>
        public int StorageCleanupInterval { get; set; } = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;

        /// <summary>
        /// Gets or set the duration that a task is stored after completition or failiure. Defaults to each hour
        /// </summary>
        public int StorageLifetimeDuration { get; set; } = (int)TimeSpan.FromMinutes(60).TotalMilliseconds;
    }
}
