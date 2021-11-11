using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Broadcast.Sample.AspNetCore.Models;
using Broadcast.EventSourcing;

namespace Broadcast.Sample.AspNetCore.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult StartTask()
		{
			BackgroundTask.Send(() => LongRunningMethod());
			return Redirect("Index");
		}

		public IActionResult ScheduleTask()
		{
			BackgroundTask.Schedule(() => Schedule(), TimeSpan.FromSeconds(15));

			return Redirect("Index");
		}

		public IActionResult RcurringTask()
		{
			BackgroundTask.Recurring("recurring", () => Recurring(DateTime.Now.ToString("o")), TimeSpan.FromSeconds(15));

			return Redirect("Index");
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		public void LongRunningMethod()
		{
			Console.WriteLine("Long Task started");
			Thread.Sleep(TimeSpan.FromSeconds(4));
			Console.WriteLine("Long Task ended");
		}

		public void Schedule()
		{
			Console.WriteLine("Scheduled Task started");
			Thread.Sleep(TimeSpan.FromSeconds(10));
			Console.WriteLine("Scheduled Task ended");
		}

		public void Recurring(string sceduleTime)
		{
			Console.WriteLine($"Recurring Task from {sceduleTime}");
		}
	}
}
