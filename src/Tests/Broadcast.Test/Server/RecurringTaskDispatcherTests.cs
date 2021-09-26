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
			_scheduler.Setup(exp => exp.Enqueue(It.IsAny<string>(), It.IsAny<Action<string>>(), It.IsAny<TimeSpan>())).Callback<string, Action<string>, TimeSpan>((id, a, t) => a.Invoke(id));
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

			_store.Setup(exp => exp.Storage<BroadcastTask>(It.IsAny<Func<IStorage, BroadcastTask>>())).Returns(() => (BroadcastTask)task);

			dispatcher.Execute(task);

			_scheduler.Verify(exp => exp.Enqueue(It.IsAny<string>(), It.IsAny<Action<string>>(), It.IsAny<TimeSpan>()), Times.Once);
		}

		[Test]
		public void RecurringTaskDispatcher_Execute_Deleted()
		{
			var dispatcher = new RecurringTaskDispatcher(_broadcaster.Object, _store.Object);
			var task = TaskFactory.CreateTask(() => Console.WriteLine("RecurringTaskDispatcher"));
			task.IsRecurring = true;
			task.Time = TimeSpan.Zero;
			task.State = TaskState.Deleted;
			
			dispatcher.Execute(task);

			_scheduler.Verify(exp => exp.Enqueue(It.IsAny<string>(), It.IsAny<Action<string>>(), It.IsAny<TimeSpan>()), Times.Never);
		}

		[Test]
		public void RecurringTaskDispatcher_Execute_Broadcaster()
		{
			var dispatcher = new RecurringTaskDispatcher(_broadcaster.Object, _store.Object);
			var task = TaskFactory.CreateTask(() => Console.WriteLine("RecurringTaskDispatcher"));
			task.IsRecurring = true;
			task.Time = TimeSpan.Zero;

			_store.Setup(exp => exp.Storage<BroadcastTask>(It.IsAny<Func<IStorage, BroadcastTask>>())).Returns(() => (BroadcastTask)task);

			dispatcher.Execute(task);

			_broadcaster.Verify(exp => exp.Process(It.Is<ITask>(t => t == task)), Times.Once);
		}

		[Test]
		public void RecurringTaskDispatcher_Execute_Scheduled_Null()
		{
			var dispatcher = new RecurringTaskDispatcher(_broadcaster.Object, _store.Object);
			var task = TaskFactory.CreateTask(() => Console.WriteLine("RecurringTaskDispatcher"));
			task.IsRecurring = true;
			task.Time = TimeSpan.Zero;

			_store.Setup(exp => exp.Storage<BroadcastTask>(It.IsAny<Func<IStorage, BroadcastTask>>())).Returns(() => (BroadcastTask)null);

			dispatcher.Execute(task);

			_broadcaster.Verify(exp => exp.Process(It.Is<ITask>(t => t == task)), Times.Never);
		}

		[Test]
		public void RecurringTaskDispatcher_Execute_Scheduled_Deleted()
		{
			var dispatcher = new RecurringTaskDispatcher(_broadcaster.Object, _store.Object);
			var task = TaskFactory.CreateTask(() => Console.WriteLine("RecurringTaskDispatcher"));
			task.IsRecurring = true;
			task.Time = TimeSpan.Zero;

			var t2 = task.Clone();
			t2.State = TaskState.Deleted;

			_store.Setup(exp => exp.Storage<BroadcastTask>(It.IsAny<Func<IStorage, BroadcastTask>>())).Returns(() => (BroadcastTask)t2);

			dispatcher.Execute(task);

			_broadcaster.Verify(exp => exp.Process(It.Is<ITask>(t => t == task)), Times.Never);
		}

		[Test]
		public void RecurringTaskDispatcher_Execute_Store()
		{
			var dispatcher = new RecurringTaskDispatcher(_broadcaster.Object, _store.Object);
			var task = TaskFactory.CreateTask(() => Console.WriteLine("RecurringTaskDispatcher"));
			task.IsRecurring = true;
			task.Time = TimeSpan.Zero;

			_store.Setup(exp => exp.Storage<BroadcastTask>(It.IsAny<Func<IStorage, BroadcastTask>>())).Returns(() => (BroadcastTask)task);

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

			_store.Setup(exp => exp.Storage<BroadcastTask>(It.IsAny<Func<IStorage, BroadcastTask>>())).Returns(() => (BroadcastTask)task);

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

			storage.Setup(exp => exp.Get<BroadcastTask>(It.IsAny<StorageKey>())).Returns(() => (BroadcastTask)task);

			dispatcher.Execute(task);

			storage.Verify(exp => exp.Set(It.Is<StorageKey>(k => k.Key == $"tasks:recurring:{task.Name}"), It.Is<RecurringTask>(t => t.Name == task.Name && t.ReferenceId == task.Id)), Times.Once);
		}
	}
}
