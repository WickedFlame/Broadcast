using Broadcast.EventSourcing;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Broadcast.Test
{
    [TestFixture]
    [SingleThreaded]
    public class SchedulerTests
    {
        [OneTimeSetUp]
        public void Setup()
        {

        }

        [Test]
        public void Broadcast_Scheduler_ThreadsCount()
        {
            var activeSchedulers = Scheduler.SchedulerCount;

            var scheduler1 = new Scheduler();
            var scheduler2 = new Scheduler();
            var scheduler3 = new Scheduler();



            Assert.IsTrue(Scheduler.SchedulerCount == activeSchedulers + 3, $"3 {Scheduler.SchedulerCount}");

            scheduler1.Dispose();
            Assert.AreEqual(Scheduler.SchedulerCount, activeSchedulers + 2, "2");

            scheduler2.Dispose();
            Assert.IsTrue(Scheduler.SchedulerCount == activeSchedulers + 1, "1");

            scheduler3.Dispose();
            Assert.IsTrue(Scheduler.SchedulerCount == activeSchedulers, "0");
        }
    }
}
