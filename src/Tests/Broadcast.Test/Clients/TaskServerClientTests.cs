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
			BackgroundTask.Setup(null);
		}

		[Test]
		public void TaskServerClient_BroadcastingClient_Default()
		{
			Assert.IsNotNull(BackgroundTask.Client);
		}

		[Test]
		public void TaskServerClient_BroadcastingClient_Default_Setup()
		{
			var def = BackgroundTask.Client;
			BackgroundTask.Setup(() => new BroadcastingClient());

			Assert.AreNotSame(def, BackgroundTask.Client);
		}

		[Test]
		public void TaskServerClient_BroadcastingClient_Default_Setup_Reset()
		{
			var def = BackgroundTask.Client;
			BackgroundTask.Setup(() => new BroadcastingClient());

			// set to null to reset
			BackgroundTask.Setup(null);

			Assert.AreSame(def, BackgroundTask.Client);
		}

		[Test]
		public void TaskServerClient_BroadcastingClient_Default_Setup_Singleton()
		{
			BackgroundTask.Setup(() => new BroadcastingClient());

			Assert.AreSame(BackgroundTask.Client, BackgroundTask.Client);
		}
	}
}
