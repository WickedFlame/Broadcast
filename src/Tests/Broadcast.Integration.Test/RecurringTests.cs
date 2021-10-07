using Broadcast.EventSourcing;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Broadcast.Integration.Test
{
    [TestFixture]
    [Category("Integration")]
	public class RecurringTests
    {
        [Test]
        public void Recurring_Simple()
        {
            var broadcaster = new Broadcaster(new TaskStore());
            broadcaster.Recurring(() => System.Diagnostics.Trace.WriteLine("Recurring"), TimeSpan.FromSeconds(0.01));

            Task.Delay(2000).Wait();

            Assert.GreaterOrEqual(broadcaster.Store.Count(), 10);
        }
    }
}
