using Broadcast.EventSourcing;
using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Broadcast.Test
{
    [TestFixture]
    public class BroadcasterTests
    {
        [Test]
        public void BroadcasterDefaultProcessorTest()
        {
            TaskStoreFactory.StoreFactory = () => new TaskStore();

            IBroadcaster broadcaster = new Broadcaster();
            for (int i = 1; i <= 10; i++)
            {
                var value = i.ToString();
                broadcaster.Send(() => Trace.WriteLine(string.Format("Test default {0}", value)));
                Assert.IsTrue(broadcaster.Context.ProcessedTasks.Last().State == TaskState.Processed);
            }

            Assert.IsTrue(broadcaster.Context.ProcessedTasks.Count() == 10);
        }

        [Test]
        public void BroadcasterDefaultProcessorWithProcessorTest()
        {
            TaskStoreFactory.StoreFactory = () => new TaskStore();

            IBroadcaster broadcaster = new Broadcaster();
            for (int i = 1; i <= 10; i++)
            {
                var value = i.ToString();
                broadcaster.Send(() => Trace.WriteLine(string.Format("Test default {0}", value)));
                Assert.IsTrue(broadcaster.Context.ProcessedTasks.Last().State == TaskState.Processed);
            }

            Assert.IsTrue(broadcaster.Context.ProcessedTasks.Count() == 10);
        }

        [Test]
        public void BroadcasterDefaultProcessorWithModeParameterTest()
        {
            TaskStoreFactory.StoreFactory = () => new TaskStore();

            IBroadcaster broadcaster = new Broadcaster(ProcessorMode.Default);
            for (int i = 1; i <= 10; i++)
            {
                var value = i.ToString();
                broadcaster.Send(() => Trace.WriteLine(string.Format("Test default {0}", value)));
                Assert.IsTrue(broadcaster.Context.ProcessedTasks.Last().State == TaskState.Processed);
            }

            Assert.IsTrue(broadcaster.Context.ProcessedTasks.Count() == 10);
        }

        [Test]
        public void BroadcasterBackgroundProcessorTest()
        {
            TaskStoreFactory.StoreFactory = () => new TaskStore();

            IBroadcaster broadcaster = new Broadcaster(ProcessorMode.Background);
            for (int i = 1; i <= 10; i++)
            {
                var value = i.ToString();
                broadcaster.Send(() => Trace.WriteLine(string.Format("Test Background {0}", value)));
            }

            System.Threading.Thread.Sleep(System.TimeSpan.FromSeconds(1));
            Assert.IsTrue(broadcaster.Context.ProcessedTasks.Count() == 10);
        }

        [Test]
        public void BroadcasterBackgroundProcessorWithModeParameterTest()
        {
            TaskStoreFactory.StoreFactory = () => new TaskStore();

            IBroadcaster broadcaster = new Broadcaster(ProcessorMode.Background);
            for (int i = 1; i <= 10; i++)
            {
                var value = i.ToString();
                broadcaster.Send(() => Trace.WriteLine(string.Format("Test Background {0}", value)));
            }

            System.Threading.Thread.Sleep(System.TimeSpan.FromSeconds(1));
            Assert.IsTrue(broadcaster.Context.ProcessedTasks.Count() == 10);
        }



        [Test]
        public void BroadcasterAsyncProcessorTest()
        {
            TaskStoreFactory.StoreFactory = () => new TaskStore();

            IBroadcaster broadcaster = new Broadcaster(ProcessorMode.Async);
            for (int i = 1; i <= 10; i++)
            {
                var value = i.ToString();
                broadcaster.Send(() => Trace.WriteLine(string.Format("Test Async {0}", value)));
            }

            System.Threading.Thread.Sleep(System.TimeSpan.FromSeconds(1));
            Assert.IsTrue(broadcaster.Context.ProcessedTasks.Count() == 10);
        }

        [Test]
        public void BroadcasterAsyncProcessorWithModeParameterTest()
        {
            TaskStoreFactory.StoreFactory = () => new TaskStore();

            IBroadcaster broadcaster = new Broadcaster(ProcessorMode.Async);
            for (int i = 1; i <= 10; i++)
            {
                var value = i.ToString();
                broadcaster.Send(() => Trace.WriteLine(string.Format("Test Async {0}", value)));
            }

            System.Threading.Thread.Sleep(System.TimeSpan.FromSeconds(1));
            Assert.IsTrue(broadcaster.Context.ProcessedTasks.Count() == 10);
        }

        [Test]
        public void BroadcasterAsyncWithStoreTest()
        {
            IBroadcaster broadcaster = new Broadcaster(ProcessorMode.Async, new TaskStore());

            for (int i = 1; i <= 10; i++)
            {
                var value = i.ToString();
                broadcaster.Send(() => Trace.WriteLine(string.Format("Test Async {0}", value)));
            }

            System.Threading.Thread.Sleep(System.TimeSpan.FromSeconds(1));
            Assert.IsTrue(broadcaster.Context.ProcessedTasks.Count() == 10);
        }

        [Test]
        public void BroadcasterBackgroundWithStoreTest()
        {
            IBroadcaster broadcaster = new Broadcaster(ProcessorMode.Background, new TaskStore());

            for (int i = 1; i <= 10; i++)
            {
                var value = i;
                broadcaster.Send(() => Trace.WriteLine(string.Format("Test Background {0}", value)));
            }

            System.Threading.Thread.Sleep(System.TimeSpan.FromSeconds(1));
            Assert.IsTrue(broadcaster.Context.ProcessedTasks.Count() == 10);
        }

        [Test]
        public void BroadcasterAsyncTestInLoop()
        {
            IBroadcaster broadcaster = new Broadcaster(ProcessorMode.Background, new TaskStore());
            var taskValues = new List<int>();

            for (int i = 1; i <= 100; i++)
            {
                // i has to be passed to a local variable to ensure thread safety
                var value = i;
                broadcaster.Send(() => taskValues.Add(value));
            }

            System.Threading.Thread.Sleep(System.TimeSpan.FromSeconds(1));

            Assert.IsTrue(broadcaster.Context.ProcessedTasks.Count() == 100);

            int v = 1;
            foreach (var value in taskValues)
            {
                Assert.IsTrue(v == value);
                v++;
            }
        }

        [Test]
        public void BroadcasterAsyncTestInLoopFail()
        {
            IBroadcaster broadcaster = new Broadcaster(ProcessorMode.Background, new TaskStore());
            var taskValues = new List<int>();

            for (int i = 1; i <= 100; i++)
            {
                // i is not passed to a local variable and therefor is not threadsafe
                // http://blogs.msdn.com/b/ericlippert/archive/2009/11/12/closing-over-the-loop-variable-considered-harmful.aspx
                broadcaster.Send(() => taskValues.Add(i));
            }

            System.Threading.Thread.Sleep(System.TimeSpan.FromSeconds(1));

            Assert.IsTrue(broadcaster.Context.ProcessedTasks.Count() == 100);

            int v = 1;
            foreach (var value in taskValues)
            {
                // all values are the max because the procession took the last variable possible
                Assert.IsTrue(v != value);
                v++;
            }
        }
    }
}
