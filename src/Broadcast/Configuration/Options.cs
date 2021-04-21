using System;

namespace Broadcast.Configuration
{
	public class Options
	{
		public static Options Default { get; private set; } = new Options();

		public string ServerName { get; set; } = Environment.MachineName;

		public static void Setup(Action<Options> setup)
		{
			var options = new Options();
			setup(options);
			Default = options;
		}
	}
}
