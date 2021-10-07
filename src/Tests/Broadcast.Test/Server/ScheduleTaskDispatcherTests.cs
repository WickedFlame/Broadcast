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
	public class ScheduleTaskDispatcherTests
	{
		private Mock<IBroadcaster> _broadcaster;
		private Mock<IScheduler> _scheduler;
		private Mock<ITaskStore> _store;

		[SetUp]
		public void Setup()
		{
			_store = new Mock<ITaskStore>();
			_scheduler = new Mock<IScheduler>();
			_scheduler.Setup(exp => exp.Enqueue(It.IsAny<string>(), It.IsAny<Action<string>>(), It.IsAny<TimeSpan>())).Callback<string, Action<string>, TimeSpan>((id, a, t) => a.Invoke(id));
			_broadcaster = new Mock<IBroadcaster>();
			_broadcaster.Setup(exp => exp.Scheduler).Returns(_scheduler.Object);
		}

		[Test]
		public void ScheduleTaskDispatcher_ctor()
		{
			Assert.DoesNotThrow(() => new ScheduleTaskDispatcher(_broadcaster.Object, _store.Object));
		}

		[Test]
		public void ScheduleTaskDispatcher_ctor_Null_Broadcaster()
		{
			Assert.Throws<ArgumentNullException>(() => new ScheduleTaskDispatcher(null, _store.Object));
		}

		[Test]
		public void ScheduleTaskDispatcher_Execute_Scheduler()
		{
			var dispatcher = new ScheduleTaskDispatcher(_broadcaster.Object, _store.Object);
			var task = TaskFactory.CreateTask(() => Console.WriteLine("ScheduleTaskDispatcher"));
			task.Time = TimeSpan.Zero;

			dispatcher.Execute(task);

			_scheduler.Verify(exp => exp.Enqueue(It.IsAny<string>(), It.IsAny<Action<string>>(), It.IsAny<TimeSpan>()), Times.Once);
		}

		[Test]
		public void ScheduleTaskDispatcher_Execute_Scheduler_Deleted()
		{
			var dispatcher = new ScheduleTaskDispatcher(_broadcaster.Object, _store.Object);
			var task = TaskFactory.CreateTask(() => Console.WriteLine("ScheduleTaskDispatcher"));
			task.Time = TimeSpan.Zero;
			task.State = TaskState.Deleted;

			dispatcher.Execute(task);
			dispatcher.Execute(task);

			_scheduler.Verify(exp => exp.Enqueue(It.IsAny<string>(), It.IsAny<Action<string>>(), It.IsAny<TimeSpan>()), Times.Never);
		}

		[Test]
		public void ScheduleTaskDispatcher_Execute_Broadcaster()
		{
			var dispatcher = new ScheduleTaskDispatcher(_broadcaster.Object, _store.Object);
			var task = TaskFactory.CreateTask(() => Console.WriteLine("ScheduleTaskDispatcher"));
			task.Time = TimeSpan.Zero;

			_store.Setup(exp => exp.Storage<BroadcastTask>(It.IsAny<Func<IStorage, BroadcastTask>>())).Returns(() => (BroadcastTask)task);

			dispatcher.Execute(task);

			_broadcaster.Verify(exp => exp.Process(It.Is<ITask>(t => t.Id == task.Id)), Times.Once);
		}

		[Test]
		public void ScheduleTaskDispatcher_Execute_Task_Null()
		{
			var dispatcher = new ScheduleTaskDispatcher(_broadcaster.Object, _store.Object);
			var task = TaskFactory.CreateTask(() => Console.WriteLine("ScheduleTaskDispatcher"));
			task.Time = TimeSpan.Zero;

			_store.Setup(exp => exp.Storage<BroadcastTask>(It.IsAny<Func<IStorage, BroadcastTask>>())).Returns(() => (BroadcastTask)null);

			dispatcher.Execute(task);

			_broadcaster.Verify(exp => exp.Process(It.Is<ITask>(t => t.Id == task.Id)), Times.Never);
		}

		[Test]
		public void ScheduleTaskDispatcher_Execute_Task_Deleted()
		{
			var dispatcher = new ScheduleTaskDispatcher(_broadcaster.Object, _store.Object);
			var task = TaskFactory.CreateTask(() => Console.WriteLine("ScheduleTaskDispatcher"));
			task.Time = TimeSpan.Zero;

			var t2 = task.Clone();
			t2.State = TaskState.Deleted;

			_store.Setup(exp => exp.Storage<BroadcastTask>(It.IsAny<Func<IStorage, BroadcastTask>>())).Returns(() => (BroadcastTask)t2);

			dispatcher.Execute(task);

			_broadcaster.Verify(exp => exp.Process(It.Is<ITask>(t => t.Id == task.Id)), Times.Never);
		}

		[Test]
		public void ScheduleTaskDispatcher_Execute_IsRecurring()
		{
			var dispatcher = new ScheduleTaskDispatcher(_broadcaster.Object, _store.Object);
			var task = TaskFactory.CreateTask(() => Console.WriteLine("ScheduleTaskDispatcher"));
			task.IsRecurring = true;
			task.Time = TimeSpan.Zero;

			dispatcher.Execute(task);

			_broadcaster.Verify(exp => exp.Process(It.IsAny<ITask>()), Times.Never);
		}

		[Test]
		public void ScheduleTaskDispatcher_Execute_Time()
		{
			var dispatcher = new ScheduleTaskDispatcher(_broadcaster.Object, _store.Object);
			var task = TaskFactory.CreateTask(() => Console.WriteLine("ScheduleTaskDispatcher"));

			dispatcher.Execute(task);

			_broadcaster.Verify(exp => exp.Process(It.IsAny<ITask>()), Times.Never);
		}

		[Test]
		public void ScheduleTaskDispatcher_Execute_SetQueue_Task()
		{
			_broadcaster.Setup(exp => exp.Name).Returns("testqueue");
			var storage = new Mock<IStorage>();
			var dispatcher = new ScheduleTaskDispatcher(_broadcaster.Object, new TaskStore(storage.Object));
			var task = TaskFactory.CreateTask(() => Console.WriteLine("ScheduleTaskDispatcher"));
			task.Time = TimeSpan.Zero;

			dispatcher.Execute(task);

			storage.Verify(exp => exp.SetValues(It.Is<StorageKey>(k => k.Key == $"tasks:values:{task.Id}"), It.Is<DataObject>(d => d["Queue"].ToString() == "testqueue")), Times.Once);
		}

		[Test]
		public void ScheduleTaskDispatcher_Execute_SetTaskToQueue()
		{
			_broadcaster.Setup(exp => exp.Name).Returns("testqueue");
			var storage = new Mock<IStorage>();
			var dispatcher = new ScheduleTaskDispatcher(_broadcaster.Object, new TaskStore(storage.Object));
			var task = TaskFactory.CreateTask(() => Console.WriteLine("ScheduleTaskDispatcher"));
			task.Time = TimeSpan.Zero;

			dispatcher.Execute(task);

			storage.Verify(exp => exp.AddToList(It.Is<StorageKey>(k => k.Key == $"queue:testqueue"), task.Id), Times.Once);
		}

		//[Test]
		//public void ScheduleTaskDispatcher_Execute_RemoveTaskFromQueue()
		//{
		//	_broadcaster.Setup(exp => exp.Name).Returns("testqueue");
		//	var storage = new Mock<IStorage>();
		//	var dispatcher = new ScheduleTaskDispatcher(_broadcaster.Object, new TaskStore(storage.Object));
		//	var task = TaskFactory.CreateTask(() => Console.WriteLine("ScheduleTaskDispatcher"));
		//	task.Time = TimeSpan.Zero;

		//	dispatcher.Execute(task);

		//	storage.Verify(exp => exp.RemoveFromList(It.Is<StorageKey>(k => k.Key == $"queue:testqueue"), task.Id), Times.Once);
		//}
	}
}
