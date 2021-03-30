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
	public class TaskServerClientApiTests
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
			var ctx = new Mock<IProcessorContext>();
			ctx.Setup(exp => exp.Open()).Returns(_processor.Object);
			ctx.Setup(exp => exp.Store).Returns(_store.Object);

			Broadcaster.Setup(s =>
			{
				s.Context = ctx.Object;
				s.Scheduler = _scheduler.Object;
			});
		}

		[Test]
		public void TaskServerClient_Api_Send_StaticTrace()
		{
			// execute a static method
			// serializeable
			TaskServerClient.Send(() => Trace.WriteLine("test"));

			_processor.Verify(exp => exp.Process(It.IsAny<ITask>()), Times.Once);
			_store.Verify(exp => exp.Add(It.IsAny<ITask>()), Times.Once);
		}

		[Test]
		public void TaskServerClient_Api_Send_Method()
		{
			// execute a local method
			// serializeable
			TaskServerClient.Send(() => TestMethod(1));

			_processor.Verify(exp => exp.Process(It.IsAny<ITask>()), Times.Once);
			_store.Verify(exp => exp.Add(It.IsAny<ITask>()), Times.Once);
		}

		[Test]
		public void TaskServerClient_Api_Send_GenericMethod()
		{
			// execute a generic method
			// serializeable
			TaskServerClient.Send(() => GenericMethod(1));

			_processor.Verify(exp => exp.Process(It.IsAny<ITask>()), Times.Once);
			_store.Verify(exp => exp.Add(It.IsAny<ITask>()), Times.Once);
		}
		








		[Test]
		public void TaskServerClient_Api_Schedule_StaticTrace()
		{
			// execute a static method
			// serializeable
			TaskServerClient.Schedule(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(1));

			_scheduler.Verify(exp => exp.Enqueue(It.IsAny<Action>(), It.IsAny<TimeSpan>()), Times.Once);
		}

		[Test]
		public void TaskServerClient_Api_Schedule_Method()
		{
			// execute a local method
			// serializeable
			TaskServerClient.Schedule(() => TestMethod(1), TimeSpan.FromSeconds(1));

			_scheduler.Verify(exp => exp.Enqueue(It.IsAny<Action>(), It.IsAny<TimeSpan>()), Times.Once);
		}

		[Test]
		public void TaskServerClient_Api_Schedule_GenericMethod()
		{
			// execute a generic method
			// serializeable
			TaskServerClient.Schedule(() => GenericMethod(1), TimeSpan.FromSeconds(1));

			_scheduler.Verify(exp => exp.Enqueue(It.IsAny<Action>(), It.IsAny<TimeSpan>()), Times.Once);
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
			// execute a local method
			// serializeable
			TaskServerClient.Recurring(() => TestMethod(1), TimeSpan.FromSeconds(0.5));

			_scheduler.Verify(exp => exp.Enqueue(It.IsAny<Action>(), It.IsAny<TimeSpan>()), Times.Once);
		}

		[Test]
		public void TaskServerClient_Api_Recurring_GenericMethod()
		{
			// execute a generic method
			// serializeable
			TaskServerClient.Recurring(() => GenericMethod(1), TimeSpan.FromSeconds(0.5));

			_scheduler.Verify(exp => exp.Enqueue(It.IsAny<Action>(), It.IsAny<TimeSpan>()), Times.Once);
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
