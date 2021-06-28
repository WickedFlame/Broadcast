using Broadcast.AspNetCore.Test.Models;
using Broadcast.Monitoring;
using Microsoft.AspNetCore.Mvc;

namespace Broadcast.AspNetCore.Test.Controllers
{
	public class DashboardApiController : ControllerBase
	{
		private readonly ITaskStore _store;

		public DashboardApiController(ITaskStore store)
		{
			_store = store;
		}

		[HttpGet]
		//[Route("/dashboard/metrics")]
		public IActionResult Get()
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
			return Ok(model);
		}
	}
}
