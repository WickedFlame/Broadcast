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
	public class BackgroundTaskApiTests
	{
		private Mock<ITaskProcessor> _processor;
		private Mock<IScheduler> _scheduler;

		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			_processor = new Mock<ITaskProcessor>();
			_scheduler = new Mock<IScheduler>();
			var store = new TaskStore();

			BroadcastServer.Setup(s =>
				s.AddScheduler(_scheduler.Object)
					.AddProcessor(_processor.Object)
					.AddTaskStore(store)
			);

			BackgroundTask.Setup(() => new BroadcastingClient(store));
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
		public void BackgroundTask_Api_Send_Method_Process()
		{
			// execute a local method
			// serializeable
			BackgroundTask.Send(() => TestMethod(1));

			BackgroundTask.Client.Store.WaitAll();

			_processor.Verify(exp => exp.Process(It.IsAny<ITask>()), Times.Once);
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


		public void TestMethod(int i) { }

		public void GenericMethod<T>(T value) { }
	}
}
