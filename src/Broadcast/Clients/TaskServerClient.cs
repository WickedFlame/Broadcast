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
		/// <returns>The Id of the task</returns>
		public static string Recurring(Expression<Action> expression, TimeSpan time)
		{
			var task = TaskFactory.CreateTask(expression);
			Broadcaster.Server.Recurring(task, time);

			return task.Id;
		}

		/// <summary>
		/// Adds a scheduled task
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="time"></param>
		/// <returns>The Id of the task</returns>
		public static string Schedule(Expression<Action> expression, TimeSpan time)
		{
			var task = TaskFactory.CreateTask(expression);
			Broadcaster.Server.Schedule(task, time);

			return task.Id;
		}

		/// <summary>
		/// Process a task
		/// </summary>
		/// <param name="expression"></param>
		/// <returns>The Id of the task</returns>
		public static string Send(Expression<Action> expression)
		{
			var task = TaskFactory.CreateTask(expression);
			Broadcaster.Server.Process(task);

			return task.Id;
		}
	}
}
