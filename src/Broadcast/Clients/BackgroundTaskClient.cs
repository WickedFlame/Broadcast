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
		/// <returns>The Id of the task</returns>
		public static string Recurring(Action expression, TimeSpan time)
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
		public static string Schedule(Action expression, TimeSpan time)
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
		public static string Send(Action expression)
		{
			var task = TaskFactory.CreateTask(expression);
			Broadcaster.Server.Send(task);

			return task.Id;
		}


		/// <summary>
		/// Adds a recurring task
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expression"></param>
		/// <param name="time"></param>
		/// <returns>The Id of the task</returns>
		public static string Recurring<T>(Func<T> expression, TimeSpan time) where T : INotification
		{
			var task = TaskFactory.CreateTask(expression);
			Broadcaster.Server.Recurring(task, time);

			return task.Id;
		}

		/// <summary>
		/// Adds a scheduled task
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expression"></param>
		/// <param name="time"></param>
		/// <returns>The Id of the task</returns>
		public static string Schedule<T>(Func<T> expression, TimeSpan time)
		{
			var task = TaskFactory.CreateTask(expression);
			Broadcaster.Server.Schedule(task, time);

			return task.Id;
		}

		/// <summary>
		/// Process a task
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expression"></param>
		/// <returns>The Id of the task</returns>
		public static string Send<T>(Func<T> expression)
		{
			var task = TaskFactory.CreateTask(expression);
			Broadcaster.Server.Send(task);

			return task.Id;
		}
	}
}
