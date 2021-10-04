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
	public class ProcessTaskDispatcherTests
	{
		private Mock<IBroadcaster> _broadcaster;
		private Mock<ITaskStore> _store;

		[SetUp]
		public void Setup()
		{
			_broadcaster = new Mock<IBroadcaster>();
			_store = new Mock<ITaskStore>();
		}

		[Test]
		public void ProcessTaskDispatcher_ctor()
		{
			Assert.DoesNotThrow(() => new ProcessTaskDispatcher(_broadcaster.Object, _store.Object));
		}

		[Test]
		public void ProcessTaskDispatcher_ctor_Null_Broadcaster()
		{
			Assert.Throws<ArgumentNullException>(() => new ProcessTaskDispatcher(null, _store.Object));
		}

		[Test]
		public void ProcessTaskDispatcher_ctor_Null_Store()
		{
			Assert.Throws<ArgumentNullException>(() => new ProcessTaskDispatcher(_broadcaster.Object, null));
		}

		[Test]
		public void ProcessTaskDispatcher_Execute()
		{
			var dispatcher = new ProcessTaskDispatcher(_broadcaster.Object, _store.Object);
			var task = TaskFactory.CreateTask(() => Console.WriteLine("ProcessTaskDispatcher"));

			dispatcher.Execute(task);

			_broadcaster.Verify(exp => exp.Process(It.Is<ITask>(t => t == task)), Times.Once);
		}

		[Test]
		public void ProcessTaskDispatcher_Execute_Deleted()
		{
			var dispatcher = new ProcessTaskDispatcher(_broadcaster.Object, _store.Object);
			var task = TaskFactory.CreateTask(() => Console.WriteLine("ProcessTaskDispatcher"));
			task.State = TaskState.Deleted;

			dispatcher.Execute(task);

			_broadcaster.Verify(exp => exp.Process(It.Is<ITask>(t => t == task)), Times.Never);
		}

		[Test]
		public void ProcessTaskDispatcher_Execute_IsRecurring()
		{
			var dispatcher = new ProcessTaskDispatcher(_broadcaster.Object, _store.Object);
			var task = TaskFactory.CreateTask(() => Console.WriteLine("ProcessTaskDispatcher"));
			task.IsRecurring = true;

			dispatcher.Execute(task);

			_broadcaster.Verify(exp => exp.Process(It.IsAny<ITask>()), Times.Never);
		}

		[Test]
		public void ProcessTaskDispatcher_Execute_Time()
		{
			var dispatcher = new ProcessTaskDispatcher(_broadcaster.Object, _store.Object);
			var task = TaskFactory.CreateTask(() => Console.WriteLine("ProcessTaskDispatcher"));
			task.Time = TimeSpan.Zero;

			dispatcher.Execute(task);

			_broadcaster.Verify(exp => exp.Process(It.IsAny<ITask>()), Times.Never);
		}

		[Test]
		public void ProcessTaskDispatcher_Execute_SetQueue_Task()
		{
			var storage = new Mock<IStorage>();
			var store = new TaskStore(storage.Object);
			_broadcaster.Setup(exp => exp.Name).Returns("testqueue");

			var dispatcher = new ProcessTaskDispatcher(_broadcaster.Object, store);
			var task = TaskFactory.CreateTask(() => Console.WriteLine("ProcessTaskDispatcher"));

			dispatcher.Execute(task);

			storage.Verify(exp => exp.SetValues(It.Is<StorageKey>(k => k.Key == $"tasks:values:{task.Id}"), It.Is<DataObject>(d => d["Queue"].ToString() == "testqueue")), Times.Once);
		}

		[Test]
		public void ProcessTaskDispatcher_Execute_SetTaskToQueue()
		{
			var storage = new Mock<IStorage>();
			var store = new TaskStore(storage.Object);
			_broadcaster.Setup(exp => exp.Name).Returns("testqueue");

			var dispatcher = new ProcessTaskDispatcher(_broadcaster.Object, store);
			var task = TaskFactory.CreateTask(() => Console.WriteLine("ProcessTaskDispatcher"));

			dispatcher.Execute(task);

			storage.Verify(exp => exp.AddToList(It.Is<StorageKey>(k => k.Key == $"queue:testqueue"), task.Id), Times.Once);
		}

		[Test]
		public void ProcessTaskDispatcher_Execute_RemoveTaskFromQueue()
		{
			var storage = new Mock<IStorage>();
			var store = new TaskStore(storage.Object);
			_broadcaster.Setup(exp => exp.Name).Returns("testqueue");

			var dispatcher = new ProcessTaskDispatcher(_broadcaster.Object, store);
			var task = TaskFactory.CreateTask(() => Console.WriteLine("ProcessTaskDispatcher"));

			dispatcher.Execute(task);

			storage.Verify(exp => exp.RemoveFromList(It.Is<StorageKey>(k => k.Key == $"queue:testqueue"), task.Id), Times.Once);
		}
	}
}
