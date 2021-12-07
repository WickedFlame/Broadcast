using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Broadcast.Configuration;
using Broadcast.EventSourcing;
using Moq;
using NUnit.Framework;

namespace Broadcast.Test.EventSourcing
{
    public  class StorageObserverTests
    {
        [Test]
        public void StorageObserver_ctor()
        {
            Assert.DoesNotThrow(() => new StorageObserver(new Mock<ITaskStore>().Object, new Options()));
        }

        [Test]
        public void StorageObserver_ctor_NoStore()
        {
            Assert.Throws<ArgumentNullException>(() => new StorageObserver(null, new Options()));
        }

        [Test]
        public void StorageObserver_StartNew()
        {
            var store = new Mock<ITaskStore>();
            store.Setup(x => x.GetEnumerator()).Returns(new List<ITask>().GetEnumerator());
            var task = new TestObserver();

            using (var observer = new StorageObserver(store.Object, new Options()))
            {
                observer.Start(task);
            }

            Assert.IsTrue(task.Executed);
        }

        public class TestObserver : IStorageObserver
        {
            public void Execute(ObserverContext context)
            {
                Executed = true;
            }

            public bool Executed { get; private set; }
        }
    }
}
