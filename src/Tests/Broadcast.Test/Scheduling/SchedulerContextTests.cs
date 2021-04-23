using System;
using System.Collections.Generic;
using System.Text;
using Broadcast.Scheduling;
using NUnit.Framework;

namespace Broadcast.Test.Scheduling
{
	public class SchedulerContextTests
	{
		[Test]
		public void SchedulerContext_ctor()
		{
			Assert.DoesNotThrow(() => new SchedulerContext());
		}

		[Test]
		public void SchedulerContext_Elapsed()
		{
			var ctx = new SchedulerContext();
			Assert.Greater(ctx.Elapsed, TimeSpan.Zero);
		}

		[Test]
		public void SchedulerContext_IsRunning()
		{
			var ctx = new SchedulerContext();
			Assert.IsFalse(ctx.IsRunning);
		}
	}
}
