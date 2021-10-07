﻿using System;
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
		}

		[Test]
		public void Broadcaster_Api_Send_StaticTrace_Processor()
		{
			var store = new TaskStore();
			var broadcaster = new Broadcaster(store, _processor.Object, _scheduler.Object);

			// execute a static method
			// serializeable
			broadcaster.Send(() => Trace.WriteLine("test"));

			broadcaster.WaitAll();

			_processor.Verify(exp => exp.Process(It.IsAny<ITask>()), Times.Once);
		}

		[Test]
		public void Broadcaster_Api_Send_StaticTrace_StoreAdd()
		{
			var broadcaster = new Broadcaster(_store.Object, _processor.Object, _scheduler.Object);

			// execute a static method
			// serializeable
			broadcaster.Send(() => Trace.WriteLine("test"));

			_store.Verify(exp => exp.Add(It.IsAny<ITask>()), Times.Once);
		}

		[Test]
		public void Broadcaster_Api_Send_Method_Process()
		{
			var store = new TaskStore();
			var broadcaster = new Broadcaster(store, _processor.Object, _scheduler.Object);

			// execute a local method
			// serializeable
			broadcaster.Send(() => TestMethod(1));

			broadcaster.WaitAll();

			_processor.Verify(exp => exp.Process(It.IsAny<ITask>()), Times.Once);
		}

		[Test]
		public void Broadcaster_Api_Send_Method_StoreAdd()
		{
			var broadcaster = new Broadcaster(_store.Object, _processor.Object, _scheduler.Object);

			// execute a local method
			// serializeable
			broadcaster.Send(() => TestMethod(1));

			_store.Verify(exp => exp.Add(It.IsAny<ITask>()), Times.Once);
		}

		[Test]
		public void Broadcaster_Api_Send_ReturnId()
		{
			var broadcaster = new Broadcaster(_store.Object, _processor.Object, _scheduler.Object);

			// execute a local method
			// serializeable
			var id = broadcaster.Send(() => TestMethod(1));

			_store.Verify(exp => exp.Add(It.Is<ITask>(t => t.Id == id)), Times.Once);
		}

		[Test]
		public void Broadcaster_Api_Send_GenericMethod()
		{
			var store = new TaskStore();
			var broadcaster = new Broadcaster(store, _processor.Object, _scheduler.Object);

			// execute a generic method
			// serializeable
			broadcaster.Send(() => GenericMethod(1));

			broadcaster.WaitAll();

			_processor.Verify(exp => exp.Process(It.IsAny<ITask>()), Times.Once);
		}

		[Test]
		public void Broadcaster_Api_Send_GenericMethod_AddStore()
		{
			var broadcaster = new Broadcaster(_store.Object, _processor.Object, _scheduler.Object);

			// execute a generic method
			// serializeable
			broadcaster.Send(() => GenericMethod(1));

			_store.Verify(exp => exp.Add(It.IsAny<ITask>()), Times.Once);
		}



		[Test]
		public void Broadcaster_Api_Schedule_StaticTrace()
		{
			var broadcaster = new Broadcaster(new TaskStore(), _processor.Object, _scheduler.Object);

			// execute a static method
			// serializeable
			broadcaster.Schedule(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(0.5));
			broadcaster.WaitAll();

			_scheduler.Verify(exp => exp.Enqueue(It.IsAny<string>(), It.IsAny<Action<string>>(), It.IsAny<TimeSpan>()), Times.Once);
		}

		[Test]
		public void Broadcaster_Api_Schedule_Method()
		{
			var broadcaster = new Broadcaster(new TaskStore(), _processor.Object, _scheduler.Object);

			// execute a local method
			// serializeable
			broadcaster.Schedule(() => TestMethod(1), TimeSpan.FromSeconds(0.5));
			broadcaster.WaitAll();

			_scheduler.Verify(exp => exp.Enqueue(It.IsAny<string>(), It.IsAny<Action<string>>(), It.IsAny<TimeSpan>()), Times.Once);
		}

		[Test]
		public void Broadcaster_Api_Schedule_GenericMethod()
		{
			var broadcaster = new Broadcaster(new TaskStore(), _processor.Object, _scheduler.Object);

			// execute a generic method
			// serializeable
			broadcaster.Schedule(() => GenericMethod(1), TimeSpan.FromSeconds(0.5));
			broadcaster.WaitAll();

			_scheduler.Verify(exp => exp.Enqueue(It.IsAny<string>(), It.IsAny<Action<string>>(), It.IsAny<TimeSpan>()), Times.Once);
		}

		[Test]
		public void Broadcaster_Api_Schedule_ReturnId()
		{
			var broadcaster = new Broadcaster(_store.Object, _processor.Object, _scheduler.Object);

			// execute a local method
			// serializeable
			var id = broadcaster.Schedule(() => TestMethod(1), TimeSpan.FromSeconds(30));

			_store.Verify(exp => exp.Add(It.Is<ITask>(t => t.Id == id)), Times.Once);
		}





		[Test]
		public void Broadcaster_Api_Recurring_StaticTrace()
		{
			var broadcaster = new Broadcaster(new TaskStore(), _processor.Object, _scheduler.Object);

			// execute a static method
			// serializeable
			broadcaster.Recurring(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(0.5));
			Task.Delay(1000).Wait();

			_scheduler.Verify(exp => exp.Enqueue(It.IsAny<string>(), It.IsAny<Action<string>>(), It.IsAny<TimeSpan>()), Times.Once);
		}

		[Test]
		public void Broadcaster_Api_Recurring_Method()
		{
			var broadcaster = new Broadcaster(new TaskStore(), _processor.Object, _scheduler.Object);

			// execute a local method
			// serializeable
			broadcaster.Recurring(() => TestMethod(1), TimeSpan.FromSeconds(0.5));
			Task.Delay(1000).Wait();

			_scheduler.Verify(exp => exp.Enqueue(It.IsAny<string>(), It.IsAny<Action<string>>(), It.IsAny<TimeSpan>()), Times.Once);
		}

		[Test]
		public void Broadcaster_Api_Recurring_GenericMethod()
		{
			var broadcaster = new Broadcaster(new TaskStore(), _processor.Object, _scheduler.Object);

			// execute a generic method
			// serializeable
			broadcaster.Recurring(() => GenericMethod(1), TimeSpan.FromSeconds(0.5));
			Task.Delay(1000).Wait();

			_scheduler.Verify(exp => exp.Enqueue(It.IsAny<string>(), It.IsAny<Action<string>>(), It.IsAny<TimeSpan>()), Times.Once);
		}

		[Test]
		public void Broadcaster_Api_Recurring_Name()
		{
			var broadcaster = new Broadcaster(new TaskStore(), _processor.Object, _scheduler.Object);

			// execute a static method
			// serializeable
			broadcaster.Recurring("Broadcaster_Api_Recurring", () => Trace.WriteLine("test"), TimeSpan.FromSeconds(0.5));

			Task.Delay(2000).Wait();

			Assert.IsTrue(broadcaster.GetProcessedTasks().All(t => t.Name == "Broadcaster_Api_Recurring"));
		}

		[Test]
		public void Broadcaster_Api_DeleteTask()
		{
			var broadcaster = new Broadcaster(new TaskStore(), _processor.Object, _scheduler.Object);

			// execute a generic method
			// serializeable
			var id = broadcaster.Send(() => GenericMethod(1));
			broadcaster.DeleteTask(id);

			Assert.That(broadcaster.Store.Count(t => t.State == TaskState.Deleted), Is.EqualTo(1));
			Assert.That(broadcaster.Store.All(t => t.State == TaskState.Deleted));
		}

		[Test]
		public void Broadcaster_Api_DeleteRecurringTask()
		{
			var broadcaster = new Broadcaster(new TaskStore(), _processor.Object, _scheduler.Object);

			// execute a generic method
			// serializeable
			broadcaster.Recurring("delete_task_name", () => GenericMethod(1), TimeSpan.FromSeconds(30));
			broadcaster.DeleteRecurringTask("delete_task_name");

			Assert.IsNull(broadcaster.Store.Storage(s => s.Get<DataObject>(new StorageKey($"tasks:recurring:delete_task_name"))));
		}

		[Test]
		public void Broadcaster_Api_DeleteRecurringTask_DeleteReferencedTask()
		{
			var broadcaster = new Broadcaster(new TaskStore(), _processor.Object, _scheduler.Object);

			// execute a generic method
			// serializeable
			broadcaster.Recurring("delete_task_name", () => GenericMethod(1), TimeSpan.FromSeconds(0.5));
			Task.Delay(1000).Wait();
			broadcaster.DeleteRecurringTask("delete_task_name");

			Assert.That(broadcaster.Store.Count(t => t.State == TaskState.Deleted), Is.EqualTo(1));
			Assert.That(broadcaster.Store.All(t => t.State == TaskState.Deleted));
		}





		[Test]
		public void Broadcaster_Api_Recurring_Update()
		{
			// execute a static method
			// serializeable
			var broadcaster = new Broadcaster(new TaskStore(), _processor.Object, _scheduler.Object);
			broadcaster.Recurring("Updateable", () => Trace.WriteLine("test"), TimeSpan.FromSeconds(30));
			Task.Delay(1000).Wait();

			var originalRecurring = broadcaster.Store.Storage(s => s.Get<DataObject>(new StorageKey($"tasks:recurring:Updateable")));
			var originalTask = broadcaster.Store.FirstOrDefault(t => t.Id == originalRecurring["ReferenceId"].ToString());

			broadcaster.Recurring("Updateable", () => Trace.WriteLine("succeeded"), TimeSpan.FromSeconds(30));
			Task.Delay(1000).Wait();

			var updatedRecurring = broadcaster.Store.Storage(s => s.Get<DataObject>(new StorageKey($"tasks:recurring:Updateable")));
			var updatedTask = broadcaster.Store.FirstOrDefault(t => t.Id == updatedRecurring["ReferenceId"].ToString()) as BroadcastTask;

			Assert.That(updatedTask.Args.Single().ToString(), Is.EqualTo("succeeded"));
		}


		[Test]
		public void Broadcaster_Api_Recurring_Update_MethodChanged()
		{
			// execute a static method
			// serializeable
			var broadcaster = new Broadcaster(new TaskStore(), _processor.Object, _scheduler.Object);
			broadcaster.Recurring("Updateable", () => Trace.WriteLine("test"), TimeSpan.FromSeconds(30));
			Task.Delay(1000).Wait();

			var originalRecurring = broadcaster.Store.Storage(s => s.Get<DataObject>(new StorageKey($"tasks:recurring:Updateable")));
			var originalTask = broadcaster.Store.FirstOrDefault(t => t.Id == originalRecurring["ReferenceId"].ToString()) as BroadcastTask;

			broadcaster.Recurring("Updateable", () => Trace.WriteLine("succeeded"), TimeSpan.FromSeconds(30));
			Task.Delay(1000).Wait();

			var updatedRecurring = broadcaster.Store.Storage(s => s.Get<DataObject>(new StorageKey($"tasks:recurring:Updateable")));
			var updatedTask = broadcaster.Store.FirstOrDefault(t => t.Id == updatedRecurring["ReferenceId"].ToString()) as BroadcastTask;

			Assert.That(originalTask.Args.Single(), Is.Not.EqualTo(updatedTask.Args.Single()));
		}

		[Test]
		public void Broadcaster_Api_Recurring_Update_SameId_Task()
		{
			// execute a static method
			// serializeable
			var broadcaster = new Broadcaster(new TaskStore(), _processor.Object, _scheduler.Object);
			broadcaster.Recurring("Updateable", () => Trace.WriteLine("test"), TimeSpan.FromSeconds(30));
			Task.Delay(1000).Wait();

			var originalRecurring = broadcaster.Store.Storage(s => s.Get<DataObject>(new StorageKey($"tasks:recurring:Updateable")));
			var originalTask = broadcaster.Store.FirstOrDefault(t => t.Id == originalRecurring["ReferenceId"].ToString());

			broadcaster.Recurring("Updateable", () => Trace.WriteLine("succeeded"), TimeSpan.FromSeconds(30));
			Task.Delay(1000).Wait();

			var updatedRecurring = broadcaster.Store.Storage(s => s.Get<DataObject>(new StorageKey($"tasks:recurring:Updateable")));
			var updatedTask = broadcaster.Store.FirstOrDefault(t => t.Id == updatedRecurring["ReferenceId"].ToString()) as BroadcastTask;

			Assert.That(originalTask.Id, Is.EqualTo(updatedTask.Id));
		}

		[Test]
		public void Broadcaster_Api_Recurring_Update_SameId_Reference()
		{
			// execute a static method
			// serializeable
			var broadcaster = new Broadcaster(new TaskStore(), _processor.Object, _scheduler.Object);
			broadcaster.Recurring("Updateable", () => Trace.WriteLine("test"), TimeSpan.FromSeconds(30));
			Task.Delay(1000).Wait();

			var originalRecurring = broadcaster.Store.Storage(s => s.Get<DataObject>(new StorageKey($"tasks:recurring:Updateable")));

			broadcaster.Recurring("Updateable", () => Trace.WriteLine("succeeded"), TimeSpan.FromSeconds(30));
			Task.Delay(1000).Wait();

			var updatedRecurring = broadcaster.Store.Storage(s => s.Get<DataObject>(new StorageKey($"tasks:recurring:Updateable")));

			Assert.That(originalRecurring["ReferenceId"].ToString(), Is.EqualTo(updatedRecurring["ReferenceId"].ToString()));
		}





		public void TestMethod(int i) { }

		public void GenericMethod<T>(T value) { }
	}
}
