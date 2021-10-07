using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Text;
using Broadcast.Composition;
using Broadcast.EventSourcing;
using NUnit.Framework;

namespace Broadcast.Test.EventSourcing
{
	public class ExpressionTaskTests
	{
		[Test]
		public void ExpressionTask_Clone()
		{
			Expression<Action> exp = () => Trace.WriteLine("test");
			var task = TaskFactory.CreateTask(exp);
			var clone = task.Clone();

			Assert.AreEqual(((BroadcastTask) task).Args, ((BroadcastTask) clone).Args);
		}
	}
}
