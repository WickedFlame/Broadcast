using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Broadcast.Scheduling;
using NUnit.Framework;

namespace Broadcast.Test.Scheduling
{
	public class ScheduleQueueTests
	{
		[Test]
		public void ScheduleQueue_ctor()
		{
			Assert.DoesNotThrow(() => new ScheduleQueue());
		}

		[Test]
		public void ScheduleQueue_Enqueue()
		{
			var task = new SchedulerTask("id", id => { }, TimeSpan.Zero);

			var queue = new ScheduleQueue();
			queue.Enqueue(task);

			Assert.AreSame(task, queue.ToList().Single());
		}

		[Test]
		public void ScheduleQueue_Enqueue_SameTaskMultipleTimes()
		{
			var task = new SchedulerTask("id", id => { }, TimeSpan.Zero);

			var queue = new ScheduleQueue();
			queue.Enqueue(task);
			Assert.DoesNotThrow(() => queue.Enqueue(task));

			// the first task is replcaed
			Assert.AreEqual(1, queue.ToList().Count());
		}

		[Test]
		public void ScheduleQueue_Dequeue()
		{
			var task = new SchedulerTask("id", id => { }, TimeSpan.Zero);

			var queue = new ScheduleQueue();
			queue.Enqueue(task);
			queue.Dequeue(task);

			Assert.IsEmpty(queue.ToList());
		}

		[Test]
		public void ScheduleQueue_Dequeue_Invalid()
		{
			var task = new SchedulerTask("id", id => { }, TimeSpan.Zero);

			var queue = new ScheduleQueue();
			queue.Dequeue(task);

			Assert.IsEmpty(queue.ToList());
		}

		[Test]
		public void ScheduleQueue_ToList()
		{
			var task = new SchedulerTask("id", id => { }, TimeSpan.Zero);

			var queue = new ScheduleQueue();
			queue.Enqueue(task);

			Assert.IsNotEmpty(queue.ToList());
		}

		[Test]
		public void ScheduleQueue_ToList_Count()
		{
			var queue = new ScheduleQueue();
			queue.Enqueue(new SchedulerTask("id1", id => { }, TimeSpan.Zero));
			queue.Enqueue(new SchedulerTask("id2", id => { }, TimeSpan.Zero));

			Assert.AreEqual(2, queue.ToList().Count());
		}
	}
}
