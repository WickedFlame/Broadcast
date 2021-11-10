using System;
using System.Collections.Generic;
using System.Text;
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
            Assert.DoesNotThrow(() => new StorageObserver(new Mock<ITaskStore>().Object));
        }

        [Test]
        public void StorageObserver_ctor_NoStore()
        {
            Assert.Throws<ArgumentNullException>(() => new StorageObserver(null));
        }
    }
}
