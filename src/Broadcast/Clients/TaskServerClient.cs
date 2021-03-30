using Broadcast.Composition;
using System;
using System.Linq.Expressions;

namespace Broadcast
{
	/// <summary>
	/// Executes Tasks on a TaskServer or cluster
	/// </summary>
	public class TaskServerClient
	{
		/// <summary>
		/// Adds a recurring task
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="time"></param>
		public static void Recurring(Expression<Action> expression, TimeSpan time)
		{
			var task = TaskFactory.CreateTask(expression);
			Broadcaster.Server.Recurring(task, time);
		}

		/// <summary>
		/// Adds a scheduled task
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="time"></param>
		public static void Schedule(Expression<Action> expression, TimeSpan time)
		{
			var task = TaskFactory.CreateTask(expression);
			Broadcaster.Server.Schedule(task, time);
		}

		/// <summary>
		/// Process a task
		/// </summary>
		/// <param name="expression"></param>
		public static void Send(Expression<Action> expression)
		{
			var task = TaskFactory.CreateTask(expression);
			Broadcaster.Server.Process(task);
		}
	}
}
