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
	public class BackgroundTaskApiTests
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
			BackgroundTask.Setup(null);
		}

		[Test]
		public void BackgroundTask_Api_Send_StaticTrace_Process()
		{
			// execute a static method
			// serializeable
			BackgroundTask.Send(() => Trace.WriteLine("test"));

			BackgroundTask.Client.Store.WaitAll();

			_processor.Verify(exp => exp.Process(It.IsAny<ITask>()), Times.Once);
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
		public void BackgroundTask_Api_Send_Method_Process()
		{
			// execute a local method
			// serializeable
			BackgroundTask.Send(() => TestMethod(1));

			BackgroundTask.Client.Store.WaitAll();

			_processor.Verify(exp => exp.Process(It.IsAny<ITask>()), Times.Once);
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
		public void BackgroundTask_Api_Send_GenericMethod_Process()
		{
			// execute a generic method
			// serializeable
			BackgroundTask.Send(() => GenericMethod(1));

			BackgroundTask.Client.Store.WaitAll();

			_processor.Verify(exp => exp.Process(It.IsAny<ITask>()), Times.Once);
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
		public void BackgroundTask_Api_Schedule_StaticTrace()
		{
			// execute a static method
			// serializeable
			BackgroundTask.Schedule(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(1));
			BackgroundTask.Client.Store.WaitAll();

			_scheduler.Verify(exp => exp.Enqueue(It.IsAny<string>(), It.IsAny<Action<string>>(), It.IsAny<TimeSpan>()), Times.Once);
		}

		[Test]
		public void BackgroundTask_Api_Schedule_Method()
		{
			// execute a local method
			// serializeable
			BackgroundTask.Schedule(() => TestMethod(1), TimeSpan.FromSeconds(1));
			BackgroundTask.Client.Store.WaitAll();

			_scheduler.Verify(exp => exp.Enqueue(It.IsAny<string>(), It.IsAny<Action<string>>(), It.IsAny<TimeSpan>()), Times.Once);
		}

		[Test]
		public void BackgroundTask_Api_Schedule_GenericMethod()
		{
			// execute a generic method
			// serializeable
			BackgroundTask.Schedule(() => GenericMethod(1), TimeSpan.FromSeconds(1));
			BackgroundTask.Client.Store.WaitAll();

			_scheduler.Verify(exp => exp.Enqueue(It.IsAny<string>(), It.IsAny<Action<string>>(), It.IsAny<TimeSpan>()), Times.Once);
		}
		
		[Test]
		public void BackgroundTask_Api_Recurring_StaticTrace()
		{
			// execute a static method
			// serializeable
			BackgroundTask.Recurring(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(0.5));

			Task.Delay(1000).Wait();

			_scheduler.Verify(exp => exp.Enqueue(It.IsAny<string>(), It.IsAny<Action<string>>(), It.IsAny<TimeSpan>()), Times.Once);
		}

		[Test]
		public void BackgroundTask_Api_Recurring_Method()
		{
			// execute a local method
			// serializeable
			BackgroundTask.Recurring(() => TestMethod(1), TimeSpan.FromSeconds(0.5));

			Task.Delay(1000).Wait();

			_scheduler.Verify(exp => exp.Enqueue(It.IsAny<string>(), It.IsAny<Action<string>>(), It.IsAny<TimeSpan>()), Times.Once);
		}

		[Test]
		public void BackgroundTask_Api_Recurring_GenericMethod()
		{
			// execute a generic method
			// serializeable
			BackgroundTask.Recurring(() => GenericMethod(1), TimeSpan.FromSeconds(0.5));

			Task.Delay(1000).Wait();

			_scheduler.Verify(exp => exp.Enqueue(It.IsAny<string>(), It.IsAny<Action<string>>(), It.IsAny<TimeSpan>()), Times.Once);
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
