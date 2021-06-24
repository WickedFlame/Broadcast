using System;
using Broadcast.EventSourcing;

namespace Broadcast.Monitoring
{
	public class TaskDescription
	{
		public string Id { get; set; }

		public string Name { get; set; }

		public bool IsRecurring { get; set; }

		public TaskState State { get; set; }

		public TimeSpan? Time { get; set; }

	}
}
