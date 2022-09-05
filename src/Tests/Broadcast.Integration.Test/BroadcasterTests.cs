using System.Threading.Tasks;
using Broadcast.EventSourcing;
using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System;
using System.Linq.Expressions;
using FluentAssertions;

namespace Broadcast.Integration.Test
{
    [TestFixture]
    [Category("Integration")]
	public class BroadcasterTests
    {
	    [Test]
	    public void Broadcaster_Process()
	    {
		    var caller = new TestCaller();
		    var task = Broadcast.Composition.TaskFactory.CreateTask(() => caller.Call());
			
		    var broadcaster = new Broadcaster(new TaskStore());
		    broadcaster.Process(task);
		    broadcaster.WaitAll();

		    Task.Delay(1000).Wait();

		    Assert.IsTrue(TestCaller.Called);
	    }

	    [Test]
	    public void Broadcaster_Process_Func()
	    {
			var caller = new TestCaller();
			var task = Broadcast.Composition.TaskFactory.CreateTask(() => caller.Call());

		    var broadcaster = new Broadcaster(new TaskStore());
		    broadcaster.Process(task);
		    broadcaster.WaitAll();

		    Task.Delay(1000).Wait();

			Assert.IsTrue(TestCaller.Called);
	    }

	    public class TestCaller
	    {
		    public TestCaller()
		    {
				Called = false;
		    }

			public static bool Called { get; set; } = false;

			public void Call()
			{
				Called = true;
			}
	    }








		[Test]
        public void BroadcasterDefaultProcessorTest_Prallel()
        {
            IBroadcaster broadcaster = new Broadcaster(new TaskStore());
            for (int i = 1; i <= 10; i++)
            {
                var value = i.ToString();
                broadcaster.Send(() => Trace.WriteLine(string.Format("Test default {0}", value)));
                //Assert.IsTrue(broadcaster.GetProcessedTasks().Last().State == TaskState.Processed);
            }

            //TODO: has to work without sleep!
            Task.Delay(1000).Wait();

            broadcaster.WaitAll();
            
			Assert.AreEqual(10, broadcaster.GetProcessedTasks().Count(), $"Expected: {10}{Environment.NewLine}  Actual: {broadcaster.GetProcessedTasks().Count()}{Environment.NewLine}  Total: {broadcaster.Store.Count()}{Environment.NewLine}  States: {string.Join(',', broadcaster.Store.Select(s => s.State.ToString()))}{Environment.NewLine}  Queue: {string.Join(',', broadcaster.Store.GetEnqueuedTasks())}{Environment.NewLine}  Dequeue: {string.Join(',', broadcaster.Store.GetFetchedTasks())}");
        }

        [Test]
        public void BroadcasterDefaultProcessorWithProcessorTest()
        {
            IBroadcaster broadcaster = new Broadcaster(new TaskStore());
            for (int i = 1; i <= 10; i++)
            {
                var value = i.ToString();
                broadcaster.Send(() => Trace.WriteLine(string.Format("Test default {0}", value)));
                //Assert.IsTrue(broadcaster.GetProcessedTasks().Last().State == TaskState.Processed);
            }

            //TODO: has to work without sleep!
			Task.Delay(1000).Wait();

			broadcaster.WaitAll();

            Assert.AreEqual(10, broadcaster.GetProcessedTasks().Count(), $"Expected: {10}{Environment.NewLine}  Actual: {broadcaster.GetProcessedTasks().Count()}{Environment.NewLine}  Total: {broadcaster.Store.Count()}{Environment.NewLine}  States: {string.Join(',', broadcaster.Store.Select(s => s.State.ToString()))}{Environment.NewLine}  Queue: {string.Join(',', broadcaster.Store.GetEnqueuedTasks())}{Environment.NewLine}  Dequeue: {string.Join(',', broadcaster.Store.GetFetchedTasks())}");
        }

        [Test]
        public void BroadcasterDefaultProcessorWithModeParameterTest()
        {
            IBroadcaster broadcaster = new Broadcaster(new TaskStore());
            for (int i = 1; i <= 10; i++)
            {
                var value = i.ToString();
                broadcaster.Send(() => Trace.WriteLine(string.Format("Test default {0}", value)));
                //Assert.IsTrue(broadcaster.GetProcessedTasks().Last().State == TaskState.Processed);
            }

            broadcaster.WaitAll();

			Assert.AreEqual(10, broadcaster.GetProcessedTasks().Count(), $"Expected: {10}{Environment.NewLine}  Actual: {broadcaster.GetProcessedTasks().Count()}{Environment.NewLine}  Total: {broadcaster.Store.Count()}{Environment.NewLine}  States: {string.Join(',', broadcaster.Store.Select(s => s.State.ToString()))}{Environment.NewLine}  Queue: {string.Join(',', broadcaster.Store.GetEnqueuedTasks())}{Environment.NewLine}  Dequeue: {string.Join(',', broadcaster.Store.GetFetchedTasks())}");
        }

        [Test]
        public void BroadcasterBackgroundProcessorTest()
        {
            IBroadcaster broadcaster = new Broadcaster(new TaskStore());
            for (int i = 1; i <= 10; i++)
            {
                var value = i.ToString();
                broadcaster.Send(() => Trace.WriteLine(string.Format("Test Background {0}", value)));
            }

            broadcaster.WaitAll();
            Task.Delay(2000).Wait();
			Assert.AreEqual(10, broadcaster.GetProcessedTasks().Count(), $"Expected: {10}{Environment.NewLine}  Actual: {broadcaster.GetProcessedTasks().Count()}{Environment.NewLine}  Total: {broadcaster.Store.Count()}{Environment.NewLine}  States: {string.Join(',', broadcaster.Store.Select(s => s.State.ToString()))}{Environment.NewLine}  Queue: {string.Join(',', broadcaster.Store.GetEnqueuedTasks())}{Environment.NewLine}  Dequeue: {string.Join(',', broadcaster.Store.GetFetchedTasks())}");
        }

        [Test]
        public void BroadcasterBackgroundProcessorWithModeParameterTest()
        {
            IBroadcaster broadcaster = new Broadcaster(new TaskStore());
            for (int i = 1; i <= 10; i++)
            {
                var value = i.ToString();
                broadcaster.Send(() => Trace.WriteLine(string.Format("Test Background {0}", value)));
            }

            broadcaster.WaitAll();

			Assert.AreEqual(10, broadcaster.GetProcessedTasks().Count(), $"Expected: {10}{Environment.NewLine}  Actual: {broadcaster.GetProcessedTasks().Count()}{Environment.NewLine}  Total: {broadcaster.Store.Count()}{Environment.NewLine}  States: {string.Join(',', broadcaster.Store.Select(s => s.State.ToString()))}{Environment.NewLine}  Queue: {string.Join(',', broadcaster.Store.GetEnqueuedTasks())}{Environment.NewLine}  Dequeue: {string.Join(',', broadcaster.Store.GetFetchedTasks())}");
        }



        [Test]
        public void BroadcasterAsyncProcessorTest()
        {
            IBroadcaster broadcaster = new Broadcaster(new TaskStore());

			for (int i = 1; i <= 10; i++)
			{
				var value = i.ToString();
				broadcaster.Send(() => Trace.WriteLine(string.Format("Test Async {0}", value)));
			}

			broadcaster.WaitAll();
			Assert.AreEqual(10, broadcaster.GetProcessedTasks().Count(), $"Expected: {10}{Environment.NewLine}  Actual: {broadcaster.GetProcessedTasks().Count()}{Environment.NewLine}  Total: {broadcaster.Store.Count()}{Environment.NewLine}  States: {string.Join(',', broadcaster.Store.Select(s => s.State.ToString()))}{Environment.NewLine}  Queue: {string.Join(',', broadcaster.Store.GetEnqueuedTasks())}{Environment.NewLine}  Dequeue: {string.Join(',', broadcaster.Store.GetFetchedTasks())}");
        }

        [Test]
        public void BroadcasterAsyncProcessorWithModeParameterTest()
        {
            IBroadcaster broadcaster = new Broadcaster(new TaskStore());
			for (int i = 1; i <= 10; i++)
            {
                var value = i.ToString();
                broadcaster.Send(() => Trace.WriteLine(string.Format("Test Async {0}", value)));
            }
			
            broadcaster.WaitAll();

            Assert.AreEqual(10, broadcaster.GetProcessedTasks().Count(), $"Expected: {10}{Environment.NewLine}  Actual: {broadcaster.GetProcessedTasks().Count()}{Environment.NewLine}  Total: {broadcaster.Store.Count()}{Environment.NewLine}  States: {string.Join(',', broadcaster.Store.Select(s => s.State.ToString()))}{Environment.NewLine}  Queue: {string.Join(',', broadcaster.Store.GetEnqueuedTasks())}{Environment.NewLine}  Dequeue: {string.Join(',', broadcaster.Store.GetFetchedTasks())}");
        }

        [Test]
        public void BroadcasterAsyncWithStoreTest()
        {
	        IBroadcaster broadcaster = new Broadcaster(new TaskStore());
            for (int i = 1; i <= 10; i++)
            {
                var value = i.ToString();
                broadcaster.Send(() => Trace.WriteLine(string.Format("Test Async {0}", value)));
            }

            broadcaster.WaitAll();
			Assert.AreEqual(10, broadcaster.GetProcessedTasks().Count(), $"Expected: {10}{Environment.NewLine}  Actual: {broadcaster.GetProcessedTasks().Count()}{Environment.NewLine}  Total: {broadcaster.Store.Count()}{Environment.NewLine}  States: {string.Join(',', broadcaster.Store.Select(s => s.State.ToString()))}{Environment.NewLine}  Queue: {string.Join(',', broadcaster.Store.GetEnqueuedTasks())}{Environment.NewLine}  Dequeue: {string.Join(',', broadcaster.Store.GetFetchedTasks())}");
        }

        [Test]
        public void BroadcasterBackgroundWithStoreTest()
        {
            IBroadcaster broadcaster = new Broadcaster(new TaskStore());

			for (int i = 1; i <= 10; i++)
            {
                var value = i;
                broadcaster.Send(() => Trace.WriteLine(string.Format("Test Background {0}", value)));
            }

			broadcaster.WaitAll();
			Assert.AreEqual(10, broadcaster.GetProcessedTasks().Count(), $"Expected: {10}{Environment.NewLine}  Actual: {broadcaster.GetProcessedTasks().Count()}{Environment.NewLine}  Total: {broadcaster.Store.Count()}{Environment.NewLine}  States: {string.Join(',', broadcaster.Store.Select(s => s.State.ToString()))}{Environment.NewLine}  Queue: {string.Join(',', broadcaster.Store.GetEnqueuedTasks())}{Environment.NewLine}  Dequeue: {string.Join(',', broadcaster.Store.GetFetchedTasks())}");
        }

        [Test]
        public void BroadcasterAsyncTestInLoop()
        {
            IBroadcaster broadcaster = new Broadcaster(new TaskStore()); 
            var taskValues = new List<int>();

            for (int i = 1; i <= 100; i++)
            {
                // i has to be passed to a local variable to ensure thread safety
                var value = i;
                broadcaster.Send(() => taskValues.Add(value));
            }

            broadcaster.WaitAll();

            Assert.AreEqual(100, broadcaster.GetProcessedTasks().Count(), $"Expected: {100}{Environment.NewLine}  Actual: {broadcaster.GetProcessedTasks().Count()}{Environment.NewLine}  Total: {broadcaster.Store.Count()}{Environment.NewLine}  States: {string.Join(',', broadcaster.Store.Select(s => s.State.ToString()))}{Environment.NewLine}  Queue: {string.Join(',', broadcaster.Store.GetEnqueuedTasks())}{Environment.NewLine}  Dequeue: {string.Join(',', broadcaster.Store.GetFetchedTasks())}");

            int v = 1;
            foreach (var value in taskValues)
            {
                Assert.IsTrue(v == value);
                v++;
            }
		}
        
        [Test]
        [Ignore("Fails when NUnit runs second time")]
        public void BroadcasterAsyncTestInLoopFail()
        {
            IBroadcaster broadcaster = new Broadcaster(new TaskStore());
            broadcaster.Send(() => Trace.WriteLine("Just for warmup"));

            var taskValues = new List<int>();
            for (int i = 1; i <= 100; i++)
            {
                // i is not passed to a local variable and therefor is not threadsafe
                // http://blogs.msdn.com/b/ericlippert/archive/2009/11/12/closing-over-the-loop-variable-considered-harmful.aspx
                broadcaster.Send(() => taskValues.Add(i));
            }

            //Thread.Sleep(TimeSpan.FromSeconds(3));
            for (int i = 1; i <= 100000; i++)
            {
                // just spend some time
            }

            Assert.IsTrue(broadcaster.GetProcessedTasks().Any(), "Broadcaseter ProcessedTasks: " + broadcaster.GetProcessedTasks().Count());

            //int v = 1;
            //foreach (var value in taskValues.ToList())
            //{
            //    // all values are the max because the procession took the last variable possible
            //    Assert.IsTrue(v != value);
            //    v++;
            //}
        }
		
        [Test]
        public void Broadcaster_Schedule()
        {
            var broadcaster = new Broadcaster(new TaskStore());
            var id = broadcaster.Schedule(() => Console.WriteLine("test"), TimeSpan.FromMinutes(1));

            id.Should().NotBeNull();
        }

        [Test]
        public void Broadcaster_Recurring()
        {
            var store = new TaskStore();
            var broadcaster = new Broadcaster(store);
            broadcaster.Recurring(() => Console.WriteLine("test"), TimeSpan.FromMinutes(1));

            store.Should().HaveCount(1);
        }

        [Test]
        public void Broadcaster_Scheduler_Simple()
        {
            var broadcaster = new Broadcaster(new TaskStore());
            broadcaster.Schedule(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(0.005));
            broadcaster.Schedule(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(0.005));
            broadcaster.Schedule(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(0.005));

            Task.Delay(1000).Wait();

            Assert.IsTrue(broadcaster.Store.Count(t => t.State == TaskState.Processed) == 3);
        }

        [Test]
        public void Broadcaster_Api_Publish()
        {
            var broadcaster = new Broadcaster(new TaskStore());
            broadcaster.Subscribe<Request, RequestHandler>();

            broadcaster.Publish(new Request(1));

            broadcaster.WaitAll();

            Assert.AreEqual(1, broadcaster.GetProcessedTasks().Count(), $"Expected: {1}{Environment.NewLine}  Actual: {broadcaster.GetProcessedTasks().Count()}{Environment.NewLine}  Total: {broadcaster.Store.Count()}{Environment.NewLine}  States: {string.Join(',', broadcaster.Store.Select(s => s.State.ToString()))}{Environment.NewLine}  Queue: {string.Join(',', broadcaster.Store.GetEnqueuedTasks())}{Environment.NewLine}  Dequeue: {string.Join(',', broadcaster.Store.GetFetchedTasks())}");
        }

        [Test]
        public void Broadcaster_Api_Publish_HandlerInstance()
        {
            var broadcaster = new Broadcaster(new TaskStore());
            broadcaster.Subscribe<Request, RequestHandler>();

            broadcaster.Publish(new Request(1));

            broadcaster.WaitAll();

            RequestHandler.StaticId.Should().Be(1);
        }

        public class Request
        {
            public Request(int id)
            {
                ID = id;
            }

            public int ID { get; set; }
        }

        class RequestHandler : IEventHandler<Request>
        {
            public static int StaticId { get; set; }

            public int Id { get; private set; }

            public void Handle(Request request)
            {
                Id = request.ID;
                StaticId = Id;
            }

            public void Handle(object request)
            {
                Handle(request as Request);
            }
        }

        private class AsyncReturner
        {
            public int GetValue(int index)
            {
                int i = index;
                Task.Delay(1000).Wait();
                return i;
            }
        }
    }
}
