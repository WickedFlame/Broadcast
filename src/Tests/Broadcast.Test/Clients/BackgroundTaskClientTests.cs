using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Broadcast.Test.Clients
{
	public class BackgroundTaskClientTests
	{
		[SetUp]
		public void Setup()
		{
			// set to null to reset
			BackgroundTaskClient.Setup(null);
		}

		[Test]
		public void BackgroundTaskClient_BroadcastingClient_Default()
		{
			Assert.IsNotNull(BackgroundTaskClient.Client);
		}

		[Test]
		public void BackgroundTaskClient_BroadcastingClient_Default_Setup()
		{
			var def = BackgroundTaskClient.Client;
			BackgroundTaskClient.Setup(() => new BroadcastingClient());

			Assert.AreNotSame(def, BackgroundTaskClient.Client);
		}

		[Test]
		public void BackgroundTaskClient_BroadcastingClient_Default_Setup_Reset()
		{
			var def = BackgroundTaskClient.Client;
			BackgroundTaskClient.Setup(() => new BroadcastingClient());

			// set to null to reset
			BackgroundTaskClient.Setup(null);

			Assert.AreSame(def, BackgroundTaskClient.Client);
		}

		[Test]
		public void BackgroundTaskClient_BroadcastingClient_Default_Setup_Singleton()
		{
			BackgroundTaskClient.Setup(() => new BroadcastingClient());

			Assert.AreSame(BackgroundTaskClient.Client, BackgroundTaskClient.Client);
		}
	}
}
