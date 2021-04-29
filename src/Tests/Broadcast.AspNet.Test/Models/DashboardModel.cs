using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Broadcast.AspNet.Test.Controllers;

namespace Broadcast.AspNet.Test.Models
{
	public class DashboardModel
	{
		public MonitorModel Monitor { get; set; }
	}

	public class MonitorModel
	{
		public IEnumerable<ServerDescription> Servers { get; set; }

		public IEnumerable<TaskDescription> Tasks { get; set; }

		public IEnumerable<RecurringTaskDescription> RecurringTasks { get; set; }
	}
}
