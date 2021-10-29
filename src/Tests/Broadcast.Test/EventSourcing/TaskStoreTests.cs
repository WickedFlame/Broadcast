using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Broadcast.Composition;
using Broadcast.Configuration;
using Broadcast.EventSourcing;
using Broadcast.Server;
using Broadcast.Storage;
using Moq;
using NUnit.Framework;

namespace Broadcast.Test.EventSourcing
{
	public class TaskStoreTests
	{
		[Test]
		public void TaskStore_ctor()
		{
			Assert.DoesNotThrow(() => new TaskStore());
		}

		[Test]
		public void TaskStore_ctor_All()
		{
			var storage = new Mock<IStorage>();
			Assert.DoesNotThrow(() => new TaskStore(new Options(), storage.Object));
		}

		[Test]
		public void TaskStore_ctor_Storage()
		{
			var storage = new Mock<IStorage>();
			Assert.DoesNotThrow(() => new TaskStore(storage.Object));
		}

		[Test]
		public void TaskStore_ctor_Null_Options()
		{
			var storage = new Mock<IStorage>();
			Assert.Throws<ArgumentNullException>(() => new TaskStore(null, storage.Object));
		}

		[Test]
		public void TaskStore_ctor_Null_Storage()
		{
			Assert.Throws<ArgumentNullException>(() => new TaskStore(new Options(), null));
		}

		[Test]
		public void TaskStore_Add()
		{
			var store = new TaskStore();

			var task = TaskFactory.CreateTask(() => Console.WriteLine("TaskStore_Dispatchers"));
			store.Add(task);

			var stored = store.Single();
			Assert.AreEqual(new { task.Id, task.Name }, new { stored.Id, stored.Name });
		}

		[Test]
		public void TaskStore_Add_Storage_AddToList()
		{
			var storage = new Mock<IStorage>();
			var store = new TaskStore(storage.Object);

			var task = TaskFactory.CreateTask(() => Console.WriteLine("TaskStore_Dispatchers"));
			store.Add(task);

			storage.Verify(exp => exp.AddToList(It.IsAny<StorageKey>(), It.IsAny<string>()), Times.Once);
		}

		[Test]
		public void TaskStore_Add_Storage_Set()
		{
			var storage = new Mock<IStorage>();
			var store = new TaskStore(storage.Object);

			var task = TaskFactory.CreateTask(() => Console.WriteLine("TaskStore_Dispatchers"));
			store.Add(task);

			storage.Verify(exp => exp.Set(It.IsAny<StorageKey>(), It.IsAny<ITask>()), Times.Once);
		}

		[Test]
		public void TaskStore_Add_Storage_PropagateEvent()
		{
			var storage = new Mock<IStorage>();
			var store = new TaskStore(storage.Object);

			var task = TaskFactory.CreateTask(() => Console.WriteLine("TaskStore_Dispatchers"));
			store.Add(task);

			storage.Verify(exp => exp.PropagateEvent(It.IsAny<StorageKey>()), Times.Once);
		}

		[Test]
		public void TaskStore_AddMultiple()
		{
			var store = new TaskStore();

			store.Add(TaskFactory.CreateTask(() => Console.WriteLine("TaskStore_Dispatchers")));
			store.Add(TaskFactory.CreateTask(() => Console.WriteLine("TaskStore_Dispatchers")));
			store.Add(TaskFactory.CreateTask(() => Console.WriteLine("TaskStore_Dispatchers")));

			Assert.AreEqual(3, store.Count());
		}

		[Test]
		public void TaskStore_AddMultiple_CollectionInitializer()
		{
			var store = new TaskStore
			{
				TaskFactory.CreateTask(() => Console.WriteLine("TaskStore_Dispatchers")), 
				TaskFactory.CreateTask(() => Console.WriteLine("TaskStore_Dispatchers")), 
				TaskFactory.CreateTask(() => Console.WriteLine("TaskStore_Dispatchers"))
			};

			Assert.AreEqual(3, store.Count());
		}
		
		[Test]
		public void TaskStore_Delete_ReloadTask()
		{
			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.Get<DataObject>(It.IsAny<StorageKey>())).Returns(new DataObject
			{
				{"State", "New"}
			});
			var store = new TaskStore(storage.Object);

			store.Delete("id");

			storage.Verify(exp => exp.Get<DataObject>(It.IsAny<StorageKey>()), Times.Once);
		}

		[Test]
		public void TaskStore_Delete_SetState()
		{
			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.Get<DataObject>(It.IsAny<StorageKey>())).Returns(new DataObject
			{
				{"State", "New"}
			});
			var store = new TaskStore(storage.Object);

			store.Delete("id");

			storage.Verify(exp => exp.Set<DataObject>(It.IsAny<StorageKey>(), It.Is<DataObject>(d => (TaskState)d["State"] == TaskState.Deleted)), Times.Once);
		}

		[Test]
		public void TaskStore_Delete_RemoveEnqueue()
		{
			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.Get<DataObject>(It.IsAny<StorageKey>())).Returns(new DataObject
			{
				{"State", "New"}
			});
			var store = new TaskStore(storage.Object);

			store.Delete("id");

			storage.Verify(exp => exp.RemoveFromList(It.Is<StorageKey>(s => s.ToString() == "tasks:enqueued"), "id"), Times.Once);
		}

		[Test]
		public void TaskStore_Delete_RemoveDequeue()
		{
			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.Get<DataObject>(It.IsAny<StorageKey>())).Returns(new DataObject
			{
				{"State", "New"}
			});
			var store = new TaskStore(storage.Object);

			store.Delete("id");

			storage.Verify(exp => exp.RemoveFromList(It.Is<StorageKey>(s => s.ToString() == "tasks:dequeued"), "id"), Times.Once);
		}

		[Test]
		public void TaskStore_Delete_NotRecurring()
		{
			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.Get<DataObject>(It.IsAny<StorageKey>())).Returns(new DataObject
			{
				{"State", "New"}
			});
			var store = new TaskStore(storage.Object);

			store.Delete("id");

			storage.Verify(exp => exp.GetKeys(It.Is<StorageKey>(s => s.ToString() == "tasks:recurring:")), Times.Never);
		}

		[Test]
		public void TaskStore_Delete_Recurring()
		{
			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.Get<DataObject>(It.IsAny<StorageKey>())).Returns(new DataObject
			{
				{"State", "New"},
				{"IsRecurring", true}
			});
			var store = new TaskStore(storage.Object);

			store.Delete("id");

			storage.Verify(exp => exp.GetKeys(It.Is<StorageKey>(s => s.ToString() == "tasks:recurring:")), Times.Once);
		}

		[Test]
		public void TaskStore_Delete_Recurring_GetRecurring()
		{
			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.Get<DataObject>(It.IsAny<StorageKey>())).Returns(new DataObject
			{
				{"State", "New"},
				{"IsRecurring", true}
			});
			storage.Setup(exp => exp.GetKeys(It.IsAny<StorageKey>())).Returns(() => new List<string> {"one", "two"});
			storage.Setup(exp => exp.Get<RecurringTask>(It.Is<StorageKey>(k => k.ToString() == "two"))).Returns(() => new RecurringTask
			{
				ReferenceId = "id"
			});
			var store = new TaskStore(storage.Object);

			store.Delete("id");

			storage.Verify(exp => exp.Get<RecurringTask>(It.IsAny<StorageKey>()), Times.Exactly(2));
		}

		[Test]
		public void TaskStore_Delete_Recurring_Delete()
		{
			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.Get<DataObject>(It.IsAny<StorageKey>())).Returns(new DataObject
			{
				{"State", "New"},
				{"IsRecurring", true}
			});
			storage.Setup(exp => exp.GetKeys(It.IsAny<StorageKey>())).Returns(() => new List<string> { "one", "two" });
			storage.Setup(exp => exp.Get<RecurringTask>(It.Is<StorageKey>(k => k.ToString() == "two"))).Returns(() => new RecurringTask
			{
				ReferenceId = "id"
			});
			var store = new TaskStore(storage.Object);

			store.Delete("id");

			storage.Verify(exp => exp.Delete(It.IsAny<StorageKey>()), Times.Once);
		}

		[Test]
		public void TaskStore_Subscriptions_ServerHeartbeatSubscriber()
		{
			var storage = new Mock<IStorage>();
			var store = new TaskStore(storage.Object);

			storage.Verify(exp => exp.RegisterSubscription(It.IsAny<ServerHeartbeatSubscriber>()), Times.Once);
		}

		[Test]
		public void TaskStore_Subscriptions_EnqueuedTaskSubscriber()
		{
			var storage = new Mock<IStorage>();
			var store = new TaskStore(storage.Object);

			storage.Verify(exp => exp.RegisterSubscription(It.IsAny<EnqueuedTaskSubscriber>()), Times.Once);
		}

		[Test]
		public void TaskStore_Dispatchers()
		{
			ITask output = null;
			var store = new TaskStore();
			store.RegisterDispatchers("id", new IDispatcher[]
			{
				new TestDispatcher(t => output = t)
			});

			var task = TaskFactory.CreateTask(() => Console.WriteLine("TaskStore_Dispatchers"));
			store.Add(task);

			store.WaitAll();

			Assert.AreEqual(new { task.Id, task.Name }, new { output.Id, output.Name });
		}

		[Test]
		public void TaskStore_Dispatchers_RegisterMultiple()
		{
			var number = 0;
			var store = new TaskStore();

			store.RegisterDispatchers("1", new IDispatcher[] { new TestDispatcher(t => number = 1) });
			store.RegisterDispatchers("2", new IDispatcher[] { new TestDispatcher(t => number = 2) });
			store.RegisterDispatchers("3", new IDispatcher[] { new TestDispatcher(t => number = 3) });

			// dispatching tasks uses a round robin implementation to select the set of dispatchers
			store.Add(TaskFactory.CreateTask(() => Console.WriteLine("TaskStore_Dispatchers")));
			store.WaitAll();
			Assert.AreEqual(1, number);

			store.Add(TaskFactory.CreateTask(() => Console.WriteLine("TaskStore_Dispatchers")));
			store.WaitAll();
			Assert.AreEqual(2, number);

			store.Add(TaskFactory.CreateTask(() => Console.WriteLine("TaskStore_Dispatchers")));
			store.WaitAll();
			Assert.AreEqual(3, number);

			store.Add(TaskFactory.CreateTask(() => Console.WriteLine("TaskStore_Dispatchers")));
			store.WaitAll();
			Assert.AreEqual(1, number);
		}

		[Test]
		public void TaskStore_Dispatchers_RegisterMultiple_SameId()
		{
			var cnt = 0;
			var store = new TaskStore();

			// dispatchers are reset before registration
			store.RegisterDispatchers("id", new IDispatcher[] {new TestDispatcher(t => cnt += 1)});
			store.RegisterDispatchers("id", new IDispatcher[] { new TestDispatcher(t => cnt += 1) });
			store.RegisterDispatchers("id", new IDispatcher[] { new TestDispatcher(t => cnt += 1) });

			store.Add(TaskFactory.CreateTask(() => Console.WriteLine("TaskStore_Dispatchers")));
			store.WaitAll();

			Assert.AreEqual(1, cnt);
		}

		[Test]
		public void TaskStore_Dispatchers_Unregister()
		{
			var cnt = 0;
			var store = new TaskStore();

			// dispatchers are reset before registration
			store.RegisterDispatchers("id", new IDispatcher[] { new TestDispatcher(t => cnt += 1) });
			store.UnregisterDispatchers("id");

			store.Add(TaskFactory.CreateTask(() => Console.WriteLine("TaskStore_Dispatchers")));

			Assert.AreEqual(0, cnt);
		}

		[Test]
		public void TaskStore_DispatchTask()
		{
			var dispatcher = new Mock<IDispatcher>();

			var store = new TaskStore();
			store.RegisterDispatchers("id", new[] {dispatcher.Object});

			var task = TaskFactory.CreateTask(() => Console.WriteLine("TaskStore"));
			store.Add(task);
			store.WaitAll();

			dispatcher.Verify(exp => exp.Execute(It.IsAny<ITask>()), Times.Once);
		}

		[Test]
		public void TaskStore_Clear_GetKeys()
		{
			var storage = new Mock<IStorage>();
			var store = new TaskStore(storage.Object);

			store.Clear();

			storage.Verify(exp => exp.GetKeys(It.IsAny<StorageKey>()), Times.Once);
		}

		[Test]
		public void TaskStore_Clear_Delete()
		{
			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.GetKeys(It.IsAny<StorageKey>())).Returns(() => new[] {"one", "two", "tree"});
			var store = new TaskStore(storage.Object);

			store.Clear();

			storage.Verify(exp => exp.Delete(It.IsAny<StorageKey>()), Times.Exactly(3));
		}

		[Test]
		public void TaskStore_Clear_Integration()
		{
			var store = new TaskStore
			{
				TaskFactory.CreateTask(() => Console.WriteLine("TaskStore_Dispatchers")),
				TaskFactory.CreateTask(() => Console.WriteLine("TaskStore_Dispatchers")),
				TaskFactory.CreateTask(() => Console.WriteLine("TaskStore_Dispatchers"))
			};

			store.Clear();

			Assert.AreEqual(0, store.Count());
		}

		[Test]
		public void TaskStore_Storage()
		{
			var storage = new Mock<IStorage>();
			var store = new TaskStore(storage.Object);

			store.Storage(s => Assert.AreSame(s, storage.Object));
		}

		[Test]
		public void TaskStore_Storage_Default_Type()
		{
			var store = new TaskStore();

			store.Storage(s => Assert.IsAssignableFrom<InmemoryStorage>(s));
		}

		[Test]
		public void TaskStore_Storage_Func_Default_Type()
		{
			var store = new TaskStore();

			Assert.IsAssignableFrom<InmemoryStorage>(store.Storage(s => s));
		}

		[Test]
		public void TaskStore_Storage_Func_ReturnValue()
		{
			var store = new TaskStore();

			Assert.IsTrue(store.Storage(s => true));
		}

		[Test]
		public void TaskStore_PropagateServer()
		{
			var server = new ServerModel
			{
				Id = "1",
				Name = "server",
				Heartbeat = DateTime.Now
			};

			var store = new TaskStore();
			store.PropagateServer(server);

			Assert.AreSame(server, store.Servers.Single());
		}

		[Test]
		public void TaskStore_PropagateServer_Cleanup()
		{
			var server = new ServerModel
			{
				Id = "1",
				Name = "server",
				Heartbeat = DateTime.Now.AddMinutes(-1)
			};

			var store = new TaskStore();
			store.PropagateServer(server);

			Assert.IsEmpty(store.Servers);
		}

		[Test]
		public void TaskStore_RemoveServer()
		{
			var server = new ServerModel
			{
				Id = "1",
				Name = "server",
				Heartbeat = DateTime.Now
			};

			var store = new TaskStore();
			store.PropagateServer(server);

			// act
			store.RemoveServer(new ServerModel
			{
				Id = "1",
				Name = "server"
			});

			Assert.IsEmpty(store.Servers);
		}

		[Test]
		public void TaskStore_RemoveServer_DeleteFromStorage()
		{
			var server = new ServerModel
			{
				Id = "1",
				Name = "server",
				Heartbeat = DateTime.Now
			};

			var store = new TaskStore();
			store.PropagateServer(server);

			// act
			store.RemoveServer(new ServerModel
			{
				Id = "1",
				Name = "server"
			});

			Assert.IsNull(store.Storage(s => s.Get<DataObject>(new StorageKey("server:1:server"))));
		}


		[Test]
		public void TaskStore_DispatchTasks_NoFetch_Get()
		{
			var storage = new Mock<IStorage>();
			var store = new TaskStore(storage.Object);

			store.DispatchTasks();

			store.WaitAll();

			storage.Verify(exp => exp.Get<string>(It.IsAny<StorageKey>()), Times.Never);
		}

		[Test]
		public void TaskStore_DispatchTasks_Fetch_NoReturn()
		{
			var storage = new Mock<IStorage>();
			var store = new TaskStore(storage.Object);

			store.DispatchTasks();

			store.WaitAll();

			storage.Verify(exp => exp.TryFetchNext(It.IsAny<StorageKey>(), It.IsAny<StorageKey>(), out It.Ref<string>.IsAny), Times.Once);
		}

		[Test]
		public void TaskStore_DispatchTasks_Fetch_WithReturn()
		{
			var id = "1";

			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.TryFetchNext(It.IsAny<StorageKey>(), It.IsAny<StorageKey>(), out id)).Returns(() => id != null).Callback(() => id = null);
			storage.Setup(exp => exp.GetList(It.IsAny<StorageKey>())).Returns(() => id == null ? Enumerable.Empty<string>() : new List<string> { "test" });

			var store = new TaskStore(storage.Object);
			store.DispatchTasks();

			store.WaitAll();

			storage.Verify(exp => exp.TryFetchNext(It.IsAny<StorageKey>(), It.IsAny<StorageKey>(), out It.Ref<string>.IsAny), Times.Exactly(2));
		}

		[Test]
		public void TaskStore_DispatchTasks_Get()
		{
			var id = "1";

			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.TryFetchNext(It.IsAny<StorageKey>(), It.IsAny<StorageKey>(), out id)).Returns(() => id != null).Callback(() => id = null);
			storage.Setup(exp => exp.GetList(It.IsAny<StorageKey>())).Returns(() => id == null ? Enumerable.Empty<string>() : new List<string> { "test" });

			var store = new TaskStore(storage.Object);
			store.DispatchTasks();
			
			store.WaitAll();

			storage.Verify(exp => exp.Get<BroadcastTask>(It.IsAny<StorageKey>()), Times.Once);
		}

		[Test]
		public void TaskStore_DispatchTasks_Get_NoFetch()
		{
			string id = null;

			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.TryFetchNext(It.IsAny<StorageKey>(), It.IsAny<StorageKey>(), out id)).Returns(() => false);

			var store = new TaskStore(storage.Object);
			store.DispatchTasks();

			store.WaitAll();

			storage.Verify(exp => exp.Get<string>(It.IsAny<StorageKey>()), Times.Never);
		}

		[Test]
		public void TaskStore_DispatchTasks_Dispatch()
		{
			var id = "1";

			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.TryFetchNext(It.IsAny<StorageKey>(), It.IsAny<StorageKey>(), out id)).Returns(() => id != null).Callback(() => id = null);
			storage.Setup(exp => exp.Get<BroadcastTask>(It.IsAny<StorageKey>())).Returns(() => new BroadcastTask());
			storage.Setup(exp => exp.GetList(It.IsAny<StorageKey>())).Returns(() => id == null ? Enumerable.Empty<string>() : new List<string> {"test"});

			var dispatcher = new Mock<IDispatcher>();

			var store = new TaskStore(storage.Object);
			store.RegisterDispatchers("1", new[] { dispatcher.Object });

			store.DispatchTasks();

			store.WaitAll();

			dispatcher.Verify(exp => exp.Execute(It.IsAny<ITask>()), Times.Once);
		}

		[Test]
		public void TaskStore_DispatchTasks_Dispatch_NoFetch()
		{
			string id = null;

			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.TryFetchNext(It.IsAny<StorageKey>(), It.IsAny<StorageKey>(), out id)).Returns(() => false);

			var dispatcher = new Mock<IDispatcher>();

			var store = new TaskStore(storage.Object);
			store.RegisterDispatchers("1", new[] { dispatcher.Object });

			store.DispatchTasks();

			store.WaitAll();

			dispatcher.Verify(exp => exp.Execute(It.IsAny<ITask>()), Times.Never);
		}


		private class TestDispatcher : IDispatcher
		{
			private readonly Action<ITask> _output;

			public TestDispatcher(Action<ITask> output)
			{
				_output = output;
			}

			public void Dispose()
			{
			}

			public void Execute(ITask task)
			{
				_output(task);
			}
		}
	}
}
