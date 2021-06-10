using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Broadcast.AspNet.Test.Models;
using Broadcast.EventSourcing;
using Broadcast.Server;
using Broadcast.Storage;
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
			var monitoring = new MonitoringService(_store);

			var model = new DashboardModel
			{
				Monitor = new MonitorModel
				{
					Servers = monitoring.GetServers(),
					Tasks = monitoring.GetAllTasks(),
					RecurringTasks = monitoring.GetRecurringTasks()
				}
			};

			return View(model);
		}

		public IActionResult Servers()
		{
			var monitoring = new MonitoringService(_store);

			var model = new DashboardModel
			{
				Monitor = new MonitorModel
				{
					Servers = monitoring.GetServers(),
					Tasks = monitoring.GetAllTasks(),
					RecurringTasks = monitoring.GetRecurringTasks()
				},
				Action = "Servers"
			};

			return View(model);
		}

		public IActionResult RecurringTasks()
		{
			var monitoring = new MonitoringService(_store);

			var model = new DashboardModel
			{
				Monitor = new MonitorModel
				{
					Servers = monitoring.GetServers(),
					Tasks = monitoring.GetAllTasks(),
					RecurringTasks = monitoring.GetRecurringTasks()
				},
				Action = "RecurringTasks"
			};

			return View(model);
		}

		public IActionResult EnqueuedTasks()
		{
			var monitoring = new MonitoringService(_store);

			var model = new DashboardModel
			{
				Monitor = new MonitorModel
				{
					Servers = monitoring.GetServers(),
					Tasks = monitoring.GetAllTasks(),
					RecurringTasks = monitoring.GetRecurringTasks()
				},
				Action = "EnqueuedTasks"
			};

			return View(model);
		}

		public IActionResult ProcessedTasks()
		{
			var monitoring = new MonitoringService(_store);

			var model = new DashboardModel
			{
				Monitor = new MonitorModel
				{
					Servers = monitoring.GetServers(),
					Tasks = monitoring.GetAllTasks(),
					RecurringTasks = monitoring.GetRecurringTasks()
				},
				Action = "ProcessedTasks"
			};

			return View(model);
		}

		public IActionResult FailedTasks()
		{
			var monitoring = new MonitoringService(_store);

			var model = new DashboardModel
			{
				Monitor = new MonitorModel
				{
					Servers = monitoring.GetServers(),
					Tasks = monitoring.GetAllTasks(),
					RecurringTasks = monitoring.GetRecurringTasks()
				},
				Action = "FailedTasks"
			};

			return View(model);
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
		}
	}

	public class MonitoringService
	{
		private readonly ITaskStore _store;

		public MonitoringService(ITaskStore store)
		{
			_store = store ?? throw new ArgumentNullException(nameof(store));
		}



		public IEnumerable<ServerDescription> GetServers()
		{
			return _store.Servers.Select(s => new ServerDescription
			{
				Id = s.Id, 
				Name = s.Name, 
				Heartbeat = s.Heartbeat
			});
		}

		public IEnumerable<TaskDescription> GetAllTasks()
		{
			return _store.Select(t => new TaskDescription
			{
				Id = t.Id, 
				Name = t.Name,
				IsRecurring = t.IsRecurring, 
				State = t.State,
				Time = t.Time
			});
		}

		public IEnumerable<RecurringTaskDescription> GetRecurringTasks()
		{
			IEnumerable<RecurringTaskDescription> recurring = null;
			_store.Storage(s =>
			{
				var keys = s.GetKeys(new StorageKey($"tasks:recurring:"));

				recurring = keys.Select(k => s.Get<RecurringTask>(new StorageKey(k)))
					.Select(m => new RecurringTaskDescription
					{
						Name = m.Name,
						NextExecution = m.NextExecution
					});
			});

			return recurring;
		}
	}

	public class ServerDescription
	{
		public string Id { get; set; }

		public string Name { get; set; }

		public DateTime Heartbeat { get; set; }
	}

	public class TaskDescription
	{
		public string Id { get; set; }

		public string Name { get; set; }

		public bool IsRecurring { get; set; }

		public TaskState State { get; set; }

		public TimeSpan? Time { get; set; }
		
	}

	public class RecurringTaskDescription
	{
		public string Name { get; set; }

		public DateTime NextExecution { get; set; }
	}
}
