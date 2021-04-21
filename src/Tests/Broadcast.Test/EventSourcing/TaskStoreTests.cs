using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Broadcast.Composition;
using Broadcast.Configuration;
using Broadcast.EventSourcing;
using NUnit.Framework;

namespace Broadcast.Test.EventSourcing
{
	public class TaskStoreTests
	{
		[Test]
		public void TaskStore()
		{
			Assert.DoesNotThrow(() => new TaskStore());
		}

		[Test]
		public void TaskStore_Options()
		{
			Assert.DoesNotThrow(() => new TaskStore(new Options()));
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
			store.RegisterDispatchers(new IDispatcher[]
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
			store.RegisterDispatchers(new IDispatcher[] {new TestDispatcher(t => cnt += 1)});
			store.RegisterDispatchers(new IDispatcher[] { new TestDispatcher(t => cnt += 1) });
			store.RegisterDispatchers(new IDispatcher[] { new TestDispatcher(t => cnt += 1) });

			store.Add(TaskFactory.CreateTask(() => Console.WriteLine("TaskStore_Dispatchers")));

			Assert.AreEqual(1, cnt);
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
