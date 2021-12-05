using System;
using System.Collections.Generic;
using System.Text;
using Broadcast.Processing;
using Moq;
using NUnit.Framework;

namespace Broadcast.Test.Processing
{
    public class ThreadCounterTests
    {
        [Test]
        public void ThreadCounter_ctor()
        {
            Assert.DoesNotThrow(() => new ThreadCounter(new Mock<IThreadList>().Object));
        }

        [Test]
        public void ThreadCounter_AddCount()
        {
            var threadList = new Mock<IThreadList>();
            new ThreadCounter(threadList.Object);

            threadList.Raise(exp => exp.ThreadCountHandler += null, new ThreadHandlerEventArgs { Name = "test1", Count = 2 });
            threadList.Raise(exp => exp.ThreadCountHandler += null, new ThreadHandlerEventArgs { Name = "test2", Count = 2 });

            Assert.AreEqual(4, ThreadCounter.GetTotalThreadCount());
        }
    }
}
