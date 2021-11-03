using System.Collections.Generic;
using Broadcast.Monitoring;

namespace Broadcast.AspNetCore.Test.Models
{
	public class DashboardModel
	{
		public MonitorModel Monitor { get; set; }

		public string Action { get; set; }
	}

	public class MonitorModel
	{
		public IEnumerable<ServerDescription> Servers { get; set; }

		public IEnumerable<TaskDescription> Tasks { get; set; }

		public IEnumerable<RecurringTaskDescription> RecurringTasks { get; set; }
	}
}
