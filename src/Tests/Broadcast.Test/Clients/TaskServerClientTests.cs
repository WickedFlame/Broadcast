using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Broadcast.Test.Clients
{
	public class TaskServerClientTests
	{
		[SetUp]
		public void Setup()
		{
			// set to null to reset
			TaskServerClient.Setup(null);
		}

		[Test]
		public void TaskServerClient_BroadcastingClient_Default()
		{
			Assert.IsNotNull(TaskServerClient.Client);
		}

		[Test]
		public void TaskServerClient_BroadcastingClient_Default_Setup()
		{
			var def = TaskServerClient.Client;
			TaskServerClient.Setup(() => new BroadcastingClient());

			Assert.AreNotSame(def, TaskServerClient.Client);
		}

		[Test]
		public void TaskServerClient_BroadcastingClient_Default_Setup_Reset()
		{
			var def = TaskServerClient.Client;
			TaskServerClient.Setup(() => new BroadcastingClient());

			// set to null to reset
			TaskServerClient.Setup(null);

			Assert.AreSame(def, TaskServerClient.Client);
		}

		[Test]
		public void TaskServerClient_BroadcastingClient_Default_Setup_Singleton()
		{
			TaskServerClient.Setup(() => new BroadcastingClient());

			Assert.AreSame(TaskServerClient.Client, TaskServerClient.Client);
		}
	}
}
