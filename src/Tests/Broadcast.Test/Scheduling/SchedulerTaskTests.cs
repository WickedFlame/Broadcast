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
			Assert.DoesNotThrow(() => new SchedulerTask("id", id => { }, TimeSpan.Zero));
		}

		[Test]
		public void SchedulerTask_ctor_Null_Action()
		{
			Assert.Throws<ArgumentNullException>(() => new SchedulerTask("id", null, TimeSpan.Zero));
		}

		[Test]
		public void SchedulerTask_ctor_Null_Id()
		{
			Assert.Throws<ArgumentNullException>(() => new SchedulerTask(null, id => { }, TimeSpan.Zero));
		}

		[Test]
		public void SchedulerTask_Action_Set()
		{
			Action<string> action = id => { };
			var task = new SchedulerTask("id", action, TimeSpan.Zero);

			Assert.AreSame(action, task.Task);
		}

		[Test]
		public void SchedulerTask_Time_Set()
		{
			TimeSpan time = TimeSpan.Zero;
			var task = new SchedulerTask("id", id => { }, time);

			Assert.AreEqual(time, task.Time);
		}
	}
}
