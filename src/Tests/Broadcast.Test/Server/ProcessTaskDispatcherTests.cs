using System;
using System.Collections.Generic;
using System.Text;
using Broadcast.Composition;
using Broadcast.EventSourcing;
using Broadcast.Server;
using Moq;
using NUnit.Framework;

namespace Broadcast.Test.Server
{
	public class ProcessTaskDispatcherTests
	{
		private Mock<IBroadcaster> _broadcaster;

		[SetUp]
		public void Setup()
		{
			_broadcaster = new Mock<IBroadcaster>();
		}

		[Test]
		public void ProcessTaskDispatcher_ctor()
		{
			Assert.DoesNotThrow(() => new ProcessTaskDispatcher(_broadcaster.Object));
		}

		[Test]
		public void ProcessTaskDispatcher_ctor_Null_Broadcaster()
		{
			Assert.Throws<ArgumentNullException>(() => new ProcessTaskDispatcher(null));
		}

		[Test]
		public void ProcessTaskDispatcher_Execute()
		{
			var dispatcher = new ProcessTaskDispatcher(_broadcaster.Object);
			var task = TaskFactory.CreateTask(() => Console.WriteLine("ProcessTaskDispatcher"));

			dispatcher.Execute(task);

			_broadcaster.Verify(exp => exp.Process(It.Is<ITask>(t => t == task)), Times.Once);
		}

		[Test]
		public void ProcessTaskDispatcher_Execute_IsRecurring()
		{
			var dispatcher = new ProcessTaskDispatcher(_broadcaster.Object);
			var task = TaskFactory.CreateTask(() => Console.WriteLine("ProcessTaskDispatcher"));
			task.IsRecurring = true;

			dispatcher.Execute(task);

			_broadcaster.Verify(exp => exp.Process(It.IsAny<ITask>()), Times.Never);
		}

		[Test]
		public void ProcessTaskDispatcher_Execute_Time()
		{
			var dispatcher = new ProcessTaskDispatcher(_broadcaster.Object);
			var task = TaskFactory.CreateTask(() => Console.WriteLine("ProcessTaskDispatcher"));
			task.Time = TimeSpan.Zero;

			dispatcher.Execute(task);

			_broadcaster.Verify(exp => exp.Process(It.IsAny<ITask>()), Times.Never);
		}
	}
}
