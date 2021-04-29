using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Broadcast.Composition;
using Broadcast.EventSourcing;
using NUnit.Framework;

namespace Broadcast.Test.EventSourcing
{
	public class TaskTests
	{
		[Test]
		public void Task_State_Initial()
		{
			var task = TaskFactory.CreateTask(() => Trace.WriteLine("Task_State"));
			Assert.AreEqual(TaskState.New, task.State);
		}

		[Test]
		public void Task_State_Initial_Time()
		{
			var task = TaskFactory.CreateTask(() => Trace.WriteLine("Task_State"));
			Assert.IsTrue(task.StateChanges[TaskState.New] > DateTime.MinValue);
		}

		[Test]
		public void Task_State_Initial_EnsureSingle()
		{
			var task = TaskFactory.CreateTask(() => Trace.WriteLine("Task_State"));
			Assert.IsTrue(task.StateChanges.Keys.Single() == TaskState.New);
		}

		[Test]
		public void Task_State_AddChange()
		{
			var task = TaskFactory.CreateTask(() => Trace.WriteLine("Task_State"));
			task.State = TaskState.Queued;

			Assert.IsTrue(task.StateChanges[TaskState.Queued] > DateTime.MinValue);
		}

		[Test]
		public void Task_State_ResetChange()
		{
			var task = TaskFactory.CreateTask(() => Trace.WriteLine("Task_State"));
			task.State = TaskState.Queued;
			var initial = task.StateChanges[TaskState.Queued];

			task.State = TaskState.Queued;

			Assert.Greater(task.StateChanges[TaskState.Queued], initial);
		}
	}
}
