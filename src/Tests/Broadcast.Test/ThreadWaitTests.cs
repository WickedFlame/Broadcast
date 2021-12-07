using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Broadcast.Test
{
    public class ThreadWaitTests
    {
        [Test]
        public void ThreadWait_ctor()
        {
            Assert.DoesNotThrow(() => new ThreadWait());
        }

        [Test]
        public void ThreadWait_IsOpen()
        {
            Assert.IsTrue(new ThreadWait().IsOpen);
        }

        [Test]
        public async Task ThreadWait_WaitOne()
        {
            var sw = new Stopwatch();
            var threadWait = new ThreadWait();

            sw.Start();
            await threadWait.WaitOne(50);

            sw.Stop();
            Assert.Greater(sw.ElapsedMilliseconds, 50);
        }

        [Test]
        public void ThreadWait_Close()
        {
            var threadWait = new ThreadWait();
            threadWait.Close();

            Assert.IsFalse(threadWait.IsOpen);
        }

        [Test]
        public void ThreadWait_Dispose()
        {
            var threadWait = new ThreadWait();
            threadWait.Dispose();

            Assert.IsFalse(threadWait.IsOpen);
        }
    }
}
