using System;
using System.Collections.Generic;
using System.Text;
using Broadcast.Composition;
using Broadcast.EventSourcing;
using Broadcast.Processing;
using Broadcast.Storage;
using Moq;
using NUnit.Framework;

namespace Broadcast.Test.Processing
{
	public class ProcessorContextExtensionsTests
	{
		[Test]
		public void ProcessorContextExtensions_SetState_State()
		{
			var task = TaskFactory.CreateTask(() => Console.WriteLine("ProcessorContextExtensions"));
			var context = new ProcessorContext(new Mock<ITaskStore>().Object);

			context.SetState(task, TaskState.Queued);

			Assert.AreEqual(TaskState.Queued, task.State);
		}

		[Test]
		public void ProcessorContextExtensions_SetState_Storage()
		{
			var task = TaskFactory.CreateTask(() => Console.WriteLine("ProcessorContextExtensions"));
			var store = new Mock<ITaskStore>();

			var context = new ProcessorContext(store.Object);

			context.SetState(task, TaskState.Queued);

			store.Verify(exp => exp.Storage(It.IsAny<Action<IStorage>>()), Times.Once);
		}

		[Test]
		public void ProcessorContextExtensions_SetState_Storage_SetValues()
		{
			var task = TaskFactory.CreateTask(() => Console.WriteLine("ProcessorContextExtensions"));
			var storage = new Mock<IStorage>();
			var store = new TaskStore(storage.Object);

			var context = new ProcessorContext(store);

			context.SetState(task, TaskState.Queued);

			storage.Verify(exp => exp.SetValues(It.Is<StorageKey>(k => k.Key == $"tasks:values:{task.Id}"), It.Is<Dictionary<string, object>>(t => (TaskState)t["State"] == TaskState.Queued && (DateTime)t[$"{TaskState.Queued}Change"] > DateTime.MinValue)), Times.Once);
		}

		[Test]
		public void ProcessorContextExtensions_SetValue_Storage()
		{
			var task = TaskFactory.CreateTask(() => Console.WriteLine("ProcessorContextExtensions"));
			var store = new Mock<ITaskStore>();

			var context = new ProcessorContext(store.Object);

			context.SetValue(task, "property", "value");

			store.Verify(exp => exp.Storage(It.IsAny<Action<IStorage>>()), Times.Once);
		}

		[Test]
		public void ProcessorContextExtensions_SetValue_Storage_SetValues()
		{
			var task = TaskFactory.CreateTask(() => Console.WriteLine("ProcessorContextExtensions"));
			var storage = new Mock<IStorage>();
			var store = new TaskStore(storage.Object);

			var context = new ProcessorContext(store);

			context.SetValue(task, "property", "value");

			storage.Verify(exp => exp.SetValues(It.Is<StorageKey>(k => k.Key == $"tasks:values:{task.Id}"), It.Is<Dictionary<string, object>>(t => (string)t["property"] == "value")), Times.Once);
		}
	}
}
