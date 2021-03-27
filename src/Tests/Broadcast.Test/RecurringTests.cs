using Broadcast.EventSourcing;
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
    public class RecurringTests
    {
        [Test]
        public void Recurring_Simple()
        {
            var broadcaster = new Broadcaster();
            broadcaster.Recurring(() => System.Diagnostics.Trace.WriteLine("Recurring"), TimeSpan.FromSeconds(0.01));

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            Assert.IsTrue(broadcaster.GetStore().Count() > 10);
        }
    }
}
