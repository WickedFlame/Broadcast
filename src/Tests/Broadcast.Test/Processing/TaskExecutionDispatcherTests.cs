using System;
using System.Collections.Generic;
using System.Text;
using Broadcast.Composition;
using Broadcast.EventSourcing;
using Broadcast.Processing;
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

			dispatcher.Execute(ctx.Object);

			task.VerifySet(exp => exp.State = TaskState.InProcess);
		}

		[Test]
		public void TaskExecutionDispatcher_Task_Processed()
		{
			var task = new Mock<ITask>();
			var dispatcher = new TaskExecutionDispatcher(task.Object);

			var ctx = new Mock<IProcessorContext>();

			dispatcher.Execute(ctx.Object);

			task.VerifySet(exp => exp.State = TaskState.Processed);
		}

		[Test]
		public void TaskExecutionDispatcher_Task_Invoke()
		{
			var task = new Mock<ITask>();
			var dispatcher = new TaskExecutionDispatcher(task.Object);

			var ctx = new Mock<IProcessorContext>();

			dispatcher.Execute(ctx.Object);

			task.Verify(exp => exp.Invoke(It.IsAny<TaskInvocation>()), Times.Once);
		}

		[Test]
		public void TaskExecutionDispatcher_ExecuteHandler()
		{
			var task = new Mock<ITask>();
			task.Setup(exp => exp.Invoke(It.IsAny<TaskInvocation>())).Returns(() => new Mock<INotification>().Object);
			var dispatcher = new TaskExecutionDispatcher(task.Object);

			var notifier = new Mock<Action<INotification>>();
			var handlers = new List<Action<INotification>>
			{
				notifier.Object
			};

			var ctx = new Mock<IProcessorContext>();
			ctx.Setup(exp => exp.NotificationHandlers.TryGetHandlers(It.IsAny<Type>(), out handlers)).Returns(() => true);

			dispatcher.Execute(ctx.Object);

			notifier.Verify(exp => exp.Invoke(It.IsAny<INotification>()), Times.Once);
		}

		[Test]
		public void TaskExecutionDispatcher_ExecuteHandler_InvalidOutput()
		{
			var task = new Mock<ITask>();
			task.Setup(exp => exp.Invoke(It.IsAny<TaskInvocation>())).Returns(() => new Mock<TaskInvocation>().Object);
			var dispatcher = new TaskExecutionDispatcher(task.Object);

			var notifier = new Mock<Action<INotification>>();
			var handlers = new List<Action<INotification>>
			{
				notifier.Object
			};

			var ctx = new Mock<IProcessorContext>();
			ctx.Setup(exp => exp.NotificationHandlers.TryGetHandlers(It.IsAny<Type>(), out handlers)).Returns(() => true);

			dispatcher.Execute(ctx.Object);

			notifier.Verify(exp => exp.Invoke(It.IsAny<INotification>()), Times.Never);
		}
	}
}
