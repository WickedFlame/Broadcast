using System;
using System.Collections.Generic;
using System.Text;
using Broadcast.Scheduling;
using NUnit.Framework;

namespace Broadcast.Test.Scheduling
{
	public class SchedulerTaskTests
	{
		[Test]
		public void SchedulerTask_ctor()
		{
			Assert.DoesNotThrow(() => new SchedulerTask(() => { }, TimeSpan.Zero));
		}

		[Test]
		public void SchedulerTask_ctor_Null_Action()
		{
			Assert.Throws<ArgumentNullException>(() => new SchedulerTask(null, TimeSpan.Zero));
		}

		[Test]
		public void SchedulerTask_Action_Set()
		{
			Action action = () => { };
			var task = new SchedulerTask(action, TimeSpan.Zero);

			Assert.AreSame(action, task.Task);
		}

		[Test]
		public void SchedulerTask_Time_Set()
		{
			TimeSpan time = TimeSpan.Zero;
			var task = new SchedulerTask(() => { }, time);

			Assert.AreEqual(time, task.Time);
		}
	}
}
