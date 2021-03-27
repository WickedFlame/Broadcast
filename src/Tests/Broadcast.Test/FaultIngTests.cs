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
            var broadcaster = new Broadcaster();

			//TODO: Refactore
			Action action = () => throw new NotImplementedException();
            broadcaster.Schedule(() => action.Invoke(), TimeSpan.FromSeconds(0.01));
            broadcaster.Schedule(() => System.Diagnostics.Debug.WriteLine("Test"), TimeSpan.FromSeconds(0.02));

            Thread.Sleep(TimeSpan.FromSeconds(1));

            var store = broadcaster.GetStore();
            Assert.IsTrue(store.Count(t => t.State == TaskState.Processed) == 2, $"Store Count is {store.Count()}, processed Count is {store.Count(t => t.State == TaskState.Processed)}");
        }

        [Test]
        public void FailingEvent()
        {
            var broadcaster = new Broadcaster();
            broadcaster.RegisterHandler(new GenericEventHandler());

            broadcaster.Schedule(() => new GenericEvent(), TimeSpan.FromSeconds(0.01));
            broadcaster.Schedule(() => new GenericEvent(), TimeSpan.FromSeconds(0.02));

            Thread.Sleep(TimeSpan.FromSeconds(1));

            Assert.IsTrue(broadcaster.GetStore().Count(t => t.State == TaskState.Processed) == 2);
        }

        [Test]
        [Ignore("Implementation not finished")]
        public void FailingTask_CheckState()
        {
            var broadcaster = new Broadcaster();
            //TODO: Refactore
			Action action = () => throw new NotImplementedException();
			broadcaster.Schedule(() => action.Invoke(), TimeSpan.FromSeconds(0.01));

            Thread.Sleep(TimeSpan.FromSeconds(1));

            //Assert.IsTrue(broadcaster.GetStore().All(t => t.State == TaskState.Faulted));
        }
		
        public class GenericEvent : INotification
        {
            public GenericEvent()
            {
            }

            //public Action Action { get; }
        }

        public class GenericEventHandler : INotificationTarget<GenericEvent>
        {
            public void Handle(GenericEvent notification)
            {
                //notification.Action.Invoke();
            }
        }

    }
}
