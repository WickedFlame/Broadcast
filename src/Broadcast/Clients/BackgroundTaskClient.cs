using Broadcast.Composition;
using System;

namespace Broadcast
{
	/// <summary>
	/// Executes Tasks as backgroundtasks. These are only executed on the local server instance and are not shared on the cluster for execution
	/// </summary>
	public class BackgroundTaskClient
	{
		// local jobs need a local server running

		/// <summary>
		/// Adds a recurring task
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="time"></param>
		/// <returns></returns>
		public static string Recurring(Action expression, TimeSpan time)
			=> Recurring(null, expression, time);

		/// <summary>
		/// Adds a recurring task
		/// </summary>
		/// <param name="name"></param>
		/// <param name="expression"></param>
		/// <param name="time"></param>
		/// <returns>The Id of the task</returns>
		public static string Recurring(string name, Action expression, TimeSpan time)
		{
			var task = TaskFactory.CreateTask(expression);
			task.Time = time;
			task.IsRecurring = true;

			if (!string.IsNullOrEmpty(name))
			{
				task.Name = name;
			}

			var factory = BroadcastingClient.Default;
			factory.Enqueue(task);

			return task.Id;
		}

		/// <summary>
		/// Adds a scheduled task
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="time"></param>
		/// <returns>The Id of the task</returns>
		public static string Schedule(Action expression, TimeSpan time)
		{
			var task = TaskFactory.CreateTask(expression);
			task.Time = time;

			var factory = BroadcastingClient.Default;
			factory.Enqueue(task);

			return task.Id;
		}

		/// <summary>
		/// Process a task
		/// </summary>
		/// <param name="expression"></param>
		/// <returns>The Id of the task</returns>
		public static string Send(Action expression)
		{
			var task = TaskFactory.CreateTask(expression);

			var factory = BroadcastingClient.Default;
			factory.Enqueue(task);

			return task.Id;
		}
	}
}
