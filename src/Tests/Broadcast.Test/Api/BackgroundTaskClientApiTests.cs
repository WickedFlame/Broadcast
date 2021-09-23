using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Broadcast.EventSourcing;
using Broadcast.Processing;
using Moq;
using NUnit.Framework;

namespace Broadcast.Test.Api
{
	[SingleThreaded]
	public class BackgroundTaskClientApiTests
	{
		private Mock<ITaskProcessor> _processor;
		private Mock<IScheduler> _scheduler;
		private Mock<ITaskStore> _store;

		[SetUp]
		public void Setup()
		{
			_processor = new Mock<ITaskProcessor>();
			_scheduler = new Mock<IScheduler>();
			_store = new Mock<ITaskStore>();

			BroadcastServer.Setup(s =>
				s.AddScheduler(_scheduler.Object)
					.AddProcessor(_processor.Object)
			);
		}

		[TearDown]
		public void Teardown()
		{
			BackgroundTaskClient.Setup(null);
		}

		[Test]
		public void BackgroundTaskClient_Api_Send_StaticTrace_Process()
		{
			// execute a static method
			// serializeable
			BackgroundTaskClient.Send(() => Trace.WriteLine("test"));

			BackgroundTaskClient.Client.Store.WaitAll();

			_processor.Verify(exp => exp.Process(It.IsAny<ITask>()), Times.Once);
		}

		[Test]
		public void BackgroundTaskClient_Api_Send_StaticTrace_StoreAdd()
		{
			BackgroundTaskClient.Setup(() => new BroadcastingClient(_store.Object));

			// execute a static method
			// serializeable
			BackgroundTaskClient.Send(() => Trace.WriteLine("test"));

			_store.Verify(exp => exp.Add(It.IsAny<ITask>()), Times.Once);
		}

		[Test]
		public void BackgroundTaskClient_Api_Send_Method_Process()
		{
			// execute a local method
			// serializeable
			BackgroundTaskClient.Send(() => TestMethod(1));

			BackgroundTaskClient.Client.Store.WaitAll();

			_processor.Verify(exp => exp.Process(It.IsAny<ITask>()), Times.Once);
		}

		[Test]
		public void BackgroundTaskClient_Api_Send_Method_StoreAdd()
		{
			BackgroundTaskClient.Setup(() => new BroadcastingClient(_store.Object));

			// execute a local method
			// serializeable
			BackgroundTaskClient.Send(() => TestMethod(1));

			_store.Verify(exp => exp.Add(It.IsAny<ITask>()), Times.Once);
		}

		[Test]
		public void BackgroundTaskClient_Api_Send_GenericMethod_Process()
		{
			// execute a generic method
			// serializeable
			BackgroundTaskClient.Send(() => GenericMethod(1));

			BackgroundTaskClient.Client.Store.WaitAll();

			_processor.Verify(exp => exp.Process(It.IsAny<ITask>()), Times.Once);
		}

		[Test]
		public void BackgroundTaskClient_Api_Send_GenericMethod_StoreAdd()
		{
			BackgroundTaskClient.Setup(() => new BroadcastingClient(_store.Object));

			// execute a generic method
			// serializeable
			BackgroundTaskClient.Send(() => GenericMethod(1));

			_store.Verify(exp => exp.Add(It.IsAny<ITask>()), Times.Once);
		}


		[Test]
		public void BackgroundTaskClient_Api_Schedule_StaticTrace()
		{
			// execute a static method
			// serializeable
			BackgroundTaskClient.Schedule(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(1));
			BackgroundTaskClient.Client.Store.WaitAll();

			_scheduler.Verify(exp => exp.Enqueue(It.IsAny<Action>(), It.IsAny<TimeSpan>()), Times.Once);
		}

		[Test]
		public void BackgroundTaskClient_Api_Schedule_Method()
		{
			// execute a local method
			// serializeable
			BackgroundTaskClient.Schedule(() => TestMethod(1), TimeSpan.FromSeconds(1));
			BackgroundTaskClient.Client.Store.WaitAll();

			_scheduler.Verify(exp => exp.Enqueue(It.IsAny<Action>(), It.IsAny<TimeSpan>()), Times.Once);
		}

		[Test]
		public void BackgroundTaskClient_Api_Schedule_GenericMethod()
		{
			// execute a generic method
			// serializeable
			BackgroundTaskClient.Schedule(() => GenericMethod(1), TimeSpan.FromSeconds(1));
			BackgroundTaskClient.Client.Store.WaitAll();

			_scheduler.Verify(exp => exp.Enqueue(It.IsAny<Action>(), It.IsAny<TimeSpan>()), Times.Once);
		}
		
		[Test]
		public void BackgroundTaskClient_Api_Recurring_StaticTrace()
		{
			// execute a static method
			// serializeable
			BackgroundTaskClient.Recurring(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(0.5));

			Task.Delay(1000).Wait();

			_scheduler.Verify(exp => exp.Enqueue(It.IsAny<Action>(), It.IsAny<TimeSpan>()), Times.Once);
		}

		[Test]
		public void BackgroundTaskClient_Api_Recurring_Method()
		{
			// execute a local method
			// serializeable
			BackgroundTaskClient.Recurring(() => TestMethod(1), TimeSpan.FromSeconds(0.5));

			Task.Delay(1000).Wait();

			_scheduler.Verify(exp => exp.Enqueue(It.IsAny<Action>(), It.IsAny<TimeSpan>()), Times.Once);
		}

		[Test]
		public void BackgroundTaskClient_Api_Recurring_GenericMethod()
		{
			// execute a generic method
			// serializeable
			BackgroundTaskClient.Recurring(() => GenericMethod(1), TimeSpan.FromSeconds(0.5));

			Task.Delay(1000).Wait();

			_scheduler.Verify(exp => exp.Enqueue(It.IsAny<Action>(), It.IsAny<TimeSpan>()), Times.Once);
		}

		[Test]
		public void BackgroundTaskClient_Api_Recurring_Name()
		{
			BackgroundTaskClient.Setup(() => new BroadcastingClient(_store.Object));
			// execute a local method
			// serializeable
			BackgroundTaskClient.Recurring("BackgroundTaskClient_Api_Recurring", () => TestMethod(1), TimeSpan.FromSeconds(0.5));

			Task.Delay(1000).Wait();

			_store.Verify(exp => exp.Add(It.Is<ITask>(t => t.Name == "BackgroundTaskClient_Api_Recurring")), Times.Once);
		}



		public void TestMethod(int i) { }

		public void GenericMethod<T>(T value) { }
	}
}
