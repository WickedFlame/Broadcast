using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Broadcast.EventSourcing;
using NUnit.Framework;

namespace Broadcast.Test.Composition
{
	public class BackgroundTaskClientTaskGenerationTests
	{
		[Test]
		public void BackgroundTaskClient_TaskGeneration_Recurring_Id()
		{
			BroadcastServer.Setup(s => { });

			// execute a static method
			// serializeable
			Assert.IsNotEmpty(BackgroundTaskClient.Recurring(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(0.5)));
		}

		[Test]
		public void BackgroundTaskClient_TaskGeneration_Schedule_Id()
		{
			BroadcastServer.Setup(s => { });

			// execute a static method
			// serializeable
			Assert.IsNotEmpty(BackgroundTaskClient.Schedule(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(0.5)));
		}

		[Test]
		public void BackgroundTaskClient_TaskGeneration_Send_Id()
		{
			BroadcastServer.Setup(s => { });

			// execute a static method
			// serializeable
			Assert.IsNotEmpty(BackgroundTaskClient.Send(() => Trace.WriteLine("test")));
		}

		[Test]
		public void BackgroundTaskClient_TaskGeneration_Recurring_Notify_Id()
		{
			BroadcastServer.Setup(s => { });

			// execute a static method
			// serializeable
			Assert.IsNotEmpty(BackgroundTaskClient.Recurring<TestClass>(() => new TestClass(1), TimeSpan.FromSeconds(0.5)));
		}

		[Test]
		public void BackgroundTaskClient_TaskGeneration_Schedule_Notify_Id()
		{
			BroadcastServer.Setup(s => { });

			// execute a static method
			// serializeable
			Assert.IsNotEmpty(BackgroundTaskClient.Schedule<TestClass>(() => new TestClass(1), TimeSpan.FromSeconds(0.5)));
		}

		[Test]
		public void BackgroundTaskClient_TaskGeneration_Send_Notify_Id()
		{
			BroadcastServer.Setup(s => { });

			// execute a static method
			// serializeable
			Assert.IsNotEmpty(BackgroundTaskClient.Send<TestClass>(() => new TestClass(1)));
		}

		public class TestClass : INotification
		{
			public TestClass(int i) { }
		}
	}
}
