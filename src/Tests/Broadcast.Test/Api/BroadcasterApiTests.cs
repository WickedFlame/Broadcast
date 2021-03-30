using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Broadcast.EventSourcing;
using Broadcast.Processing;
using Moq;
using NUnit.Framework;

namespace Broadcast.Test.Api
{
	[SingleThreaded]
	public class BroadcasterApiTests
	{
		private Mock<ITaskProcessor> _processor;
		private Mock<IScheduler> _scheduler;
		private Mock<ITaskStore> _store;
		private Mock<IProcessorContext> _context;

		[SetUp]
		public void Setup()
		{
			_processor = new Mock<ITaskProcessor>();
			_scheduler = new Mock<IScheduler>();
			_store = new Mock<ITaskStore>();
			_context = new Mock<IProcessorContext>();
			_context.Setup(exp => exp.Open()).Returns(_processor.Object);
			_context.Setup(exp => exp.Store).Returns(_store.Object);
		}

		[Test]
		public void Broadcaster_Api_Send_StaticTrace()
		{
			var broadcaster = new Broadcaster {Context = _context.Object, Scheduler = _scheduler.Object};

			// execute a static method
			// serializeable
			broadcaster.Send(() => Trace.WriteLine("test"));

			_processor.Verify(exp => exp.Process(It.IsAny<ITask>()), Times.Once);
			_store.Verify(exp => exp.Add(It.IsAny<ITask>()), Times.Once);
		}

		[Test]
		public void Broadcaster_Api_Send_Method()
		{
			var broadcaster = new Broadcaster {Context = _context.Object, Scheduler = _scheduler.Object};

			// execute a local method
			// serializeable
			broadcaster.Send(() => TestMethod(1));

			_processor.Verify(exp => exp.Process(It.IsAny<ITask>()), Times.Once);
			_store.Verify(exp => exp.Add(It.IsAny<ITask>()), Times.Once);
		}

		[Test]
		public void Broadcaster_Api_Send_GenericMethod()
		{
			var broadcaster = new Broadcaster {Context = _context.Object, Scheduler = _scheduler.Object};

			// execute a generic method
			// serializeable
			broadcaster.Send(() => GenericMethod(1));

			_processor.Verify(exp => exp.Process(It.IsAny<ITask>()), Times.Once);
			_store.Verify(exp => exp.Add(It.IsAny<ITask>()), Times.Once);
		}

		[Test]
		public void Broadcaster_Api_Send_Notification_Class()
		{
			var broadcaster = new Broadcaster {Context = _context.Object, Scheduler = _scheduler.Object };

			// send a event to a handler
			// serializeable Func<TestClass>
			broadcaster.Send<TestClass>(() => new TestClass(1));

			_processor.Verify(exp => exp.Process(It.IsAny<ITask>()), Times.Once);
			_store.Verify(exp => exp.Add(It.IsAny<ITask>()), Times.Once);
		}

		[Test]
		public void Broadcaster_Api_Send_Notification_Method()
		{
			var broadcaster = new Broadcaster {Context = _context.Object, Scheduler = _scheduler.Object};

			// send a event to a handler
			// serializeable Func<TestClass>
			broadcaster.Send<TestClass>(() => Returnable(1));

			_processor.Verify(exp => exp.Process(It.IsAny<ITask>()), Times.Once);
			_store.Verify(exp => exp.Add(It.IsAny<ITask>()), Times.Once);
		}



		[Test]
		public void Broadcaster_Api_Schedule_StaticTrace()
		{
			var broadcaster = new Broadcaster {Context = _context.Object, Scheduler = _scheduler.Object};

			// execute a static method
			// serializeable
			broadcaster.Schedule(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(0.5));

			_scheduler.Verify(exp => exp.Enqueue(It.IsAny<Action>(), It.IsAny<TimeSpan>()), Times.Once);
		}

		[Test]
		public void Broadcaster_Api_Schedule_Method()
		{
			var broadcaster = new Broadcaster {Context = _context.Object, Scheduler = _scheduler.Object};

			// execute a local method
			// serializeable
			broadcaster.Schedule(() => TestMethod(1), TimeSpan.FromSeconds(0.5));

			_scheduler.Verify(exp => exp.Enqueue(It.IsAny<Action>(), It.IsAny<TimeSpan>()), Times.Once);
		}

		[Test]
		public void Broadcaster_Api_Schedule_GenericMethod()
		{
			var broadcaster = new Broadcaster {Context = _context.Object, Scheduler = _scheduler.Object};

			// execute a generic method
			// serializeable
			broadcaster.Schedule(() => GenericMethod(1), TimeSpan.FromSeconds(0.5));

			_scheduler.Verify(exp => exp.Enqueue(It.IsAny<Action>(), It.IsAny<TimeSpan>()), Times.Once);
		}

		[Test]
		public void Broadcaster_Api_Schedule_Notification_Class()
		{
			var broadcaster = new Broadcaster {Context = _context.Object, Scheduler = _scheduler.Object};

			// send a event to a handler
			// Nonserializeable Func<TestClass>
			broadcaster.Schedule<TestClass>(() => new TestClass(1), TimeSpan.FromSeconds(0.5));

			_scheduler.Verify(exp => exp.Enqueue(It.IsAny<Action>(), It.IsAny<TimeSpan>()), Times.Once);
		}

		[Test]
		public void Broadcaster_Api_Schedule_Notification_Method()
		{
			var broadcaster = new Broadcaster {Context = _context.Object, Scheduler = _scheduler.Object};

			// send a event to a handler
			// Nonserializeable Func<TestClass>
			broadcaster.Schedule<TestClass>(() => Returnable(1), TimeSpan.FromSeconds(0.5));

			_scheduler.Verify(exp => exp.Enqueue(It.IsAny<Action>(), It.IsAny<TimeSpan>()), Times.Once);
		}








		[Test]
		public void Broadcaster_Api_Recurring_StaticTrace()
		{
			var broadcaster = new Broadcaster {Context = _context.Object, Scheduler = _scheduler.Object};

			// execute a static method
			// serializeable
			broadcaster.Recurring(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(0.5));

			_scheduler.Verify(exp => exp.Enqueue(It.IsAny<Action>(), It.IsAny<TimeSpan>()), Times.Once);
		}

		[Test]
		public void Broadcaster_Api_Recurring_Method()
		{
			var broadcaster = new Broadcaster {Context = _context.Object, Scheduler = _scheduler.Object};

			// execute a local method
			// serializeable
			broadcaster.Recurring(() => TestMethod(1), TimeSpan.FromSeconds(0.5));

			_scheduler.Verify(exp => exp.Enqueue(It.IsAny<Action>(), It.IsAny<TimeSpan>()), Times.Once);
		}

		[Test]
		public void Broadcaster_Api_Recurring_GenericMethod()
		{
			var broadcaster = new Broadcaster {Context = _context.Object, Scheduler = _scheduler.Object};

			// execute a generic method
			// serializeable
			broadcaster.Recurring(() => GenericMethod(1), TimeSpan.FromSeconds(0.5));

			_scheduler.Verify(exp => exp.Enqueue(It.IsAny<Action>(), It.IsAny<TimeSpan>()), Times.Once);
		}

		[Test]
		public void Broadcaster_Api_Recurring_Notification_Class()
		{
			var broadcaster = new Broadcaster {Context = _context.Object, Scheduler = _scheduler.Object};

			// send a event to a handler
			// Nonserializeable Func<TestClass>
			broadcaster.Recurring<TestClass>(() => new TestClass(1), TimeSpan.FromSeconds(0.5));

			_scheduler.Verify(exp => exp.Enqueue(It.IsAny<Action>(), It.IsAny<TimeSpan>()), Times.Once);
		}

		[Test]
		public void Broadcaster_Api_Recurring_Notification_Method()
		{
			var broadcaster = new Broadcaster {Context = _context.Object, Scheduler = _scheduler.Object};

			// send a event to a handler
			// Nonserializeable Func<TestClass>
			broadcaster.Recurring<TestClass>(() => Returnable(1), TimeSpan.FromSeconds(0.5));

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
