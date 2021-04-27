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

			store.Verify(exp => exp.Storage(It.IsAny<Action<IStorage>>()), Times.Once);
		}

		[Test]
		public void EnqueuedTaskSubscriber_RaiseEvent_GetList_Initial()
		{
			var storage = new Mock<IStorage>();
			var subscriber = new EnqueuedTaskSubscriber(new TaskStore(storage.Object));

			subscriber.RaiseEvent();

			storage.Verify(exp => exp.GetList<string>(It.IsAny<StorageKey>()), Times.Once);
		}

		[Test]
		public void EnqueuedTaskSubscriber_RaiseEvent_GetList_Full()
		{
			var called = false;

			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.GetList<string>(It.IsAny<StorageKey>())).Returns(() => called ? new string[] { } : new[] { "1" }).Callback(() => called = true);
			var subscriber = new EnqueuedTaskSubscriber(new TaskStore(storage.Object));

			subscriber.RaiseEvent();

			storage.Verify(exp => exp.GetList<string>(It.IsAny<StorageKey>()), Times.Exactly(2));
		}

		[Test]
		public void EnqueuedTaskSubscriber_RaiseEvent_RemoveFromList()
		{
			var called = false;

			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.GetList<string>(It.IsAny<StorageKey>())).Returns(() => called ? new string[] { } : new[] { "1" }).Callback(() => called = true);
			var subscriber = new EnqueuedTaskSubscriber(new TaskStore(storage.Object));

			subscriber.RaiseEvent();

			storage.Verify(exp => exp.RemoveFromList(It.IsAny<StorageKey>(), It.IsAny<string>()), Times.Once);
		}

		[Test]
		public void EnqueuedTaskSubscriber_RaiseEvent_Get()
		{
			var called = false;

			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.GetList<string>(It.IsAny<StorageKey>())).Returns(() => called ? new string[] { } : new[] { "1" }).Callback(() => called = true);
			storage.Setup(exp => exp.RemoveFromList(It.IsAny<StorageKey>(), It.IsAny<string>())).Returns(() => true);

			var subscriber = new EnqueuedTaskSubscriber(new TaskStore(storage.Object));

			subscriber.RaiseEvent();

			storage.Verify(exp => exp.Get<ITask>(It.IsAny<StorageKey>()), Times.Once);
		}

		[Test]
		public void EnqueuedTaskSubscriber_RaiseEvent_Get_NoDelete()
		{
			var called = false;

			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.GetList<string>(It.IsAny<StorageKey>())).Returns(() => called ? new string[] { } : new[] { "1" }).Callback(() => called = true);
			storage.Setup(exp => exp.RemoveFromList(It.IsAny<StorageKey>(), It.IsAny<string>())).Returns(() => false);

			var subscriber = new EnqueuedTaskSubscriber(new TaskStore(storage.Object));

			subscriber.RaiseEvent();

			storage.Verify(exp => exp.Get<ITask>(It.IsAny<StorageKey>()), Times.Never);
		}

		[Test]
		public void EnqueuedTaskSubscriber_RaiseEvent_AddToList()
		{
			var called = false;

			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.GetList<string>(It.IsAny<StorageKey>())).Returns(() => called ? new string[] { } : new[] { "1" }).Callback(() => called = true);
			storage.Setup(exp => exp.RemoveFromList(It.IsAny<StorageKey>(), It.IsAny<string>())).Returns(() => true);

			var subscriber = new EnqueuedTaskSubscriber(new TaskStore(storage.Object));

			subscriber.RaiseEvent();

			storage.Verify(exp => exp.AddToList(It.IsAny<StorageKey>(), "1"), Times.Once);
		}

		[Test]
		public void EnqueuedTaskSubscriber_RaiseEvent_AddToList_NoDelete()
		{
			var called = false;

			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.GetList<string>(It.IsAny<StorageKey>())).Returns(() => called ? new string[] { } : new[] { "1" }).Callback(() => called = true);
			storage.Setup(exp => exp.RemoveFromList(It.IsAny<StorageKey>(), It.IsAny<string>())).Returns(() => false);

			var subscriber = new EnqueuedTaskSubscriber(new TaskStore(storage.Object));

			subscriber.RaiseEvent();

			storage.Verify(exp => exp.AddToList(It.IsAny<StorageKey>(), "1"), Times.Never);
		}

		[Test]
		public void EnqueuedTaskSubscriber_RaiseEvent_Dispatch()
		{
			var called = false;

			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.GetList<string>(It.IsAny<StorageKey>())).Returns(() => called ? new string[] { } : new[] { "1" }).Callback(() => called = true);
			storage.Setup(exp => exp.RemoveFromList(It.IsAny<StorageKey>(), It.IsAny<string>())).Returns(() => true);

			var dispatcher = new Mock<IDispatcher>();

			var store = new TaskStore(storage.Object);
			store.RegisterDispatchers("1", new[] {dispatcher.Object});

			var subscriber = new EnqueuedTaskSubscriber(store);

			subscriber.RaiseEvent();

			dispatcher.Verify(exp => exp.Execute(It.IsAny<ITask>()), Times.Once);
		}

		[Test]
		public void EnqueuedTaskSubscriber_RaiseEvent_Dispatch_NoDelete()
		{
			var called = false;

			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.GetList<string>(It.IsAny<StorageKey>())).Returns(() => called ? new string[] { } : new[] { "1" }).Callback(() => called = true);
			storage.Setup(exp => exp.RemoveFromList(It.IsAny<StorageKey>(), It.IsAny<string>())).Returns(() => false);

			var dispatcher = new Mock<IDispatcher>();

			var store = new TaskStore(storage.Object);
			store.RegisterDispatchers("1", new[] { dispatcher.Object });

			var subscriber = new EnqueuedTaskSubscriber(store);

			subscriber.RaiseEvent();

			dispatcher.Verify(exp => exp.Execute(It.IsAny<ITask>()), Times.Never);
		}
	}
}
