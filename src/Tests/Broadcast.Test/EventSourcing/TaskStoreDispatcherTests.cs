using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Broadcast.EventSourcing;
using Broadcast.Processing;
using Broadcast.Storage;
using Moq;
using NUnit.Framework;

namespace Broadcast.Test.EventSourcing
{
	public class TaskStoreDispatcherTests
	{
		[Test]
		public void TaskStoreDispatcher_ctor()
		{
			Assert.DoesNotThrow(() => new TaskStoreDispatcher(new DispatcherLock(), new Mock<IStorage>().Object));
		}

		[Test]
		public void TaskStoreDispatcher_ctor_NoLock()
		{
			Assert.Throws<ArgumentNullException>(() => new TaskStoreDispatcher(null, new Mock<IStorage>().Object));
		}

		[Test]
		public void TaskStoreDispatcher_ctor_NoStorage()
		{
			Assert.Throws<ArgumentNullException>(() => new TaskStoreDispatcher(new DispatcherLock(), null));
		}

		[Test]
		public void TaskStoreDispatcher_Execute()
		{
			var id = "1";

			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.TryFetchNext(It.IsAny<StorageKey>(), It.IsAny<StorageKey>(), out id)).Returns(() => id != null).Callback(() => id = null);
			storage.Setup(exp => exp.Get<BroadcastTask>(It.IsAny<StorageKey>())).Returns(() => new BroadcastTask());

			var dispatcher = new TaskStoreDispatcher(new DispatcherLock(), storage.Object);

			var context = new StorageDispatcherContext
			{
				Dispatchers = new DispatcherStorage(),
				ResetEvent = new System.Threading.ManualResetEventSlim()
			};

			dispatcher.Execute(context);

			storage.Verify(exp => exp.TryFetchNext(It.IsAny<StorageKey>(), It.IsAny<StorageKey>(), out It.Ref<string>.IsAny), Times.Exactly(2));
		}

		[Test]
		public void TaskStoreDispatcher_Execut_Locked()
		{
			var id = "1";

			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.TryFetchNext(It.IsAny<StorageKey>(), It.IsAny<StorageKey>(), out id)).Returns(() => id != null).Callback(() => id = null);
			storage.Setup(exp => exp.Get<BroadcastTask>(It.IsAny<StorageKey>())).Returns(() => new BroadcastTask());

			var locker = new DispatcherLock();
			locker.Lock();
			var dispatcher = new TaskStoreDispatcher(locker, storage.Object);

			var context = new StorageDispatcherContext
			{
				Dispatchers = new DispatcherStorage(),
				ResetEvent = new System.Threading.ManualResetEventSlim()
			};

			dispatcher.Execute(context);

			storage.Verify(exp => exp.TryFetchNext(It.IsAny<StorageKey>(), It.IsAny<StorageKey>(), out It.Ref<string>.IsAny), Times.Never);
		}

		[Test]
		public void TaskStoreDispatcher_Execute_NoTask()
		{
			var id = "1";

			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.TryFetchNext(It.IsAny<StorageKey>(), It.IsAny<StorageKey>(), out id)).Returns(() => id != null).Callback(() => id = null);
			storage.Setup(exp => exp.Get<BroadcastTask>(It.IsAny<StorageKey>())).Returns(() => null);

			var dispatcher = new TaskStoreDispatcher(new DispatcherLock(), storage.Object);

			var subDispatcher = new Mock<IDispatcher>();
			var dispatcherStorage = new DispatcherStorage();
			dispatcherStorage.Add("1", new[] {subDispatcher.Object});

			var context = new StorageDispatcherContext
			{
				Dispatchers = dispatcherStorage,
				ResetEvent = new System.Threading.ManualResetEventSlim()
			};

			dispatcher.Execute(context);

			subDispatcher.Verify(exp => exp.Execute(It.IsAny<ITask>()), Times.Never);
		}

		[Test]
		public void TaskStoreDispatcher_Execute_NoTask_RemoveFromStorage()
		{
			var id = "1";

			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.TryFetchNext(It.IsAny<StorageKey>(), It.IsAny<StorageKey>(), out id)).Returns(() => id != null).Callback(() => id = null);
			storage.Setup(exp => exp.Get<BroadcastTask>(It.IsAny<StorageKey>())).Returns(() => null);

			var dispatcher = new TaskStoreDispatcher(new DispatcherLock(), storage.Object);

			var context = new StorageDispatcherContext
			{
				Dispatchers = new DispatcherStorage(),
				ResetEvent = new System.Threading.ManualResetEventSlim()
			};

			dispatcher.Execute(context);

			storage.Verify(exp => exp.RemoveFromList(It.IsAny<StorageKey>(), It.IsAny<string>()), Times.Once);
		}

		[Test]
		public void TaskStoreDispatcher_Execute_NoTask_ResetEvent()
		{
			var id = "1";

			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.TryFetchNext(It.IsAny<StorageKey>(), It.IsAny<StorageKey>(), out id)).Returns(() => id != null).Callback(() => id = null);
			storage.Setup(exp => exp.Get<BroadcastTask>(It.IsAny<StorageKey>())).Returns(() => null);

			var dispatcher = new TaskStoreDispatcher(new DispatcherLock(), storage.Object);

			var resetEvent = new System.Threading.ManualResetEventSlim();
			var context = new StorageDispatcherContext
			{
				Dispatchers = new DispatcherStorage(),
				ResetEvent = resetEvent
			};

			dispatcher.Execute(context);

			Assert.IsFalse(resetEvent.IsSet);
		}

		[Test]
		public void TaskStoreDispatcher_Execute_Dispatchers()
		{
			var id = "1";

			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.TryFetchNext(It.IsAny<StorageKey>(), It.IsAny<StorageKey>(), out id)).Returns(() => id != null).Callback(() => id = null);
			storage.Setup(exp => exp.Get<BroadcastTask>(It.IsAny<StorageKey>())).Returns(() => new BroadcastTask());

			var dispatcher = new TaskStoreDispatcher(new DispatcherLock(), storage.Object);

			var subDispatcher = new Mock<IDispatcher>();
			var dispatcherStorage = new DispatcherStorage();
			dispatcherStorage.Add("1", new[] { subDispatcher.Object });

			var context = new StorageDispatcherContext
			{
				Dispatchers = dispatcherStorage,
				ResetEvent = new System.Threading.ManualResetEventSlim()
			};

			dispatcher.Execute(context);

			subDispatcher.Verify(exp => exp.Execute(It.IsAny<ITask>()), Times.Once);
		}

		[Test]
		public void TaskStoreDispatcher_Execute_ResetEvent()
		{
			var id = "1";

			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.TryFetchNext(It.IsAny<StorageKey>(), It.IsAny<StorageKey>(), out id)).Returns(() => id != null).Callback(() => id = null);
			storage.Setup(exp => exp.Get<BroadcastTask>(It.IsAny<StorageKey>())).Returns(() => new BroadcastTask());

			var dispatcher = new TaskStoreDispatcher(new DispatcherLock(), storage.Object);

			var resetEvent = new System.Threading.ManualResetEventSlim();

			var context = new StorageDispatcherContext
			{
				Dispatchers = new DispatcherStorage(),
				ResetEvent = resetEvent
			};

			dispatcher.Execute(context);

			Assert.That(resetEvent.IsSet);
		}

		[Test]
		public void TaskStoreDispatcher_Execute_Dispatchers_GetNext()
		{
			var id = "1";

			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.TryFetchNext(It.IsAny<StorageKey>(), It.IsAny<StorageKey>(), out id)).Returns(() => id != null).Callback(() => id = null);
			storage.Setup(exp => exp.Get<BroadcastTask>(It.IsAny<StorageKey>())).Returns(() => new BroadcastTask());

			var dispatcherStorage = new Mock<IDispatcherStorage>();

			var dispatcher = new TaskStoreDispatcher(new DispatcherLock(), storage.Object);

			var context = new StorageDispatcherContext
			{
				Dispatchers = dispatcherStorage.Object,
				ResetEvent = new System.Threading.ManualResetEventSlim()
			};

			dispatcher.Execute(context);

			dispatcherStorage.Verify(exp => exp.GetNext(), Times.Once);
		}

		[Test]
		public void TaskStoreDispatcher_Execute_Dispatchers_GetNext_Queue()
		{
			var id = "1";
			var task = new BroadcastTask
			{
				Id = id
			};

			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.TryFetchNext(It.IsAny<StorageKey>(), It.IsAny<StorageKey>(), out id)).Returns(() => id != null).Callback(() => id = null);
			storage.Setup(exp => exp.Get<BroadcastTask>(It.IsAny<StorageKey>())).Returns(() => task);

			// get the queue assigned to the extravalues of the task
			storage.Setup(exp => exp.Get<DataObject>(It.Is<StorageKey>(k => k.Key == $"tasks:values:{task.Id}"))).Returns(() => new DataObject
			{
				{"Queue", "server1"}
			});
			// get the key of the server containing the queue
			storage.Setup(exp => exp.GetKeys(It.Is<StorageKey>(k => k.Key == "server:server1:"))).Returns(() => new[] {"server:server1:serverid"});
			// get the id of the server
			storage.Setup(exp => exp.Get<DataObject>(It.Is<StorageKey>(k => k.Key == "server:server1:serverid"))).Returns(() => new DataObject
			{
				{"Id", "serverid"}
			});

			var dispatcherStorage = new Mock<IDispatcherStorage>();

			var dispatcher = new TaskStoreDispatcher(new DispatcherLock(), storage.Object);

			var context = new StorageDispatcherContext
			{
				Dispatchers = dispatcherStorage.Object,
				ResetEvent = new System.Threading.ManualResetEventSlim()
			};

			dispatcher.Execute(context);

			dispatcherStorage.Verify(exp => exp.GetNext("serverid"), Times.Once);
		}
	}
}
