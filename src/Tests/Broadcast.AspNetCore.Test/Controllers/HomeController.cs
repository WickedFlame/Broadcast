using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Broadcast.AspNetCore.Test.Models;

namespace Broadcast.AspNetCore.Test.Controllers
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

		public IActionResult CreateLongTaskInvalidInvocation()
		{
			BackgroundTaskClient.Send(() => LongRunningMethod());
			return Redirect("Index");
		}

		public IActionResult CreateLongTask()
		{
			var service = new TaskService();
			BackgroundTaskClient.Send(() => service.LongRunningMethod());
			return Redirect("Index");
		}

		public IActionResult CreateFailingTask()
		{
			var service = new TaskService();
			BackgroundTaskClient.Send(() => service.FailingMethod());
			return Redirect("Index");
		}

		public IActionResult MultipleTask()
		{
			var service = new TaskService();
			for(var i = 1;i<=10;i++)
			{
				BackgroundTaskClient.Send(() => service.OutputMethod(i));
			}
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
	}

	public class TaskService
	{
		public void LongRunningMethod()
		{
			Console.WriteLine("Long Task started");
			Thread.Sleep(TimeSpan.FromSeconds(4));
			Console.WriteLine("Long Task ended");
		}

		public void OutputMethod(int number)
		{
			Console.WriteLine("Long Task started");
			Thread.Sleep(TimeSpan.FromSeconds(2));
			Trace.WriteLine($"Task number {number}");
			Console.WriteLine("Long Task ended");
		}

		public void FailingMethod()
		{
			throw new Exception("This is a failing task");
		}
	}
}
