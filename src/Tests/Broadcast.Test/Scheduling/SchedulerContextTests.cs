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
        public void SchedulerContext_ThreadWait()
        {
            var ctx = new SchedulerContext();
            Assert.IsNotNull(ctx.ThreadWait);
        }

		[Test]
		public void SchedulerContext_ThreadWait_IsOpen()
		{
			var ctx = new SchedulerContext();
			Assert.IsTrue(ctx.ThreadWait.IsOpen);
		}
	}
}
