using Broadcast.Composition;
using System;
using System.Linq.Expressions;

namespace Broadcast
{
	public class TaskServerClient
	{
		public static void Recurring(Expression<Action> expression, TimeSpan time)
		{
			var task = TaskFactory.CreateTask(expression);
			Broadcaster.Server.Recurring(task, time);
		}

		public static void Schedule(Expression<Action> expression, TimeSpan time)
		{
			var task = TaskFactory.CreateTask(expression);
			Broadcaster.Server.Schedule(task, time);
		}

		public static void Send(Expression<Action> expression)
		{
			var task = TaskFactory.CreateTask(expression);
			Broadcaster.Server.Process(task);
		}
	}
}
