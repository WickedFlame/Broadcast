using System;
using System.Collections.Generic;
using System.Text;
using Broadcast.EventSourcing;
using Broadcast.Processing;
using Broadcast.Storage;
using Moq;
using NUnit.Framework;

namespace Broadcast.Test.EventSourcing
{
    public class ReschedulingDispatcherTests
    {
        [Test]
        public void ReschedulingDispatcher_ctor()
        {
            Assert.DoesNotThrow(() => new ReschedulingDispatcher());
        }

        [Test]
        public void ReschedulingDispatcher_Execute()
        {
            var store = new Mock<ITaskStore>();
            store.Setup(x => x.Storage<IEnumerable<string>>(It.IsAny<Func<IStorage, IEnumerable<string>>>())).Returns(new[]
            {
                "id1",
                "id2"
            });
            store.Setup(x => x.Storage<BroadcastTask>(It.IsAny<Func<IStorage, BroadcastTask>>())).Returns(() => new BroadcastTask { State = TaskState.New });

            var dispatcherLock = new DispatcherLock();
            var context = new ObserverContext(dispatcherLock, store.Object);

            var dispatcher = new ReschedulingDispatcher();
            dispatcher.Execute(context);

            store.Verify(x => x.DispatchTasks(), Times.Once);
        }

        [Test]
        public void ReschedulingDispatcher_Execute_TimePassed()
        {
            var store = new Mock<ITaskStore>();
            store.Setup(x => x.Storage<IEnumerable<string>>(It.IsAny<Func<IStorage, IEnumerable<string>>>())).Returns(new[]
            {
                "id1"
            });
            store.Setup(x => x.Storage<BroadcastTask>(It.IsAny<Func<IStorage, BroadcastTask>>())).Returns(() => new BroadcastTask
            {
                State = TaskState.New,
                CreatedAt = DateTime.Now.Subtract(TimeSpan.FromMinutes(1)),
                Time = TimeSpan.FromSeconds(1)
            });

            var dispatcherLock = new DispatcherLock();
            var context = new ObserverContext(dispatcherLock, store.Object);

            var dispatcher = new ReschedulingDispatcher();
            dispatcher.Execute(context);

            store.Verify(x => x.DispatchTasks(), Times.Once);
        }

        [Test]
        public void ReschedulingDispatcher_Execute_TimeNotPassed()
        {
            var store = new Mock<ITaskStore>();
            store.Setup(x => x.Storage<IEnumerable<string>>(It.IsAny<Func<IStorage, IEnumerable<string>>>())).Returns(new[]
            {
                "id1"
            });
            store.Setup(x => x.Storage<BroadcastTask>(It.IsAny<Func<IStorage, BroadcastTask>>())).Returns(() => new BroadcastTask
            {
                State = TaskState.New,
                CreatedAt = DateTime.Now,
                Time = TimeSpan.FromMinutes(1)
            });

            var dispatcherLock = new DispatcherLock();
            var context = new ObserverContext(dispatcherLock, store.Object);

            var dispatcher = new ReschedulingDispatcher();
            dispatcher.Execute(context);

            store.Verify(x => x.DispatchTasks(), Times.Never);
        }

        [Test]
        public void ReschedulingDispatcher_Execute_Locked()
        {
            var store = new Mock<ITaskStore>();
            var dispatcherLock = new DispatcherLock();
            dispatcherLock.Lock();

            var context = new ObserverContext(dispatcherLock, store.Object);

            var dispatcher = new ReschedulingDispatcher();
            dispatcher.Execute(context);

            store.Verify(x => x.DispatchTasks(), Times.Never);
        }
    }
}
