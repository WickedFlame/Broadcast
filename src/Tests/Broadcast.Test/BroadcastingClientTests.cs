using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Broadcast.Composition;
using Broadcast.EventSourcing;
using NUnit.Framework;

namespace Broadcast.Test
{
	public class BroadcastingClientTests
	{
		[Test]
		public void BroadcastingClient_ctor()
		{
			Assert.DoesNotThrow(() => new BroadcastingClient());
		}

		[Test]
		public void BroadcastingClient_ctor_TaskStore()
		{
			Assert.DoesNotThrow(() => new BroadcastingClient(new TaskStore()));
		}

		[Test]
		public void BroadcastingClient_TaskStore()
		{
			var store = new TaskStore();
			var client = new BroadcastingClient(store);

			var task = TaskFactory.CreateTask(() => Console.WriteLine("BroadcastingClient"));
			client.Enqueue(task);

			var stored = store.Single();
			Assert.AreEqual(new {task.Id, task.Name}, new {stored.Id, stored.Name});
		}

		[Test]
		public void BroadcastingClient_TaskStore_Default()
		{
			TaskStore.Default.Clear();

			var client = new BroadcastingClient();

			var task = TaskFactory.CreateTask(() => Console.WriteLine("BroadcastingClient"));
			client.Enqueue(task);

			var stored = TaskStore.Default.Single();
			Assert.AreEqual(new { task.Id, task.Name }, new { stored.Id, stored.Name });
		}
	}
}
