using System;

namespace Broadcast.Monitoring
{
	public class RecurringTaskDescription
	{
		public string Name { get; set; }

		public DateTime NextExecution { get; set; }
	}
}
