using System;
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
			var broadcaster = new Broadcaster(new TaskStore());

			// execute a static method
			// serializeable
			broadcaster.Send(() => Trace.WriteLine("test"));

			broadcaster.WaitAll();
			Assert.AreEqual(1, broadcaster.GetProcessedTasks().Count());
		}

		[Test]
		public void Broadcaster_Api_Send_Method()
		{
			var broadcaster = new Broadcaster(new TaskStore());

			// execute a local method
			// serializeable
			broadcaster.Send(() => TestMethod(1));

			broadcaster.WaitAll();
			Assert.AreEqual(1, broadcaster.GetProcessedTasks().Count());
		}

		[Test]
		public void Broadcaster_Api_Send_GenericMethod()
		{
			var broadcaster = new Broadcaster(new TaskStore());

			// execute a generic method
			// serializeable
			broadcaster.Send(() => GenericMethod(1));

			broadcaster.WaitAll();
			Assert.AreEqual(1, broadcaster.GetProcessedTasks().Count());
		}

		[Test]
		public void Broadcaster_Api_Execute_Local()
		{
			var broadcaster = new Broadcaster(new TaskStore());
			var i = 0;

			// execute a static method
			// serializeable
			broadcaster.Execute(() => i = 1);

			broadcaster.WaitAll();
			Assert.AreEqual(1, broadcaster.GetProcessedTasks().Count());
			Assert.AreEqual(1, i);
		}

		[Test]
		public void Broadcaster_Api_Execute_StaticTrace()
		{
			var broadcaster = new Broadcaster(new TaskStore());

			// execute a static method
			// serializeable
			broadcaster.Execute(() => Trace.WriteLine("test"));

			broadcaster.WaitAll();
			Assert.AreEqual(1, broadcaster.GetProcessedTasks().Count());
		}

		[Test]
		public void Broadcaster_Api_Execute_Method()
		{
			var broadcaster = new Broadcaster(new TaskStore());

			// execute a local method
			// serializeable
			broadcaster.Execute(() => TestMethod(1));

			broadcaster.WaitAll();
			Assert.AreEqual(1, broadcaster.GetProcessedTasks().Count());
		}

		[Test]
		public void Broadcaster_Api_Execute_GenericMethod()
		{
			var broadcaster = new Broadcaster(new TaskStore());

			// execute a generic method
			// serializeable
			broadcaster.Execute(() => GenericMethod(1));

			broadcaster.WaitAll();
			Assert.AreEqual(1, broadcaster.GetProcessedTasks().Count());
		}


		[Test]
		public void Broadcaster_Api_Schedule_StaticTrace()
		{
			var broadcaster = new Broadcaster(new TaskStore());

			// execute a static method
			// serializeable
			broadcaster.Schedule(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(0.5));

			Task.Delay(1500).Wait();

			Assert.GreaterOrEqual(broadcaster.GetProcessedTasks().Count(), 1);
		}

		[Test]
		public void Broadcaster_Api_Schedule_Method()
		{
			var broadcaster = new Broadcaster(new TaskStore());

			// execute a local method
			// serializeable
			broadcaster.Schedule(() => TestMethod(1), TimeSpan.FromSeconds(0.5));

			Task.Delay(1500).Wait();

			Assert.GreaterOrEqual(broadcaster.GetProcessedTasks().Count(), 1);
		}

		[Test]
		public void Broadcaster_Api_Schedule_GenericMethod()
		{
			var broadcaster = new Broadcaster(new TaskStore());

			// execute a generic method
			// serializeable
			broadcaster.Schedule(() => GenericMethod(1), TimeSpan.FromSeconds(0.5));

			Task.Delay(1500).Wait();

			Assert.GreaterOrEqual(broadcaster.GetProcessedTasks().Count(), 1);
		}








		[Test]
		public void Broadcaster_Api_Recurring_StaticTrace()
		{
			var broadcaster = new Broadcaster(new TaskStore());

			// execute a static method
			// serializeable
			broadcaster.Recurring(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(0.5));

			Task.Delay(2000).Wait();

			Assert.GreaterOrEqual(broadcaster.GetProcessedTasks().Count(), 2);
		}

		[Test]
		public void Broadcaster_Api_Recurring_Method()
		{
			var broadcaster = new Broadcaster(new TaskStore());

			// execute a local method
			// serializeable
			broadcaster.Recurring(() => TestMethod(1), TimeSpan.FromSeconds(0.5));

			Task.Delay(2000).Wait();

			Assert.GreaterOrEqual(broadcaster.GetProcessedTasks().Count(), 2);
		}

		[Test]
		public void Broadcaster_Api_Recurring_GenericMethod()
		{
			var broadcaster = new Broadcaster(new TaskStore());

			// execute a generic method
			// serializeable
			broadcaster.Recurring(() => GenericMethod(1), TimeSpan.FromSeconds(0.5));

			Task.Delay(2000).Wait();

			Assert.GreaterOrEqual(broadcaster.GetProcessedTasks().Count(), 2);
		}
		

		public void TestMethod(int i) { }

		public void GenericMethod<T>(T value) { }
	}
}
