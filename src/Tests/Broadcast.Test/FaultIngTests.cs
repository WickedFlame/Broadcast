using Broadcast.EventSourcing;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Broadcast.Test
{
    [TestFixture]
    public class FaultIngTests
    {
        [Test]
        public void FailingTask()
        {
            var broadcaster = new Broadcaster(new TaskStore());

			//TODO: Refactore
			Action action = () => throw new NotImplementedException();
            broadcaster.Schedule(() => action.Invoke(), TimeSpan.FromSeconds(0.01));
            broadcaster.Schedule(() => System.Diagnostics.Trace.WriteLine("Test"), TimeSpan.FromSeconds(0.02));

            Task.Delay(1000).Wait();

			broadcaster.WaitAll();

            var store = broadcaster.Store;
            Assert.IsTrue(store.Count(t => t.State == TaskState.Processed) == 1, $"Store Count is {store.Count()}, processed Count is {store.Count(t => t.State == TaskState.Processed)}{Environment.NewLine}  States: {string.Join(',', broadcaster.Store.Select(s => s.State.ToString()))}");
            Assert.IsTrue(store.Count(t => t.State == TaskState.Faulted) == 1, $"Store Count is {store.Count()}, processed Count is {store.Count(t => t.State == TaskState.Faulted)}{Environment.NewLine}  States: {string.Join(',', broadcaster.Store.Select(s => s.State.ToString()))}");
		}

    }
}
