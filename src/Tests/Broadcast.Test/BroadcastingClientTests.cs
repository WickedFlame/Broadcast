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
		[SetUp]
		public void Setup()
		{
			// set to null to reset
			BroadcastingClient.Setup(null);
		}

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

			Assert.AreSame(task, store.Single());
		}

		[Test]
		public void BroadcastingClient_TaskStore_Default()
		{
			TaskStore.Default.Clear();

			var client = new BroadcastingClient();

			var task = TaskFactory.CreateTask(() => Console.WriteLine("BroadcastingClient"));
			client.Enqueue(task);

			Assert.AreSame(task, TaskStore.Default.Single());
		}

		[Test]
		public void BroadcastingClient_Default()
		{
			Assert.IsNotNull(BroadcastingClient.Default);
		}

		[Test]
		public void BroadcastingClient_Default_Setup()
		{
			var def = BroadcastingClient.Default;
			BroadcastingClient.Setup(() => new BroadcastingClient());
			
			Assert.AreNotSame(def, BroadcastingClient.Default);
		}

		[Test]
		public void BroadcastingClient_Default_Setup_Reset()
		{
			var def = BroadcastingClient.Default;
			BroadcastingClient.Setup(() => new BroadcastingClient());

			// set to null to reset
			BroadcastingClient.Setup(null);

			Assert.AreSame(def, BroadcastingClient.Default);
		}

		[Test]
		public void BroadcastingClient_Default_Setup_Singleton()
		{
			BroadcastingClient.Setup(() => new BroadcastingClient());
			
			Assert.AreSame(BroadcastingClient.Default, BroadcastingClient.Default);
		}
	}
}
