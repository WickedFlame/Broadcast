﻿using Broadcast.EventSourcing;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Broadcast.Scheduling;
using Moq;

namespace Broadcast.Test.Scheduling
{
    [TestFixture]
    [SingleThreaded]
    public class SchedulerTests
    {
        [OneTimeSetUp]
        public void Setup()
        {

        }

        [Test]
        public void Scheduler_ctor()
        {
	        Assert.DoesNotThrow(() => new Scheduler());
        }

        [Test]
        public void Scheduler_ctor_Queue()
        {
	        Assert.DoesNotThrow(() => new Scheduler(new ScheduleQueue()));
        }

        [Test]
        public void Scheduler_ctor_Null_Queue()
        {
	        Assert.Throws<ArgumentNullException>(() => new Scheduler(null));
        }

        [Test]
        public void Scheduler_Enqueue()
        {
	        var queue = new Mock<IScheduleQueue>();

	        using var scheduler = new Scheduler(queue.Object);
	        scheduler.Enqueue("id", id => { }, TimeSpan.Zero);

	        queue.Verify(exp => exp.Enqueue(It.IsAny<SchedulerTask>()), Times.Once);
        }

        [Test]
        public void Scheduler_ScheduledTasks()
        {
	        using var scheduler = new Scheduler();
	        scheduler.Enqueue("id1", id => { }, TimeSpan.FromMinutes(10));
	        scheduler.Enqueue("id2", id => { }, TimeSpan.FromMinutes(10));

	        Assert.AreEqual(2, scheduler.ScheduledTasks().Count());
        }

        [Test]
        public void Scheduler_ScheduledTasks_SameId()
        {
	        using var scheduler = new Scheduler();
	        scheduler.Enqueue("id", id => { }, TimeSpan.FromMinutes(10));
	        scheduler.Enqueue("id", id => { }, TimeSpan.FromMinutes(10));

	        Assert.AreEqual(1, scheduler.ScheduledTasks().Count());
        }

		[Test]
        public void Scheduler_ScheduledTasks_CountQueue()
        {
	        var queue = new ScheduleQueue();

	        using var scheduler = new Scheduler(queue);
	        scheduler.Enqueue("id", id => { }, TimeSpan.FromMinutes(10));
	        scheduler.Enqueue("id", id => { }, TimeSpan.FromMinutes(10));

	        Assert.AreEqual(queue.ToList().Count(), scheduler.ScheduledTasks().Count());
        }

        [Test]
        public void Scheduler_ScheduledTasks_DelayWithProcessed()
        {
	        using var scheduler = new Scheduler();
	        scheduler.Enqueue("id", id => { }, TimeSpan.Zero);
	        scheduler.Enqueue("id", id => { }, TimeSpan.FromMinutes(10));

	        Task.Delay(1000).Wait();

			Assert.AreEqual(1, scheduler.ScheduledTasks().Count());
        }
    }
}
