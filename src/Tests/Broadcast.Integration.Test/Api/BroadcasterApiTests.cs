﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Broadcast.Configuration;
using Broadcast.EventSourcing;
using NUnit.Framework;

namespace Broadcast.Integration.Test.Api
{
	[SingleThreaded]
	[Explicit]
	[Category("Integration")]
	public class BroadcasterApiTests
	{
		[Test]
		public void Broadcaster_Api_Send_StaticTrace()
		{
			var broadcaster = new Broadcaster ();

			// execute a static method
			// serializeable
			broadcaster.Send(() => Trace.WriteLine("test"));

			broadcaster.WaitAll();
			Assert.AreEqual(1, broadcaster.Context.ProcessedTasks.Count());
		}

		[Test]
		public void Broadcaster_Api_Send_Method()
		{
			var broadcaster = new Broadcaster ();

			// execute a local method
			// serializeable
			broadcaster.Send(() => TestMethod(1));

			broadcaster.WaitAll();
			Assert.AreEqual(1, broadcaster.Context.ProcessedTasks.Count());
		}

		[Test]
		public void Broadcaster_Api_Send_GenericMethod()
		{
			var broadcaster = new Broadcaster ();

			// execute a generic method
			// serializeable
			broadcaster.Send(() => GenericMethod(1));

			broadcaster.WaitAll();
			Assert.AreEqual(1, broadcaster.Context.ProcessedTasks.Count());
		}

		[Test]
		public void Broadcaster_Api_Send_Notification_Class()
		{
			var broadcaster = new Broadcaster {Context = new ProcessorContext(new TaskStore(new Options())) };

			// send a event to a handler
			// serializeable Func<TestClass>
			broadcaster.Send<TestClass>(() => new TestClass(1));

			broadcaster.WaitAll();
			Assert.AreEqual(1, broadcaster.Context.ProcessedTasks.Count());
		}

		[Test]
		public void Broadcaster_Api_Send_Notification_Method()
		{
			var broadcaster = new Broadcaster ();

			// send a event to a handler
			// serializeable Func<TestClass>
			broadcaster.Send<TestClass>(() => Returnable(1));

			broadcaster.WaitAll();
			Assert.AreEqual(1, broadcaster.Context.ProcessedTasks.Count());
		}



		[Test]
		public void Broadcaster_Api_Schedule_StaticTrace()
		{
			var broadcaster = new Broadcaster ();

			// execute a static method
			// serializeable
			broadcaster.Schedule(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(0.5));

			Task.Delay(1500).Wait();

			Assert.GreaterOrEqual(broadcaster.Context.ProcessedTasks.Count(), 1);
		}

		[Test]
		public void Broadcaster_Api_Schedule_Method()
		{
			var broadcaster = new Broadcaster();

			// execute a local method
			// serializeable
			broadcaster.Schedule(() => TestMethod(1), TimeSpan.FromSeconds(0.5));

			Task.Delay(1500).Wait();

			Assert.GreaterOrEqual(broadcaster.Context.ProcessedTasks.Count(), 1);
		}

		[Test]
		public void Broadcaster_Api_Schedule_GenericMethod()
		{
			var broadcaster = new Broadcaster ();

			// execute a generic method
			// serializeable
			broadcaster.Schedule(() => GenericMethod(1), TimeSpan.FromSeconds(0.5));

			Task.Delay(1500).Wait();

			Assert.GreaterOrEqual(broadcaster.Context.ProcessedTasks.Count(), 1);
		}

		[Test]
		public void Broadcaster_Api_Schedule_Notification_Class()
		{
			var broadcaster = new Broadcaster ();

			// send a event to a handler
			// Nonserializeable Func<TestClass>
			broadcaster.Schedule<TestClass>(() => new TestClass(1), TimeSpan.FromSeconds(0.5));

			Task.Delay(1500).Wait();

			Assert.GreaterOrEqual(broadcaster.Context.ProcessedTasks.Count(), 1);
		}

		[Test]
		public void Broadcaster_Api_Schedule_Notification_Method()
		{
			var broadcaster = new Broadcaster ();

			// send a event to a handler
			// Nonserializeable Func<TestClass>
			broadcaster.Schedule<TestClass>(() => Returnable(1), TimeSpan.FromSeconds(0.5));

			Task.Delay(1500).Wait();

			Assert.GreaterOrEqual(broadcaster.Context.ProcessedTasks.Count(), 1);
		}








		[Test]
		public void Broadcaster_Api_Recurring_StaticTrace()
		{
			var broadcaster = new Broadcaster ();

			// execute a static method
			// serializeable
			broadcaster.Recurring(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(0.5));

			Task.Delay(2000).Wait();

			Assert.GreaterOrEqual(broadcaster.Context.ProcessedTasks.Count(), 2);
		}

		[Test]
		public void Broadcaster_Api_Recurring_Method()
		{
			var broadcaster = new Broadcaster ();

			// execute a local method
			// serializeable
			broadcaster.Recurring(() => TestMethod(1), TimeSpan.FromSeconds(0.5));

			Task.Delay(2000).Wait();

			Assert.GreaterOrEqual(broadcaster.Context.ProcessedTasks.Count(), 2);
		}

		[Test]
		public void Broadcaster_Api_Recurring_GenericMethod()
		{
			var broadcaster = new Broadcaster ();

			// execute a generic method
			// serializeable
			broadcaster.Recurring(() => GenericMethod(1), TimeSpan.FromSeconds(0.5));

			Task.Delay(2000).Wait();

			Assert.GreaterOrEqual(broadcaster.Context.ProcessedTasks.Count(), 2);
		}

		[Test]
		public void Broadcaster_Api_Recurring_Notification_Class()
		{
			var broadcaster = new Broadcaster ();

			// send a event to a handler
			// Nonserializeable Func<TestClass>
			broadcaster.Recurring<TestClass>(() => new TestClass(1), TimeSpan.FromSeconds(0.5));

			Task.Delay(2000).Wait();

			Assert.GreaterOrEqual(broadcaster.Context.ProcessedTasks.Count(), 2);
		}

		[Test]
		public void Broadcaster_Api_Recurring_Notification_Method()
		{
			var broadcaster = new Broadcaster();

			// send a event to a handler
			// Nonserializeable Func<TestClass>
			broadcaster.Recurring<TestClass>(() => Returnable(1), TimeSpan.FromSeconds(0.5));

			Task.Delay(2000).Wait();

			Assert.GreaterOrEqual(broadcaster.Context.ProcessedTasks.Count(), 2);
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
