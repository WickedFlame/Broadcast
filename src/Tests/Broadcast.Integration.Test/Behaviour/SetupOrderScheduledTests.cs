using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BDTest.Attributes;
using BDTest.Test;
using Broadcast.EventSourcing;
using Broadcast.Storage;
using FluentAssertions;
using NUnit.Framework;

namespace Broadcast.Integration.Test.Behaviour
{
	[SingleThreaded]
	[Category("Integration")]
	[Story(AsA = "Developer", IWant = "To start a server and schedule tasks", SoThat = "all tasks are processed independently of the Broadcaster starting order.")]
	public class SetupOrderScheduledTests : BDTestBase
	{
		[Test]
		[Ignore("Adding server after tasks does not work yet")]
		[ScenarioText("Start a server after the tasks are scheduled to the storage")]
		public void AddServerAfterTaskSchedule()
		{
			WithContext<BdContext>(context =>
				Given(() => CreateStore(context))
					.And(() => ScheduleTasks(context).Wait())
					.When(() => StartServer(context).Wait())
					.Then(() => AllTasksAreProcessed(context))
					.BDTest()
			);
		}

		[Test]
		[ScenarioText("Start the server before scheduling the tasks")]
		public void ServerThenAddTaskSchedule()
		{
			WithContext<BdContext>(context =>
				Given(() => CreateStore(context))
					.And(() => StartServer(context).Wait())
					.When(() => ScheduleTasks(context).Wait())
					.Then(() => AllTasksAreProcessed(context))
					.BDTest()
			);
		}

		public void CreateStore(BdContext context)
		{
			context.Store = new TaskStore(new InmemoryStorage());
		}

		private BdContext ScheduleTasks(BdContext context)
		{
			var client = new BroadcastingClient(context.Store);
			for (var i = 0; i < 10; i++)
			{
				client.Schedule(() => System.Diagnostics.Trace.WriteLine($"Executed task {i + 1}"), TimeSpan.FromSeconds(1));
			}

			return context;
		}

		public BdContext StartServer(BdContext context)
		{
			context.Server = new Broadcaster(context.Store);

			return context;
		}

		public void AllTasksAreProcessed(BdContext context)
		{
			context.Store.All(t => t.State == TaskState.Processed).Should().BeTrue();
		}
	}
}
