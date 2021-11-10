using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BDTest.Test;
using Broadcast.EventSourcing;
using Broadcast.Storage;
using FluentAssertions;
using NUnit.Framework;

namespace Broadcast.Integration.Test.Behaviour
{
    public class StopServerInMultisetup : BDTestBase
	{
        [Test]
		public void StopServerInMultisetup_StopServer()
		{
			WithContext<BdContext>(context =>
				Given(() => CreateStoreAndServers(context))
					.And(() => ScheduleTasks(context))
					.When(() => StopOneServer(context).Wait())
					.Then(() => HalfTasksAreProcessed(context))
					.BDTest()
			);
		}

		[Test]
		public void StopServerInMultisetup_RestartServer()
		{
			WithContext<BdContext>(context =>
				Given(() => CreateStoreAndServers(context))
					.And(() => ScheduleTasks(context))
					.And(() => StopOneServer(context).Wait())
					.When(() => StartNewServer(context).Wait())
					.Then(() => AllTasksAreProcessed(context))
					.BDTest()
			);
		}

		public void CreateStoreAndServers(BdContext context)
		{
			context.Store = new TaskStore(new InmemoryStorage());

			context.Context.Add("store1", new Broadcaster(context.Store));
			context.Context.Add("store2", new Broadcaster(context.Store));
		}

		private BdContext ScheduleTasks(BdContext context)
		{
			var client = new BroadcastingClient(context.Store);
			client.Schedule(() => TestTask.Add(), TimeSpan.FromSeconds(1));
			client.Schedule(() => TestTask.Add(), TimeSpan.FromSeconds(1));

			return context;
		}

		public BdContext StopOneServer(BdContext context)
		{
			((Broadcaster)context.Context["store2"]).Dispose();

			return context;
		}

		public BdContext StartNewServer(BdContext context)
        {
			context.Context.Add("store3", new Broadcaster(context.Store));
			return context;
        }

		public void HalfTasksAreProcessed(BdContext context)
        {
			context.Store.Count(t => t.State == TaskState.Processed).Should().Be(1);
			context.Store.Count(t => t.State == TaskState.New).Should().Be(1);
		}

		public void AllTasksAreProcessed(BdContext context)
		{
			context.Store.All(t => t.State == TaskState.Processed).Should().BeTrue();
		}

		public class TestTask
		{
			public static int Count { get; set; }

			public static void Add()
			{
				Count++;
			}
		}
	}
}
