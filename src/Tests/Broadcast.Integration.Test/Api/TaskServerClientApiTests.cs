using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Broadcast.Integration.Test.Api
{
	[SingleThreaded]
	[Explicit]
	[Category("Integration")]
	public class TaskServerClientApiTests
	{
		[Test]
		public void TaskServerClient_Api_Send_StaticTrace()
		{
			Broadcaster.Setup(s => { });

			// execute a static method
			// serializeable
			TaskServerClient.Send(() => Trace.WriteLine("test"));

			Broadcaster.Server.WaitAll();
			Assert.AreEqual(1, Broadcaster.Server.Context.ProcessedTasks.Count());
		}

		[Test]
		public void TaskServerClient_Api_Send_Method()
		{
			Broadcaster.Setup(s => { });

			// execute a local method
			// serializeable
			TaskServerClient.Send(() => TestMethod(1));

			Broadcaster.Server.WaitAll();
			Assert.AreEqual(1, Broadcaster.Server.Context.ProcessedTasks.Count());
		}

		[Test]
		public void TaskServerClient_Api_Send_GenericMethod()
		{
			Broadcaster.Setup(s => { });

			// execute a generic method
			// serializeable
			TaskServerClient.Send(() => GenericMethod(1));

			Broadcaster.Server.WaitAll();
			Assert.AreEqual(1, Broadcaster.Server.Context.ProcessedTasks.Count());
		}
		








		[Test]
		public void TaskServerClient_Api_Schedule_StaticTrace()
		{
			Broadcaster.Setup(s => { });

			// execute a static method
			// serializeable
			TaskServerClient.Schedule(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(1));

			Task.Delay(1500).Wait();

			Assert.GreaterOrEqual(Broadcaster.Server.Context.ProcessedTasks.Count(), 1);
		}

		[Test]
		public void TaskServerClient_Api_Schedule_Method()
		{
			Broadcaster.Setup(s => { });

			// execute a local method
			// serializeable
			TaskServerClient.Schedule(() => TestMethod(1), TimeSpan.FromSeconds(1));

			Task.Delay(1500).Wait();

			Assert.GreaterOrEqual(Broadcaster.Server.Context.ProcessedTasks.Count(), 1);
		}

		[Test]
		public void TaskServerClient_Api_Schedule_GenericMethod()
		{
			Broadcaster.Setup(s => { });

			// execute a generic method
			// serializeable
			TaskServerClient.Schedule(() => GenericMethod(1), TimeSpan.FromSeconds(1));

			Task.Delay(1500).Wait();

			Assert.GreaterOrEqual(Broadcaster.Server.Context.ProcessedTasks.Count(), 1);
		}
		


		[Test]
		public void TaskServerClient_Api_Recurring_StaticTrace()
		{
			Broadcaster.Setup(s => { });

			// execute a static method
			// serializeable
			TaskServerClient.Recurring(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(0.5));

			Task.Delay(2000).Wait();

			Assert.GreaterOrEqual(Broadcaster.Server.Context.ProcessedTasks.Count(), 2);
		}

		[Test]
		public void TaskServerClient_Api_Recurring_Method()
		{
			Broadcaster.Setup(s => { });

			// execute a local method
			// serializeable
			TaskServerClient.Recurring(() => TestMethod(1), TimeSpan.FromSeconds(0.5));

			Task.Delay(2000).Wait();

			Assert.GreaterOrEqual(Broadcaster.Server.Context.ProcessedTasks.Count(), 2);
		}

		[Test]
		public void TaskServerClient_Api_Recurring_GenericMethod()
		{
			Broadcaster.Setup(s => { });

			// execute a generic method
			// serializeable
			TaskServerClient.Recurring(() => GenericMethod(1), TimeSpan.FromSeconds(0.5));

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
