using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Broadcast.Composition;
using Broadcast.EventSourcing;
using Broadcast.Server;

namespace Broadcast
{
	public static class BroadcastingClientExtensions
	{
		/// <summary>
		/// Create or update a recurring task. 
		/// Recurring Tasks are referenced by the name. 
		/// </summary>
		/// <param name="client"></param>
		/// <param name="name">Unitque name of the recurring Task</param>
		/// <param name="expression"></param>
		/// <param name="time"></param>
		/// <returns>The Id of the task. This is not the same as the name of the RecurringTask</returns>
		public static string Recurring(this IBroadcastingClient client, string name, Expression<Action> expression, TimeSpan time)
		{
			var task = TaskFactory.CreateTask(expression);
			task.Time = time;
			task.IsRecurring = true;
            task.TaskType = TaskType.Recurring;

			if (!string.IsNullOrEmpty(name))
			{
				task.Name = name;
			}

			// check if the recurring task is already registered
			var existing = client.Store.Storage(s => s.Get<RecurringTask>(new Storage.StorageKey($"tasks:recurring:{task.Name}")));
			if (existing != null)
			{
				// update the existing recurring and the task by setting the id of the old task
				// this way all properties get overridden with th enew values
				task.Id = existing.ReferenceId;
			}

			// add the task to the store
			// the store will propagate the task to the registered servers
			client.Enqueue(task);

			return task.Id;
		}

		/// <summary>
		/// Adds a scheduled task
		/// </summary>
		/// <param name="client"></param>
		/// <param name="expression"></param>
		/// <param name="time"></param>
		/// <returns>The Id of the task</returns>
		public static string Schedule(this IBroadcastingClient client, Expression<Action> expression, TimeSpan time)
		{
			var task = TaskFactory.CreateTask(expression);
			task.Time = time;
            task.TaskType = TaskType.Scheduled;

			client.Enqueue(task);

			return task.Id;
		}

		/// <summary>
		/// Process a task
		/// </summary>
		/// <param name="client"></param>
		/// <param name="expression"></param>
		/// <returns>The Id of the task</returns>
		public static string Send(this IBroadcastingClient client, Expression<Action> expression)
		{
			var task = TaskFactory.CreateTask(expression);
            task.TaskType = TaskType.Simple;

			client.Enqueue(task);

			return task.Id;
		}

		/// <summary>
		/// Deltete a <see cref="ITask"/> from the Executionpipeline.
		/// If a task is allready in the state of <see cref="TaskState.Processing"/> the delete will be ignored.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="taskId"></param>
		public static void DeleteTask(this IBroadcastingClient client, string taskId)
		{
			client.Store.Delete(taskId);
		}

		/// <summary>
		/// Delete a recurring task with the associated task execution
		/// </summary>
		/// <param name="client"></param>
		/// <param name="name">The name of the recurring Task</param>
		public static void DeleteRecurringTask(this IBroadcastingClient client, string name)
		{
			var recurring = client.Store.Storage(s => s.Get<RecurringTask>(new Storage.StorageKey($"tasks:recurring:{name}")));
			if (recurring != null)
			{
				client.Store.Delete(recurring.ReferenceId);
			}
		}
	}
}
