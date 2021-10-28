using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Broadcast.EventSourcing;
using NUnit.Framework;

namespace Broadcast.Integration.Test.Api
{
	[SingleThreaded]
	[Explicit]
	[Category("Integration")]
	public class TaskServerClientApiTests
	{
		[SetUp]
		public void Setup()
		{
			TaskStore.Default.Clear();
			BroadcastServer.Setup(s => { });
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			TaskStore.Default.Clear();
			BroadcastServer.Setup(s => { });
		}

		[Test]
		public void TaskServerClient_Api_Send_StaticTrace()
		{
			// execute a static method
			// serializeable
			BackgroundTask.Send(() => Trace.WriteLine("test"));

			BroadcastServer.Server.WaitAll();
			Assert.AreEqual(1, BroadcastServer.Server.GetProcessedTasks().Count());
		}

		[Test]
		public void TaskServerClient_Api_Send_Method()
		{
			// execute a local method
			// serializeable
			BackgroundTask.Send(() => TestMethod(1));

			BroadcastServer.Server.WaitAll();
			Assert.AreEqual(1, BroadcastServer.Server.GetProcessedTasks().Count());
		}

		[Test]
		public void TaskServerClient_Api_Send_GenericMethod()
		{
			// execute a generic method
			// serializeable
			BackgroundTask.Send(() => GenericMethod(1));

			BroadcastServer.Server.WaitAll();
			Assert.AreEqual(1, BroadcastServer.Server.GetProcessedTasks().Count());
		}
		








		[Test]
		public void TaskServerClient_Api_Schedule_StaticTrace()
		{
			// execute a static method
			// serializeable
			BackgroundTask.Schedule(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(1));

			Task.Delay(1500).Wait();

			Assert.GreaterOrEqual(BroadcastServer.Server.GetProcessedTasks().Count(), 1);
		}

		[Test]
		public void TaskServerClient_Api_Schedule_Method()
		{
			// execute a local method
			// serializeable
			BackgroundTask.Schedule(() => TestMethod(1), TimeSpan.FromSeconds(1));

			Task.Delay(1500).Wait();

			Assert.GreaterOrEqual(BroadcastServer.Server.GetProcessedTasks().Count(), 1);
		}

		[Test]
		public void TaskServerClient_Api_Schedule_GenericMethod()
		{
			// execute a generic method
			// serializeable
			BackgroundTask.Schedule(() => GenericMethod(1), TimeSpan.FromSeconds(1));

			Task.Delay(1500).Wait();

			Assert.GreaterOrEqual(BroadcastServer.Server.GetProcessedTasks().Count(), 1);
		}
		


		[Test]
		public void TaskServerClient_Api_Recurring_StaticTrace()
		{
			// execute a static method
			// serializeable
			BackgroundTask.Recurring(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(0.5));

			Task.Delay(2000).Wait();

			Assert.GreaterOrEqual(BroadcastServer.Server.GetProcessedTasks().Count(), 2);
		}

		[Test]
		public void TaskServerClient_Api_Recurring_Method()
		{
			// execute a local method
			// serializeable
			BackgroundTask.Recurring(() => TestMethod(1), TimeSpan.FromSeconds(0.5));

			Task.Delay(2000).Wait();

			Assert.GreaterOrEqual(BroadcastServer.Server.GetProcessedTasks().Count(), 2);
		}

		[Test]
		public void TaskServerClient_Api_Recurring_GenericMethod()
		{
			// execute a generic method
			// serializeable
			BackgroundTask.Recurring(() => GenericMethod(1), TimeSpan.FromSeconds(0.5));

			Task.Delay(2000).Wait();

			Assert.GreaterOrEqual(BroadcastServer.Server.GetProcessedTasks().Count(), 2);
		}

		[Test]
		public void TaskServerClient_Api_Recurring_Name()
		{
			// execute a generic method
			// serializeable
			BackgroundTask.Recurring("TaskServerClient_Api_Recurring", () => GenericMethod(1), TimeSpan.FromSeconds(0.5));

			Task.Delay(2000).Wait();

			Assert.IsTrue(BroadcastServer.Server.GetProcessedTasks().All(t => t.Name == "TaskServerClient_Api_Recurring"));
		}

		public void TestMethod(int i) { }

		public void GenericMethod<T>(T value) { }
	}
}
