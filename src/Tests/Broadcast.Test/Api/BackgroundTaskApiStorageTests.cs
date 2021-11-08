using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Broadcast.EventSourcing;
using Broadcast.Processing;
using Broadcast.Storage;
using Moq;
using NUnit.Framework;

namespace Broadcast.Test.Api
{
	[SingleThreaded]
	public class BackgroundTaskApiStorageTests
	{
		private Mock<ITaskProcessor> _processor;
		private Mock<IScheduler> _scheduler;
		private Mock<ITaskStore> _store;

		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			_processor = new Mock<ITaskProcessor>();
			_scheduler = new Mock<IScheduler>();
			_store = new Mock<ITaskStore>();
			//var store = new TaskStore();

			BroadcastServer.Setup(s =>
				s.AddScheduler(_scheduler.Object)
					.AddProcessor(_processor.Object)
					.AddTaskStore(_store.Object)
			);

			BackgroundTask.Setup(() => new BroadcastingClient(_store.Object));
		}

		[OneTimeTearDown]
		public void OneTimeTeardown()
		{
			BackgroundTask.Setup(() => new BroadcastingClient());
		}

		[SetUp]
		public void Setup()
		{
			_processor.Reset();
			_scheduler.Reset();
			_store.Reset();
		}

		[Test]
		public void BackgroundTask_Api_Send_StaticTrace_StoreAdd()
		{
			BackgroundTask.Setup(() => new BroadcastingClient(_store.Object));

			// execute a static method
			// serializeable
			BackgroundTask.Send(() => Trace.WriteLine("test"));

			_store.Verify(exp => exp.Add(It.IsAny<ITask>()), Times.Once);
		}
		
		[Test]
		public void BackgroundTask_Api_Send_Method_StoreAdd()
		{
			BackgroundTask.Setup(() => new BroadcastingClient(_store.Object));

			// execute a local method
			// serializeable
			BackgroundTask.Send(() => TestMethod(1));

			_store.Verify(exp => exp.Add(It.IsAny<ITask>()), Times.Once);
		}

		[Test]
		public void BackgroundTask_Api_Send_GenericMethod_StoreAdd()
		{
			BackgroundTask.Setup(() => new BroadcastingClient(_store.Object));

			// execute a generic method
			// serializeable
			BackgroundTask.Send(() => GenericMethod(1));

			_store.Verify(exp => exp.Add(It.IsAny<ITask>()), Times.Once);
		}

		[Test]
		public void BackgroundTask_Api_Recurring_Name()
		{
			BackgroundTask.Setup(() => new BroadcastingClient(_store.Object));
			// execute a local method
			// serializeable
			BackgroundTask.Recurring("BackgroundTask_Api_Recurring", () => TestMethod(1), TimeSpan.FromSeconds(0.5));

			Task.Delay(1000).Wait();

			_store.Verify(exp => exp.Add(It.Is<ITask>(t => t.Name == "BackgroundTask_Api_Recurring")), Times.Once);
		}



		public void TestMethod(int i) { }

		public void GenericMethod<T>(T value) { }
	}
}
