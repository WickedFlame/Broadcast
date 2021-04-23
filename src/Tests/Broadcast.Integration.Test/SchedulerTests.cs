using Broadcast.EventSourcing;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Broadcast.Scheduling;

namespace Broadcast.Integration.Test
{
	[SingleThreaded]
	[Explicit]
	[Category("Integration")]
	public class SchedulerTests
    {
        [OneTimeSetUp]
        public void Setup()
        {

        }

        [Test]
        public void Scheduler_ctor()
        {
	        Assert.DoesNotThrow(() => new Scheduler());
        }

		[Test]
        public void Scheduler_ExecutionCheck()
        {
            var scheduler = new Scheduler();
            scheduler.Enqueue(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(0.005));
            scheduler.Enqueue(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(0.005));
            scheduler.Enqueue(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(0.005));

            Task.Delay(1000).Wait();

            Assert.IsFalse(scheduler.ScheduledTasks().Any());
        }

        [Test]
        public void Scheduler_TaskCount()
        {
            int cnt = 0;
            var scheduler = new Scheduler();
            scheduler.Enqueue(() => { cnt++; }, TimeSpan.FromSeconds(0.01));
            scheduler.Enqueue(() => { cnt++; }, TimeSpan.FromSeconds(0.01));
            scheduler.Enqueue(() => { cnt++; }, TimeSpan.FromSeconds(0.01));
            //Assert.That(cnt == 0);

            Task.Delay(1000).Wait();

            Assert.That(cnt == 3, $"Effective Count: {cnt}");
        }
    }
}
