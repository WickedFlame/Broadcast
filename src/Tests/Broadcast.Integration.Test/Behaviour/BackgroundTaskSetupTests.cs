using System;
using System.Collections.Generic;
using System.Text;
using BDTest.Attributes;
using BDTest.Test;
using Broadcast.EventSourcing;
using FluentAssertions;
using NUnit.Framework;

namespace Broadcast.Integration.Test.Behaviour
{
	[Story(AsA = "Developer", IWant = "I want to change the default BroadcastingClient", SoThat = "the Broadcaster uses another store")]
	public class BackgroundTaskSetupTests : BDTestBase
	{
		[TearDown]
		public void Teardown()
		{
			BackgroundTask.Setup(() => new BroadcastingClient());
		}

		[Test]
		[ScenarioText("The BroadcastingClient is set to a new store")]
		public void OverwriteTheClient()
		{
			WithContext<BdContext>(context => 
				Given(() => BackgroundTaskIsSetToDefault())
				.When(() => BackgroundTaskIsSetup())
				.Then(() => TaskStoreIsNotTheSameAsDefault())
				.BDTest()
			);
		}

		[Test]
		[ScenarioText("The BroadcastingClient is reset to the default store")]
		public void ResetTheClient()
		{
			WithContext<BdContext>(context =>
				Given(() => BackgroundTaskIsSetup())
					.When(() => BackgroundTaskIsSetToDefault())
					.Then(() => TaskStoreIsSameAsDefault())
					.BDTest()
			);
		}
		
		private void BackgroundTaskIsSetup()
		{
			BackgroundTask.Setup(() => new BroadcastingClient(new TaskStore()));
		}

		private void TaskStoreIsNotTheSameAsDefault()
		{
			BackgroundTask.Client.Store.Should().NotBeSameAs(TaskStore.Default);
		}

		private void BackgroundTaskIsSetToDefault()
		{
			BackgroundTask.Setup(() => new BroadcastingClient());
		}

		private void TaskStoreIsSameAsDefault()
		{
			BackgroundTask.Client.Store.Should().BeSameAs(TaskStore.Default);
		}
	}
}
