using Broadcast.EventSourcing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Linq;

namespace Broadcast.Test
{
    [TestClass]
    public class BroadcasterTests
    {
        [TestMethod]
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

        [TestMethod]
        public void BroadcasterDefaultProcessorWithProcessorTest()
        {
            TaskStoreFactory.StoreFactory = () => new TaskStore();

            IBroadcaster broadcaster = new Broadcaster(new ProcessorContext());
            for (int i = 1; i <= 10; i++)
            {
                var value = i.ToString();
                broadcaster.Send(() => Trace.WriteLine(string.Format("Test default {0}", value)));
                Assert.IsTrue(broadcaster.Context.ProcessedTasks.Last().State == TaskState.Processed);
            }

            Assert.IsTrue(broadcaster.Context.ProcessedTasks.Count() == 10);
        }

        [TestMethod]
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



        [TestMethod]
        public void BroadcasterBackgroundProcessorTest()
        {
            TaskStoreFactory.StoreFactory = () => new TaskStore();

            IBroadcaster broadcaster = new Broadcaster(new ProcessorContext(ProcessorMode.Background));
            for (int i = 1; i <= 10; i++)
            {
                var value = i.ToString();
                broadcaster.Send(() => Trace.WriteLine(string.Format("Test Background {0}", value)));
            }

            System.Threading.Thread.Sleep(System.TimeSpan.FromSeconds(1));
            Assert.IsTrue(broadcaster.Context.ProcessedTasks.Count() == 10);
        }

        [TestMethod]
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



        [TestMethod]
        public void BroadcasterAsyncProcessorTest()
        {
            TaskStoreFactory.StoreFactory = () => new TaskStore();

            IBroadcaster broadcaster = new Broadcaster(new ProcessorContext(ProcessorMode.Async));
            for (int i = 1; i <= 10; i++)
            {
                var value = i.ToString();
                broadcaster.Send(() => Trace.WriteLine(string.Format("Test Async {0}", value)));
            }

            System.Threading.Thread.Sleep(System.TimeSpan.FromSeconds(1));
            Assert.IsTrue(broadcaster.Context.ProcessedTasks.Count() == 10);
        }

        [TestMethod]
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
		
		[TestMethod]
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

        [TestMethod]
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
