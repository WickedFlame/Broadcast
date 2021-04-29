using System;
using System.Collections.Generic;
using System.Text;
using Broadcast.Composition;
using Broadcast.EventSourcing;
using Broadcast.Server;
using Broadcast.Storage;
using Moq;
using NUnit.Framework;

namespace Broadcast.Test.Server
{
	public class RecurringTaskDispatcherTests
	{
		private Mock<IBroadcaster> _broadcaster;
		private Mock<ITaskStore> _store;
		private Mock<IScheduler> _scheduler;

		[SetUp]
		public void Setup()
		{
			_scheduler = new Mock<IScheduler>();
			_scheduler.Setup(exp => exp.Enqueue(It.IsAny<Action>(), It.IsAny<TimeSpan>())).Callback<Action, TimeSpan>((a, t) => a.Invoke());
			_broadcaster = new Mock<IBroadcaster>();
			_broadcaster.Setup(exp => exp.Scheduler).Returns(_scheduler.Object);

			_store = new Mock<ITaskStore>();
		}

		[Test]
		public void RecurringTaskDispatcher_ctor()
		{
			Assert.DoesNotThrow(() => new RecurringTaskDispatcher(_broadcaster.Object, _store.Object));
		}

		[Test]
		public void RecurringTaskDispatcher_ctor_Null_Broadcaster()
		{
			Assert.Throws<ArgumentNullException>(() => new RecurringTaskDispatcher(null, _store.Object));
		}

		[Test]
		public void RecurringTaskDispatcher_ctor_Null_Store()
		{
			Assert.Throws<ArgumentNullException>(() => new RecurringTaskDispatcher(_broadcaster.Object, null));
		}

		[Test]
		public void RecurringTaskDispatcher_Execute_Schedule()
		{
			var dispatcher = new RecurringTaskDispatcher(_broadcaster.Object, _store.Object);
			var task = TaskFactory.CreateTask(() => Console.WriteLine("RecurringTaskDispatcher"));
			task.IsRecurring = true;
			task.Time = TimeSpan.Zero;

			dispatcher.Execute(task);

			_scheduler.Verify(exp => exp.Enqueue(It.IsAny<Action>(), It.IsAny<TimeSpan>()), Times.Once);
		}

		[Test]
		public void RecurringTaskDispatcher_Execute_Broadcaster()
		{
			var dispatcher = new RecurringTaskDispatcher(_broadcaster.Object, _store.Object);
			var task = TaskFactory.CreateTask(() => Console.WriteLine("RecurringTaskDispatcher"));
			task.IsRecurring = true;
			task.Time = TimeSpan.Zero;

			dispatcher.Execute(task);

			_broadcaster.Verify(exp => exp.Process(It.Is<ITask>(t => t == task)), Times.Once);
		}

		[Test]
		public void RecurringTaskDispatcher_Execute_Store()
		{
			var dispatcher = new RecurringTaskDispatcher(_broadcaster.Object, _store.Object);
			var task = TaskFactory.CreateTask(() => Console.WriteLine("RecurringTaskDispatcher"));
			task.IsRecurring = true;
			task.Time = TimeSpan.Zero;

			dispatcher.Execute(task);

			_store.Verify(exp => exp.Add(It.IsAny<ITask>()), Times.Once);
		}

		[Test]
		public void RecurringTaskDispatcher_Execute_IsRecurring()
		{
			var dispatcher = new RecurringTaskDispatcher(_broadcaster.Object, _store.Object);
			var task = TaskFactory.CreateTask(() => Console.WriteLine("RecurringTaskDispatcher"));
			task.Time = TimeSpan.Zero;

			dispatcher.Execute(task);

			_broadcaster.Verify(exp => exp.Process(It.IsAny<ITask>()), Times.Never);
		}

		[Test]
		public void RecurringTaskDispatcher_Execute_Time()
		{
			var dispatcher = new RecurringTaskDispatcher(_broadcaster.Object, _store.Object);
			var task = TaskFactory.CreateTask(() => Console.WriteLine("RecurringTaskDispatcher"));
			task.IsRecurring = true;

			dispatcher.Execute(task);

			_broadcaster.Verify(exp => exp.Process(It.IsAny<ITask>()), Times.Never);
		}

		[Test]
		public void RecurringTaskDispatcher_Execute_StoreRecurringTask()
		{
			var dispatcher = new RecurringTaskDispatcher(_broadcaster.Object, _store.Object);
			var task = TaskFactory.CreateTask(() => Console.WriteLine("RecurringTaskDispatcher"));
			task.IsRecurring = true;
			task.Time = TimeSpan.Zero;

			dispatcher.Execute(task);

			_store.Verify(exp => exp.Storage(It.IsAny<Action<IStorage>>()), Times.Once);
		}

		[Test]
		public void RecurringTaskDispatcher_Execute_StoreRecurringTask_Storage()
		{
			var storage = new Mock<IStorage>();
			var dispatcher = new RecurringTaskDispatcher(_broadcaster.Object, new TaskStore(storage.Object));
			var task = TaskFactory.CreateTask(() => Console.WriteLine("RecurringTaskDispatcher"));
			task.Name = "testTask";
			task.IsRecurring = true;
			task.Time = TimeSpan.Zero;

			dispatcher.Execute(task);

			storage.Verify(exp => exp.Set(It.Is<StorageKey>(k => k.Key == $"tasks:recurring:{task.Name}"), It.Is<RecurringTask>(t => t.Name == task.Name && t.Id == task.Id)), Times.Once);
		}
	}
}
