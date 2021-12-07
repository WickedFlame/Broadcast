using Broadcast.EventSourcing;
using System;
using System.Linq.Expressions;

namespace Broadcast
{
	/// <summary>
	/// Executes Tasks in the background or on a TaskServer or cluster
	/// </summary>
	[Obsolete("Use BackgroundTask for simpler use")]
	public class BackgroundTaskClient
	{
		/// <summary>
		/// Setup a new instance for the default <see cref="IBroadcastingClient"/>.
		/// Setup with null to reset to the default
		/// </summary>
		/// <param name="setup"></param>
		[Obsolete("Use BackgroundTask")]
		public static void Setup(Func<IBroadcastingClient> setup) 
			=> BackgroundTask.Setup(setup);

		/// <summary>
		/// Create or update a recurring task.
		/// The name of the Recurring Task is generated based on the <see cref="Expression"/>.
		/// If multiple Recurring Tasks have the same signature, this could cause some confusion
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="time"></param>
		/// <returns>The Id of the task. This is not the same as the name of the RecurringTask</returns>
		[Obsolete("Use BackgroundTask")]
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
		[Obsolete("Use BackgroundTask")]
		public static string Recurring(string name, Expression<Action> expression, TimeSpan time)
			=> BackgroundTask.Recurring(name, expression, time);

		/// <summary>
		/// Adds a scheduled task
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="time"></param>
		/// <returns>The Id of the task</returns>
		[Obsolete("Use BackgroundTask")]
		public static string Schedule(Expression<Action> expression, TimeSpan time)
			=> BackgroundTask.Schedule(expression, time);

		/// <summary>
		/// Process a task
		/// </summary>
		/// <param name="expression"></param>
		/// <returns>The Id of the task</returns>
		[Obsolete("Use BackgroundTask")]
		public static string Send(Expression<Action> expression)
			=> BackgroundTask.Send(expression);

		/// <summary>
		/// Deltete a <see cref="ITask"/> from the Executionpipeline.
		/// If a task is allready in the state of <see cref="TaskState.Processing"/> the delete will be ignored.
		/// </summary>
		/// <param name="taskId"></param>
		[Obsolete("Use BackgroundTask")]
		public static void DeleteTask(string taskId)
			=> BackgroundTask.DeleteTask(taskId);

		/// <summary>
		/// Delete a recurring task with the associated task execution
		/// </summary>
		/// <param name="name">The name of the recurring Task</param>
		[Obsolete("Use BackgroundTask")]
		public static void DeleteRecurringTask(string name)
			=> BackgroundTask.DeleteRecurringTask(name);
	}
}
