using System;
using Broadcast.Storage;

namespace Broadcast.Configuration
{
	/// <summary>
	/// The options used by the Storage
	/// </summary>
	public class Options
	{
        /// <summary>
        /// Gets or set the milliseconds that the storage cleanup task is run. Defaults to each minute (60000 ms)
        /// </summary>
        public int StorageCleanupInterval { get; set; } = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;

        /// <summary>
        /// Gets or set the duration in milliseconds that a task is stored after completition or failiure. Defaults to each hour
        /// </summary>
        public int StorageLifetimeDuration { get; set; } = (int)TimeSpan.FromMinutes(60).TotalMilliseconds;
    }
}
