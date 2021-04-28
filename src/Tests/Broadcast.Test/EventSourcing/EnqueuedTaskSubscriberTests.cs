using System;
using System.Collections.Generic;
using System.Text;
using Broadcast.EventSourcing;
using Broadcast.Storage;
using Moq;
using NUnit.Framework;

namespace Broadcast.Test.EventSourcing
{
	public class EnqueuedTaskSubscriberTests
	{
		[Test]
		public void EnqueuedTaskSubscriber_ctor()
		{
			Assert.DoesNotThrow(() => new EnqueuedTaskSubscriber(new TaskStore()));
		}

		[Test]
		public void EnqueuedTaskSubscriber_ctor_Null_Taskstore()
		{
			Assert.Throws<ArgumentNullException>(() => new EnqueuedTaskSubscriber(null));
		}

		[Test]
		public void EnqueuedTaskSubscriber_RaiseEvent_Store()
		{
			var store = new Mock<ITaskStore>();
			var subscriber = new EnqueuedTaskSubscriber(store.Object);

			subscriber.RaiseEvent();

			store.Verify(exp => exp.DispatchTasks(), Times.Once);
		}
	}
}
