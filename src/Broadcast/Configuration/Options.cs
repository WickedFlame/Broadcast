using System;
using Broadcast.Storage;

namespace Broadcast.Configuration
{
	public class Options
	{
		public static Options Default { get; private set; } = new Options();

		public string ServerName { get; set; } = Environment.MachineName;

		/// <summary>
		/// Gets or set the milliseconds that the Hearbeat is propagated to the <see cref="IStorage"/>
		/// </summary>
		public int HeartbeatDelay { get; set; } = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;

		public static void Setup(Action<Options> setup)
		{
			var options = new Options();
			setup(options);
			Default = options;
		}
	}
}
