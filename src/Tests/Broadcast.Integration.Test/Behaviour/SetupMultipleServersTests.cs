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
    [SingleThreaded]
    [Category("Integration")]
	public class SetupMultipleServersTests : BDTestBase
    {
        [SetUp]
        public void Setup()
        {
            TestTask.Count = 0;
        }

        [Test]
        public void AddTask_SetupMultipleServers_ExecuteTasksOnce()
        {
            WithContext<BdContext>(context =>
                Given(() => CreateStore(context))
                    .And(() => AddTask(context).Wait())
                    .When(() => StartMultipleServers(context).Wait())
                    .Then(() => TaskIsProcessedOnce())
                    .BDTest()
            );
        }

        [Test]
        public void ScheduleTask_SetupMultipleServers_ExecuteTasksOnce()
        {
            WithContext<BdContext>(context =>
                Given(() => CreateStore(context))
                    .And(() => ScheduleTask(context).Wait())
                    .When(() => StartMultipleServers(context).Wait())
                    .Then(() => TaskIsProcessedOnce())
                    .BDTest()
            );
        }

        [Test]
        public void SetupMultipleServers_AddTask_ExecuteTasksOnce()
        {
            WithContext<BdContext>(context =>
                Given(() => CreateStore(context))
                    .And(() => StartMultipleServers(context).Wait())
                    .When(() => AddTask(context).Wait())
                    .Then(() => TaskIsProcessedOnce())
                    .BDTest()
            );
        }

        [Test]
        public void SetupMultipleServers_ScheduleTask_ExecuteTasksOnce()
        {
            WithContext<BdContext>(context =>
                Given(() => CreateStore(context))
                    .And(() => StartMultipleServers(context).Wait())
                    .When(() => ScheduleTask(context).Wait())
                    .Then(() => TaskIsProcessedOnce())
                    .BDTest()
            );
        }

        public void CreateStore(BdContext context)
		{
			context.Store = new TaskStore(new InmemoryStorage());
		}

		private BdContext ScheduleTask(BdContext context)
		{
			var client = new BroadcastingClient(context.Store);
			client.Schedule(() => TestTask.Add(), TimeSpan.FromMilliseconds(50));

			return context;
		}

        private BdContext AddTask(BdContext context)
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
