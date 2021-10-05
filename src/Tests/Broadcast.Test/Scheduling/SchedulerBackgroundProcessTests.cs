using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Broadcast.Scheduling;
using NUnit.Framework;

namespace Broadcast.Test.Scheduling
{
	public class SchedulerBackgroundProcessTests
	{
		[Test]
		public void SchedulerBackgroundProcess_ctor()
		{
			Assert.DoesNotThrow(() => new SchedulerBackgroundProcess(new ScheduleQueue()));
		}

		[Test]
		public void SchedulerBackgroundProcess_ctor_Null_Queue()
		{
			Assert.Throws<ArgumentNullException>(() => new SchedulerBackgroundProcess(null));
		}

		[Test]
		public void SchedulerBackgroundProcess_Execute()
		{
			var queue = new ScheduleQueue();
			var dispatcher = new SchedulerBackgroundProcess(queue);

			var ctx = new SchedulerContext
			{
				IsRunning = true
			};
			queue.Enqueue(new SchedulerTask("id", id => { ctx.IsRunning = false; }, TimeSpan.Zero));

			dispatcher.Execute(ctx);

			Assert.IsFalse(ctx.IsRunning);
		}

		[Test]
		public void SchedulerBackgroundProcess_Execute_Dequeu()
		{
			var queue = new ScheduleQueue();
			var dispatcher = new SchedulerBackgroundProcess(queue);

			var ctx = new SchedulerContext
			{
				IsRunning = true
			};
			queue.Enqueue(new SchedulerTask("id", id => { ctx.IsRunning = false; }, TimeSpan.Zero));

			dispatcher.Execute(ctx);

			Assert.IsEmpty(queue.ToList());
		}

		[Test]
		public void SchedulerBackgroundProcess_Execute_CheckTime()
		{
			var queue = new ScheduleQueue();
			var dispatcher = new SchedulerBackgroundProcess(queue);

			var ctx = new SchedulerContext
			{
				IsRunning = true
			};
			queue.Enqueue(new SchedulerTask("id1", id => { }, TimeSpan.FromMinutes(10)));
			queue.Enqueue(new SchedulerTask("id2", id => { }, TimeSpan.Zero));
			queue.Enqueue(new SchedulerTask("id3", id => { }, TimeSpan.Zero));
			queue.Enqueue(new SchedulerTask("id4", id => { ctx.IsRunning = false; }, TimeSpan.Zero));

			dispatcher.Execute(ctx);

			Assert.AreEqual(1, queue.ToList().Count());
		}
	}
}
