using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;

namespace Broadcast.Test.Api
{
	[SingleThreaded]
	public class BackgroundTaskClientApiTests
	{
		[Test]
		public void BackgroundTaskClient_Api_Send_StaticTrace()
		{
			Broadcaster.Setup(s => { });

			// execute a static method
			// serializeable
			BackgroundTaskClient.Send(() => Trace.WriteLine("test"));

			Broadcaster.Server.WaitAll();
			Assert.AreEqual(1, Broadcaster.Server.Context.ProcessedTasks.Count());
		}

		[Test]
		public void BackgroundTaskClient_Api_Send_Method()
		{
			Broadcaster.Setup(s => { });

			// execute a local method
			// serializeable
			BackgroundTaskClient.Send(() => TestMethod(1));

			Broadcaster.Server.WaitAll();
			Assert.AreEqual(1, Broadcaster.Server.Context.ProcessedTasks.Count());
		}

		[Test]
		public void BackgroundTaskClient_Api_Send_GenericMethod()
		{
			Broadcaster.Setup(s => { });

			// execute a generic method
			// serializeable
			BackgroundTaskClient.Send(() => GenericMethod(1));

			Broadcaster.Server.WaitAll();
			Assert.AreEqual(1, Broadcaster.Server.Context.ProcessedTasks.Count());
		}

		[Test]
		public void BackgroundTaskClient_Api_Send_Notification_Class()
		{
			Broadcaster.Setup(s => { });

			// send a event to a handler
			// serializeable Func<TestClass>
			BackgroundTaskClient.Send<TestClass>(() => new TestClass(1));

			Broadcaster.Server.WaitAll();
			Assert.AreEqual(1, Broadcaster.Server.Context.ProcessedTasks.Count());
		}

		[Test]
		public void BackgroundTaskClient_Api_Send_Notification_Method()
		{
			Broadcaster.Setup(s => { });

			// send a event to a handler
			// serializeable Func<TestClass>
			BackgroundTaskClient.Send<TestClass>(() => Returnable(1));

			Broadcaster.Server.WaitAll();
			Assert.AreEqual(1, Broadcaster.Server.Context.ProcessedTasks.Count());
		}

		[Test]
		public void BackgroundTaskClient_Api_Send_Notification_Local()
		{
			Broadcaster.Setup(s => { });

			// send a local action
			// Nonserializeable
			BackgroundTaskClient.Send(() =>
			{
				Trace.WriteLine("test");
			});

			Broadcaster.Server.WaitAll();
			Assert.AreEqual(1, Broadcaster.Server.Context.ProcessedTasks.Count());
		}




		[Test]
		public void BackgroundTaskClient_Api_Schedule_StaticTrace()
		{
			Broadcaster.Setup(s => { });

			// execute a static method
			// serializeable
			BackgroundTaskClient.Schedule(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(1));

			Thread.Sleep(TimeSpan.FromSeconds(1.5));

			Assert.GreaterOrEqual(Broadcaster.Server.Context.ProcessedTasks.Count(), 1);
		}

		[Test]
		public void BackgroundTaskClient_Api_Schedule_Method()
		{
			Broadcaster.Setup(s => { });

			// execute a local method
			// serializeable
			BackgroundTaskClient.Schedule(() => TestMethod(1), TimeSpan.FromSeconds(1));

			Thread.Sleep(TimeSpan.FromSeconds(1.5));

			Assert.GreaterOrEqual(Broadcaster.Server.Context.ProcessedTasks.Count(), 1);
		}

		[Test]
		public void BackgroundTaskClient_Api_Schedule_GenericMethod()
		{
			Broadcaster.Setup(s => { });

			// execute a generic method
			// serializeable
			BackgroundTaskClient.Schedule(() => GenericMethod(1), TimeSpan.FromSeconds(1));

			Thread.Sleep(TimeSpan.FromSeconds(1.5));

			Assert.GreaterOrEqual(Broadcaster.Server.Context.ProcessedTasks.Count(), 1);
		}

		[Test]
		public void BackgroundTaskClient_Api_Schedule_Notification_Class()
		{
			Broadcaster.Setup(s => { });

			// send a event to a handler
			// Nonserializeable Func<TestClass>
			BackgroundTaskClient.Schedule<TestClass>(() => new TestClass(1), TimeSpan.FromSeconds(1));

			Thread.Sleep(TimeSpan.FromSeconds(1.5));

			Assert.GreaterOrEqual(Broadcaster.Server.Context.ProcessedTasks.Count(), 1);
		}

		[Test]
		public void BackgroundTaskClient_Api_Schedule_Notification_Method()
		{
			Broadcaster.Setup(s => { });

			// send a event to a handler
			// Nonserializeable Func<TestClass>
			BackgroundTaskClient.Schedule<TestClass>(() => Returnable(1), TimeSpan.FromSeconds(1));

			Thread.Sleep(TimeSpan.FromSeconds(1.5));

			Assert.GreaterOrEqual(Broadcaster.Server.Context.ProcessedTasks.Count(), 1);
		}

		[Test]
		public void BackgroundTaskClient_Api_Schedule_Notification_Lopcal()
		{
			Broadcaster.Setup(s => { }); 

			// send a local action
			// Nonserializeable
			BackgroundTaskClient.Schedule(() =>
			{
				Trace.WriteLine("test");
			}, TimeSpan.FromSeconds(1));

			Thread.Sleep(TimeSpan.FromSeconds(1.5));

			Assert.GreaterOrEqual(Broadcaster.Server.Context.ProcessedTasks.Count(), 1);
		}




		[Test]
		public void BackgroundTaskClient_Api_Recurring_StaticTrace()
		{
			Broadcaster.Setup(s => { });

			// execute a static method
			// serializeable
			BackgroundTaskClient.Recurring(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(0.5));

			Thread.Sleep(TimeSpan.FromSeconds(2));

			Assert.GreaterOrEqual(Broadcaster.Server.Context.ProcessedTasks.Count(), 2);
		}

		[Test]
		public void BackgroundTaskClient_Api_Recurring_Method()
		{
			Broadcaster.Setup(s => { });

			// execute a local method
			// serializeable
			BackgroundTaskClient.Recurring(() => TestMethod(1), TimeSpan.FromSeconds(0.5));

			Thread.Sleep(TimeSpan.FromSeconds(2));

			Assert.GreaterOrEqual(Broadcaster.Server.Context.ProcessedTasks.Count(), 2);
		}

		[Test]
		public void BackgroundTaskClient_Api_Recurring_GenericMethod()
		{
			Broadcaster.Setup(s => { });

			// execute a generic method
			// serializeable
			BackgroundTaskClient.Recurring(() => GenericMethod(1), TimeSpan.FromSeconds(0.5));

			Thread.Sleep(TimeSpan.FromSeconds(2));

			Assert.GreaterOrEqual(Broadcaster.Server.Context.ProcessedTasks.Count(), 2);
		}

		[Test]
		public void BackgroundTaskClient_Api_Recurring_Notification_Class()
		{
			Broadcaster.Setup(s => { });

			// send a event to a handler
			// Nonserializeable Func<TestClass>
			BackgroundTaskClient.Recurring<TestClass>(() => new TestClass(1), TimeSpan.FromSeconds(0.5));

			Thread.Sleep(TimeSpan.FromSeconds(2));

			Assert.GreaterOrEqual(Broadcaster.Server.Context.ProcessedTasks.Count(), 2);
		}

		[Test]
		public void BackgroundTaskClient_Api_Recurring_Notification_Method()
		{
			Broadcaster.Setup(s => { });

			// send a event to a handler
			// Nonserializeable Func<TestClass>
			BackgroundTaskClient.Recurring<TestClass>(() => Returnable(1), TimeSpan.FromSeconds(0.5));

			Thread.Sleep(TimeSpan.FromSeconds(2));

			Assert.GreaterOrEqual(Broadcaster.Server.Context.ProcessedTasks.Count(), 2);
		}

		[Test]
		public void BackgroundTaskClient_Api_Recurring_Notification_Lopcal()
		{
			Broadcaster.Setup(s => { });

			// send a local action
			// Nonserializeable
			BackgroundTaskClient.Recurring(() =>
			{
				Trace.WriteLine("test");
			}, TimeSpan.FromSeconds(0.5));

			Thread.Sleep(TimeSpan.FromSeconds(2));

			Assert.GreaterOrEqual(Broadcaster.Server.Context.ProcessedTasks.Count(), 2);
		}





		public void TestMethod(int i) { }

		public void GenericMethod<T>(T value) { }

		public TestClass Returnable(int i) => new TestClass(i);

		public class TestClass : INotification
		{
			public TestClass(int i) { }
		}
	}
}
