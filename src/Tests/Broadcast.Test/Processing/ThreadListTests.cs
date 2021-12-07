using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Broadcast.Processing;
using NUnit.Framework;

namespace Broadcast.Test.Processing
{
    public class ThreadListTests
    {
        [Test]
        public void ThreadList_ctor()
        {
            Assert.DoesNotThrow(() => new ThreadList());
        }

        [Test]
        public void ThreadList_Add()
        {
            var list = new ThreadList();
            list.Add(Task.Factory.StartNew(() => { Task.Delay(500).Wait(); }));

            Assert.AreEqual(1, list.Count());
        }

        [Test]
        public void ThreadList_Add_RemoveOnFinish()
        {
            var list = new ThreadList();
            list.Add(Task.Factory.StartNew(() => {  }));

            Task.Delay(10).Wait();

            Assert.AreEqual(0, list.Count());
        }

        [Test]
        public void ThreadList_Remove()
        {
            var list = new ThreadList();

            var task = Task.Factory.StartNew(() => { Task.Delay(50).Wait(); });
            list.Add(task);
            list.Remove(task);

            Assert.AreEqual(0, list.Count());
        }

        [Test]
        public void ThreadList_WaitAll()
        {
            var list = new ThreadList();

            var task = Task.Factory.StartNew(() => { Task.Delay(50).Wait(); });
            list.Add(task);
            list.WaitAll();

            Assert.AreEqual(0, list.Count());
        }
    }
}
