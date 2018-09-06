﻿using Broadcast.EventSourcing;
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
    public class FaultIngTests
    {
        [Test]
        public void FailingTask()
        {
            TaskStoreFactory.StoreFactory = () => new TaskStore();
            var store = TaskStoreFactory.GetStore();

            var broadcaster = new Broadcaster();
            broadcaster.Schedule(() => throw new NotImplementedException(), TimeSpan.FromSeconds(0.01));
            broadcaster.Schedule(() => System.Diagnostics.Debug.WriteLine("Test"), TimeSpan.FromSeconds(0.02));

            Thread.Sleep(TimeSpan.FromSeconds(1));

            Assert.IsTrue(store.Count(t => t.State == TaskState.Processed) == 2);
        }

        [Test]
        [Ignore("Implementation not finished")]
        public void FailingTask_CheckState()
        {
            TaskStoreFactory.StoreFactory = () => new TaskStore();
            var store = TaskStoreFactory.GetStore();

            var broadcaster = new Broadcaster();
            broadcaster.Schedule(() => throw new NotImplementedException(), TimeSpan.FromSeconds(0.01));

            Thread.Sleep(TimeSpan.FromSeconds(1));

            //Assert.IsTrue(store.All(t => t.State == TaskState.Faulted));
        }
    }
}
