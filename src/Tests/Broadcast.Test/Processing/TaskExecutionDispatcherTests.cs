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
	public class TaskExecutionDispatcherTests
	{
		[Test]
		public void TaskExecutionDispatcher_ctor()
		{
			var task = TaskFactory.CreateTask(() => Console.WriteLine("TaskExecutionDispatcher"));
			Assert.DoesNotThrow(() => new TaskExecutionDispatcher(task));
		}

		[Test]
		public void TaskExecutionDispatcher_ctor_NoTask()
		{
			Assert.Throws<ArgumentNullException>(() => new TaskExecutionDispatcher(null));
		}

		[Test]
		public void TaskExecutionDispatcher_Task_InProcess()
		{
			var task = new Mock<ITask>();
			var dispatcher = new TaskExecutionDispatcher(task.Object);

			var ctx = new Mock<IProcessorContext>();
			ctx.Setup(exp => exp.Store).Returns(() => new Mock<ITaskStore>().Object);

			dispatcher.Execute(ctx.Object);

			task.VerifySet(exp => exp.State = TaskState.InProcess);
		}

		[Test]
		public void TaskExecutionDispatcher_Task_Processed()
		{
			var task = new Mock<ITask>();
			var dispatcher = new TaskExecutionDispatcher(task.Object);

			var ctx = new Mock<IProcessorContext>();
			ctx.Setup(exp => exp.Store).Returns(() => new Mock<ITaskStore>().Object);

			dispatcher.Execute(ctx.Object);

			task.VerifySet(exp => exp.State = TaskState.Processed);
		}

		[Test]
		public void TaskExecutionDispatcher_Task_SetState()
		{
			var task = new Mock<ITask>();
			var dispatcher = new TaskExecutionDispatcher(task.Object);

			var store = new Mock<ITaskStore>();

			var ctx = new Mock<IProcessorContext>();
			ctx.Setup(exp => exp.Store).Returns(() => store.Object);

			dispatcher.Execute(ctx.Object);

			store.Verify(exp => exp.Storage(It.IsAny<Action<IStorage>>()), Times.Exactly(4));
		}

		[Test]
		public void TaskExecutionDispatcher_Task_SetValue_ExecutionTime()
		{
			var task = new Mock<ITask>();
			task.Setup(exp => exp.Id).Returns("TestTask");
			var dispatcher = new TaskExecutionDispatcher(task.Object);

			var storage = new InmemoryStorage();
			var store = new TaskStore(storage);

			var ctx = new Mock<IProcessorContext>();
			ctx.Setup(exp => exp.Store).Returns(() => store);

			dispatcher.Execute(ctx.Object);

			var values = storage.Get<DataObject>(new StorageKey($"tasks:values:TestTask"));
			// there is no task that takes time so sometimes the executiontime is 0...
			Assert.GreaterOrEqual((long)values["ExecutionTime"], 0);
		}

		[Test]
		public void TaskExecutionDispatcher_Task_SetValue_State()
		{
			var task = new Mock<ITask>();
			task.Setup(exp => exp.Id).Returns("TestTask");
			var dispatcher = new TaskExecutionDispatcher(task.Object);

			var storage = new InmemoryStorage();
			var store = new TaskStore(storage);

			var ctx = new Mock<IProcessorContext>();
			ctx.Setup(exp => exp.Store).Returns(() => store);

			dispatcher.Execute(ctx.Object);

			var values = storage.Get<DataObject>(new StorageKey($"tasks:values:TestTask"));
			Assert.AreEqual((TaskState)values["State"], TaskState.Processed);
		}

		[Test]
		public void TaskExecutionDispatcher_Task_SetValue_ProcessedChange()
		{
			var task = new Mock<ITask>();
			task.Setup(exp => exp.Id).Returns("TestTask");
			var dispatcher = new TaskExecutionDispatcher(task.Object);

			var storage = new InmemoryStorage();
			var store = new TaskStore(storage);

			var ctx = new Mock<IProcessorContext>();
			ctx.Setup(exp => exp.Store).Returns(() => store);

			dispatcher.Execute(ctx.Object);

			var values = storage.Get<DataObject>(new StorageKey($"tasks:values:TestTask"));
			Assert.Greater((DateTime)values["ProcessedAt"], DateTime.MinValue);
		}

		[Test]
		public void TaskExecutionDispatcher_Task_SetValue_InProcessChange()
		{
			var task = new Mock<ITask>();
			task.Setup(exp => exp.Id).Returns("TestTask");
			var dispatcher = new TaskExecutionDispatcher(task.Object);

			var storage = new InmemoryStorage();
			var store = new TaskStore(storage);

			var ctx = new Mock<IProcessorContext>();
			ctx.Setup(exp => exp.Store).Returns(() => store);

			dispatcher.Execute(ctx.Object);

			var values = storage.Get<DataObject>(new StorageKey($"tasks:values:TestTask"));
			Assert.Greater((DateTime)values["InProcessAt"], DateTime.MinValue);
		}

		[Test]
		public void TaskExecutionDispatcher_Task_Faulted()
		{
			var task = new Mock<ITask>();
			task.Setup(exp => exp.Invoke(It.IsAny<TaskInvocation>())).Throws<Exception>();

			var dispatcher = new TaskExecutionDispatcher(task.Object);

			var ctx = new Mock<IProcessorContext>();
			ctx.Setup(exp => exp.Store).Returns(() => new Mock<ITaskStore>().Object);

			dispatcher.Execute(ctx.Object);

			task.VerifySet(exp => exp.State = TaskState.Faulted);
		}

		[Test]
		public void TaskExecutionDispatcher_Task_Invoke()
		{
			var task = new Mock<ITask>();
			var dispatcher = new TaskExecutionDispatcher(task.Object);

			var ctx = new Mock<IProcessorContext>();
			ctx.Setup(exp => exp.Store).Returns(() => new Mock<ITaskStore>().Object);

			dispatcher.Execute(ctx.Object);

			task.Verify(exp => exp.Invoke(It.IsAny<TaskInvocation>()), Times.Once);
		}

		[Test]
		public void TaskExecutionDispatcher_Execute_Exception()
		{
			// test if the execution continues when an exception is thrown

			var task = new Mock<ITask>();
			task.Setup(exp => exp.Invoke(It.IsAny<TaskInvocation>())).Throws<Exception>();

			var dispatcher = new TaskExecutionDispatcher(task.Object);

			var ctx = new Mock<IProcessorContext>();
			ctx.Setup(exp => exp.Store).Returns(() => new Mock<ITaskStore>().Object);

			Assert.DoesNotThrow(() => dispatcher.Execute(ctx.Object));
		}

		[Test]
		public void TaskExecutionDispatcher_RemoveFromDequeuedList()
		{
			var task = new Mock<ITask>();
			task.Setup(exp => exp.Id).Returns("TestTask");
			var dispatcher = new TaskExecutionDispatcher(task.Object);

			var storage = new Mock<IStorage>();
			var store = new TaskStore(storage.Object);

			var ctx = new Mock<IProcessorContext>();
			ctx.Setup(exp => exp.Store).Returns(() => store);

			dispatcher.Execute(ctx.Object);

			storage.Verify(exp => exp.RemoveFromList(It.Is<StorageKey>(k => k.Key == "tasks:dequeued"), task.Object.Id), Times.Once);
		}
	}
}
