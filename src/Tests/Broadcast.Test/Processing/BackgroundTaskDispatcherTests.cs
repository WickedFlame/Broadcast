using System;
using System.Collections.Generic;
using System.Text;
using Broadcast.Composition;
using Broadcast.EventSourcing;
using Broadcast.Processing;
using Broadcast.Server;
using Moq;
using NUnit.Framework;

namespace Broadcast.Test.Processing
{
	public class BackgroundTaskDispatcherTests
	{
		private Mock<IBackgroundServerProcess<IProcessorContext>> _server;
		private Mock<IProcessorContext> _context;

		[SetUp]
		public void Setup()
		{
			_server = new Mock<IBackgroundServerProcess<IProcessorContext>>();
			_context = new Mock<IProcessorContext>();
		}

		[Test]
		public void BackgroundTaskDispatcher_ctor()
		{
			var locker = new DispatcherLock();
			Assert.DoesNotThrow(() => new BackgroundTaskDispatcher(locker, new TaskQueue(), _server.Object));
		}

		[Test]
		public void BackgroundTaskDispatcher_ctor_Locker_Null()
		{
			Assert.Throws<ArgumentNullException>(() => new BackgroundTaskDispatcher(null, new TaskQueue(), _server.Object));
		}

		[Test]
		public void BackgroundTaskDispatcher_ctor_Queue_Null()
		{
			var locker = new DispatcherLock();
			Assert.Throws<ArgumentNullException>(() => new BackgroundTaskDispatcher(locker, null, _server.Object));
		}

		[Test]
		public void BackgroundTaskDispatcher_ctor_Server_Null()
		{
			var locker = new DispatcherLock();
			Assert.Throws<ArgumentNullException>(() => new BackgroundTaskDispatcher(locker, new TaskQueue(), null));
		}

		[Test]
		public void BackgroundTaskDispatcher_Locker_IsLocked()
		{
			var locker = new DispatcherLock();
			locker.Lock();

			var queue = new Mock<ITaskQueue>();

			var dispatcher = new BackgroundTaskDispatcher(locker, queue.Object, _server.Object);
			dispatcher.Execute(_context.Object);

			queue.Verify(exp => exp.TryDequeue(out It.Ref<ITask>.IsAny), Times.Never);
		}

		[Test]
		public void BackgroundTaskDispatcher_Locker_Lock_Unlock()
		{
			var locker = new DispatcherLock();

			var dispatcher = new BackgroundTaskDispatcher(locker, new TaskQueue(), _server.Object);
			dispatcher.Execute(_context.Object);

			Assert.IsFalse(locker.IsLocked());
		}

		[Test]
		public void BackgroundTaskDispatcher_Dequeue_Task()
		{
			var queue = new TaskQueue();
			var task = new Mock<ITask>();
			queue.Enqueue(task.Object);

			var dispatcher = new BackgroundTaskDispatcher(new DispatcherLock(), queue, _server.Object);
			dispatcher.Execute(_context.Object);

			task.VerifySet(exp => exp.State = TaskState.Dequeued, Times.Once);
		}

		[Test]
		public void BackgroundTaskDispatcher_Server_StartNew()
		{
			var queue = new TaskQueue();
			queue.Enqueue(TaskFactory.CreateTask(() => Console.WriteLine("BackgroundTaskDispatcher")));
			queue.Enqueue(TaskFactory.CreateTask(() => Console.WriteLine("BackgroundTaskDispatcher")));

			var dispatcher = new BackgroundTaskDispatcher(new DispatcherLock(), queue, _server.Object);
			dispatcher.Execute(_context.Object);

			_server.Verify(exp => exp.StartNew(It.IsAny<TaskExecutionDispatcher>()), Times.Exactly(2));
		}
	}
}
