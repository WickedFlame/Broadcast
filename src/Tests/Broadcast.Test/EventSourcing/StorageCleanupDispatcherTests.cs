using System;
using System.Collections.Generic;
using System.Text;
using Broadcast.Configuration;
using Broadcast.EventSourcing;
using Broadcast.Processing;
using Broadcast.Storage;
using Moq;
using NUnit.Framework;

namespace Broadcast.Test.EventSourcing
{
    public class StorageCleanupDispatcherTests
    {
        [Test]
        public void StorageCleanupDispatcher_ctor()
        {
            Assert.DoesNotThrow(() => new StorageCleanupDispatcher(new Options()));
        }

        [Test]
        public void StorageCleanupDispatcher_Execute()
        {
            var store = new Mock<ITaskStore>();
            store.Setup(x => x.GetEnumerator()).Returns(() => new List<ITask>().GetEnumerator());

            var dispatcher = new StorageCleanupDispatcher(new Options());
            dispatcher.Execute(new ObserverContext(new DispatcherLock(), store.Object));

            store.Verify(x => x.GetEnumerator(), Times.Once);
        }

        [TestCase(TaskState.Processed)]
        [TestCase(TaskState.Faulted)]
        [TestCase(TaskState.Deleted)]
        public void StorageCleanupDispatcher_Execute_Clean(TaskState state)
        {
            var task = new BroadcastTask { State = state };
            task.StateChanges[task.State] = DateTime.Now.Subtract(TimeSpan.FromMinutes(10));
            var store = new Mock<ITaskStore>();
            store.Setup(x => x.GetEnumerator()).Returns(() => new List<ITask> { task }.GetEnumerator());

            var dispatcher = new StorageCleanupDispatcher(new Options { StorageLifetimeDuration = 5000 });
            dispatcher.Execute(new ObserverContext(new DispatcherLock(), store.Object));

            store.Verify(x => x.Storage(It.IsAny<Action<IStorage>>()), Times.Once);
        }

        [TestCase(TaskState.New)]
        [TestCase(TaskState.Processing)]
        [TestCase(TaskState.Dequeued)]
        [TestCase(TaskState.Queued)]
        public void StorageCleanupDispatcher_Execute_Clean_InvalidState(TaskState state)
        {
            var task = new BroadcastTask { State = state };
            task.StateChanges[task.State] = DateTime.Now.Subtract(TimeSpan.FromMinutes(10));
            var store = new Mock<ITaskStore>();
            store.Setup(x => x.GetEnumerator()).Returns(() => new List<ITask> { task }.GetEnumerator());

            var dispatcher = new StorageCleanupDispatcher(new Options { StorageLifetimeDuration = 5000 });
            dispatcher.Execute(new ObserverContext(new DispatcherLock(), store.Object));

            store.Verify(x => x.Storage(It.IsAny<Action<IStorage>>()), Times.Never);
        }

        [TestCase(TaskState.Processed)]
        [TestCase(TaskState.Faulted)]
        [TestCase(TaskState.Deleted)]
        public void StorageCleanupDispatcher_Execute_Clean_NotExpired(TaskState state)
        {
            var task = new BroadcastTask { State = state };
            var store = new Mock<ITaskStore>();
            store.Setup(x => x.GetEnumerator()).Returns(() => new List<ITask> { task }.GetEnumerator());

            var dispatcher = new StorageCleanupDispatcher(new Options { StorageLifetimeDuration = 5000 });
            dispatcher.Execute(new ObserverContext(new DispatcherLock(), store.Object));

            store.Verify(x => x.Storage(It.IsAny<Action<IStorage>>()), Times.Never);
        }
    }
}
