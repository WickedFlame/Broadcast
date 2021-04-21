using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Broadcast.EventSourcing;
using NUnit.Framework;

namespace Broadcast.Integration.Test.Composition
{
	[SingleThreaded]
	[Explicit]
	[Category("Integration")]
	public class BackgroundTaskClientTaskGenerationTests
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
		public void BackgroundTaskClient_TaskGeneration_Send()
		{
			// execute a static method
			// serializeable
			BackgroundTaskClient.Send(() => Trace.WriteLine("test"));

			Assert.IsAssignableFrom<ActionTask>(Broadcaster.Server.GetStore().Single());
		}

		[Test]
		public void BackgroundTaskClient_TaskGeneration_Schedule()
		{
			// execute a static method
			// serializeable
			BackgroundTaskClient.Schedule(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(0.5));

			Task.Delay(1000).Wait();

			Assert.IsAssignableFrom<ActionTask>(Broadcaster.Server.GetStore().Last());
		}

		[Test]
		public void BackgroundTaskClient_TaskGeneration_Recurring()
		{
			// execute a static method
			// serializeable
			BackgroundTaskClient.Recurring(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(0.5));

			Task.Delay(1000).Wait();

			Assert.IsAssignableFrom<ActionTask>(Broadcaster.Server.GetStore().First());
		}

		[Test]
		public void BackgroundTaskClient_TaskGeneration_Send_Notify()
		{
			// execute a static method
			// serializeable
			BackgroundTaskClient.Send<TestClass>(() => new TestClass(1));

			Assert.IsAssignableFrom<DelegateTask<TestClass>>(Broadcaster.Server.GetStore().First());
		}

		[Test]
		public void BackgroundTaskClient_TaskGeneration_Schedule_Notify()
		{
			// execute a static method
			// serializeable
			BackgroundTaskClient.Schedule<TestClass>(() => new TestClass(1), TimeSpan.FromSeconds(0.5));

			Task.Delay(1000).Wait();

			Assert.IsAssignableFrom<DelegateTask<TestClass>>(Broadcaster.Server.GetStore().First());
		}

		[Test]
		public void BackgroundTaskClient_TaskGeneration_Recurring_Notify()
		{
			// execute a static method
			// serializeable
			BackgroundTaskClient.Recurring<TestClass>(() => new TestClass(1), TimeSpan.FromSeconds(0.5));

			Task.Delay(1000).Wait();

			Assert.IsAssignableFrom<DelegateTask<TestClass>>(Broadcaster.Server.GetStore().First());
		}

		public class TestClass : INotification
		{
			public TestClass(int i) { }
		}
	}
}
