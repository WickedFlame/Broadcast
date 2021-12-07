using System;
using System.Collections.Generic;
using System.Text;
using Broadcast.Composition;
using Broadcast.Configuration;
using Broadcast.EventSourcing;
using Broadcast.Processing;
using Broadcast.Storage;
using Moq;
using NUnit.Framework;

namespace Broadcast.Test.Processing
{
	public class TaskProcessorTests
	{
		[Test]
		public void TaskProcessor_ctor()
		{
			var store = new Mock<ITaskStore>();
			Assert.DoesNotThrow(() => new TaskProcessor(store.Object, new ProcessorOptions()));
		}

		[Test]
		public void TaskProcessor_ctor_Null_Options()
		{
			var store = new Mock<ITaskStore>();
			Assert.Throws<ArgumentNullException>(() => new TaskProcessor(store.Object, null));
		}

		[Test]
		public void TaskProcessor_ctor_Null_Store()
		{
			Assert.Throws<ArgumentNullException>(() => new TaskProcessor(null, new ProcessorOptions()));
		}

		[Test]
		public void TaskProcessor_Queue()
		{
			var store = new Mock<ITaskStore>();
			var processor = new TaskProcessor(store.Object, new ProcessorOptions());

			Assert.IsNotNull(processor.Queue);
		}

		[Test]
		public void TaskProcessor_Process_SetState()
		{
			var store = new Mock<ITaskStore>();
			var processor = new TaskProcessor(store.Object, new ProcessorOptions());

			var task = TaskFactory.CreateTask(() => Console.WriteLine("TaskProcessor"));
			processor.Process(task);

			Assert.AreEqual(TaskState.Queued, task.State);
		}

		[Test]
		public void TaskProcessor_Process_SetState_Storage()
		{
			var store = new Mock<ITaskStore>();
			var processor = new TaskProcessor(store.Object, new ProcessorOptions());

			processor.Process(TaskFactory.CreateTask(() => Console.WriteLine("TaskProcessor")));

			store.Verify(exp => exp.Storage(It.IsAny<Action<IStorage>>()), Times.AtLeastOnce);
		}

		[Test]
		public void TaskProcessor_Process_ValidateState_Queued()
		{
			var store = new Mock<ITaskStore>();
			var processor = new TaskProcessor(store.Object, new ProcessorOptions());

			var task = TaskFactory.CreateTask(() => Console.WriteLine("TaskProcessor"));
			processor.Process(task);

			Assert.IsTrue(task.StateChanges.ContainsKey(TaskState.Queued));
		}
	}
}
