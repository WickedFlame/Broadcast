﻿using System;
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
	public class BackgroundTaskClientApiTests
	{
		[SetUp]
		public void Setup()
		{
			TaskStore.Default.Clear();
			Broadcaster.Setup(s => { });
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			TaskStore.Default.Clear();
			Broadcaster.Setup(s => { });
		}

		[Test]
		public void BackgroundTaskClient_Api_Send_StaticTrace()
		{
			// execute a static method
			// serializeable
			BackgroundTaskClient.Send(() => Trace.WriteLine("test"));

			Broadcaster.Server.WaitAll();
			Assert.AreEqual(1, Broadcaster.Server.Context.ProcessedTasks.Count());
		}

		[Test]
		public void BackgroundTaskClient_Api_Send_Method()
		{
			// execute a local method
			// serializeable
			BackgroundTaskClient.Send(() => TestMethod(1));

			Broadcaster.Server.WaitAll();
			Assert.AreEqual(1, Broadcaster.Server.Context.ProcessedTasks.Count());
		}

		[Test]
		public void BackgroundTaskClient_Api_Send_GenericMethod()
		{
			// execute a generic method
			// serializeable
			BackgroundTaskClient.Send(() => GenericMethod(1));

			Broadcaster.Server.WaitAll();
			Assert.AreEqual(1, Broadcaster.Server.Context.ProcessedTasks.Count());
		}

		[Test]
		public void BackgroundTaskClient_Api_Send_Notification_Class()
		{
			// send a event to a handler
			// serializeable Func<TestClass>
			BackgroundTaskClient.Send<TestClass>(() => new TestClass(1));

			Broadcaster.Server.WaitAll();
			Assert.AreEqual(1, Broadcaster.Server.Context.ProcessedTasks.Count());
		}

		[Test]
		public void BackgroundTaskClient_Api_Send_Notification_Method()
		{
			// send a event to a handler
			// serializeable Func<TestClass>
			BackgroundTaskClient.Send<TestClass>(() => Returnable(1));

			Broadcaster.Server.WaitAll();
			Assert.AreEqual(1, Broadcaster.Server.Context.ProcessedTasks.Count());
		}

		[Test]
		public void BackgroundTaskClient_Api_Send_Notification_Local()
		{
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
			// execute a static method
			// serializeable
			BackgroundTaskClient.Schedule(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(1));

			Task.Delay(1500).Wait();

			Assert.GreaterOrEqual(Broadcaster.Server.Context.ProcessedTasks.Count(), 1);
		}

		[Test]
		public void BackgroundTaskClient_Api_Schedule_Method()
		{
			// execute a local method
			// serializeable
			BackgroundTaskClient.Schedule(() => TestMethod(1), TimeSpan.FromSeconds(1));

			Task.Delay(1500).Wait();

			Assert.GreaterOrEqual(Broadcaster.Server.Context.ProcessedTasks.Count(), 1);
		}

		[Test]
		public void BackgroundTaskClient_Api_Schedule_GenericMethod()
		{
			// execute a generic method
			// serializeable
			BackgroundTaskClient.Schedule(() => GenericMethod(1), TimeSpan.FromSeconds(1));

			Task.Delay(1500).Wait();

			Assert.GreaterOrEqual(Broadcaster.Server.Context.ProcessedTasks.Count(), 1);
		}

		[Test]
		public void BackgroundTaskClient_Api_Schedule_Notification_Class()
		{
			// send a event to a handler
			// Nonserializeable Func<TestClass>
			BackgroundTaskClient.Schedule<TestClass>(() => new TestClass(1), TimeSpan.FromSeconds(1));

			Task.Delay(1500).Wait();

			Assert.GreaterOrEqual(Broadcaster.Server.Context.ProcessedTasks.Count(), 1);
		}

		[Test]
		public void BackgroundTaskClient_Api_Schedule_Notification_Method()
		{
			// send a event to a handler
			// Nonserializeable Func<TestClass>
			BackgroundTaskClient.Schedule<TestClass>(() => Returnable(1), TimeSpan.FromSeconds(1));

			Task.Delay(1500).Wait();

			Assert.GreaterOrEqual(Broadcaster.Server.Context.ProcessedTasks.Count(), 1);
		}

		[Test]
		public void BackgroundTaskClient_Api_Schedule_Notification_Lopcal()
		{
			// send a local action
			// Nonserializeable
			BackgroundTaskClient.Schedule(() =>
			{
				Trace.WriteLine("test");
			}, TimeSpan.FromSeconds(1));

			Task.Delay(1500).Wait();

			Assert.GreaterOrEqual(Broadcaster.Server.Context.ProcessedTasks.Count(), 1);
		}




		[Test]
		public void BackgroundTaskClient_Api_Recurring_StaticTrace()
		{
			// execute a static method
			// serializeable
			BackgroundTaskClient.Recurring(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(0.5));

			Task.Delay(2000).Wait();

			Assert.GreaterOrEqual(Broadcaster.Server.Context.ProcessedTasks.Count(), 2);
		}

		[Test]
		public void BackgroundTaskClient_Api_Recurring_Method()
		{
			// execute a local method
			// serializeable
			BackgroundTaskClient.Recurring(() => TestMethod(1), TimeSpan.FromSeconds(0.5));

			Task.Delay(2000).Wait();

			Assert.GreaterOrEqual(Broadcaster.Server.Context.ProcessedTasks.Count(), 2);
		}

		[Test]
		public void BackgroundTaskClient_Api_Recurring_GenericMethod()
		{
			// execute a generic method
			// serializeable
			BackgroundTaskClient.Recurring(() => GenericMethod(1), TimeSpan.FromSeconds(0.5));

			Task.Delay(2000).Wait();

			Assert.GreaterOrEqual(Broadcaster.Server.Context.ProcessedTasks.Count(), 2);
		}

		[Test]
		public void BackgroundTaskClient_Api_Recurring_Notification_Class()
		{
			// send a event to a handler
			// Nonserializeable Func<TestClass>
			BackgroundTaskClient.Recurring<TestClass>(() => new TestClass(1), TimeSpan.FromSeconds(0.5));

			Task.Delay(2000).Wait();

			Assert.GreaterOrEqual(Broadcaster.Server.Context.ProcessedTasks.Count(), 2);
		}

		[Test]
		public void BackgroundTaskClient_Api_Recurring_Notification_Method()
		{
			// send a event to a handler
			// Nonserializeable Func<TestClass>
			BackgroundTaskClient.Recurring<TestClass>(() => Returnable(1), TimeSpan.FromSeconds(0.5));

			Task.Delay(2000).Wait();

			Assert.GreaterOrEqual(Broadcaster.Server.Context.ProcessedTasks.Count(), 2);
		}

		[Test]
		public void BackgroundTaskClient_Api_Recurring_Notification_Lopcal()
		{
			// send a local action
			// Nonserializeable
			BackgroundTaskClient.Recurring(() =>
			{
				Trace.WriteLine("test");
			}, TimeSpan.FromSeconds(0.5));

			Task.Delay(2000).Wait();

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
