using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Broadcast.Scheduling;
using NUnit.Framework;

namespace Broadcast.Test.Scheduling
{
	public class SchedulerTaskDispatcherTests
	{
		[Test]
		public void SchedulerTaskDispatcher_ctor()
		{
			Assert.DoesNotThrow(() => new SchedulerTaskDispatcher(new ScheduleQueue()));
		}

		[Test]
		public void SchedulerTaskDispatcher_ctor_Null_Queue()
		{
			Assert.Throws<ArgumentNullException>(() => new SchedulerTaskDispatcher(null));
		}

		[Test]
		public void SchedulerTaskDispatcher_Execute()
		{
			var queue = new ScheduleQueue();
			var dispatcher = new SchedulerTaskDispatcher(queue);

			var ctx = new SchedulerContext
			{
				IsRunning = true
			};
			queue.Enqueue(new SchedulerTask(() => { ctx.IsRunning = false; }, TimeSpan.Zero));

			dispatcher.Execute(ctx);

			Assert.IsFalse(ctx.IsRunning);
		}

		[Test]
		public void SchedulerTaskDispatcher_Execute_Dequeu()
		{
			var queue = new ScheduleQueue();
			var dispatcher = new SchedulerTaskDispatcher(queue);

			var ctx = new SchedulerContext
			{
				IsRunning = true
			};
			queue.Enqueue(new SchedulerTask(() => { ctx.IsRunning = false; }, TimeSpan.Zero));

			dispatcher.Execute(ctx);

			Assert.IsEmpty(queue.ToList());
		}

		[Test]
		public void SchedulerTaskDispatcher_Execute_CheckTime()
		{
			var queue = new ScheduleQueue();
			var dispatcher = new SchedulerTaskDispatcher(queue);

			var ctx = new SchedulerContext
			{
				IsRunning = true
			};
			queue.Enqueue(new SchedulerTask(() => { }, TimeSpan.FromMinutes(10)));
			queue.Enqueue(new SchedulerTask(() => { }, TimeSpan.Zero));
			queue.Enqueue(new SchedulerTask(() => { }, TimeSpan.Zero));
			queue.Enqueue(new SchedulerTask(() => { ctx.IsRunning = false; }, TimeSpan.Zero));

			dispatcher.Execute(ctx);

			Assert.AreEqual(1, queue.ToList().Count());
		}
	}
}
