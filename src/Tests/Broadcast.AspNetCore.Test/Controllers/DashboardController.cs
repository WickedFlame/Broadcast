using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Broadcast.AspNetCore.Test.Models;
using Broadcast.EventSourcing;
using Broadcast.Monitoring;
using Broadcast.Server;
using Broadcast.Storage;
using Microsoft.AspNetCore.Mvc;

namespace Broadcast.AspNetCore.Test.Controllers
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
}
