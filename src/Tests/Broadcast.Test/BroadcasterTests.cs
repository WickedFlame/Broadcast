using Broadcast.EventSourcing;
using NUnit.Framework;
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
                var value = i.ToString();
                broadcaster.Send(() => Trace.WriteLine(string.Format("Test Background {0}", value)));
            }

            System.Threading.Thread.Sleep(System.TimeSpan.FromSeconds(1));
            Assert.IsTrue(broadcaster.Context.ProcessedTasks.Count() == 10);
        }
    }
}
