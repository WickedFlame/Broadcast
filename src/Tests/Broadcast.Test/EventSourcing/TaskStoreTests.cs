using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Broadcast.Composition;
using Broadcast.Configuration;
using Broadcast.EventSourcing;
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
			Assert.Throws<ArgumentNullException>(() => new TaskStore(Options.Default, null));
		}

		[Test]
		public void TaskStore_Add()
		{
			var store = new TaskStore();

			var task = TaskFactory.CreateTask(() => Console.WriteLine("TaskStore_Dispatchers"));
			store.Add(task);

			Assert.AreSame(task, store.Single());
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

			Assert.AreSame(output, task);
		}

		[Test]
		public void TaskStore_Dispatchers_RegisterMultiple()
		{
			var cnt = 0;
			var store = new TaskStore();

			// dispatchers are reset before registration
			store.RegisterDispatchers("1", new IDispatcher[] { new TestDispatcher(t => cnt += 1) });
			store.RegisterDispatchers("2", new IDispatcher[] { new TestDispatcher(t => cnt += 1) });
			store.RegisterDispatchers("3", new IDispatcher[] { new TestDispatcher(t => cnt += 1) });

			store.Add(TaskFactory.CreateTask(() => Console.WriteLine("TaskStore_Dispatchers")));

			Assert.AreEqual(3, cnt);
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
		public void TaskStore_Clear()
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
