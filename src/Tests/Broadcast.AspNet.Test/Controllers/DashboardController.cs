using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Broadcast.AspNet.Test.Models;
using Broadcast.EventSourcing;
using Microsoft.AspNetCore.Mvc;

namespace Broadcast.AspNet.Test.Controllers
{
	public class DashboardController : Controller
	{
		private readonly ITaskStore _store;

		public DashboardController(ITaskStore store)
		{
			_store = store;
		}

		public IActionResult Index()
		{
			var model = new DashboardModel
			{
				Monitor = new Monitor(_store)
			};

			return View(model);
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}

	public class Monitor
	{
		public Monitor(ITaskStore store)
		{
			Servers = store.Servers.Select(s => new Server {Id = s.Id, Name = s.Name, Heartbeat = s.Heartbeat});
			Tasks = store.Select(t => new TaskModel {Id = t.Id, IsRecurring = t.IsRecurring, State = t.State, Time = t.Time});
		}

		

		public IEnumerable<Server> Servers { get; set; }

		public IEnumerable<TaskModel> Tasks { get; set; }
	}

	public class Server
	{
		public string Id { get; set; }

		public string Name { get; set; }

		public DateTime Heartbeat { get; set; }
	}

	public class TaskModel
	{
		public string Id { get; set; }

		public bool IsRecurring { get; set; }

		public TaskState State { get; set; }

		public TimeSpan? Time { get; set; }
	}
}
