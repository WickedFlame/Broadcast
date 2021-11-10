using System;
using System.Collections.Generic;
using System.Text;
using BDTest.Test;
using Broadcast.EventSourcing;
using Broadcast.Storage;
using FluentAssertions;
using NUnit.Framework;

namespace Broadcast.Integration.Test.Behaviour
{
    public class SetupMultipleServersTests : BDTestBase
    {
        [Test]
        public void SetupMultipleServers_ExecuteTasksOnce()
        {
            WithContext<BdContext>(context =>
                Given(() => CreateStore(context))
                    .And(() => ScheduleTasks(context).Wait())
                    .When(() => StartMultipleServers(context).Wait())
                    .Then(() => TaskIsProcessedOnce())
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
			client.Send(() => TestTask.Add());

			return context;
		}

		public BdContext StartMultipleServers(BdContext context)
		{
			context.Context.Add("store1", new Broadcaster(context.Store));
			context.Context.Add("store2", new Broadcaster(context.Store));
			context.Context.Add("store3", new Broadcaster(context.Store));
			context.Context.Add("store4", new Broadcaster(context.Store));
			context.Context.Add("store5", new Broadcaster(context.Store));

			return context;
		}

		public void TaskIsProcessedOnce()
		{
			TestTask.Count.Should().Be(1);
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
