using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Broadcast.Composition;
using Broadcast.EventSourcing;
using Broadcast.Server;

namespace Broadcast
{
	/// <summary>
	/// Extensions for Broadcaster
	/// </summary>
	public static class BroadcasterExtensions
	{
		/// <summary>
		/// Send a delegate to the task processor server
		/// </summary>
		/// <param name="broadcaster"></param>
		/// <param name="action"></param>
		public static void Execute(this IBroadcaster broadcaster, Expression<Action> action)
		{
			var task = TaskFactory.CreateTask(action);

			broadcaster.Store.Add(task);
		}

		/// <summary>
		/// Send a delegate to the task processor server
		/// </summary>
		/// <param name="broadcaster"></param>
		/// <param name="action">The Task to process</param>
		public static string Send(this IBroadcaster broadcaster, Expression<Action> action)
		{
			var task = TaskFactory.CreateTask(action);

			broadcaster.Store.Add(task);

			return task.Id;
		}

		/// <summary>
		/// Schedules a new task. The task will be executed at the time passed
		/// </summary>
		/// <param name="broadcaster"></param>
		/// <param name="expression">The task to execute</param>
		/// <param name="time">The time to execute the task at</param>
		public static string Schedule(this IBroadcaster broadcaster, Expression<Action> expression, TimeSpan time)
		{
			var task = TaskFactory.CreateTask(expression);
			task.Time = time;

			broadcaster.Store.Add(task);

			return task.Id;
		}

		/// <summary>
		/// Create a recurring task
		/// </summary>
		/// <param name="broadcaster"></param>
		/// <param name="expression">The task to execute</param>
		/// <param name="time">The interval time to execute the task at</param>
		public static void Recurring(this IBroadcaster broadcaster, Expression<Action> expression, TimeSpan time)
			=> Recurring(broadcaster, null, expression, time);

		/// <summary>
		/// Create a recurring task
		/// </summary>
		/// <param name="broadcaster"></param>
		/// <param name="name"></param>
		/// <param name="expression">The task to execute</param>
		/// <param name="time">The interval time to execute the task at</param>
		public static void Recurring(this IBroadcaster broadcaster, string name, Expression<Action> expression, TimeSpan time)
		{
			var task = TaskFactory.CreateTask(expression);
			task.Time = time;
			task.IsRecurring = true;

			if (!string.IsNullOrEmpty(name))
			{
				task.Name = name;
			}

			broadcaster.Store.Add(task);
		}
		
		/// <summary>
		/// Gets all Tasks that have been processed
		/// </summary>
		/// <param name="broadcaster"></param>
		public static IEnumerable<ITask> GetProcessedTasks(this IBroadcaster broadcaster)
		{
			return broadcaster.Store.Where(s => s.State == TaskState.Processed);
		}

		/// <summary>
		/// Deltete a <see cref="ITask"/> from the Executionpipeline.
		/// If a task is allready in the state of <see cref="TaskState.Processing"/> the delete will be ignored
		/// </summary>
		/// <param name="broadcaster"></param>
		/// <param name="taskId"></param>
		public static void DeleteTask(this IBroadcaster broadcaster, string taskId)
		{
			broadcaster.Store.Delete(taskId);
		}

		/// <summary>
		/// Delete a recurring task with the associated task execution
		/// </summary>
		/// <param name="broadcaster"></param>
		/// <param name="name">The name of the recurring Task</param>
		public static void DeleteRecurringTask(this IBroadcaster broadcaster, string name)
		{
			var recurring = broadcaster.Store.Storage(s => s.Get<RecurringTask>(new Storage.StorageKey($"tasks:recurring:{name}")));
			if (recurring != null)
			{
				broadcaster.Store.Delete(recurring.ReferenceId);
			}
		}
	}
}
