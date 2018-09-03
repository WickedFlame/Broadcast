using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Broadcast.Test
{
    [TestFixture]
    public class SchedulerTests
    {
        [Test]
        public void Broadcast_Scheduler_ThreadsCount()
        {
            var scheduler1 = new Scheduler();
            var scheduler2 = new Scheduler();
            var scheduler3 = new Scheduler();

            Assert.IsTrue(Scheduler.SchedulerCount > 2);

            scheduler1.Dispose();
            Assert.IsTrue(Scheduler.SchedulerCount > 1);

            scheduler2.Dispose();
            Assert.IsTrue(Scheduler.SchedulerCount > 0);
        }

        [Test]
        public void Broadcast_Scheduler_TaskCount()
        {
            int cnt = 0;
            var scheduler = new Scheduler();
            scheduler.Enqueue(() => cnt++, TimeSpan.FromMilliseconds(1));
            scheduler.Enqueue(() => cnt++, TimeSpan.FromMilliseconds(1));
            scheduler.Enqueue(() => cnt++, TimeSpan.FromMilliseconds(1));
            Assert.That(cnt == 0);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            Assert.That(cnt == 3);
        }
    }
}
