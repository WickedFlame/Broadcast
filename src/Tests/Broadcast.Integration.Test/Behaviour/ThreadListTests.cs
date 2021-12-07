using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BDTest.Attributes;
using BDTest.Test;
using Broadcast.Processing;
using FluentAssertions;
using NUnit.Framework;

namespace Broadcast.Integration.Test.Behaviour
{
    public class ThreadListTests : BDTestBase
    {
        [TearDown]
        public void Teardown()
        {
            Task.Delay(800).Wait();
        }

        [Test]
        [ScenarioText("Create a ThreadList to check the amount of threads")]
        public void CountSingleThreadInThreadList()
        {
            WithContext<ThreadListContext>(context =>
                Given(() => CreateThreadList(context))
                    .When(() => StartSingleThread(context))
                    .Then(() => ThreadCounterShowsCounts(1))
                    .BDTest()
            );
        }

        [Test]
        [ScenarioText("Create a ThreadList to check the amount of threads when multiple are added")]
        public void CountThreadsInThreadList()
        {
            WithContext<ThreadListContext>(context =>
                Given(() => CreateThreadList(context))
                    .When(() => StartThreeThreads(context))
                    .Then(() => ThreadCounterShowsCounts(3))
                    .BDTest()
            );
        }

        [Test]
        [ScenarioText("Create multiple ThreadLists to check the amount of threads are cumulated")]
        public void CountThreadsInMultipleThreadLists()
        {
            WithContext<ThreadListContext>(context =>
                Given(() => CreateThreadList(context))
                    .And(() => CreateThreadList(context))
                    .And(() => CreateThreadList(context))
                    .When(() => StartThreeThreadsOnThreeLists(context))
                    .Then(() => ThreadCounterShowsCounts(3))
                    .BDTest()
            );
        }

        public void CreateThreadList(ThreadListContext context)
        {
            context.ThreadLists.Add(new ThreadList());
        }

        private void StartSingleThread(ThreadListContext context)
        {
            context.ThreadLists[0].Add(Task.Factory.StartNew(() => { Task.Delay(500).Wait(); }));
        }

        private void StartThreeThreads(ThreadListContext context)
        {
            context.ThreadLists[0].Add(Task.Factory.StartNew(() => { Task.Delay(500).Wait(); }));
            context.ThreadLists[0].Add(Task.Factory.StartNew(() => { Task.Delay(500).Wait(); }));
            context.ThreadLists[0].Add(Task.Factory.StartNew(() => { Task.Delay(500).Wait(); }));
        }

        private void StartThreeThreadsOnThreeLists(ThreadListContext context)
        {
            context.ThreadLists[0].Add(Task.Factory.StartNew(() => { Task.Delay(500).Wait(); }));
            context.ThreadLists[1].Add(Task.Factory.StartNew(() => { Task.Delay(500).Wait(); }));
            context.ThreadLists[2].Add(Task.Factory.StartNew(() => { Task.Delay(500).Wait(); }));
        }

        private void ThreadCounterShowsCounts(int count)
        {
            ThreadCounter.GetTotalThreadCount().Should().Be(count);
        }
    }

    public class ThreadListContext
    {
        public List<ThreadList> ThreadLists { get; } = new List<ThreadList>();
    }
}
