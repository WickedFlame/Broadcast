using System;
using System.Collections.Generic;
using System.Text;
using Broadcast.Composition;
using Broadcast.EventSourcing;
using Broadcast.Server;
using Moq;
using NUnit.Framework;

namespace Broadcast.Test.Server
{
	public class ScheduleTaskDispatcherTests
	{
		private Mock<IBroadcaster> _broadcaster;
		private Mock<IScheduler> _scheduler;

		[SetUp]
		public void Setup()
		{
			_scheduler = new Mock<IScheduler>();
			_scheduler.Setup(exp => exp.Enqueue(It.IsAny<Action>(), It.IsAny<TimeSpan>())).Callback<Action, TimeSpan>((a, t) => a.Invoke());
			_broadcaster = new Mock<IBroadcaster>();
			_broadcaster.Setup(exp => exp.Scheduler).Returns(_scheduler.Object);
		}

		[Test]
		public void ScheduleTaskDispatcher_ctor()
		{
			Assert.DoesNotThrow(() => new ScheduleTaskDispatcher(_broadcaster.Object));
		}

		[Test]
		public void ScheduleTaskDispatcher_ctor_Null_Broadcaster()
		{
			Assert.Throws<ArgumentNullException>(() => new ScheduleTaskDispatcher(null));
		}

		[Test]
		public void ScheduleTaskDispatcher_Execute_Scheduler()
		{
			var dispatcher = new ScheduleTaskDispatcher(_broadcaster.Object);
			var task = TaskFactory.CreateTask(() => Console.WriteLine("ScheduleTaskDispatcher"));
			task.Time = TimeSpan.Zero;

			dispatcher.Execute(task);

			_scheduler.Verify(exp => exp.Enqueue(It.IsAny<Action>(), It.IsAny<TimeSpan>()), Times.Once);
		}

		[Test]
		public void ScheduleTaskDispatcher_Execute_Broadcaster()
		{
			var dispatcher = new ScheduleTaskDispatcher(_broadcaster.Object);
			var task = TaskFactory.CreateTask(() => Console.WriteLine("ScheduleTaskDispatcher"));
			task.Time = TimeSpan.Zero;

			dispatcher.Execute(task);

			_broadcaster.Verify(exp => exp.Process(It.Is<ITask>(t => t == task)), Times.Once);
		}

		[Test]
		public void ScheduleTaskDispatcher_Execute_IsRecurring()
		{
			var dispatcher = new ScheduleTaskDispatcher(_broadcaster.Object);
			var task = TaskFactory.CreateTask(() => Console.WriteLine("ScheduleTaskDispatcher"));
			task.IsRecurring = true;
			task.Time = TimeSpan.Zero;

			dispatcher.Execute(task);

			_broadcaster.Verify(exp => exp.Process(It.IsAny<ITask>()), Times.Never);
		}

		[Test]
		public void ScheduleTaskDispatcher_Execute_Time()
		{
			var dispatcher = new ScheduleTaskDispatcher(_broadcaster.Object);
			var task = TaskFactory.CreateTask(() => Console.WriteLine("ScheduleTaskDispatcher"));

			dispatcher.Execute(task);

			_broadcaster.Verify(exp => exp.Process(It.IsAny<ITask>()), Times.Never);
		}
	}
}
