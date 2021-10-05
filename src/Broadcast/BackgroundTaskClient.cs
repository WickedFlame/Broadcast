using Broadcast.Composition;
using System;
using System.Linq.Expressions;
using Broadcast.Configuration;
using Broadcast.EventSourcing;
using Broadcast.Server;
using Broadcast.Storage;

namespace Broadcast
{
	/// <summary>
	/// Executes Tasks in the background or on a TaskServer or cluster
	/// </summary>
	public class BackgroundTaskClient
	{
		private static readonly ItemFactory<IBroadcastingClient> ItemFactory = new ItemFactory<IBroadcastingClient>(() => new BroadcastingClient());

		/// <summary>
		/// Gets the default instance of the <see cref="IBroadcastingClient"/>
		/// </summary>
		public static IBroadcastingClient Client => ItemFactory.Factory();

		/// <summary>
		/// Setup a new instance for the default <see cref="IBroadcastingClient"/>.
		/// Setup with null to reset to the default
		/// </summary>
		/// <param name="setup"></param>
		public static void Setup(Func<IBroadcastingClient> setup)
		{
			ItemFactory.Factory = setup;
		}

		/// <summary>
		/// Create or update a recurring task.
		/// The name of the Recurring Task is generated based on the <see cref="Expression"/>.
		/// If multiple Recurring Tasks have the same signature, this could cause some confusion
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="time"></param>
		/// <returns>The Id of the task. This is not the same as the name of the RecurringTask</returns>
		public static string Recurring(Expression<Action> expression, TimeSpan time)
			=> Recurring(null, expression, time);

		/// <summary>
		/// Create or update a recurring task. 
		/// Recurring Tasks are referenced by the name. 
		/// </summary>
		/// <param name="name">Unitque name of the recurring Task</param>
		/// <param name="expression"></param>
		/// <param name="time"></param>
		/// <returns>The Id of the task. This is not the same as the name of the RecurringTask</returns>
		public static string Recurring(string name, Expression<Action> expression, TimeSpan time)
		{
			var task = TaskFactory.CreateTask(expression);
			task.Time = time;
			task.IsRecurring = true;

			if (!string.IsNullOrEmpty(name))
			{
				task.Name = name;
			}

			// check if the recurring task is already registered
			var existing = Client.Store.Storage(s => s.Get<RecurringTask>(new Storage.StorageKey($"tasks:recurring:{task.Name}")));
			if (existing != null)
			{
				// update the existing recurring and the task by setting the id of the old task
				// this way all properties get overridden with th enew values
				task.Id = existing.ReferenceId;
			}

			// add the task to the store
			// the store will propagate the task to the registered servers
			Client.Enqueue(task);

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
			task.Time = time;

			Client.Enqueue(task);

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

			Client.Enqueue(task);

			return task.Id;
		}

		/// <summary>
		/// Deltete a <see cref="ITask"/> from the Executionpipeline.
		/// If a task is allready in the state of <see cref="TaskState.Processing"/> the delete will be ignored.
		/// </summary>
		/// <param name="taskId"></param>
		public static void DeleteTask(string taskId)
		{
			Client.Store.Delete(taskId);
		}

		/// <summary>
		/// Delete a recurring task with the associated task execution
		/// </summary>
		/// <param name="name">The name of the recurring Task</param>
		public static void DeleteRecurringTask(string name)
		{
			var recurring = Client.Store.Storage(s => s.Get<RecurringTask>(new Storage.StorageKey($"tasks:recurring:{name}")));
			if (recurring != null)
			{
				Client.Store.Delete(recurring.ReferenceId);
			}
		}
	}
}
